namespace LostFilmMonitoring.DAO.Interfaces;

/// <summary>
/// Provides functionality for managing subscription in storage.
/// </summary>
public interface ISubscriptionDao
{
    /// <summary>
    /// Load subscriptions by series name and quality.
    /// </summary>
    /// <param name="seriesName">SeriesName.</param>
    /// <param name="quality">Quality.</param>
    /// <returns>Subscriptions.</returns>
    Task<string[]> LoadUsersIdsAsync(string seriesName, string quality);

    /// <summary>
    /// Save subscription.
    /// </summary>
    /// <param name="userId">UserId.</param>
    /// <param name="subscriptions">Subscriptions.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    Task SaveAsync(string userId, Subscription[] subscriptions);

    /// <summary>
    /// Load all subscriptions for a given user.
    /// </summary>
    /// <param name="userId">User Id.</param>
    /// <returns>Array of subscriptions that user has.</returns>
    Task<Subscription[]> LoadAsync(string userId);
}
