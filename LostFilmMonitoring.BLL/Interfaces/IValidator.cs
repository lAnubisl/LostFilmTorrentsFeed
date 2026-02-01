namespace LostFilmMonitoring.BLL.Interfaces;

/// <summary>
/// Common interface for all validators.
/// </summary>
/// <typeparam name="TRequestModel">Type of a model to validate.</typeparam>
public interface IValidator<TRequestModel>
{
    /// <summary>
    /// Validates model.
    /// </summary>
    /// <param name="model">Instance of model to validate.</param>
    /// <returns>A <see cref="Task{TReValidationResultsult}"/> representing the result of the asynchronous operation.</returns>
    Task<ValidationResult> ValidateAsync(TRequestModel model);
}
