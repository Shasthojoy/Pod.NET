namespace PodNET
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Represents a collection of static extension method for objects of the <see cref="BinaryReader"/> type.
    /// </summary>
    internal static class BinaryReaderExtensions
    {
        // ---- METHODS (INTERNAL) -------------------------------------------------------------------------------------

        /// <summary>
        /// Reads a string from the current stream. The string has a prefix of 1 byte determining the length of the
        /// string and no postfix. Each character is encoded with an XOR byte starting at 0xFF and decreasing by 1 each
        /// byte.
        /// </summary>
        /// <param name="br">The extended <see cref="BinaryReader"/>.</param>
        /// <returns>The string being read.</returns>
        internal static string ReadPodString(this BinaryReader br)
        {
            byte length = br.ReadByte();
            byte[] characters = new byte[length];
            for (int i = 0; i < length; i++)
            {
                characters[i] = (byte)(br.ReadByte() ^ ~i);
            }
            return Encoding.ASCII.GetString(characters);
        }
    }
}
