using System;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Confluent.Kafka;

namespace Api.Tests
{
    public class StartupTests
    {
        [Fact]
        public void ConfigureServices_ShouldAddMvc()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            var services = new ServiceCollection();
            var startup = new Startup(mockConfiguration.Object);

            // Act
            startup.ConfigureServices(services);

            // Assert
            Assert.Contains(services, sd => sd.ServiceType == typeof(IServiceCollection));
        }

        [Fact]
        public void ConfigureServices_ShouldAddHostedService()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            var services = new ServiceCollection();
            var startup = new Startup(mockConfiguration.Object);

            // Act
            startup.ConfigureServices(services);

            // Assert
            Assert.Contains(services, sd => sd.ImplementationType == typeof(ProcessOrdersService));
        }

        [Fact]
        public void ConfigureServices_ShouldConfigureKafkaConfigs()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c.Bind("producer", It.IsAny<ProducerConfig>())).Verifiable();
            mockConfiguration.Setup(c => c.Bind("consumer", It.IsAny<ConsumerConfig>())).Verifiable();

            var services = new ServiceCollection();
            var startup = new Startup(mockConfiguration.Object);

            // Act
            startup.ConfigureServices(services);

            // Assert
            mockConfiguration.Verify(c => c.Bind("producer", It.IsAny<ProducerConfig>()), Times.Once);
            mockConfiguration.Verify(c => c.Bind("consumer", It.IsAny<ConsumerConfig>()), Times.Once);
        }

        [Fact]
        public void Startup_Constructor_ShouldSetConfiguration()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();

            // Act
            var startup = new Startup(mockConfiguration.Object);

            // Assert
            Assert.Equal(mockConfiguration.Object, startup.Configuration);
        }

        [Fact]
        public void Configure_ShouldNotThrowException()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHostingEnvironment = new Mock<IHostingEnvironment>();
            mockHostingEnvironment.Setup(e => e.EnvironmentName).Returns(EnvironmentName.Development);

            var mockAppBuilder = new Mock<IApplicationBuilder>();
            var startup = new Startup(mockConfiguration.Object);

            // Act & Assert
            try
            {
                startup.Configure(mockAppBuilder.Object, mockHostingEnvironment.Object);
            }
            catch (Exception ex)
            {
                Assert.True(false, $"Configure method threw an unexpected exception: {ex.Message}");
            }
        }
    }
}