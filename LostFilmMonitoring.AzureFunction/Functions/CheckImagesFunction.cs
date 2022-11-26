// <copyright file="CheckImagesFunction.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.AzureFunction.Functions
{
    /// <summary>
    /// Responsible for updating RSS feeds.
    /// </summary>
    public class CheckImagesFunction
    {
        private readonly ILogger logger;
        private readonly DownloadCoverImagesCommand command;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckImagesFunction"/> class.
        /// </summary>
        /// <param name="logger">Instance of <see cref="ILogger"/>.</param>
        /// <param name="command">Instance of <see cref="UpdateFeedsCommand"/>.</param>
        public CheckImagesFunction(ILogger logger, DownloadCoverImagesCommand command)
        {
            this.logger = logger?.CreateScope(nameof(UpdateRssFeedFunction)) ?? throw new ArgumentNullException(nameof(logger));
            this.command = command ?? throw new ArgumentNullException(nameof(command));
        }

        /// <summary>
        /// Azure Function Entry Point.
        /// </summary>
        /// <param name="myTimer">Timer object.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Function("CheckImagesFunction")]
        public async Task RunAsync([TimerTrigger("0 */30 * * * *")] object myTimer)
        {
            this.logger.Info($"Start {DateTime.Now}");
            await this.command.ExecuteAsync();
            this.logger.Info($"Finish: {DateTime.Now}");
        }
    }
}
