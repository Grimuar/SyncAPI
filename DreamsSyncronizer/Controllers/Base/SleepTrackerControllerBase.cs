using System.Security.Claims;
using System.Text;
using DreamsSyncronizer.Models.Api;
using DreamsSyncronizer.Models.Db;
using DreamsSyncronizer.Models.Db.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamsSyncronizer.Controllers.Base;

public abstract class SleepTrackerControllerBase : Controller
{
    #region Dependencies

    protected readonly SleepTrackerDbContext DbConnection;
    protected readonly IHttpContextAccessor HttpContextAccessor;
    protected readonly ILogger LogService;

    #endregion

    protected SleepTrackerControllerBase(SleepTrackerDbContext dbConnection,
                                         IHttpContextAccessor httpContextAccessor,
                                         ILogger logService)
    {
        DbConnection = dbConnection;
        HttpContextAccessor = httpContextAccessor;
        LogService = logService;
    }

    #region Protected Methods

    protected async Task<UserDb> GetUserFromRequestAsync()
    {
        UserDb result = null;

        var claimValue = HttpContextAccessor?.HttpContext?.User.FindFirst("id")?.Value ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(claimValue)
            && int.TryParse(claimValue, out var idFromClaim))
        {
            result = await DbConnection.Users.FindAsync(idFromClaim);
        }

        return result;
    }

    protected ObjectResult ClientError(string code)
    {
        return StatusCode(400, new ErrorResponse(code));
    }

    protected ObjectResult SystemError(Exception ex)
    {
        var sb = new StringBuilder();
        sb.Append("message: ");
        sb.Append(ex?.Message ?? "empty");

        sb.AppendLine();

        sb.Append("stacktrace: ");
        sb.Append(ex?.StackTrace ?? "empty");

        return StatusCode(500, new ErrorResponse(sb.ToString()));
    }

    #endregion
}