// <copyright file="Configuration.cs" company="Alexander Panfilenok">
// MIT License
// Copyright (c) 2023 Alexander Panfilenok
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the 'Software'), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// </copyright>

namespace LostFilmMonitoring.BLL;

/// <inheritdoc/>
public class LostFilmMonoitoringBllConfiguration : IConfiguration
{
    private readonly string[] torrentAnnounceListPatterns;

    /// <summary>
    /// Initializes a new instance of the <see cref="LostFilmMonoitoringBllConfiguration"/> class.
    /// </summary>
    /// <param name="provider">Instance of <see cref="IConfigurationValuesProvider"/>.</param>
    public LostFilmMonoitoringBllConfiguration(IConfigurationValuesProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);
        this.BaseUrl = provider.GetValue("BASEURL") ?? throw new ArgumentException("Environment variable 'BASEURL' is not defined.");
        this.BaseUSESS = provider.GetValue("BASEFEEDCOOKIE") ?? throw new ArgumentException("Environment variable 'BASEFEEDCOOKIE' is not defined.");
        this.BaseUID = provider.GetValue("BASELINKUID") ?? throw new ArgumentException("Environment variable 'BASELINKUID' is not defined.");
        this.torrentAnnounceListPatterns = provider.GetValue("TORRENTTRACKERS")?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? throw new ArgumentException("Environment variable 'TORRENTTRACKERS' is not defined.");
        this.ImagesDirectory = provider.GetValue("IMAGESDIRECTORY") ?? "images";
        this.TorrentsDirectory = provider.GetValue("TORRENTSDIRECTORY") ?? "torrentfiles";
    }

    /// <inheritdoc/>
    public string BaseUSESS { get; private set; }

    /// <inheritdoc/>
    public string ImagesDirectory { get; private set; }

    /// <inheritdoc/>
    public string TorrentsDirectory { get; private set; }

    /// <inheritdoc/>
    public string BaseUrl { get; private set; }

    /// <inheritdoc/>
    public string BaseUID { get; private set; }

    /// <inheritdoc/>
    public string[] GetTorrentAnnounceList(string linkUid)
    {
        return this.torrentAnnounceListPatterns
            .Select(p => string.Format(CultureInfo.InvariantCulture, p, linkUid ?? this.BaseUID))
            .Select(s => s)
            .ToArray();
    }
}
