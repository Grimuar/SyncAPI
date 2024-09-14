using ClosedXML.Excel;
using DreamsSyncronizer.Models.Api.Excel.Request;

namespace DreamsSyncronizer.Infrastructure.Helpers;

public static class ExcelReportHelper
{
    public static Exception GenerateExcel(ExcelWorkbookRequest workbook, string pathForSave)
    {
        Exception result = null;

        try
        {
            var xlWorkbook = new XLWorkbook();

            foreach (var worksheet in workbook.Worksheets)
            {
                var xlWorksheet = xlWorkbook.Worksheets.Add(worksheet.Name);

                foreach (var row in worksheet.Rows.OrderBy(x => x.RowNumber))
                {
                    foreach (var cell in row.Cells.OrderBy(x => x.ColumnNumber))
                    {
                        var xlCell = xlWorksheet.Cell(row.RowNumber, cell.ColumnNumber);

                        xlCell.Value = cell.Value;

                        ProcessBackground(cell, xlCell);

                        ProcessDateFormat(cell, xlCell);

                        ProcessOutsideBorder(cell, xlCell);

                        ProcessAlignment(cell, xlCell);
                    }
                }

                foreach (var column in worksheet.Columns)
                {
                    xlWorksheet.Column(column.ColumnNumber).Width = column.Width;
                }
            }

            xlWorkbook.SaveAs(pathForSave);
        }
        catch (Exception ex)
        {
            result = ex;
        }

        return result;
    }

    #region Private Methods

    private static void ProcessBackground(ExcelCellRequest cell, IXLCell xlCell)
    {
        try
        {
            if (!string.IsNullOrEmpty(cell.Background))
            {
                if (cell.Background == "transparent")
                {
                    xlCell.Style.Fill.BackgroundColor = XLColor.Transparent;
                }
                else
                {
                    xlCell.Style.Fill.BackgroundColor = XLColor.FromHtml(cell.Background);
                }
            }
        }
        catch (Exception e)
        {
            //
        }
    }

    private static void ProcessDateFormat(ExcelCellRequest cell, IXLCell xlCell)
    {
        try
        {
            if (!string.IsNullOrEmpty(cell.DateFormat))
            {
                xlCell.Style.DateFormat.SetFormat(cell.DateFormat);
            }
        }
        catch (Exception e)
        {
            //
        }
    }

    private static void ProcessOutsideBorder(ExcelCellRequest cell, IXLCell xlCell)
    {
        try
        {
            if (!string.IsNullOrEmpty(cell.OutsideBorderColor))
            {
                xlCell.Style.Border.OutsideBorderColor = XLColor.FromHtml(cell.OutsideBorderColor);
            }

            if (!string.IsNullOrEmpty(cell.OutsideBorderStyle))
            {
                if (Enum.TryParse(typeof(XLBorderStyleValues), cell.OutsideBorderStyle, true, out var enumValue))
                {
                    xlCell.Style.Border.OutsideBorder = (XLBorderStyleValues)enumValue;
                }
            }
        }
        catch (Exception e)
        {
            //
        }
    }

    private static void ProcessAlignment(ExcelCellRequest cell, IXLCell xlCell)
    {
        try
        {
            if (!string.IsNullOrEmpty(cell.AlignmentVertical))
            {
                if (Enum.TryParse(typeof(XLAlignmentVerticalValues), cell.AlignmentVertical, true, out var enumValue))
                {
                    xlCell.Style.Alignment.Vertical = (XLAlignmentVerticalValues)enumValue;
                }
            }
        }
        catch (Exception e)
        {
            //
        }
    }

    #endregion
}