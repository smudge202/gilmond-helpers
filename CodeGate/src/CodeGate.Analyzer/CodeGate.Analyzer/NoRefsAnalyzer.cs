using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeGate.Analyzer
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class NoRefsAnalyzer : DiagnosticAnalyzer
	{
		public const string DiagnosticId = DiagnosticIds.NoRefs;
		private const string Title = "Do not use ref keyword";
        const string MessageFormat = "The ref keyword on parameter '{0}' is likely a lazy way of extending the result set of the method.";
        private const string Category = "Usage";
        const string HelpUri = "http://blog.devbot.net/conventions-refs/";
        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, helpLinkUri: HelpUri);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.Parameter);
		}

        static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var parameterSyntax = (ParameterSyntax)context.Node;
            var refModifier = parameterSyntax.Modifiers.FirstOrDefault(x => x.ValueText == "ref");
            if (refModifier == default(SyntaxToken)) return;
            var methodDeclarationSyntax = parameterSyntax.Ancestors().OfType<MethodDeclarationSyntax>().First();
            var diagnostic = Diagnostic.Create(Rule, methodDeclarationSyntax.GetLocation(), parameterSyntax.Identifier.ValueText);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
