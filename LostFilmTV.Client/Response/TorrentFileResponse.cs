// <copyright file="TorrentFileResponse.cs" company="Alexander Panfilenok">
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

namespace LostFilmTV.Client.Response;

/// <summary>
/// Represents torrent file with content.
/// </summary>
public class TorrentFileResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TorrentFileResponse"/> class.
    /// </summary>
    /// <param name="fileName">File Name.</param>
    /// <param name="content">Content stream.</param>
    internal TorrentFileResponse(string fileName, Stream content)
    {
        this.FileName = fileName;
        this.Content = content;
    }

    /// <summary>
    /// Gets File Name.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// Gets content stream.
    /// </summary>
    public Stream Content { get; }
}
