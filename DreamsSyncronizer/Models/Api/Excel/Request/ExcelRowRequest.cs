using System.Text.Json.Serialization;

namespace DreamsSyncronizer.Models.Api.Excel.Request;

public sealed class ExcelRowRequest
{
    [JsonPropertyName("rowNumber")]
    public int RowNumber { get; set; }

    [JsonPropertyName("cells")]
    public List<ExcelCellRequest> Cells { get; set; } = new();
}