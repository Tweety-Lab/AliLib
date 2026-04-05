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
    public class ConstantExportedStringAnalyzer : DiagnosticAnalyzer
    {
        public static DiagnosticDescriptor Rule { get; } = new DiagnosticDescriptor(
            id: "ALI0003",
            title: "Constant Exported Strings",
            messageFormat: "Exported String '{0}' must be a constant.",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Exported strings must be constants to be properly exported."
            );

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.FieldDeclaration);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var field = (FieldDeclarationSyntax)context.Node;

            foreach (var variable in field.Declaration.Variables)
            {
                var symbol = context.SemanticModel.GetDeclaredSymbol(variable) as IFieldSymbol;

                if (symbol == null)
                    continue;

                bool hasAttribute = symbol.GetAttributes().Any(a => a.AttributeClass?.Name == "ExportedStringAttribute");

                if (!hasAttribute)
                    continue;

                if (!symbol.IsConst)
                {
                    var diagnostic = Diagnostic.Create(Rule, variable.GetLocation(), symbol.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
