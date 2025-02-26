using System;
using System.IO;
using System.Threading.Tasks;

namespace AIBot.Plugins
{
    public class FileSystemPlugin
    {
        public async Task<string> ReadFileAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
            }

            using (StreamReader reader = new StreamReader(filePath))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public async Task WriteFileAsync(string filePath, string content)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
            }

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                await writer.WriteAsync(content);
            }
        }
    }
}
