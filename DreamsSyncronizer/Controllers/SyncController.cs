using DreamsSyncronizer.Controllers.Base;
using DreamsSyncronizer.Models.Api.Sync.Request;
using DreamsSyncronizer.Models.Api.Sync.Response;
using DreamsSyncronizer.Models.Db.Sync;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace DreamsSyncronizer.Controllers;

[Produces("application/json")]
[ApiController]
[Route("api/[controller]/[action]")]
public sealed class SyncController : SleepTrackerControllerBase
{
    public SyncController(SleepTrackerDbContext dbConnection,
                          IHttpContextAccessor httpContextAccessor,
                          ILogger<SyncController> logService)
        : base(dbConnection, httpContextAccessor, logService)
    { }

    /// <summary>
    /// Проверка наличия данных для linkId. Если данные есть, то возвращаем ключи и lastUpdate
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckData([FromBody] CheckDataRequest request)
    {
        var dbUser = await GetUserFromRequestAsync();
        if (dbUser == null)
        {
            return ClientError("4446");
        }

        var lastUpdate = DateTime.MinValue;
        var keys = new List<string>();

        var hasData = await DbConnection.SyncPartDbs.AnyAsync(x => x.LinkId == request.LinkId);
        if (hasData)
        {
            var serverData = await DbConnection.SyncPartDbs.Where(x => x.LinkId == request.LinkId).ToArrayAsync();

            keys.AddRange(serverData.Select(x => x.Key));

            lastUpdate = serverData.Max(x => x.Timestamp);
        }

        var result = new CheckDataResponse()
        {
            LastDate = lastUpdate,
            Keys = keys
        };

        return Ok(result);
    }


    /// <summary>
    /// Проверка актуальности данных. Проверяем, какие наборы идентичны, какие нужно скачать из МП, какие нужно загрузить в МП
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckRelevance([FromBody] CheckRelevanceRequest request)
    {
        var dbUser = await GetUserFromRequestAsync();
        if (dbUser == null)
        {
            return ClientError("4446");
        }

        var similar = new HashSet<string>();
        var onlyOnServer = new HashSet<string>();
        var onlyOnClient = new HashSet<string>();

        //проверяем какие данные есть в БД
        var dataFromServer = await DbConnection.SyncPartDbs.Where(x => x.LinkId == request.LinkId).ToListAsync();

        //сначала проверяем то, что приходит с мп
        foreach (var dataFromClient in request.Items)
        {
            CheckFromClientToServer(dataFromServer, dataFromClient, similar, onlyOnServer, onlyOnClient);
        }

        //делаем то же самое, но со стороны сервера
        foreach (var serverEntry in dataFromServer)
        {
            CheckFromServerToClient(request.Items, serverEntry, similar, onlyOnServer, onlyOnClient);
        }

        var result = new CheckRelevanceResponse()
        {
            Actual = similar.ToArray(),
            ForUpload = onlyOnClient.ToArray(),
            ForDownload = onlyOnServer.ToArray()
        };

        return Ok(result);
    }

    /// <summary>
    /// Получаем данные из МП
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Upload([FromBody] UploadRequest request)
    {
        var dbUser = await GetUserFromRequestAsync();
        if (dbUser == null)
        {
            return ClientError("4446");
        }

        try
        {
            var responseItems = new List<UploadResponseItem>();

            var utcNow = DateTime.UtcNow;

            var dataFromClient = request.Items ?? Array.Empty<UploadRequestItem>();

            var serverEntries = await DbConnection.SyncPartDbs
                                                  .Where(x => x.LinkId == request.LinkId)
                                                  .ToListAsync();

            var toAdd = new List<SyncPartDb>();

            foreach (var clientEntry in dataFromClient)
            {
                var serverEntry = serverEntries.Find(x => x.Key == clientEntry.Key);
                if (serverEntry == null)
                {
                    serverEntry = new SyncPartDb
                    {
                        LinkId = request.LinkId,
                        Key = clientEntry.Key
                    };

                    toAdd.Add(serverEntry);
                }

                serverEntry.Timestamp = utcNow;
                serverEntry.Hash = clientEntry.Hash;
                serverEntry.JsonData = clientEntry.JsonData;
                serverEntry.ClientDbVersion = request.DbVersion;

                responseItems.Add(new UploadResponseItem()
                {
                    Key = clientEntry.Key,
                    Timestamp = utcNow
                });
            }

            if (toAdd.Count != 0)
            {
                await DbConnection.SyncPartDbs.AddRangeAsync(toAdd);
            }

            await DbConnection.SaveChangesAsync();

            var result = new UploadResponse()
            {
                Items = responseItems.ToArray()
            };

            return Ok(result);
        }
        catch (Exception e)
        {
            LogService.LogError(e, "Upload()");

            return SystemError(e);
        }
    }

