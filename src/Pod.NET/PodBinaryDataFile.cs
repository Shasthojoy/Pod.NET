namespace PodNET
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Represents the base of all data files used by Pod and the general layout of them.
    /// </summary>
    public class PodBinaryDataFile
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="PodBinaryDataFile"/> class from the file with the given file
        /// name.
        /// </summary>
        /// <param name="fileName">The name of the file to be loaded.</param>
        public PodBinaryDataFile(string fileName)
            : this(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read), false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PodBinaryDataFile"/> class from the given stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> from which the data will be loaded.</param>
        /// <param name="leaveOpen">true to leave the stream open after loading, false to close it.</param>
        public PodBinaryDataFile(Stream stream, bool leaveOpen)
        {
            // Create the settings for the cryptor
            PodCryptoTransformConfig cryptoConfig = new PodCryptoTransformConfig()
            {
                FileSize = (uint)stream.Length
            };

            // Read the contents through the encrypting stream
            using (CryptoStream cryptoStream = new CryptoStream(stream, new PodCryptoTransform(cryptoConfig),
                CryptoStreamMode.Read))
            using (BinaryReader reader = new BinaryReader(cryptoStream, Encoding.ASCII, leaveOpen))
            {
                ReadHeader(reader);
                // Store the retrieved coder key
                CoderKey = cryptoConfig.CoderKey;

                ReadData(reader);
            }
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the size of the file as provided in the header.
        /// </summary>
        public uint FileSize
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the offset table.
        /// </summary>
        public uint[] Offsets
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the XOR key in which data is normally encoded.
        /// </summary>
        public uint CoderKey
        {
            get;
            private set;
        }

        // ---- METHODS (PROTECTED) ------------------------------------------------------------------------------------

        /// <summary>
        /// Called by the constructor of <see cref="PodBinaryDataFile"/> to provide the possibility for inheriting
        /// classes to load data file specific contents.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> to read custom data with.</param>
        protected virtual void ReadData(BinaryReader reader)
        {
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private void ReadHeader(BinaryReader reader)
        {
            // Header contains the file size, the offset count and the offset table.
            FileSize = reader.ReadUInt32();

            // Offset Table may contain unused file pointers which contain the coder key.
            Offsets = new uint[reader.ReadUInt32()];
            for (uint i = 0; i < Offsets.Length; i++)
            {
                Offsets[i] = reader.ReadUInt32();
            }
        }
    }
}
