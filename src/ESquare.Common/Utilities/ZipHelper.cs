using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace ESquare.Common.Utilities
{
    /// <summary>
    /// Helper to compress file stream into zip file
    /// </summary>
    public class ZipHelper
    {
        public static async Task<Stream> CompressAsync(string fileName, List<KeyValuePair<string, Stream>> fileStreams)
        {
            MemoryStream memoryStream = new MemoryStream();

            using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (KeyValuePair<string, Stream> file in fileStreams)
                {
                    ZipArchiveEntry zipEntry = archive.CreateEntry(Path.GetFileName(file.Key), CompressionLevel.Optimal);

                    using (Stream entryStream = zipEntry.Open())
                    {
                        file.Value.CopyTo(entryStream);
                    }
                }
            }

            await memoryStream.FlushAsync();
            memoryStream.Position = 0;

            return memoryStream;
        }
    }
}