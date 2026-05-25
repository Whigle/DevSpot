using DevSpot.Services.Interfaces;

namespace DevSpot.Services;

public class LocalFileService : IFileService
{
    private const int MAX_FILE_SIZE = 5 * 1024 * 1024;
    private const string FILE_EXT = ".pdf";

    private readonly string _saveFolder;
    private readonly IWebHostEnvironment _env;

    public LocalFileService(IWebHostEnvironment env)
    {
        _env = env;
        _saveFolder = Path.Combine(_env.WebRootPath, "uploads", "cv");
    }

    public async Task<string> UploadCvAsync(IFormFile file)
    {
        if (file.Length > MAX_FILE_SIZE)
        {
            throw new ArgumentException("File too large");
        }

        if (Path.GetExtension(file.FileName) != FILE_EXT)
        {
            throw new ArgumentException("File extension not supported");
        }

        if (!file.ContentType.Equals("application/pdf", StringComparison.CurrentCultureIgnoreCase))
        {
            throw new ArgumentException("File type not supported");
        }

        if (!Directory.Exists(_saveFolder))
        {
            Directory.CreateDirectory(_saveFolder);
        }

        var guid = Guid.NewGuid().ToString();
        var uniqueFileName = guid + FILE_EXT;
        var filePath = Path.Combine(_saveFolder, uniqueFileName);

        await using var fs = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(fs);

        return filePath;
    }

    public async Task<byte[]> DownloadCvAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return Array.Empty<byte>();
        }

        return await File.ReadAllBytesAsync(filePath);
    }

    public void DeleteCv(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return;
        }

        File.Delete(filePath);
    }
}