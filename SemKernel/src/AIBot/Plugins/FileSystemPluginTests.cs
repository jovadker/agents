using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace AIBot.Plugins.Tests
{
    public class FileSystemPluginTests
    {
        private readonly FileSystemPlugin _fileSystemPlugin;

        public FileSystemPluginTests()
        {
            _fileSystemPlugin = new FileSystemPlugin();
        }

        [Fact]
        public async Task ReadFileAsync_ShouldReturnFileContent()
        {
            // Arrange
            var filePath = "testfile.txt";
            var expectedContent = "Hello, World!";
            await File.WriteAllTextAsync(filePath, expectedContent);

            // Act
            var content = await _fileSystemPlugin.ReadFileAsync(filePath);

            // Assert
            Assert.Equal(expectedContent, content);

            // Cleanup
            File.Delete(filePath);
        }

        [Fact]
        public async Task WriteFileAsync_ShouldWriteContentToFile()
        {
            // Arrange
            var filePath = "testfile.txt";
            var contentToWrite = "Hello, World!";

            // Act
            await _fileSystemPlugin.WriteFileAsync(filePath, contentToWrite);

            // Assert
            var writtenContent = await File.ReadAllTextAsync(filePath);
            Assert.Equal(contentToWrite, writtenContent);

            // Cleanup
            File.Delete(filePath);
        }

        [Fact]
        public async Task ReadFileAsync_ShouldThrowException_WhenFilePathIsNullOrEmpty()
        {
            // Arrange
            string filePath = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _fileSystemPlugin.ReadFileAsync(filePath));
        }

        [Fact]
        public async Task WriteFileAsync_ShouldThrowException_WhenFilePathIsNullOrEmpty()
        {
            // Arrange
            string filePath = null;
            var contentToWrite = "Hello, World!";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _fileSystemPlugin.WriteFileAsync(filePath, contentToWrite));
        }
    }
}
