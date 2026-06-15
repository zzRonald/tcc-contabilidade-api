namespace TCC.Contabilidade.Application.Interfaces;

public interface IStorageService
{
    Task<string> SaveAsync(Stream fileStream, string fileName);
    Task DeleteAsync(string filePath);
}
