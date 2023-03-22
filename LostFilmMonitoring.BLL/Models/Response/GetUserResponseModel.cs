// <copyright file="GetUserResponseModel.cs" company="Alexander Panfilenok">
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
