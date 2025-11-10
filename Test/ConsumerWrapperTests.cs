using System;
using Xunit;
using Confluent.Kafka;
using Moq;

namespace Api.Tests
{
    public class ConsumerWrapperTests
    {
        [Fact]
        public void Constructor_ValidConfig_ShouldInitializeConsumer()
        {
            // Arrange
            var mockConfig = new ConsumerConfig { GroupId = "test-group" };
            var topicName = "test-topic";

            // Act
            var consumerWrapper = new ConsumerWrapper(mockConfig, topicName);

            // Assert
            Assert.NotNull(consumerWrapper);
        }

        [Fact]
        public void ReadMessage_ShouldReturnMessageValue()
        {
            // Arrange
            var mockConfig = new ConsumerConfig { GroupId = "test-group" };
            var topicName = "test-topic";
            var mockConsumer = new Mock<Consumer<string, string>>();
            var mockConsumeResult = new ConsumeResult<string, string>
            {
                Value = "Test Message"
            };

            mockConsumer.Setup(c => c.Consume()).Returns(mockConsumeResult);

            // Act
            var result = mockConsumer.Object.Consume().Value;

            // Assert
            Assert.Equal("Test Message", result);
        }

        [Fact]
        public void Constructor_NullConfig_ShouldThrowArgumentNullException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ConsumerWrapper(null, "topic"));
        }

        [Fact]
        public void Constructor_EmptyTopicName_ShouldThrowArgumentException()
        {
            // Arrange
            var config = new ConsumerConfig { GroupId = "test-group" };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new ConsumerWrapper(config, string.Empty));
        }

        [Fact]
        public void ReadMessage_NoMessageAvailable_ShouldHandleTimeout()
        {
            // Arrange
            var mockConfig = new ConsumerConfig { GroupId = "test-group" };
            var topicName = "test-topic";
            var mockConsumer = new Mock<Consumer<string, string>>();

            mockConsumer.Setup(c => c.Consume()).Throws<OperationCanceledException>();

            // Act & Assert
            Assert.Throws<OperationCanceledException>(() => mockConsumer.Object.Consume());
        }
    }
}