// <copyright file="IModelPersister.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.BLL.Interfaces;

/// <summary>
/// Responsible for persisting ViewModels for static website.
/// </summary>
public interface IModelPersister
{
    /// <summary>
    /// Persists model in persistent storage.
    /// </summary>
    /// <typeparam name="T">Type of model to persist.</typeparam>
    /// <param name="modelName">Name of the ViewModel.</param>
    /// <param name="model">Model to persist.</param>
    /// <returns>Awaitable task.</returns>
    Task PersistAsync<T>(string modelName, T model);

    /// <summary>
    /// Load model from the storage.
    /// </summary>
    /// <typeparam name="T">Type of model to load.</typeparam>
    /// <param name="modelName">Name of the ViewModel.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    Task<T?> LoadAsync<T>(string modelName)
        where T : class;
}
