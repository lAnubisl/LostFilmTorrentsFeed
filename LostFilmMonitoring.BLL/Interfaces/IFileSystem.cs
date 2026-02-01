namespace LostFilmMonitoring.BLL.Interfaces;

/// <summary>
/// Abstraction over file storage.
/// </summary>
public interface IFileSystem
{
    /// <summary>
    /// Check if file exists in the file system.
    /// </summary>
    /// <param name="directory">Directory to check file.</param>
    /// <param name="fileName">File name to check.</param>
    /// <returns>True - file exists. False - file does not exist.</returns>
    Task<bool> ExistsAsync(string directory, string fileName);

    /// <summary>
    /// Save file to the file system.
    /// </summary>
    /// <param name="directory">Directory to save file.</param>
    /// <param name="fileName">File name.</param>
    /// <param name="contentType">Content-Type property of the file.</param>
    /// <param name="contentStream">File content.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    Task SaveAsync(string directory, string fileName, string contentType, Stream contentStream);
}
