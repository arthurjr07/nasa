using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using NASA.BackgroundServices;
using NUnit.Framework;
using System;
using System.IO.Abstractions;
using System.Net.Http;


namespace Tests
{
    public class ImageDownloaderServiceTests
    {
        public class ConstructorTests
        {
            [Test]
            public void Should_ThrowArgumentNullException_WhenPassedNull_HttpClient()
            {
                //Arrange
                var mockHttpClient = new Mock<HttpClient>(MockBehavior.Strict);
                var mockEnvironment = new Mock<IHostingEnvironment>(MockBehavior.Strict);
                var mockLogger = new Mock<ILogger<ImageDownloaderService>>(MockBehavior.Strict);
                var mockFileSystem = new Mock<IFileSystem>(MockBehavior.Strict);


                var target = new Func<ImageDownloaderService>(() =>
                 new ImageDownloaderService(
                     null,
                     mockEnvironment.Object,
                     mockLogger.Object,
                     mockFileSystem.Object
                     ));

                //Act
                ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => target());

                //Assert
                ex.ParamName.Should().Be("httpClient");
            }

            [Test]
            public void Should_ThrowArgumentNullException_WhenPassedNull_HostingEnvironment()
            {
                //Arrange
                var mockHttpClient = new Mock<HttpClient>(MockBehavior.Strict);
                var mockEnvironment = new Mock<IHostingEnvironment>(MockBehavior.Strict);
                var mockLogger = new Mock<ILogger<ImageDownloaderService>>(MockBehavior.Strict);
                var mockFileSystem = new Mock<IFileSystem>(MockBehavior.Strict);

                var target = new Func<ImageDownloaderService>(() =>
                 new ImageDownloaderService(
                     mockHttpClient.Object,
                     null,
                     mockLogger.Object,
                     mockFileSystem.Object
                     ));

                //Act
                ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => target());

                //Assert
                ex.ParamName.Should().Be("environment");
            }

            [Test]
            public void Should_ThrowArgumentNullException_WhenPassedNull_Logger()
            {
                //Arrange
                var mockHttpClient = new Mock<HttpClient>(MockBehavior.Strict);
                var mockEnvironment = new Mock<IHostingEnvironment>(MockBehavior.Strict);
                var mockLogger = new Mock<ILogger<ImageDownloaderService>>(MockBehavior.Strict);
                var mockFileSystem = new Mock<IFileSystem>(MockBehavior.Strict);

                var target = new Func<ImageDownloaderService>(() =>
                 new ImageDownloaderService(
                     mockHttpClient.Object,
                     mockEnvironment.Object,
                     null,
                     mockFileSystem.Object
                     ));

                //Act
                ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => target());

                //Assert
                ex.ParamName.Should().Be("logger");
            }
        }
    }
}