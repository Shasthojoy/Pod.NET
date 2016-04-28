namespace PodNET
{
    using System;

    /// <summary>
    /// Represents the initial configuration of a encryption and decryption progress of Pod binary data files.
    /// </summary>
    internal class PodCryptoTransformConfig
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="PodCryptoTransformConfig"/> class.
        /// </summary>
        internal PodCryptoTransformConfig()
        {
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets a value indicating whether the cryptor currently encrypts (true) or decrypts (false).
        /// </summary>
        internal bool Encrypt
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size of the loaded binary file. It is used to determine the coding key for the first data
        /// block.
        /// </summary>
        internal uint FileSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the coder key which is normally retrieved by XORing the file size specified in the header with
        /// the given <see cref="FileSize"/>.
        /// </summary>
        internal uint CoderKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the blocksize of file blocks after which checksums occur. This is normally determined by
        /// checking if possible checksums are at a position which would result in a blocksize of which a multiple is
        /// the <see cref="FileSize"/>.
        /// </summary>
        internal uint BlockSize
        {
            get;
            set;
        }
    }
}
