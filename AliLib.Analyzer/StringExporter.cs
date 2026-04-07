using AliLib.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using System.Threading;

namespace AliLib.Analyzer
{
    /// <summary>
    /// The generator responsible for exporting fields marked with <see cref="ExportedStringAttribute"/>.
    /// </summary>
    /// <remarks>
    /// Due to its generator nature, this is a hack and kind of pushes the nature of C# analyzers. It's our best solution for now.
    /// </remarks>
    [Generator]
    public class StringExporter : IIncrementalGenerator
    {
        private const string FullAttributeName = "AliLib.Core.ExportedStringAttribute";

        /// <inheritdoc/>
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            IncrementalValueProvider<string> modPath = context.AnalyzerConfigOptionsProvider
                .Select((options, _) =>
                {
                    options.GlobalOptions.TryGetValue("build_property.ModPath", out string value);
                    return value;
                });

            IncrementalValuesProvider<ExportedStringInfo?> exportedFields = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    fullyQualifiedMetadataName: FullAttributeName,
                    predicate: IsConstantStringField,
                    transform: TransformField);

            IncrementalValuesProvider<ExportedStringInfo> nonNullFields = exportedFields.Where(info => info.HasValue).Select((info, _) => info.Value);

            context.RegisterSourceOutput(modPath.Combine(nonNullFields.Collect()), RunLogic);
        }

        private static bool IsConstantStringField(SyntaxNode node, CancellationToken ct)
        {
            VariableDeclaratorSyntax declarator = node as VariableDeclaratorSyntax;
            if (declarator == null)
                return false;

            VariableDeclarationSyntax declaration = declarator.Parent as VariableDeclarationSyntax;
            if (declaration == null)
                return false;

            FieldDeclarationSyntax field = declaration.Parent as FieldDeclarationSyntax;
            if (field == null)
                return false;

            foreach (SyntaxToken modifier in field.Modifiers)
            {
                if (modifier.IsKind(SyntaxKind.ConstKeyword))
                    return true;
            }

            return false;
        }

        private static ExportedStringInfo? TransformField(GeneratorAttributeSyntaxContext context, CancellationToken ct)
        {
            IFieldSymbol fieldSymbol = context.TargetSymbol as IFieldSymbol;
            if (fieldSymbol == null)
                return null;

            if (fieldSymbol.Type.SpecialType != SpecialType.System_String)
                return null;

            if (!fieldSymbol.IsConst)
                return null;

            string constantValue = fieldSymbol.ConstantValue as string;
            if (constantValue == null)
                return null;

            string exportPath = null;
            foreach (AttributeData attribute in context.Attributes)
            {
                if (attribute.ConstructorArguments.Length > 0)
                {
                    exportPath = attribute.ConstructorArguments[0].Value as string;
                    break;
                }
            }

            if (exportPath == null)
                return null;

            return new ExportedStringInfo(
                fieldSymbol.Name,
                constantValue,
                fieldSymbol.ContainingType.ToDisplayString(),
                fieldSymbol.ContainingNamespace.ToDisplayString(),
                exportPath);
        }

        private static void RunLogic(SourceProductionContext context, (string Left, ImmutableArray<ExportedStringInfo> Right) input)
        {
            string modPath = input.Left;
            ImmutableArray<ExportedStringInfo> fields = input.Right;

            if (modPath == null || fields.IsEmpty)
                return;

            foreach (ExportedStringInfo field in fields)
            {
#pragma warning disable RS1035 // Do not use APIs banned for analyzers
                string normalizedExportPath = field.ExportPath.Replace('/', Path.DirectorySeparatorChar);

                string fullPath = Path.Combine(modPath, normalizedExportPath);

                string parentDir = Path.GetDirectoryName(fullPath);
                if (parentDir != null && !Directory.Exists(parentDir))
                    Directory.CreateDirectory(parentDir);

                try
                {
                    File.WriteAllText(fullPath, field.ConstantValue, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        new DiagnosticDescriptor(
                            "ALI001", "Export failed",
                            "Could not write exported string to '{0}': {1}",
                            "StringExporter", DiagnosticSeverity.Error, true),
                        Location.None, fullPath, ex.Message));
                }
#pragma warning restore RS1035 // Do not use APIs banned for analyzers
            }
        }
    }

    public struct ExportedStringInfo
    {
        public string FieldName { get; }
        public string ConstantValue { get; }
        public string ContainingType { get; }
        public string Namespace { get; }
        public string ExportPath { get; }

        public ExportedStringInfo(string fieldName, string constantValue, string containingType, string @namespace, string exportPath)
        {
            FieldName = fieldName;
            ConstantValue = constantValue;
            ContainingType = containingType;
            Namespace = @namespace;
            ExportPath = exportPath;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ExportedStringInfo other))
                return false;

            return FieldName == other.FieldName
                && ConstantValue == other.ConstantValue
                && ContainingType == other.ContainingType
                && Namespace == other.Namespace
                && ExportPath == other.ExportPath;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + (FieldName != null ? FieldName.GetHashCode() : 0);
                hash = hash * 31 + (ConstantValue != null ? ConstantValue.GetHashCode() : 0);
                hash = hash * 31 + (ContainingType != null ? ContainingType.GetHashCode() : 0);
                hash = hash * 31 + (Namespace != null ? Namespace.GetHashCode() : 0);
                hash = hash * 31 + (ExportPath != null ? ExportPath.GetHashCode() : 0);
                return hash;
            }
        }
    }
}
