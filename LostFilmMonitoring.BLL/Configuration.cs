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
public class Configuration : IConfiguration
{
    private readonly string[] torrentAnnounceListPatterns;

    /// <summary>
    /// Initializes a new instance of the <see cref="Configuration"/> class.
    /// </summary>
    /// <param name="provider">Instance of <see cref="IConfigurationValuesProvider"/>.</param>
    public Configuration(IConfigurationValuesProvider provider)
    {
        this.BaseUrl = provider.GetValue(EnvironmentVariables.BaseUrl) ?? throw new Exception($"Environment variable '{EnvironmentVariables.BaseUrl}' is not defined.");
        this.BaseUSESS = provider.GetValue(EnvironmentVariables.BaseFeedCookie) ?? throw new Exception($"Environment variable '{EnvironmentVariables.BaseFeedCookie}' is not defined.");
        this.BaseUID = provider.GetValue(EnvironmentVariables.BaseLinkUID) ?? throw new Exception($"Environment variable '{EnvironmentVariables.BaseLinkUID}' is not defined.");
        this.torrentAnnounceListPatterns = provider.GetValue(EnvironmentVariables.TorrentTrackers)?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? throw new Exception($"Environment variable '{EnvironmentVariables.TorrentTrackers}' is not defined.");
    }

    /// <inheritdoc/>
    public string BaseUSESS { get; private set; }

    /// <inheritdoc/>
    public string BaseUrl { get; private set; }

    /// <inheritdoc/>
    public string BaseUID { get; private set; }

    /// <inheritdoc/>
    public string[] GetTorrentAnnounceList(string link_uid)
    {
        return this.torrentAnnounceListPatterns
            .Select(p => string.Format(p, link_uid ?? this.BaseUID))
            .Select(s => s)
            .ToArray();
    }
}
