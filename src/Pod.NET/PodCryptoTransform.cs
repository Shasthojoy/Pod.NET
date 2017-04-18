namespace PodNET
{
    using System;
    using System.Security.Cryptography;

    /// <summary>
    /// Represents the encryption and decryption transformations for  POD binary files.
    /// </summary>
    internal class PodCryptoTransform : ICryptoTransform
    {
        // ---- MEMBERS ------------------------------------------------------------------------------------------------

        private uint _bytesRead;
        private uint _blockChecksum;
        private uint _lastBlockDword;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="PodCryptoTransform"/> class with the specified initial
        /// configuration.
        /// </summary>
        /// <param name="encrypt">Determines whether to encrypt or decrypt.</param>
        /// <param name="previousDword">The previous uint with which is XORed.</param>
        internal PodCryptoTransform(PodCryptoTransformConfig config)
        {
            Config = config;
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets a value indicating whether the current transform can be reused.
        /// </summary>
        public bool CanReuseTransform
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether multiple blocks can be transformed.
        /// </summary>
        public bool CanTransformMultipleBlocks
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the input block size.
        /// </summary>
        public int InputBlockSize
        {
            get { return sizeof(uint); }
        }

        /// <summary>
        /// Gets the output block size.
        /// </summary>
        public int OutputBlockSize
        {
            get { return InputBlockSize; }
        }

        /// <summary>
        /// Gets the initial configuration used by this cryptor.
        /// </summary>
        internal PodCryptoTransformConfig Config
        {
            get;
            private set;
        }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        public void Dispose()
        {
        }

        /// <summary>
        /// Transforms the specified region of the input byte array and copies the resulting transform to the specified
        /// region of the output byte array.
        /// </summary>
        /// <param name="inputBuffer">The input for which to compute the transform.</param>
        /// <param name="inputOffset">The offset into the input byte array from which to begin using data.</param>
        /// <param name="inputCount">The number of bytes in the input byte array to use as data.</param>
        /// <param name="outputBuffer">The output to which to write the transform.</param>
        /// <param name="outputOffset">The offset into the output byte array from which to begin writing data.</param>
        /// <returns>The number of bytes written.</returns>
        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer,
            int outputOffset)
        {
            int realInputCount = 0;
            for (int i = 0; i < inputCount; i += InputBlockSize)
            {
                // Read an uint from the input buffer.
                uint dword = BitConverter.ToUInt32(inputBuffer, inputOffset + i);

                // Retrieve the coder key through the file size in the header.
                if (_bytesRead == 0)
                {
                    Config.CoderKey = dword ^ Config.FileSize;
                }

                uint decodedDword = 0;
                // Decode the data. 1st block always XOR. Depending on coder key, other blocks may use special coder.
                if (Config.BlockSize > 0 && _bytesRead > Config.BlockSize
                    && (Config.CoderKey == 0x00005CA8 || Config.CoderKey == 0x0000D13F))
                {
                    uint key = 0;
                    switch (_lastBlockDword & 0x00030000)
                    {
                        case 0x00000000: key = _lastBlockDword - 0x50A4A89D; break;
                        case 0x00010000: key = 0x3AF70BC4 - _lastBlockDword; break;
                        case 0x00020000: key = (_lastBlockDword + 0x07091971) << 1; break;
                        case 0x00030000: key = (0x11E67316 - _lastBlockDword) << 1; break;
                    }
                    switch (_lastBlockDword & 0x00000003)
                    {
                        case 0x00000000: decodedDword = ~dword ^ key; break;
                        case 0x00000001: decodedDword = ~dword ^ ~key; break;
                        case 0x00000002: decodedDword = dword ^ ~key; break;
                        case 0x00000003: decodedDword = dword ^ key ^ 0x0000FFFF; break;
                    }
                }
                else
                {
                    // This is the first block or no special coder key, do simple XOR.
                    decodedDword = dword ^ Config.CoderKey;
                }

                // Block size proofing.
                bool isBlockEnd = false;
                if (Config.BlockSize == 0)
                {
                    // Block size yet unknown, check if this could be a valid block size.
                    if (decodedDword == _blockChecksum && (Config.FileSize % _bytesRead) == 0)
                    {
                        // Obviously a checksum at a possible position, thus we can calculate the size of a block.
                        Config.BlockSize = _bytesRead + (uint)InputBlockSize;
                        isBlockEnd = true;
                    }
                }
                else
                {
                    // Block size is known, check if this is the end of the block.
                    if (_bytesRead % Config.BlockSize == 0)
                    {
                        if (decodedDword != _blockChecksum)
                        {
                            throw new InvalidOperationException("Invalid checksum");
                        }
                        isBlockEnd = true;
                    }
                }

                if (isBlockEnd)
                {
                    _blockChecksum = 0;
                    _lastBlockDword = 0;
                }
                else
                {
                    unchecked { _blockChecksum += decodedDword; }
                    _lastBlockDword = decodedDword;
                    realInputCount += InputBlockSize;

                    // Copy to output.
                    byte[] transformedData = BitConverter.GetBytes(decodedDword);
                    Array.Copy(transformedData, 0, outputBuffer, outputOffset + i, transformedData.Length);
                }

                _bytesRead += (uint)InputBlockSize;
            }

            // Return the amount of bytes of real data (without the metadata like checksums).
            return realInputCount;
        }

        /// <summary>
        /// Transforms the specified region (the last or partial block in the stream) of the specified byte array.
        /// </summary>
        /// <param name="inputBuffer">The input for which to compute the transform.</param>
        /// <param name="inputOffset">The offset into the byte array from which to begin using data. </param>
        /// <param name="inputCount">The number of bytes in the byte array to use as data. </param>
        /// <returns>The computed transform.</returns>
        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            byte[] decryptedData = new byte[inputCount];
            TransformBlock(inputBuffer, inputOffset, inputCount, decryptedData, 0);
            return decryptedData;
        }
    }
}
