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
            string filePath = "testfile.txt";
            string expectedContent = "Hello, World!";
            await File.WriteAllTextAsync(filePath, expectedContent);

            // Act
            string actualContent = await _fileSystemPlugin.ReadFileAsync(filePath);

            // Assert
            Assert.Equal(expectedContent, actualContent);

            // Cleanup
            File.Delete(filePath);
        }

        [Fact]
        public async Task WriteFileAsync_ShouldWriteContentToFile()
        {
            // Arrange
            string filePath = "testfile.txt";
            string contentToWrite = "Hello, World!";

            // Act
            await _fileSystemPlugin.WriteFileAsync(filePath, contentToWrite);
            string actualContent = await File.ReadAllTextAsync(filePath);

            // Assert
            Assert.Equal(contentToWrite, actualContent);

            // Cleanup
            File.Delete(filePath);
        }
    }
}
