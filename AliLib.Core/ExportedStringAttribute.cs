using System;

namespace AliLib.Core;

/// <summary>
/// Marks a string field or property as an exported mod asset (i.e., a json).
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ExportedStringAttribute : Attribute
{
    /// <summary> The path the string will be exported to relative to the mod folder root. </summary>
    public string ExportPath { get; set; }

    /// <summary> Initializes a new instance of the <see cref="ExportedStringAttribute"/> class. </summary>
    public ExportedStringAttribute(string exportPath) => ExportPath = exportPath;
}
