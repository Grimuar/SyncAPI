using System.Text.Json.Serialization;

namespace DreamsSyncronizer.Models.Api.Excel.Request;

public sealed class ExcelWorkbookRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("worksheets")]
    public List<ExcelWorksheetRequest> Worksheets { get; set; } = new();
}