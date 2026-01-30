namespace LostFilmMonitoring.BLL.Models.Request;

/// <summary>
/// Represents user data.
/// </summary>
public class EditUserRequestModel
{
    /// <summary>
    /// Gets or sets the user Id.
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Gets or sets unique user identity from the torrent tracker side.
    /// </summary>
    public string? TrackerId { get; set; }
}
