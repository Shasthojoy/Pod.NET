namespace PodNET.BL4
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Represents a BL4 track as in 3Dfx POD 2.2.9.0.
    /// </summary>
    public class BL4Track : PodBinaryDataFile
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="BL4Track"/> class.
        /// </summary>
        public BL4Track(string fileName)
            : base(fileName)
        {
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the array of <see cref="TrackEvent"/> instances.
        /// </summary>
        public TrackEvent[] Events;

        // ---- METHODS (PROTECTED) ------------------------------------------------------------------------------------

        /// <summary>
        /// Called by the constructor of <see cref="PodBinaryDataFile"/> to provide the possibility for inheriting
        /// classes to load data file specific contents.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> to read custom data with.</param>
        protected override void ReadData(BinaryReader reader)
        {
            ReadReserved(reader);
            ReadEvents(reader);
            ReadMacros(reader);
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private void ReadReserved(BinaryReader reader)
        {
            // Reserved1 (must be 0x00000003)
            uint reserved1 = reader.ReadUInt32();

            // Reserved2 (unused)
            uint reserved2 = reader.ReadUInt32();
        }

        private void ReadEvents(BinaryReader reader)
        {
            uint nameCount = reader.ReadUInt32();
            uint bufferSize = reader.ReadUInt32();

            Events = new TrackEvent[nameCount];
            for (int i = 0; i < nameCount; i++)
            {
                Events[i] = new TrackEvent(reader);
            }
        }

        private void ReadMacros(BinaryReader reader)
        {
            
        }
    }
}
