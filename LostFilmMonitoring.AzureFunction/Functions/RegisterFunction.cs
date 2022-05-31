// <copyright file="RegisterFunction.cs" company="Alexander Panfilenok">
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
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using LostFilmMonitoring.BLL.Commands;
    using LostFilmMonitoring.BLL.Models.Request;
    using LostFilmMonitoring.BLL.Models.Response;
    using LostFilmMonitoring.Common;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Azure.Functions.Worker.Http;
    using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
    using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
    using Newtonsoft.Json;

    /// <summary>
    /// Responsible for registering user.
    /// </summary>
    public class RegisterFunction
    {
        private readonly ILogger logger;
        private readonly ICommand<EditUserRequestModel, EditUserResponseModel> command;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterFunction"/> class.
        /// </summary>
        /// <param name="logger">Instance of <see cref="ILogger"/>.</param>
        /// <param name="command">Instance of <see cref="ICommand{EditUserRequestModel, EditUserResponseModel}"/>.</param>
        public RegisterFunction(ILogger logger, ICommand<EditUserRequestModel, EditUserResponseModel> command)
        {
            this.logger = logger?.CreateScope(nameof(RegisterFunction)) ?? throw new ArgumentNullException(nameof(logger));
            this.command = command ?? throw new ArgumentNullException(nameof(command));
        }

        /// <summary>
        /// Azure Function Entry Point.
        /// </summary>
        /// <param name="req">Instance of <see cref="HttpRequestData"/>.</param>
        /// <returns>A <see cref="Task{HttpResponseData}"/> representing the result of the asynchronous operation.</returns>
        [Function("RegisterFunction")]
        [OpenApiOperation(operationId: "RegisterFunction", tags: new[] { "user" }, Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(EditUserRequestModel), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(EditUserResponseModel))]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            this.logger.Info($"Call: {nameof(this.RunAsync)}(HttpRequestData)");
            var responseModel = await this.command.ExecuteAsync(ModelBinder.Bind<EditUserRequestModel>(req));
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(JsonConvert.SerializeObject(responseModel));
            return response;
        }
    }
}
