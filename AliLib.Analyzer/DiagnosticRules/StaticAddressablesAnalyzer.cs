using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace AliLib.Analyzer.DiagnosticRules
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StaticAddressablesAnalyzer : DiagnosticAnalyzer
    {
        public static DiagnosticDescriptor Rule { get; } = new DiagnosticDescriptor(
            id: "ALI0002",
            title: "Static Addressables",
            messageFormat: "Addressable asset '{0}' must be static.",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Properties marked with [Addressable] must be static to be loaded correctly."
            );

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.PropertyDeclaration);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var property = (PropertyDeclarationSyntax)context.Node;
            var symbol = context.SemanticModel.GetDeclaredSymbol(property);

            if (symbol is null)
                return;

            bool hasAttribute = symbol.GetAttributes().Any(a => a.AttributeClass?.Name == "AddressableAttribute");

            if (!hasAttribute)
                return;

            if (!symbol.IsStatic)
                context.ReportDiagnostic(Diagnostic.Create(Rule, property.GetLocation(), symbol.Name));
        }
    }

}
