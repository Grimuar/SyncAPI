using System.Net.Http.Headers;
using DreamsSyncronizer.Controllers.Base;
using DreamsSyncronizer.Models.Api;
using DreamsSyncronizer.Models.Api.Push.Request;
using DreamsSyncronizer.Models.Api.System.Request;
using DreamsSyncronizer.Models.Api.TestsAndUtility.Mail.Request;
using DreamsSyncronizer.Models.Db;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using DreamsSyncronizer.Common.Mail;
using DreamsSyncronizer.Infrastructure.Helpers;
using DreamsSyncronizer.Models.Api.System.Response;

namespace DreamsSyncronizer.Controllers;

//GET method, way is api/System/HealthCheck
[Produces("application/json")]
[ApiController]
[Route("api/[controller]/[action]")]
public sealed class SystemController : SleepTrackerControllerBase
{
    public SystemController(SleepTrackerDbContext dbConnection,
                            IHttpContextAccessor httpContextAccessor,
                            ILogger<SystemController> logService)
        : base(dbConnection, httpContextAccessor, logService)
    { }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult HealthCheck()
    {
        return Ok("Success");
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult ServerTime()
    {
        var response = new ServerTimeResponse()
        {
            ServerTimeUtc = DateTime.UtcNow
        };

        return Ok(response);
    }

    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Version([FromBody] VersionRequest request)
    {
        var key = request.Key;
        var version = request.Version;
        var store = request.Store;

        if (key != "Secret_Key")
        {
            return ClientError("4442");
        }

        var versionAppFromDb = await DbConnection.VersionApps.FirstOrDefaultAsync(v => v.Store == store);

        if (versionAppFromDb != null)
        {
            versionAppFromDb.Version = version;
        }
        else
        {
            var versionApp = new VersionApp
            {
                Store = store,
                Version = version
            };

            await DbConnection.VersionApps.AddAsync(versionApp);
            await DbConnection.SaveChangesAsync();
            LogService.LogInformation($"Change info: Store-{store} version: {version}");
            return Ok(versionApp);
        }


        await DbConnection.SaveChangesAsync();
        LogService.LogInformation("______Info for new version app added: {@versionAppFromDb}", versionAppFromDb);
        return Ok(versionAppFromDb);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Count([FromBody] CountRequest request)
    {
        string key = request.Key;

        if (key != "Secret_Key")
        {
            return ClientError("4442");
        }

        var usersCount = await DbConnection.Users.CountAsync();

        return Ok(usersCount);
    }

    /// <summary>
    /// Тестовый метод для отправки Push-уведомлений в APNS
    /// </summary>
    /// <param name="notifyRequest"></param>
    /// <returns></returns>
    //[HttpPost]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    // public async Task<IActionResult> Push([FromBody] PushNotifyTestRequest notifyRequest)
    // {
    //     var message = notifyRequest.Message;
    //     var url = string.Format("https://api.sandbox.push.apple.com/3/device/{0}", notifyRequest.DeviceToken);
    //     var request = new HttpRequestMessage(HttpMethod.Post, url);
    //
    //     //токен авторизации для приложения, токен самого приложения
    //     request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", notifyRequest.BearerProviderToken);
    //
    //     //backgrorund для тихой доставки
    //     request.Headers.TryAddWithoutValidation("apns-push-type", "alert");
    //
    //     //не обязательно/id сообщения, в случае ошибки apns вернет ошибку с этим id
    //     request.Headers.TryAddWithoutValidation("apns-id", Guid.NewGuid().ToString());
    //
    //     //срок доставки сообщения в секундах, 0 если не нужно хранить
    //     request.Headers.TryAddWithoutValidation("apns-expiration", Convert.ToString("0"));
    //
    //     //приоритет сообщения 10-немедленная отправка, 5-отправка с учетом настроек энергопотребления,
    //     //1- без пробуждения и оповещения на устройстве
    //     request.Headers.TryAddWithoutValidation("apns-priority", Convert.ToString("10"));
    //
    //     //тут надо разобраться не понял с адресом  и суффиксами =)
    //     request.Headers.TryAddWithoutValidation("apns-topic", "com.example.MyApp");
    //
    //     //устанавливаем вручную версию протокола(для apns обязательно юзать Http/2)
    //     request.Version = new Version(2, 0);
    //
    //     request.Content = new StringContent($"{{\"aps\":{{\"alert\":\"{message}\"}},\"Some Data\":\"SomeValue\"}}");
    //     using (HttpClient httpClient = new HttpClient())
    //     {
    //         HttpResponseMessage responseMessage = await httpClient.SendAsync(request)
    //                                                               .ContinueWith(response =>
    //                                                               {
    //                                                                   LogService.LogInformation("________Info from server response with error: {@request}", request);
    //                                                                   var respResult = response.Result.ToString();
    //
    //                                                                   LogService.LogInformation("________Info from server response with error: {@respresult}",
    //                                                                                             respResult);
    //
    //                                                                   return response.Result; //здесь будет отправляться ошибка
    //                                                               });
    //
    //         if (responseMessage != null)
    //         {
    //             string apnsResponseString = await responseMessage.Content.ReadAsStringAsync();
    //             LogService.LogInformation("________Info from server with Status 200: {@apnsResponseString}",
    //                                       apnsResponseString);
    //
    //             return Ok(apnsResponseString);
    //         }
    //     }
    //
    //     LogService.LogInformation("________Info from end of method no one work(((: {@request}", request);
    //     return Ok(request);
    // }


    // [HttpPost]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    // public async Task<IActionResult> MailSend([FromBody] MailSendRequest request)
    // {
    //     var subject = EmailSubjectHelper.GetRepairSubject(request.Locale);
    //     var message = EmailSubjectHelper.GetRepairMessage(request.Locale);
    //
    //     await EmailService.SendEmailAsync(request.Email, subject, message);
    //     LogService.LogTrace($"Message {request.Message} sended to {request.Email} with theme {request.Subject}");
    //     return Ok("Mail sended!");
    // }


    // [HttpPost]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    // public async Task OnGetAsync()
    //    {
    //        var path = env.ContentRootPath;
    //        path = path + "\\Auth.json";
    //        FirebaseApp app = null;
    //        try
    //        {
    //            app = FirebaseApp.Create(new AppOptions()
    //            {
    //                Credential = GoogleCredential.FromFile(path)
    //            }, "myApp");
    //        }
    //        catch (Exception ex)
    //        {
    //            app = FirebaseApp.GetInstance("myApp");
    //        }
    //
    //        var fcm = FirebaseAdmin.Messaging.FirebaseMessaging.GetMessaging(app);
    //        Message message = new Message()
    //        {
    //            Notification = new Notification
    //            {
    //                Title = "My push notification title",
    //                Body = "Content for this push notification"
    //            },
    //            Data = new Dictionary<string, string>()
    //            {
    //                { "AdditionalData1", "data 1" },
    //                { "AdditionalData2", "data 2" },
    //                { "AdditionalData3", "data 3" },
    //            },
    //
    //            Topic = "WebsiteUpdates"
    //        };
    //        
    //        this.result = await fcm.SendAsync(message);
    //    }
}