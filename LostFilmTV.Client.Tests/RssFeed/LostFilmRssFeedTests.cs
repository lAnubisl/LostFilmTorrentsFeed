// <copyright file="LostFilmRssFeedTests.cs" company="Alexander Panfilenok">
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

namespace LostFilmTV.Client.Tests.RssFeed
{
    [ExcludeFromCodeCoverage]
    public class LostFilmRssFeedTests
    {
        private Mock<IHttpClientFactory> httpControllerFactory;
        private Mock<ILogger> logger;
        private MockHttpMessageHandler mockHttp;

        [SetUp]
        public void Setup()
        {
            this.httpControllerFactory = new();
            this.logger = new();
            this.logger.Setup(l => l.CreateScope(It.IsAny<string>())).Returns(this.logger.Object);
            this.mockHttp = new();
            this.httpControllerFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient(mockHttp));
        }

        [Test]
        public void Constructor_should_throw_exception_when_logger_null()
        {
            var action = () => new LostFilmRssFeed(null!, this.httpControllerFactory.Object);
            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("logger");
        }

        [Test]
        public void Constructor_should_throw_exception_when_logger_createScope_null()
        {
            var action = () => new LostFilmRssFeed(new Mock<ILogger>().Object, this.httpControllerFactory.Object);
            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("logger");
        }

        [Test]
        public void Constructor_should_throw_exception_when_httpClientFactory_null()
        {
            var action = () => new LostFilmRssFeed(this.logger.Object, null!);
            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("httpClientFactory");
        }

        [Test]
        public async Task LoadFeedItemsAsync_should_return_items()
        {
            mockHttp
                .When(HttpMethod.Get, "https://www.lostfilm.tv/rss.xml")
                .Respond("application/xml", Helper.GetEmbeddedResource($"LostFilmTV.Client.Tests.TestData.LostFilmFeed1.xml"));
            var result = await GetService().LoadFeedItemsAsync();
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }

        [Test]
        public async Task LoadFeedItemsAsync_should_return_empty_when_content_is_empty()
        {
            mockHttp
                .When(HttpMethod.Get, "https://www.lostfilm.tv/rss.xml")
                .Respond("application/xml", string.Empty);
            var result = await GetService().LoadFeedItemsAsync();
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        private LostFilmRssFeed GetService() => new(this.logger.Object, this.httpControllerFactory.Object);
    }
}
