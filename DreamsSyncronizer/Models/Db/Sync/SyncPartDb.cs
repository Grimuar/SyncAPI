using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DreamsSyncronizer.Models.Db.Sync;

[Index(nameof(LinkId), nameof(Key))]
public sealed class SyncPartDb
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int DbId { get; set; }

    public string LinkId { get; set; }
    public string Key { get; set; }
    public string JsonData { get; set; }
    public string Hash { get; set; }
    public int ClientDbVersion { get; set; }
    public DateTime Timestamp { get; set; }
}