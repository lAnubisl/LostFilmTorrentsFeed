namespace LostFilmMonitoring.BLL.Models.Request;

/// <summary>
/// Defines user selected subscriptions.
/// </summary>
public class EditSubscriptionRequestModel
{
    /// <summary>
    /// Gets or Sets User Id.
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Gets or sets Selected Items.
    /// </summary>
#pragma warning disable SA1011 // Closing square brackets should be spaced correctly
    public SubscriptionItem[]? Items { get; set; }
#pragma warning restore SA1011 // Closing square brackets should be spaced correctly
}
