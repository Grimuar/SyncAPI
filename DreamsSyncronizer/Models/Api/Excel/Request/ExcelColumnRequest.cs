using System.Text.Json.Serialization;

namespace DreamsSyncronizer.Models.Api.Excel.Request;

public sealed class ExcelColumnRequest
{
    [JsonPropertyName("columnNumber")]
    public int ColumnNumber { get; set; }
    
    [JsonPropertyName("width")]
    public int Width { get; set; }
    
    [JsonPropertyName("alignmentHorizontal")]
    public string AlignmentHorizontal { get; set; }
    
    [JsonPropertyName("alignmentVertical")]
    public string AlignmentVertical { get; set; }
}