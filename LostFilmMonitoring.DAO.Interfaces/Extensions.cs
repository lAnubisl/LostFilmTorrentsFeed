namespace LostFilmMonitoring.DAO.Interfaces;

/// <summary>
/// Extension methods.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Generate XML document from array or FeedItems.
    /// </summary>
    /// <param name="items">FeedItem[].</param>
    /// <returns>XML.</returns>
    public static string GenerateXml(this FeedItem[] items)
    {
        var rss = new XDocument();
        var channel = new XElement("channel");
        rss.Add(new XElement("rss", new XAttribute("version", "2.0"), channel));
        foreach (var item in items)
        {
            channel.Add(item.ToXElement());
        }

        return rss.ToString();
    }

    /// <summary>
    /// Parse <see cref="XDocument"/>.
    /// </summary>
    /// <param name="doc">XDocument to parse.</param>
    /// <returns>SortedSet.</returns>
    public static SortedSet<FeedItem> GetItems(this XDocument doc)
    {
        if (doc.Root == null)
        {
            return new SortedSet<FeedItem>();
        }

        var entries = from item in doc.Root.Descendants()
                      .First(i => i.Name.LocalName == "channel")
                      .Elements()
                      .Where(i => i.Name.LocalName == "item")
                      select new FeedItem(item);
        return new SortedSet<FeedItem>(entries);
    }
}
