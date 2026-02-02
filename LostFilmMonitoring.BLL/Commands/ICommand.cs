namespace LostFilmMonitoring.BLL.Commands;

/// <summary>
/// Command definition that returns value.
/// </summary>
/// <typeparam name="TRequestModel">Type of RequestModel.</typeparam>
/// <typeparam name="TResponseModel">Type of ResponseMode.</typeparam>
public interface ICommand<TRequestModel, TResponseModel>
    where TRequestModel : class
{
    /// <summary>
    /// Execute command that accepts request model and returns response model.
    /// </summary>
    /// <param name="request">Instance of TRequestModel.</param>
    /// <returns>A <see cref="Task{TResponseModel}"/> representing the result of the asynchronous operation.</returns>
    Task<TResponseModel> ExecuteAsync(TRequestModel? request);
}

/// <summary>
/// Command definition that returns no value.
/// </summary>
/// <typeparam name="TRequestModel">Type of RequestModel.</typeparam>
public interface ICommand<in TRequestModel>
    where TRequestModel : class
{
    /// <summary>
    /// Execute command that accepts request model and returns response model.
    /// </summary>
    /// <param name="request">Instance of TRequestModel.</param>
    /// <returns>A <see cref="Task{TResponseModel}"/> representing the result of the asynchronous operation.</returns>
    Task ExecuteAsync(TRequestModel? request);
}

/// <summary>
/// Command definition that does not return any value.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Execute general command that does not return any value.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    Task ExecuteAsync();
}
