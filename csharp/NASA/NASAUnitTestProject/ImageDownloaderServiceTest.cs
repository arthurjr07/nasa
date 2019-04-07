using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NASA.BackgroundServices;
using NASA.Insfrastructures.Services;
using NUnit.Framework;
using System;
using System.IO.Abstractions;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    public class ImageDownloaderServiceTests
    {
        public class ConstructorTests
        {
            [Test]
            public void Should_ThrowArgumentNullException_WhenPassedNull_RoverService()
            {
                //Arrange
                var mockRoverService = new Mock<IRoverService>(MockBehavior.Strict);
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
                ex.ParamName.Should().Be("roverService");
            }

            [Test]
            public void Should_ThrowArgumentNullException_WhenPassedNull_HostingEnvironment()
            {
                //Arrange
                var mockRoverService = new Mock<IRoverService>(MockBehavior.Strict);
                var mockEnvironment = new Mock<IHostingEnvironment>(MockBehavior.Strict);
                var mockLogger = new Mock<ILogger<ImageDownloaderService>>(MockBehavior.Strict);
                var mockFileSystem = new Mock<IFileSystem>(MockBehavior.Strict);

                var target = new Func<ImageDownloaderService>(() =>
                 new ImageDownloaderService(
                     mockRoverService.Object,
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
                var mockRoverService = new Mock<IRoverService>(MockBehavior.Strict);
                var mockEnvironment = new Mock<IHostingEnvironment>(MockBehavior.Strict);
                var mockLogger = new Mock<ILogger<ImageDownloaderService>>(MockBehavior.Strict);
                var mockFileSystem = new Mock<IFileSystem>(MockBehavior.Strict);

                var target = new Func<ImageDownloaderService>(() =>
                 new ImageDownloaderService(
                     mockRoverService.Object,
                     mockEnvironment.Object,
                     null,
                     mockFileSystem.Object
                     ));

                //Act
                ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => target());

                //Assert
                ex.ParamName.Should().Be("logger");
            }

            [Test]
            public void Should_ThrowArgumentNullException_WhenPassedNull_FileSystem()
            {
                //Arrange
                var mockRoverService = new Mock<IRoverService>(MockBehavior.Strict);
                var mockEnvironment = new Mock<IHostingEnvironment>(MockBehavior.Strict);
                var mockLogger = new Mock<ILogger<ImageDownloaderService>>(MockBehavior.Strict);
                var mockFileSystem = new Mock<IFileSystem>(MockBehavior.Strict);

                var target = new Func<ImageDownloaderService>(() =>
                 new ImageDownloaderService(
                     mockRoverService.Object,
                     mockEnvironment.Object,
                     mockLogger.Object,
                     null
                     ));

                //Act
                ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => target());

                //Assert
                ex.ParamName.Should().Be("fileSystem");
            }
        }

        public class ExecuteAsyncMethodTests
        {
            [Test]
            public void Should_Not_Process_When_File_DoesNot_Exists()
            {
                //Arrange
                var mockRoverService = new Mock<IRoverService>(MockBehavior.Strict);
                var mockEnvironment = new Mock<IHostingEnvironment>(MockBehavior.Strict);
                var mockLogger = NullLogger<ImageDownloaderService>.Instance;
                var mockFileSystem = new Mock<IFileSystem>(MockBehavior.Strict);



                mockEnvironment.Setup(s => s.WebRootPath).Returns("c:\temp");
                mockFileSystem.Setup(s => s.Path.Combine(It.IsAny<string>(), It.IsAny<string>())).Returns("c:\temp");
                mockFileSystem.Setup(s => s.File.Exists(It.IsAny<string>())).Returns(false);

                var target = new ImageDownloaderService(
                     mockRoverService.Object,
                     mockEnvironment.Object,
                     mockLogger,
                     mockFileSystem.Object);


                MethodInfo methodInfo = typeof(ImageDownloaderService)
                    .GetMethod("ExecuteAsync", BindingFlags.NonPublic | BindingFlags.Instance);

                var cancellationTokenSource = new CancellationTokenSource();
                object[] parameters = { cancellationTokenSource.Token };
                cancellationTokenSource.CancelAfter(3000);

                //Act
                methodInfo.Invoke(target, parameters);

                //Assert
                mockEnvironment.Verify(s => s.WebRootPath);
                mockFileSystem.Verify(s => s.Path.Combine(It.IsAny<string>(), It.IsAny<string>()));
                mockRoverService.VerifyNoOtherCalls();
            }
        }
    }
}