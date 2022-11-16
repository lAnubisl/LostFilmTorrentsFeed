// <copyright file="ReteOrgRssFeedTests.cs" company="Alexander Panfilenok">
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
    public class ReteOrgRssFeedTests
    {
        private Mock<IHttpClientFactory> httpClientFactory;
        private Mock<ILogger> logger;
        private MockHttpMessageHandler mockHttp;

        [SetUp]
        public void Setup()
        {
            this.httpClientFactory = new();
            this.logger = new();
            this.logger.Setup(l => l.CreateScope(It.IsAny<string>())).Returns(this.logger.Object);
            this.mockHttp = new();
            this.httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient(mockHttp));
        }

        [Test]
        public void Constructor_should_throw_exception_when_logger_null()
        {
            var action = () => new ReteOrgRssFeed(null!, this.httpClientFactory.Object);
            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("logger");
        }

        [Test]
        public void Constructor_should_throw_exception_when_logger_createScope_null()
        {
            var action = () => new ReteOrgRssFeed(new Mock<ILogger>().Object, this.httpClientFactory.Object);
            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("logger");
        }

        [Test]
        public void Constructor_should_throw_exception_when_httpClientFactory_null()
        {
            var action = () => new ReteOrgRssFeed(this.logger.Object, null!);
            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("httpClientFactory");
        }

        [Test]
        public async Task LoadFeedItemsAsync_should_return_items()
        {
            mockHttp
                .When(HttpMethod.Get, "https://insearch.site/rssdd.xml")
                .Respond("application/xml", Helper.GetEmbeddedResource($"LostFilmTV.Client.Tests.TestData.BaseFeed1.xml"));
            var result = await GetService().LoadFeedItemsAsync();
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }

        [Test]
        public async Task LoadFeedItemsAsync_should_return_empty_when_content_is_empty()
        {
            mockHttp
                .When(HttpMethod.Get, "https://insearch.site/rssdd.xml")
                .Respond("application/xml", string.Empty);
            var result = await GetService().LoadFeedItemsAsync();
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Test]
        public async Task LoadFeedItemsAsync_should_return_empty_when_TaskCanceledException()
        {
            mockHttp
                .When(HttpMethod.Get, "https://insearch.site/rssdd.xml")
                .Throw(new TaskCanceledException());
            var result = await GetService().LoadFeedItemsAsync();
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Test]
        public async Task LoadFeedItemsAsync_should_return_empty_when_IOException()
        {
            mockHttp
                .When(HttpMethod.Get, "https://insearch.site/rssdd.xml")
                .Throw(new IOException());
            var result = await GetService().LoadFeedItemsAsync();
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Test]
        public async Task LoadFeedItemsAsync_should_return_empty_when_Exception()
        {
            mockHttp
                .When(HttpMethod.Get, "https://insearch.site/rssdd.xml")
                .Throw(new Exception());
            var result = await GetService().LoadFeedItemsAsync();
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Test]
        public async Task LoadFeedItemsAsync_should_return_empty_when_rss_broken()
        {
            mockHttp
                .When(HttpMethod.Get, "https://insearch.site/rssdd.xml")
                .Respond("application/xml", "BROKEN RSS DATA");
            var result = await GetService().LoadFeedItemsAsync();
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        private ReteOrgRssFeed GetService() => new(this.logger.Object, this.httpClientFactory.Object);
    }
}
