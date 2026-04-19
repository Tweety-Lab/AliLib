using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace AliLib.Analyzer.DiagnosticRules
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ObsoleteCallsAnalyzer : DiagnosticAnalyzer
    {
        /// <summary> All the obsolete methods we should warn for. </summary>
        public static Dictionary<string, string> ObsoleteMethods { get; } = new Dictionary<string, string>()
        {
            { "Catalog.LoadAssetAsync", "This method does not support AliLib. Use Catalog.LoadCachedAssetAsync instead." }
        };

        private static readonly ImmutableArray<DiagnosticDescriptor> descriptors =
            ObsoleteMethods.Keys.Select(key => new DiagnosticDescriptor(
                id: "ALI0004",
                title: $"{key} is obsolete",
                messageFormat: ObsoleteMethods[key],
                category: "Usage",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true
            )).ToImmutableArray();

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => descriptors;

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
        }

        private void Analyze(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            var symbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol;
            if (symbol == null)
                return;

            string fullName = $"{symbol.ContainingType.Name}.{symbol.Name}";

            if (!ObsoleteMethods.TryGetValue(fullName, out string message))
                return;

            var descriptor = descriptors.FirstOrDefault(d => d.Title.ToString().StartsWith(fullName));
            if (descriptor == null)
                return;

            context.ReportDiagnostic(Diagnostic.Create(descriptor, invocation.GetLocation()));
        }
    }
}
