namespace LostFilmMonitoring.BLL.Models.Response;

/// <summary>
/// Describes the response for user's request to.
/// </summary>
public class EditSubscriptionResponseModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EditSubscriptionResponseModel"/> class.
    /// </summary>
    /// <param name="validationResult">Instance of <see cref="ValidationResult"/>.</param>
    internal EditSubscriptionResponseModel(ValidationResult validationResult)
    {
        this.ValidationResult = validationResult;
    }

    /// <summary>
    /// Gets instance of <see cref="ValidationResult"/> representing the result of validation process.
    /// </summary>
    public ValidationResult ValidationResult { get; }
}
