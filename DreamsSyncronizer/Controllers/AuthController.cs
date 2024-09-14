using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DreamsSyncronizer.Common;
using DreamsSyncronizer.Common.Mail;
using DreamsSyncronizer.Controllers.Base;
using DreamsSyncronizer.Infrastructure.Generators;
using DreamsSyncronizer.Infrastructure.Helpers;
using DreamsSyncronizer.Models.Api.Auth.Request;
using DreamsSyncronizer.Models.Api.Auth.Response;
using DreamsSyncronizer.Models.Db.Auth;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;


namespace DreamsSyncronizer.Controllers;

[Produces("application/json")]
[ApiController]
[Route("api/[controller]/[action]")]
public sealed class AuthController : SleepTrackerControllerBase
{
    public AuthController(SleepTrackerDbContext dbConnection,
                          IHttpContextAccessor httpContextAccessor,
                          ILogger<AuthController> logger)
        : base(dbConnection, httpContextAccessor, logger)
    { }

    /// <summary>
    /// Registration user in api
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// POST /auth/register
    /// {
    /// "Login": "string",
    /// "Password": "string"
    /// "ConfirmPassword" : "string"
    /// }
    /// </remarks>
    /// <param name="request"> LoginModel object</param>
    /// <returns>{
    /// "access_token": "JwtToken",
    /// "login": "UserLogin"
    ///           }
    /// </returns>
    /// <response code="200">Always</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelValidator.CheckEmailFormat(request.Login))
        {
            return ClientError("4447");
        }

        if (!ModelValidator.CheckLengthCondition(request.Password, 6))
        {
            return ClientError("4448");
        }

        var userFromDb = await DbConnection.Users.FirstOrDefaultAsync(u => u.Login == request.Login);
        if (userFromDb != null)
        {
            return ClientError("4443");
        }

        var user = new UserDb
        {
            Login = request.Login,
            PasswordHash = PasswordHandler.ComputeSha256(request.Password),
            LinkId = Guid.NewGuid().ToString()
        };

        await DbConnection.Users.AddAsync(user);
        
        var rowCount = await DbConnection.SaveChangesAsync();
        if (rowCount > 0)
        {
            var emailContent = EmailSubjectHelper.GetRegistrationText(HttpContext.Request.Headers.AcceptLanguage);

            var messageWithData = $"{emailContent.Body} {request.Password}.";

            await EmailService.SendEmailAsync(request.Login, emailContent.Subject, messageWithData);

            LogService.LogTrace($"Message {messageWithData} sent to {request.Login} with theme \"Complete registration\"");
        }
        
