namespace LostFilmTV.Client.RssFeed;

/// <summary>
/// Represents base rss feed.
/// </summary>
public abstract class BaseRssFeed
{
    private readonly ActivitySource activitySource = new (ActivitySourceNames.RssFeed);
    private readonly IHttpClientFactory httpClientFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseRssFeed"/> class.
    /// </summary>
    /// <param name="logger">Logger.</param>
    /// <param name="httpClientFactory">IHttpClientFactory.</param>
    protected BaseRssFeed(ILogger logger, IHttpClientFactory httpClientFactory)
    {
        this.Logger = logger;
        this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    /// <summary>
    /// Gets Logger.
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Downloads content by URL.
    /// </summary>
    /// <param name="rssUri">URL.</param>
    /// <returns>Content.</returns>
    protected async Task<string> DownloadRssTextAsync(string rssUri)
    {
        return await this.DownloadRssTextAsync(rssUri, null);
    }

    /// <summary>
    /// Downloads content by URL.
    /// </summary>
    /// <param name="rssUri">URL.</param>
    /// <param name="requestHeaders">Additional headers to add for the request to external service.</param>
    /// <returns>Content.</returns>
    protected async Task<string> DownloadRssTextAsync(string rssUri, Dictionary<string, string>? requestHeaders)
    {
        this.Logger.Info($"Call: {nameof(this.DownloadRssTextAsync)}({rssUri})");
        using var client = this.httpClientFactory.CreateClient();
        var message = new HttpRequestMessage(HttpMethod.Get, rssUri);
        if (requestHeaders != null)
        {
            foreach (var header in requestHeaders)
            {
                message.Headers.Add(header.Key, header.Value);
            }
        }

        try
        {
            HttpResponseMessage? responseMessage = null;
            using (Activity? activity = this.activitySource.StartActivity("DownloadRssTextAsync"))
            {
                activity?.SetTag("url", rssUri);
                activity?.SetTag("method", message.Method.ToString());
                activity?.SetTag("Type", "Lostfilm");
                responseMessage = await client.SendAsync(message).ConfigureAwait(false);
            }

            var rssText = await responseMessage.Content.ReadAsStringAsync();
            this.Logger.Debug(rssText);
            return rssText;
        }
        catch (TaskCanceledException ex)
        {
            this.Logger.Log(ex);
            throw new RemoteServiceUnavailableException(ex);
        }
        catch (IOException ex)
        {
            this.Logger.Log(ex);
            throw new RemoteServiceUnavailableException(ex);
        }
        catch (Exception ex)
        {
            this.Logger.Log(ex);
            throw new RemoteServiceUnavailableException(ex);
        }
    }

    /// <summary>
    /// Reads Feed item objects from RSS content.
    /// </summary>
    /// <param name="rssText">RSS content.</param>
    /// <returns>Feed item objects.</returns>
    protected SortedSet<FeedItemResponse> GetItems(string rssText)
    {
        this.Logger.Info($"Call: {nameof(this.GetItems)}(rssText)");
        if (string.IsNullOrWhiteSpace(rssText))
        {
            this.Logger.Error("RSS content is empty.");
            return [];
        }

        XDocument document = null!;
        try
        {
            document = Parse(rssText);
        }
        catch (Exception ex)
        {
            this.Logger.Log($"Error parsing RSS data: {Environment.NewLine}'{rssText}'", ex);
            return [];
        }

        var list = new List<FeedItemResponse>();
        var channel = document.Root?.Descendants().First(i => i.Name.LocalName == "channel");
        if (channel != null)
        {
            var items = channel.Elements().Where(i => i.Name.LocalName == "item");
            foreach (var item in items)
            {
                if (ReteOrgFeedItemResponse.TryParseFromXElement(item, out var parsed, this.Logger.Error) && parsed != null)
                {
                    list.Add(parsed);
                }
            }
        }

        return new SortedSet<FeedItemResponse>(list);
    }

    /// <summary>
    /// Generages XDocument from input string.
    /// </summary>
    /// <param name="rssString">Input string.</param>
    /// <returns>XDocument.</returns>
    private static XDocument Parse(string rssString)
    {
        string normalizedRssText = NormalizeXmlContent(rssString);
        try
        {
            return XDocument.Parse(normalizedRssText);
        }
        catch
        {
            return XDocument.Parse(rssString);
        }
    }

    private static string NormalizeXmlContent(string rssString)
    {
        if (string.IsNullOrEmpty(rssString))
        {
            return rssString;
        }

        // Walk the RSS string manually and normalize text content in two places:
        // 1) quoted attribute values, and 2) element text nodes.
        //
        // This preserves the original XML structure while ensuring any raw text
        // content is HTML-encoded before parsing, which helps the parser recover
        // from malformed entity usage in real RSS feeds.
        var builder = new System.Text.StringBuilder(rssString.Length);
        int index = 0;

        while (index < rssString.Length)
        {
            char current = rssString[index];
            if (current == '"')
            {
                index = ProcessQuotedAttribute(rssString, index, builder);
            }
            else if (current == '>')
            {
                index = ProcessTextNodeContent(rssString, index, builder);
            }
            else
            {
                builder.Append(current);
                index++;
            }
        }

        return builder.ToString();
    }

    private static int ProcessQuotedAttribute(string rssString, int quoteIndex, System.Text.StringBuilder builder)
    {
        // Preserve the opening quote then normalize the attribute's raw value.
        builder.Append('"');
        int start = quoteIndex + 1;
        int end = rssString.IndexOf('"', start);
        if (end < 0)
        {
            AppendNormalizedContent(builder, rssString[start..]);
            return rssString.Length;
        }

        AppendNormalizedContent(builder, rssString[start..end]);
        builder.Append('"');
        return end + 1;
    }

    private static int ProcessTextNodeContent(string rssString, int startBracketIndex, System.Text.StringBuilder builder)
    {
        // Preserve the closing bracket before normalizing the element text.
        builder.Append('>');
        int start = startBracketIndex + 1;
        int end = rssString.IndexOf('<', start);
        if (end < 0)
        {
            AppendNormalizedContent(builder, rssString[start..]);
            return rssString.Length;
        }

        AppendNormalizedContent(builder, rssString[start..end]);
        builder.Append('<');
        return end + 1;
    }

    private static void AppendNormalizedContent(System.Text.StringBuilder builder, string content)
    {
        // Decode any encoded entities first, then re-encode to valid XML.
        // This is the key recovery step for broken or raw entity text.
        builder.Append(HttpUtility.HtmlEncode(HttpUtility.HtmlDecode(content)));
    }
}
