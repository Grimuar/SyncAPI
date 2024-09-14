using DreamsSyncronizer.Models.Db;
using DreamsSyncronizer.Models.Db.Auth;
using DreamsSyncronizer.Models.Db.Sync;
using Microsoft.EntityFrameworkCore;

namespace DreamsSyncronizer;

public sealed class SleepTrackerDbContext : DbContext
{
    public DbSet<UserDb> Users { get; set; }
    public DbSet<UserDeviceDb> UserDeviceDbs { get; set; }
    public DbSet<SyncPartDb> SyncPartDbs { get; set; }
    public DbSet<VersionApp> VersionApps { get; set; }

    public SleepTrackerDbContext(DbContextOptions<SleepTrackerDbContext> options)
        : base(options)
    {
        //Database.EnsureCreated();
    }
}