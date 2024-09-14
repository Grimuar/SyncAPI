namespace DreamsSyncronizer;

public class DbInitializer
{
    public static void Initialize(SleepTrackerDbContext dbContext)
    {
        dbContext.Database.EnsureCreated();
        Console.WriteLine("DBCreated in DBInitializer___________________________");
    }
}