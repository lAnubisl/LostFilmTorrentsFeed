// <copyright file="ModelBinder.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.AzureFunction
{
    /// <summary>
    /// Responsible for binding <see cref="HttpRequestData"/> to model instance.
    /// </summary>
    internal class ModelBinder
    {
        /// <summary>
        /// Binds instance of <see cref="HttpRequestData"/> to requested model type.
        /// </summary>
        /// <typeparam name="T">Type of model to bind to.</typeparam>
        /// <param name="req">Instance of <see cref="HttpRequestData"/>.</param>
        /// <returns>Instance of T.</returns>
        internal static T? Bind<T>(HttpRequestData req)
            where T : class
        {
            try
            {
                using var reader = new StreamReader(req.Body);
                var json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                return null;
            }
        }
    }
}
