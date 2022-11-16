// <copyright file="ConfigurationTests.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.BLL.Tests
{
    [ExcludeFromCodeCoverage]
    public class ConfigurationTests
    {
        private Mock<IConfigurationValuesProvider> providerMock;

        [SetUp]
        public void Setup()
        {
            providerMock = new();
        }

        [Test]
        public void AllProperties_should_return_values()
        {
            providerMock.Setup(x => x.GetValue("BASEURL")).Returns("BASEURL");
            providerMock.Setup(x => x.GetValue("BASEFEEDCOOKIE")).Returns("BASEFEEDCOOKIE");
            providerMock.Setup(x => x.GetValue("BASELINKUID")).Returns("BASELINKUID");
            providerMock.Setup(x => x.GetValue("TORRENTTRACKERS")).Returns("#1{0},#2{0},#3{0}");
            providerMock.Setup(x => x.GetValue("IMAGESDIRECTORY")).Returns("IMAGESDIRECTORY");
            providerMock.Setup(x => x.GetValue("TORRENTSDIRECTORY")).Returns("TORRENTSDIRECTORY");
            var service = GetService();
            service.BaseUID.Should().BeEquivalentTo("BASELINKUID");
            service.BaseUSESS.Should().BeEquivalentTo("BASEFEEDCOOKIE");
            service.ImagesDirectory.Should().BeEquivalentTo("IMAGESDIRECTORY");
            service.TorrentsDirectory.Should().BeEquivalentTo("TORRENTSDIRECTORY");
            service.BaseUrl.Should().BeEquivalentTo("BASEURL");
            service.GetTorrentAnnounceList("USERID").Should().BeEquivalentTo(new[] { "#1USERID", "#2USERID", "#3USERID" });
        }

        [Test]
        public void GetTorrentAnnounceList_should_have_default_value()
        {
            providerMock.Setup(x => x.GetValue("BASEURL")).Returns("BASEURL");
            providerMock.Setup(x => x.GetValue("BASEFEEDCOOKIE")).Returns("BASEFEEDCOOKIE");
            providerMock.Setup(x => x.GetValue("BASELINKUID")).Returns("BASELINKUID");
            providerMock.Setup(x => x.GetValue("TORRENTTRACKERS")).Returns("#1{0},#2{0},#3{0}");
            providerMock.Setup(x => x.GetValue("IMAGESDIRECTORY")).Returns("IMAGESDIRECTORY");
            providerMock.Setup(x => x.GetValue("TORRENTSDIRECTORY")).Returns("TORRENTSDIRECTORY");
            var service = GetService();
            service.GetTorrentAnnounceList(null!).Should().BeEquivalentTo(new[] { "#1BASELINKUID", "#2BASELINKUID", "#3BASELINKUID" });
        }

        [Test]
        public void ImagesDirectory_should_have_default_value()
        {
            providerMock.Setup(x => x.GetValue("BASEURL")).Returns("BASEURL");
            providerMock.Setup(x => x.GetValue("BASEFEEDCOOKIE")).Returns("BASEFEEDCOOKIE");
            providerMock.Setup(x => x.GetValue("BASELINKUID")).Returns("BASELINKUID");
            providerMock.Setup(x => x.GetValue("TORRENTTRACKERS")).Returns("#1{0},#2{0},#3{0}");
            providerMock.Setup(x => x.GetValue("IMAGESDIRECTORY")).Returns(null as string);
            providerMock.Setup(x => x.GetValue("TORRENTSDIRECTORY")).Returns("TORRENTSDIRECTORY");
            var service = GetService();
            service.ImagesDirectory.Should().BeEquivalentTo("images");
        }

        [Test]
        public void TorrentsDirectory_should_have_default_value()
        {
            providerMock.Setup(x => x.GetValue("BASEURL")).Returns("BASEURL");
            providerMock.Setup(x => x.GetValue("BASEFEEDCOOKIE")).Returns("BASEFEEDCOOKIE");
            providerMock.Setup(x => x.GetValue("BASELINKUID")).Returns("BASELINKUID");
            providerMock.Setup(x => x.GetValue("TORRENTTRACKERS")).Returns("#1{0},#2{0},#3{0}");
            providerMock.Setup(x => x.GetValue("IMAGESDIRECTORY")).Returns("IMAGESDIRECTORY");
            providerMock.Setup(x => x.GetValue("TORRENTSDIRECTORY")).Returns(null as string);
            var service = GetService();
            service.TorrentsDirectory.Should().BeEquivalentTo("torrentfiles");
        }

        [Test]
        public void Constructor_should_fail_when_BASEURL_not_set()
        {
            providerMock.Setup(x => x.GetValue("BASEURL")).Returns(null as string);
            providerMock.Setup(x => x.GetValue("BASEFEEDCOOKIE")).Returns("BASEFEEDCOOKIE");
            providerMock.Setup(x => x.GetValue("BASELINKUID")).Returns("BASELINKUID");
            providerMock.Setup(x => x.GetValue("TORRENTTRACKERS")).Returns("#1{0},#2{0},#3{0}");
            providerMock.Setup(x => x.GetValue("IMAGESDIRECTORY")).Returns("IMAGESDIRECTORY");
            providerMock.Setup(x => x.GetValue("TORRENTSDIRECTORY")).Returns("TORRENTSDIRECTORY");
            
            var action = () => GetService();
            action.Should().Throw<Exception>().WithMessage("Environment variable 'BASEURL' is not defined.");
        }

        [Test]
        public void Constructor_should_fail_when_BASEFEEDCOOKIEL_not_set()
        {
            providerMock.Setup(x => x.GetValue("BASEURL")).Returns("BASEURL");
            providerMock.Setup(x => x.GetValue("BASEFEEDCOOKIE")).Returns(null as string);
            providerMock.Setup(x => x.GetValue("BASELINKUID")).Returns("BASELINKUID");
            providerMock.Setup(x => x.GetValue("TORRENTTRACKERS")).Returns("#1{0},#2{0},#3{0}");
            providerMock.Setup(x => x.GetValue("IMAGESDIRECTORY")).Returns("IMAGESDIRECTORY");
            providerMock.Setup(x => x.GetValue("TORRENTSDIRECTORY")).Returns("TORRENTSDIRECTORY");

            var action = () => GetService();
            action.Should().Throw<Exception>().WithMessage("Environment variable 'BASEFEEDCOOKIE' is not defined.");
        }

        [Test]
        public void Constructor_should_fail_when_BASELINKUID_not_set()
        {
            providerMock.Setup(x => x.GetValue("BASEURL")).Returns("BASEURL");
            providerMock.Setup(x => x.GetValue("BASEFEEDCOOKIE")).Returns("BASEFEEDCOOKIE");
            providerMock.Setup(x => x.GetValue("BASELINKUID")).Returns(null as string);
            providerMock.Setup(x => x.GetValue("TORRENTTRACKERS")).Returns("#1{0},#2{0},#3{0}");
            providerMock.Setup(x => x.GetValue("IMAGESDIRECTORY")).Returns("IMAGESDIRECTORY");
            providerMock.Setup(x => x.GetValue("TORRENTSDIRECTORY")).Returns("TORRENTSDIRECTORY");

            var action = () => GetService();
            action.Should().Throw<Exception>().WithMessage("Environment variable 'BASELINKUID' is not defined.");
        }

        [Test]
        public void Constructor_should_fail_when_TORRENTTRACKERS_not_set()
        {
            providerMock.Setup(x => x.GetValue("BASEURL")).Returns("BASEURL");
            providerMock.Setup(x => x.GetValue("BASEFEEDCOOKIE")).Returns("BASEFEEDCOOKIE");
            providerMock.Setup(x => x.GetValue("BASELINKUID")).Returns("BASELINKUID");
            providerMock.Setup(x => x.GetValue("TORRENTTRACKERS")).Returns(null as string);
            providerMock.Setup(x => x.GetValue("IMAGESDIRECTORY")).Returns("IMAGESDIRECTORY");
            providerMock.Setup(x => x.GetValue("TORRENTSDIRECTORY")).Returns("TORRENTSDIRECTORY");

            var action = () => GetService();
            action.Should().Throw<Exception>().WithMessage("Environment variable 'TORRENTTRACKERS' is not defined.");
        }

        private Configuration GetService() => new (providerMock.Object);
    }
}
