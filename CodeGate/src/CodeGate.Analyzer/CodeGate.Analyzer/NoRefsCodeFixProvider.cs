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
		private const string RefactorReturnTypeTitle = "Remove ref keyword";

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

			// TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
			var diagnostic = context.Diagnostics.First();
			var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var methodDeclarationSyntax = root.FindToken(diagnosticSpan.Start).Parent.DescendantNodesAndSelf().OfType<MethodDeclarationSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: RefactorReturnTypeTitle,
                    createChangedDocument: c => RefactorReturnTypeAsync(context.Document, methodDeclarationSyntax, c),
                    equivalenceKey: RefactorReturnTypeTitle),
                diagnostic);
        }

        private async Task<Document> RefactorReturnTypeAsync(Document document, MethodDeclarationSyntax methodDeclarationSyntax, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclarationSyntax);
            SyntaxNode root;
            if (!document.TryGetSyntaxRoot(out root)) return document;
            var invocationSyntaxes = GetInvocationSyntaxes(root, semanticModel, methodSymbol, cancellationToken);
            
            // Determine ref argument usage (return only or read/return)
            var flowAnalysis = semanticModel.AnalyzeDataFlow(methodDeclarationSyntax.Body);
            var oldParameterListSyntax = methodDeclarationSyntax.ParameterList;
            var refParameters = oldParameterListSyntax.Parameters.Where(x => x.Modifiers.Any(y => y.ValueText == "ref"));

            // TODO : Improve ref parameter replacement to account for usage

            // Remove ref parameters from parameter list
            var newParameterListSyntax = SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(oldParameterListSyntax.Parameters.Except(refParameters)));
            root = root.ReplaceNode(oldParameterListSyntax, newParameterListSyntax);

            // Change return type of method
            var oldReturnTypeSyntax = methodDeclarationSyntax.ReturnType;
            var newReturnTypeSyntax = refParameters.First().Type; // TODO: Handle multiple with return object/struct
            root = root.ReplaceNode(oldReturnTypeSyntax, newReturnTypeSyntax);

            // TODO : Correct invocations to account for changes

            return await Task.FromResult(document.WithSyntaxRoot(root));
        }

        private InvocationExpressionSyntax[] GetInvocationSyntaxes(SyntaxNode root, SemanticModel semanticModel, IMethodSymbol methodSymbol, CancellationToken cancellationToken)
        {
            return root
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Where(invocationSyntax => semanticModel.GetSymbolInfo(invocationSyntax, cancellationToken).Symbol == methodSymbol)
                .ToArray();
        }
	}
}