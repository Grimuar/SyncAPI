using System.Text.Json.Serialization;

namespace DreamsSyncronizer.Models.Api.Excel.Request;

public sealed class ExcelWorksheetRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("columns")]
    public List<ExcelColumnRequest> Columns { get; set; } = new();

    [JsonPropertyName("rows")]
    public List<ExcelRowRequest> Rows { get; set; } = new(); 
    
    public ExcelCellRequest Cell(int row, int column)
    {
        var dataRow = Rows.Find(x => x.RowNumber == row);
        if (dataRow == null)
        {
            dataRow = new ExcelRowRequest() { RowNumber = row };
            Rows.Add(dataRow);
        }

        var cell = dataRow.Cells.Find(x => x.ColumnNumber == column);
        if (cell == null)
        {
            cell = new ExcelCellRequest() { ColumnNumber = column };
            dataRow.Cells.Add(cell);
        }

        return cell;
    }
    public ExcelColumnRequest Column(int column)
    {
        var excelColumn = Columns.Find(x => x.ColumnNumber == column);
        if (excelColumn == null)
        {
            excelColumn = new ExcelColumnRequest()
            {
                ColumnNumber = column
            };
            Columns.Add(excelColumn);
        }

        return excelColumn;
    }
}