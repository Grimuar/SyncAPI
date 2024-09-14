using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace DreamsSyncronizer.Infrastructure.Helpers;

public static class FileSystemHelper
{
    public static void CreateFolder(string folderPath)
    {
        try
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }
        catch (Exception e)
        {
            //
        }
    }

    public static void DeleteFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch (Exception e)
        {
            //
        }
    }
}