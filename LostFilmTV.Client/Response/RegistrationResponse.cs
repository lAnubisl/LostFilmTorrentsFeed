// <copyright file="RegistrationResponse.cs" company="Alexander Panfilenok">
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

namespace LostFilmTV.Client.Response
{
    using Newtonsoft.Json;

    /// <summary>
    /// Object which contains registration request execution results.
    /// </summary>
    public class RegistrationResponse
    {
        /// <summary>
        /// Gets or sets Error.
        /// </summary>
        [JsonProperty("error")]
        public int Error { get; set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets UserId.
        /// </summary>
        [JsonProperty("uid")]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether request was successful.
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets Result.
        /// </summary>
        [JsonProperty("result")]
        public string Result { get; set; }

        /// <summary>
        /// Gets or sets Cookie.
        /// </summary>
        public string Cookie { get; set; }
    }
}
