// <copyright file="ICommand.cs" company="Alexander Panfilenok">
// MIT License
// Copyright (c) 2021 Alexander Panfilenok
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

namespace LostFilmMonitoring.BLL.Commands
{
    /// <summary>
    /// Command definition that returns value.
    /// </summary>
    /// <typeparam name="TRequestModel">Type of RequestModel.</typeparam>
    /// <typeparam name="TResponseModel">Type of ResponseMode.</typeparam>
    public interface ICommand<TRequestModel, TResponseModel>
        where TRequestModel : class
    {
        /// <summary>
        /// Execute command that accepts request model and returns response model.
        /// </summary>
        /// <param name="request">Instance of TRequestModel.</param>
        /// <returns>A <see cref="Task{TResponseModel}"/> representing the result of the asynchronous operation.</returns>
        Task<TResponseModel> ExecuteAsync(TRequestModel? request);
    }

    /// <summary>
    /// Command definition that does not return any value.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Execute general command that does not return any value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task ExecuteAsync();
    }
}
