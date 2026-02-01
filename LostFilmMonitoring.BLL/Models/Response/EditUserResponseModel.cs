namespace LostFilmMonitoring.BLL.Models.Response;

/// <summary>
/// Represents the response for Edit User operation.
/// </summary>
public class EditUserResponseModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EditUserResponseModel"/> class.
    /// </summary>
    /// <param name="validationResult">Instance of <see cref="ValidationResult"/>.</param>
    internal EditUserResponseModel(ValidationResult validationResult)
    {
        this.UserId = null;
        this.ValidationResult = validationResult ?? throw new ArgumentNullException(nameof(validationResult));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditUserResponseModel"/> class.
    /// </summary>
    /// <param name="userId">User Id.</param>
    internal EditUserResponseModel(string userId)
    {
        this.UserId = userId;
        this.ValidationResult = ValidationResult.Ok;
    }

    /// <summary>
    /// Gets an instance of <see cref="ValidationResult"/> representing validation result.
    /// </summary>
    public ValidationResult ValidationResult { get; }

    /// <summary>
    /// Gets user Id.
    /// </summary>
    public string? UserId { get; }
}
