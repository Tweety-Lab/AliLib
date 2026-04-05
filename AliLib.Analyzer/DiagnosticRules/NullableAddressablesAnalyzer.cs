using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace AliLib.Analyzer.DiagnosticRules
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NullableAddressablesAnalyzer : DiagnosticAnalyzer
    {
        public static DiagnosticDescriptor Rule { get; } = new DiagnosticDescriptor(
            id: "ALI0001",
            title: "Nullable Addressables",
            messageFormat: "Addressable asset '{0}' should be nullable.",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Properties marked with [Addressable] should be nullable due to assets being loaded asynchronously."
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

            bool isNullable = symbol.Type.NullableAnnotation == NullableAnnotation.Annotated;

            if (!isNullable)
                context.ReportDiagnostic(Diagnostic.Create(Rule, property.GetLocation(), symbol.Name));
        }
    }

}
