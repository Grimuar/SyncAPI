using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DreamsSyncronizer.Models.Db.Auth;

[Index(nameof(Login))]
public class UserDb
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int DbId { get; set; }

    [Required]
    [MaxLength(30)]
    public string Login { get; set; }

    [Required]
    [MaxLength(150)]
    public string PasswordHash { get; set; }

    public string LinkId { get; set; }

    public List<UserDeviceDb> DeviceDbs { get; set; } = new();
}