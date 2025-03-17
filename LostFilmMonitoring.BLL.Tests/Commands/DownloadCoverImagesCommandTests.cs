// <copyright file="DownloadCoverImagesCommandTests.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.BLL.Tests.Commands;

[ExcludeFromCodeCoverage]
public class DownloadCoverImagesCommandTests
{
    private Mock<ISeriesDao>? seriesDao;
    private Mock<Common.ILogger>? logger;
    private Mock<IFileSystem>? fileSystem;
    private Mock<IConfiguration>? configuration;
    private Mock<ITmdbClient>? tmdbClient;
    private Mock<IDictionaryDao>? dictionaryDao;
    private Mock<IImageProcessor>? imageProcessor;

    [SetUp]
    public void Setup()
    {
        this.fileSystem = new();
        this.dictionaryDao = new();
        this.configuration = new();
        this.tmdbClient = new();
        this.seriesDao = new();
        this.logger = new();
        this.imageProcessor = new();
        this.logger.Setup(l => l.CreateScope(It.IsAny<string>())).Returns(this.logger.Object);
    }

    private DownloadCoverImagesCommand GetService()
            => new(this.logger!.Object, this.fileSystem!.Object, this.configuration!.Object, this.seriesDao!.Object, this.tmdbClient!.Object, this.dictionaryDao!.Object, this.imageProcessor!.Object);
}
