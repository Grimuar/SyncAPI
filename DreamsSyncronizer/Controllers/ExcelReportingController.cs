using DreamsSyncronizer.Controllers.Base;
using DreamsSyncronizer.Infrastructure.Helpers;
using DreamsSyncronizer.Models.Api.Excel.Request;
using Microsoft.AspNetCore.Mvc;

namespace DreamsSyncronizer.Controllers;

[Produces("application/json")]
[ApiController]
[Route("api/[controller]/[action]")]
public sealed class ExcelReportingController : SleepTrackerControllerBase
{
    public ExcelReportingController(SleepTrackerDbContext dbConnection,
                                    IHttpContextAccessor httpContextAccessor,
                                    ILogger<ExcelReportingController> logService)
        : base(dbConnection, httpContextAccessor, logService)
    { }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GenerateReport([FromBody] ExcelWorkbookRequest request)
    {
        if (request == null)
            return BadRequest("Request is null");

        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Request.Name is null");

        try
        {
            var folder = Path.Combine("temp", "excel_reports");
            FileSystemHelper.CreateFolder(folder);

            var pathForSave = Path.Combine(folder, request.Name + ".xlsx");

            var reportException = ExcelReportHelper.GenerateExcel(request, pathForSave);
            if (reportException == null)
            {
                var bytes = await System.IO.File.ReadAllBytesAsync(pathForSave);
                FileSystemHelper.DeleteFile(pathForSave);

                return File(bytes, "application/octet-stream");
            }
            else
            {
                return SystemError(reportException);
            }
        }
        catch (Exception e)
        {
            LogService.LogError(e, "GenerateReport()");

            return SystemError(e);
        }
    }
}