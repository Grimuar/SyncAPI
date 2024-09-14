using System.Text.Json.Serialization;

namespace DreamsSyncronizer.Models.Api.Excel.Request;

public sealed class ExcelCellRequest
{
    [JsonPropertyName("columnNumber")]
    public int ColumnNumber { get; set; }
    
    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonPropertyName("background")]
    public string Background { get; set; }
    
    [JsonPropertyName("dateformat")]
    public string DateFormat { get; set; }
    
    [JsonPropertyName("alignmentVertical")]
    public string AlignmentVertical { get; set; }
    
    [JsonPropertyName("alignmentHorizontal")]
    public string AlignmentHorizontal { get; set; }

    [JsonPropertyName("outsideBorderStyle")]
    public string OutsideBorderStyle { get; set; }
    
    [JsonPropertyName("outsideBorderColor")]
    public string OutsideBorderColor { get; set; }
}

public static class AlignmentVerticalValues
{
    public const string Bottom = "Bottom";
    public const string Center = "Center";
    public const string Distributed = "Distributed";
    public const string Justify = "Justify";
    public const string Top = "Top";
}

public static class AlignmentHorizontalValues
{
    public const string Center = "Center";
    public const string CenterContinuous = "CenterContinuous";
    public const string Distributed = "Distributed";
    public const string Fill = "Fill";
    public const string General = "General";
    public const string Justify = "Justify";
    public const string Left = "Left";
    public const string Right = "Right";
}

public static class BorderStyleValues
{
    public const string DashDot = "DashDot";
    public const string DashDotDot = "DashDotDot";
    public const string Dashed = "Dashed";
    public const string Dotted = "Dotted";
    public const string Double = "Double";
    public const string Hair = "Hair";
    public const string Medium = "Medium";
    public const string MediumDashDot = "MediumDashDot";
    public const string MediumDashDotDot = "MediumDashDotDot";
    public const string MediumDashed = "MediumDashed";
    public const string None = "None";
    public const string SlantDashDot = "SlantDashDot";
    public const string Thick = "Thick";
    public const string Thin = "Thin";
}