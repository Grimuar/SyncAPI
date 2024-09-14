using System.Security.Claims;
using DreamsSyncronizer.Models.Api;
using DreamsSyncronizer.Models.Api.Push.Request;
using DreamsSyncronizer.Models.Db;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamsSyncronizer.Controllers;

[Produces("application/json")]
[ApiController]
[Route("api/[controller]/[action]")]
public class PushController : Controller
{
    private readonly SleepTrackerDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PushController(SleepTrackerDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Register(PushNotifyRequest request)
    {
        //todo регистрация устройства в БД, настроить уникальность необходимых свойств
        // после логина вызываем метод и добавляем устройство

        var loginJwt = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
        var userFromDb = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == loginJwt);
        if (userFromDb == null)
        {
            return StatusCode(400, new ErrorResponse("4446"));
        }

        var userDeviceDb = new UserDeviceDb
        {
            UserId = userFromDb.DbId,
            DeviceType = request.DeviceType,
            DeviceToken = request.DeviceToken,
            DeviceId = request.DeviceId
        };

        await _dbContext.UserDeviceDbs.AddAsync(userDeviceDb);
        await _dbContext.SaveChangesAsync();
        return Ok(userDeviceDb);
    }


    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Unregister(PushNotifyRequest request)
    {
        var loginJwt = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
        var userFromDb = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == loginJwt);
        if (userFromDb == null)
        {
            return StatusCode(400, new ErrorResponse("4446"));
        }

        //todo удаление устройства из БД
        //вызываем при удалении аккаунта или устройства по какому свойству искать устройство?

        await _dbContext.UserDeviceDbs.Where(x => x.DeviceId == request.DeviceId).ExecuteDeleteAsync();

        return Ok();
    }
}