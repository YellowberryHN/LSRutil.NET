﻿using System;
using System.IO;
using System.Text;

namespace LSRutil.RF
{
    public class RfWriter
    {
        private Stream headerStream;
        private BinaryWriter headerWriter;
        private Stream dataStream;
        private BinaryWriter dataWriter;
        /// <summary>
        /// Saves the archive as a header and data file with specified streams.
        /// </summary>
        /// <exception cref="IOException"></exception>
        public void WriteArchive(ResourceArchive archive, Stream headerStream, Stream dataStream)
        {
            this.headerStream = headerStream;
            this.dataStream = dataStream;

            using (headerWriter = new BinaryWriter(headerStream))
            using (dataWriter = new BinaryWriter(dataStream))
            {
                foreach (var resFile in archive.resources)
                {
                    headerWriter.Write(resFile.filepath.Length+1); // Compensates for null terminator to be added.
                    headerWriter.Write((uint)((DateTimeOffset)resFile.timestamp).ToUnixTimeSeconds());
                    headerWriter.Write(resFile.compressionType);
                    headerWriter.Write(resFile.compressedSize);
                    headerWriter.Write(resFile.offset);
                    headerWriter.Write(Encoding.ASCII.GetBytes(resFile.filepath+"\0"));
                    dataWriter.Write(resFile.data);
                }
            }
            
            
        }
        
        /// <summary>
        /// Saves the archive as a header and data file with specified filename.
        /// </summary>
        /// <param name="file">The file to save to. Extensions are optional.</param>
        /// <exception cref="IOException"></exception>
        public void WriteArchive(ResourceArchive archive, string file)
        {
            var fileExt = Path.GetExtension(file);
            if(fileExt==".rfh"||fileExt==".rfd") file = Path.ChangeExtension(file, null);
            headerStream = new FileStream(file+".rfh", FileMode.CreateNew);
            dataStream = new FileStream(file+".rfd", FileMode.CreateNew);

            WriteArchive(archive, headerStream, dataStream);
        }
    }
}