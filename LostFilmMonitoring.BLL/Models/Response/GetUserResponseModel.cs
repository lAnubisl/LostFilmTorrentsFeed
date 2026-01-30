namespace LostFilmMonitoring.BLL.Models.Response;

/// <summary>
/// Model for user edit screen.
/// </summary>
public class GetUserResponseModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserResponseModel"/> class.
    /// </summary>
    /// <param name="user">Instance of <see cref="User"/>.</param>
    internal GetUserResponseModel(User user)
    {
        this.TrackerId = user.TrackerId;
        this.ValidationResult = ValidationResult.Ok;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserResponseModel"/> class.
    /// </summary>
    /// <param name="validationResult">Instance of <see cref="ValidationResult"/>.</param>
    internal GetUserResponseModel(ValidationResult validationResult)
    {
        this.TrackerId = null;
        this.ValidationResult = validationResult;
    }

    /// <summary>
    /// Gets unique user identity from the torrent tracker side.
    /// </summary>
    public string? TrackerId { get; }

    /// <summary>
    /// Gets an instance of <see cref="ValidationResult"/> representing validation result.
    /// </summary>
    public ValidationResult ValidationResult { get; }
}