        return Ok();
    }


    /// <summary>
    /// Authentication user in api
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// POST /auth/login
    /// {
    /// "Login": "string",
    /// "Password": "string"
    /// }
    /// </remarks>
    /// <param name="request"> LoginModel object</param>
    /// <returns>{
    /// "access_token": "JwtToken",
    /// "login": "UserLogin"
    ///           }
    /// </returns>
    /// <response code="200">Always</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelValidator.CheckEmailFormat(request.Login))
        {
            return ClientError("4447");
        }

        if (!ModelValidator.CheckLengthCondition(request.Password, 6))
        {
            return ClientError("4448");
        }

        var userFromDb = await DbConnection.Users.FirstOrDefaultAsync(u => u.Login == request.Login);
        if (userFromDb == null)
        {
            return ClientError("4444");
        }

        if (PasswordHandler.ComputeSha256(request.Password) != userFromDb.PasswordHash)
        {
            return ClientError("4445");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userFromDb.Login),
            new Claim("guid", userFromDb.LinkId),
            new Claim("id", userFromDb.DbId.ToString())
        };

        var securityToken = new JwtSecurityToken(issuer: AuthOptions.ISSUER,
                                                 audience: AuthOptions.AUDIENCE,
                                                 claims: claims,
                                                 expires: DateTime.UtcNow.AddDays(7),
                                                 signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                                                                                            SecurityAlgorithms.HmacSha256));

        var encodedToken = new JwtSecurityTokenHandler().WriteToken(securityToken);

        var response = new LoginResponse(encodedToken, userFromDb.Login, userFromDb.LinkId);

        return Ok(response);
    }


    /// <summary>
    /// Edit user in api
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// PUT /auth/update
    /// {
    /// "OldPassword": "string",
    /// "NewPassword": "string",
    /// "ConfirmNewPassword": "string"
    /// }
    /// </remarks>
    /// <param name="request"> LoginModel object</param>
    /// <returns>{
    /// "UserDb"
    ///           }
    /// </returns>
    /// <response code="200">Success</response>
    /// <response code="401">If the user is unauthorized</response>
    [HttpPut]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        if (!ModelValidator.CheckLengthCondition(request.NewPassword, 6))
        {
            return ClientError("4448");
        }

        var dbUser = await GetUserFromRequestAsync();
        if (dbUser == null)
        {
            return ClientError("4446");
        }

        dbUser.PasswordHash = PasswordHandler.ComputeSha256(request.NewPassword);
        var rowCount = await DbConnection.SaveChangesAsync();
        if (rowCount > 0)
        {
            var emailContent = EmailSubjectHelper.GetChangePasswordText(HttpContext.Request.Headers.AcceptLanguage.ToString());
            var messageWithPassword = $"{emailContent.Body} {request.NewPassword}.";

            await EmailService.SendEmailAsync(dbUser.Login, emailContent.Subject, messageWithPassword);
        }

        return Ok();
    }

    /// <summary>
    /// Delete user in api and all his data (Working cascade delete)
    /// ConfirmDelete = 1 - Confirmation,0 - No Confirmation 
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// PUT /auth/update
    /// {
    /// "Login": "string",
    /// "Password": "string",
    /// "ConfirmDelete": "1"
    /// }
    /// </remarks>
    /// <param name="request"> LoginModel object</param>
    /// <returns>{
    /// "access_token": "JwtToken",
    /// "login": "UserLogin"
    ///           }
    /// </returns>
    /// <response code="200">Always</response>
    /// <response code="401">If the user is unauthorized</response>
    [HttpDelete]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete([FromBody] DeleteRequest request)
    {
        if (!ModelValidator.CheckEmailFormat(request.Login))
        {
            return ClientError("4447");
        }

        if (!ModelValidator.CheckLengthCondition(request.Password, 6))
        {
            return ClientError("4448");
        }

        //Here we get login user from jwt token
        var userFromDb = await GetUserFromRequestAsync();
        if (userFromDb == null)
        {
            return ClientError("4446");
        }

        //todo для удаления пользователя нужно сравнить пароли
        if (PasswordHandler.ComputeSha256(request.Password) != userFromDb.PasswordHash)
        {
            return ClientError("4445");
        }

        DbConnection.Users.Remove(userFromDb);

        var syncParts = DbConnection.SyncPartDbs.Where(x => x.LinkId == userFromDb.LinkId);
        DbConnection.SyncPartDbs.RemoveRange(syncParts);

        await DbConnection.SaveChangesAsync();

        return Ok();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        if (!ModelValidator.CheckEmailFormat(request.Email))
        {
            return ClientError("4447");
        }

        var userFromDb = await DbConnection.Users.FirstOrDefaultAsync(u => u.Login == request.Email);
        if (userFromDb == null)
        {
            return ClientError("4449");
        }

        var sentPassword = StringRandomizer.RandomString(8);
        userFromDb.PasswordHash = PasswordHandler.ComputeSha256(sentPassword);
        var rowCount = await DbConnection.SaveChangesAsync();

        if (rowCount > 0)
        {
            var emailContent = EmailSubjectHelper.GetResetPasswordText(HttpContext.Request.Headers.AcceptLanguage);
            var messageWithPassword = $"{emailContent.Body} {sentPassword}.";

            await EmailService.SendEmailAsync(request.Email, emailContent.Subject, messageWithPassword);

            LogService.LogTrace($"Message {messageWithPassword} sent to {request.Email} with theme \"ResetPassword Password\"");
        }

        return Ok();
    }
}