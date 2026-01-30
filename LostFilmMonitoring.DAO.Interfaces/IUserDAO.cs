namespace LostFilmMonitoring.DAO.Interfaces;

using LostFilmMonitoring.DAO.Interfaces.DomainModels;

/// <summary>
/// Provides functionality for managing users in storage.
/// </summary>
public interface IUserDao
{
    /// <summary>
    /// Load user.
    /// </summary>
    /// <param name="userId">UserId.</param>
    /// <returns>User.</returns>
    Task<User?> LoadAsync(string userId);

    /// <summary>
    /// Load users.
    /// </summary>
    /// <returns>All users.</returns>
    Task<User[]> LoadAsync();

    /// <summary>
    /// Create new user.
    /// </summary>
    /// <param name="user">User to create.</param>
    /// <returns>New user GUID.</returns>
    Task SaveAsync(User user);

    /// <summary>
    /// Update user.
    /// </summary>
    /// <param name="userId">User Id to update.</param>
    /// <returns>Task.</returns>
    Task DeleteAsync(string userId);
}