    /// <summary>
    /// отправляем данные в МП
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Download([FromBody] DownloadRequest request)
    {
        var dbUser = await GetUserFromRequestAsync();
        if (dbUser == null)
        {
            return ClientError("4446");
        }

        var responseItems = new List<DownloadResponseItem>();

        var keysForDownloading = request.Keys ?? Array.Empty<string>();

        var serverEntries = await DbConnection.SyncPartDbs
                                              .Where(x => x.LinkId == request.LinkId)
                                              .ToDictionaryAsync(x => x.Key, y => y);

        foreach (var key in keysForDownloading)
        {
            if (serverEntries.TryGetValue(key, out var serverEntry))
            {
                responseItems.Add(new DownloadResponseItem()
                {
                    Key = serverEntry.Key,
                    JsonData = serverEntry.JsonData,
                    Timestamp = serverEntry.Timestamp,
                    Hash = serverEntry.Hash,
                    DbVersion = serverEntry.ClientDbVersion
                });
            }
        }

        var result = new DownloadResponse()
        {
            Items = responseItems.ToArray()
        };

        return Ok(result);
    }

    /// <summary>
    /// очистка данных при принудительной загрузке на сервер
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Clear([FromBody] ClearRequest request)
    {
        var dbUser = await GetUserFromRequestAsync();
        if (dbUser == null)
        {
            return ClientError("4446");
        }

        try
        {
            var serverData = await DbConnection.SyncPartDbs
                                               .Where(x => x.LinkId == request.LinkId)
                                               .ToArrayAsync();

            DbConnection.SyncPartDbs.RemoveRange(serverData);

            await DbConnection.SaveChangesAsync();

            return Ok();
        }
        catch (Exception e)
        {
            LogService.LogError(e, "Clear()");

            return SystemError(e);
        }
    }

    #region Private Methods

    private static void CheckFromClientToServer(List<SyncPartDb> dataFromServer,
                                                CheckRequestItem clientEntry,
                                                HashSet<string> similar,
                                                HashSet<string> onlyOnServer,
                                                HashSet<string> onlyOnClient)
    {
        var serverEntry = dataFromServer.Find(x => x.Key == clientEntry.Key);
        if (serverEntry != null)
        {
            //если хеши одинаковые, то данные актуальные
            if (serverEntry.Hash == clientEntry.Hash)
            {
                similar.Add(clientEntry.Key);
            }
            else if (serverEntry.Timestamp > clientEntry.Timestamp)
            {
                //если серверная метка больше клиентской, то данные нужно скачать на мп
                onlyOnServer.Add(clientEntry.Key);
            }
            else if (serverEntry.Timestamp < clientEntry.Timestamp)
            {
                //если клиентская метка больше серверной, то данные нужно загрузить на сервер
                onlyOnClient.Add(clientEntry.Key);
            }
        }
        else
        {
            //если на сервере нет такой пачки, то загружаем на сервер
            onlyOnClient.Add(clientEntry.Key);
        }
    }

    private static void CheckFromServerToClient(List<CheckRequestItem> dataFromClient,
                                                SyncPartDb serverEntry,
                                                HashSet<string> similar,
                                                HashSet<string> onlyOnServer,
                                                HashSet<string> onlyOnClient)
    {
        var clientEntry = dataFromClient.Find(x => x.Key == serverEntry.Key);
        if (clientEntry != null)
        {
            if (clientEntry.Hash == serverEntry.Hash)
            {
                //данные идентичны
                similar.Add(serverEntry.Key);
            }
            else if (clientEntry.Timestamp > serverEntry.Timestamp)
            {
                //если клиентская метка больше серверной, то данные нужно загрузить на сервер
                onlyOnClient.Add(serverEntry.Key);
            }
            else if (clientEntry.Timestamp < serverEntry.Timestamp)
            {
                //если серверная метка больше клиентской, то данные нужно скачать на мп
                onlyOnServer.Add(serverEntry.Key);
            }
        }
        else
        {
            //в мп нет пачки данных, поэтому скачиваем ее
            onlyOnServer.Add(serverEntry.Key);
        }
    }

    #endregion
}