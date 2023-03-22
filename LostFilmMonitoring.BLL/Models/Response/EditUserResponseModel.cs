// <copyright file="EditUserResponseModel.cs" company="Alexander Panfilenok">
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
/// Represents the response for Edit User operation.
/// </summary>
public class EditUserResponseModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EditUserResponseModel"/> class.
    /// </summary>
    /// <param name="validationResult">Instance of <see cref="ValidationResult"/>.</param>
    internal EditUserResponseModel(ValidationResult validationResult)
    {
        this.UserId = null;
        this.ValidationResult = validationResult ?? throw new ArgumentNullException(nameof(validationResult));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditUserResponseModel"/> class.
    /// </summary>
    /// <param name="userId">User Id.</param>
    internal EditUserResponseModel(string userId)
    {
        this.UserId = userId;
        this.ValidationResult = ValidationResult.Ok;
    }

    /// <summary>
    /// Gets an instance of <see cref="ValidationResult"/> representing validation result.
    /// </summary>
    public ValidationResult ValidationResult { get; }

    /// <summary>
    /// Gets user Id.
    /// </summary>
    public string? UserId { get; }
}
