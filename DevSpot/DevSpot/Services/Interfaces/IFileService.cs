namespace DevSpot.Services.Interfaces;

public interface IFileService
{
    Task<string> UploadCvAsync(IFormFile file);
    Task<byte[]> DownloadCvAsync(string filePath);
    void DeleteCv(string filePath);
}