public class FileWatcherService : BackgroundService
{
    private readonly string directoryPath = @"C:\Users\User\Desktop\IMS\Input";
    private readonly string fileToWatch = "*.txt";
    private readonly IServiceProvider _serviceProvider;
    private readonly DataParserService _dataParserService;
    private readonly string parsedDirectoryPath = @"C:\Users\User\Desktop\IMS\Parsed";
    private static readonly TimeSpan FileAgeThreshold = TimeSpan.FromSeconds(5); // Adjust as needed


    public FileWatcherService(IServiceProvider serviceProvider, DataParserService dataParserService)
    {
        _serviceProvider = serviceProvider;
        _dataParserService = dataParserService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Check for the existence of files in the directory
            string[] files = Directory.GetFiles(directoryPath, fileToWatch);

            foreach (string filePath in files)
            {
                if (IsFileDuplicate(filePath))
                {
                    // Delete the duplicate file
                    try
                    {
                        File.Delete(filePath);
                        Console.WriteLine($"Duplicate file deleted: {filePath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting the duplicate file: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"File found: {filePath}");
                    // Call the non-static method in DataParserService to process the file
                    _dataParserService.ProcessFile(filePath);
                }
            }

            await Task.Delay(1000, stoppingToken); // Delay between checks
        }
    }
    private bool IsFileDuplicate(string filePath)
    {
        // Check if the file exists in the parsed directory
        string parsedFilePath = Path.Combine(parsedDirectoryPath, Path.GetFileName(filePath));
        return File.Exists(parsedFilePath);
    }
    private bool IsFileBeingWritten(string filePath)
    {
        // Check if the file was written to in the last few seconds
        var fileInfo = new FileInfo(filePath);
        var fileAge = DateTime.Now - fileInfo.LastWriteTime;
        return fileAge < FileAgeThreshold;
    }
}

