using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGate.Analyzer
{
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NoRefsCodeFixProvider)), Shared]
	public class NoRefsCodeFixProvider : CodeFixProvider
	{
		const string RefactorReturnTypeTitle = "Remove ref keyword";

		public sealed override ImmutableArray<string> FixableDiagnosticIds
		{
			get { return ImmutableArray.Create(NoRefsAnalyzer.DiagnosticId); }
		}

		public sealed override FixAllProvider GetFixAllProvider()
		{
			return WellKnownFixAllProviders.BatchFixer;
		}

		public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

			var diagnostic = context.Diagnostics.First();
			var diagnosticSpan = diagnostic.Location.SourceSpan;

			var methodDeclarationSyntax = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

			// Register a code action that will invoke the fix.
			context.RegisterCodeFix(
				CodeAction.Create(
					title: RefactorReturnTypeTitle,
					createChangedDocument: c => RefactorReturnTypeAsync(context.Document, methodDeclarationSyntax, c),
					equivalenceKey: RefactorReturnTypeTitle),
				diagnostic);
		}

		async Task<Document> RefactorReturnTypeAsync(Document document, MethodDeclarationSyntax methodDeclarationSyntax, CancellationToken cancellationToken)
		{
			var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
			var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclarationSyntax);
			SyntaxNode root;
			if (!document.TryGetSyntaxRoot(out root)) return document;
			var invocationSyntaxes = GetInvocationSyntaxes(root, semanticModel, methodSymbol, cancellationToken);
			
			// Determine ref argument usage (return only or read/return)
			var flowAnalysis = semanticModel.AnalyzeDataFlow(methodDeclarationSyntax.Body);
			var oldParameterListSyntax = methodDeclarationSyntax.ParameterList;
			var refParameters = oldParameterListSyntax.Parameters.Where(x => x.Modifiers.Any(m => m.ValueText == "ref"));
			var readParameters = refParameters.Where(refParameter => flowAnalysis.ReadInside.Contains(semanticModel.GetDeclaredSymbol(refParameter)));
			var unreadParameters = refParameters.Except(readParameters);

			// TODO : Improve ref parameter replacement to account for usage
			var newParameters = oldParameterListSyntax.Parameters.Except(unreadParameters)
				.Select(p => !readParameters.Contains(p) ? p : // keep existing normal parameters
					SyntaxFactory.Parameter( // create non-ref parameters if they're used
						p.AttributeLists,
						SyntaxFactory.TokenList(p.Modifiers.Where(m => m.ValueText != "ref")),
						p.Type,
						p.Identifier,
						p.Default));

			// Remove ref parameters from parameter list
			var newParameterListSyntax = SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(newParameters));
			root = root.ReplaceNode(oldParameterListSyntax, newParameterListSyntax);

			// Change return type of method
			var oldReturnTypeSyntax = methodDeclarationSyntax.ReturnType;
			var newReturnTypeSyntax = refParameters.First().Type; // TODO: Handle multiple with return object/struct
			if (oldReturnTypeSyntax != newReturnTypeSyntax)
				root = root.ReplaceNode(oldReturnTypeSyntax, newReturnTypeSyntax);

			// TODO : Correct invocations to account for changes

			return await Task.FromResult(document.WithSyntaxRoot(root));
		}

		InvocationExpressionSyntax[] GetInvocationSyntaxes(SyntaxNode root, SemanticModel semanticModel, IMethodSymbol methodSymbol, CancellationToken cancellationToken)
		{
			return root
				.DescendantNodes()
				.OfType<InvocationExpressionSyntax>()
				.Where(invocationSyntax => semanticModel.GetSymbolInfo(invocationSyntax, cancellationToken).Symbol == methodSymbol)
				.ToArray();
		}
	}
}