using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DreamsSyncronizer.Models.Db.Auth;

namespace DreamsSyncronizer.Models.Db;

public sealed class UserDeviceDb
{
    [Key]
    public int DbId { get; set; }
    
    /// <summary>
    /// DeviceTypeEnum
    /// </summary>
    public int DeviceType { get; set; }
    
    /// <summary>
    /// уникальный id устройства(использовать для поиска записи в этой
    /// таблице для удаления записи)
    /// </summary>
    public string DeviceId { get; set; }
    public string DeviceToken { get; set; }
    
    public int UserId { get; set; }
    
    [JsonIgnore]
    public UserDb UserDb { get; set; }
}