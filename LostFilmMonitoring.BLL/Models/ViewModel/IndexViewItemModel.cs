namespace LostFilmMonitoring.BLL.Models.ViewModel;

/// <summary>
/// Represents information to be shown in home screen.
/// </summary>
public class IndexViewItemModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IndexViewItemModel"/> class.
    /// </summary>
    /// <param name="series">Series.</param>
    public IndexViewItemModel(Series series)
    {
        this.Name = series.Name;
        this.ImageFileName = series.Id + ".jpg";
        this.Id = series.Id.ToString();
    }

    /// <summary>
    /// Gets or sets item name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets image file name.
    /// </summary>
    public string ImageFileName { get; set; }

    /// <summary>
    /// Gets or sets item id.
    /// </summary>
    public string Id { get; set; }
}