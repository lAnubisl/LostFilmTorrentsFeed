// <copyright file="UserTableEntity.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.DAO.Azure;

/// <summary>
/// Describes user in Azure Table Storage.
/// </summary>
public class UserTableEntity : ITableEntity
{
    /// <inheritdoc/>
    public string PartitionKey { get; set; } = null!;

    /// <inheritdoc/>
    public string RowKey { get; set; } = null!;

    /// <inheritdoc/>
    public DateTimeOffset? Timestamp { get; set; }

    /// <inheritdoc/>
    public ETag ETag { get; set; }

    /// <summary>
    /// Gets or sets user identifier which should be in link of torrent file announces.
    /// <![CDATA[http://bt.tracktor.in/tracker.php/1b07a52cb12a12945e15cca756f83789/announce
    ///          http://bt99.tracktor.in/tracker.php/1b07a52cb12a12945e15cca756f83789/announce
    ///          http://bt0.tracktor.in/tracker.php/1b07a52cb12a12945e15cca756f83789/announce
    ///          http://user5.newtrack.info/tracker.php/1b07a52cb12a12945e15cca756f83789/announce
    ///          http://user1.newtrack.info/tracker.php/1b07a52cb12a12945e15cca756f83789/announce]]>
    /// Here '1b07a52cb12a12945e15cca756f83789' is the user id.
    /// </summary>
    public string TrackerId { get; set; } = null!;
}