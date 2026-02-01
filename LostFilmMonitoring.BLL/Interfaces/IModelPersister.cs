namespace LostFilmMonitoring.BLL.Interfaces;

/// <summary>
/// Responsible for persisting ViewModels for static website.
/// </summary>
public interface IModelPersister
{
    /// <summary>
    /// Persists model in persistent storage.
    /// </summary>
    /// <typeparam name="T">Type of model to persist.</typeparam>
    /// <param name="modelName">Name of the ViewModel.</param>
    /// <param name="model">Model to persist.</param>
    /// <returns>Awaitable task.</returns>
    Task PersistAsync<T>(string modelName, T model);

    /// <summary>
    /// Load model from the storage.
    /// </summary>
    /// <typeparam name="T">Type of model to load.</typeparam>
    /// <param name="modelName">Name of the ViewModel.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    Task<T?> LoadAsync<T>(string modelName)
        where T : class;
}
