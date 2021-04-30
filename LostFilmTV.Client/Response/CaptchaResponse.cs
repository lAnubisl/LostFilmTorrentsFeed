// <copyright file="CaptchaResponse.cs" company="Alexander Panfilenok">
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
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// CaptchaResponse.
    /// </summary>
    public class CaptchaResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CaptchaResponse"/> class.
        /// </summary>
        /// <param name="cookie">Cookie.</param>
        /// <param name="captchaBytes">CaptchaBytes.</param>
        private CaptchaResponse(string cookie, byte[] captchaBytes)
        {
            this.CaptchaBytes = captchaBytes;
            this.Cookie = cookie;
        }

        /// <summary>
        /// Gets CaptchaBytes.
        /// </summary>
        public byte[] CaptchaBytes { get; }

        /// <summary>
        /// Gets Cookie.
        /// </summary>
        public string Cookie { get; }

        /// <summary>
        /// Creates a ne instance of CaptchaResponse.
        /// </summary>
        /// <param name="response">HttpResponseMessage.</param>
        /// <returns>CaptchaResponse.</returns>
        internal static async Task<CaptchaResponse> Build(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsByteArrayAsync();
            var setCookieHeader = response.Headers.Where(h => h.Key == "Set-Cookie").Select(h => h.Value).FirstOrDefault()?.First();
            if (setCookieHeader == null)
            {
                throw new LostFilmException("GetNewCaptcha: Cannot get cookie.", response);
            }

            var cookieKey = setCookieHeader.Substring(setCookieHeader.IndexOf("=") + 1);
            cookieKey = cookieKey.Substring(0, cookieKey.IndexOf(";"));
            return new CaptchaResponse(cookieKey, content);
        }
    }
}
