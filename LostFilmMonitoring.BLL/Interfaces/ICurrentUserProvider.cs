namespace LostFilmMonitoring.BLL.Interfaces;

/// <summary>
/// Responsible for accessing current user Id.
/// </summary>
public interface ICurrentUserProvider
{
    /// <summary>
    /// Get current user id.
    /// </summary>
    /// <returns>UserID.</returns>
    Guid GetCurrentUserId();

    /// <summary>
    /// Set current user id.
    /// </summary>
    /// <param name="userId">UserId.</param>
    void SetCurrentUserId(Guid userId);
}
