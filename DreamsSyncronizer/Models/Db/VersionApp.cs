using System.ComponentModel.DataAnnotations;

namespace DreamsSyncronizer.Models.Db;

public class VersionApp
{
    [Key]
    public int DbId { get; set; }
    
    public string Store { get; set; }
    
    public string Version { get; set; }
}