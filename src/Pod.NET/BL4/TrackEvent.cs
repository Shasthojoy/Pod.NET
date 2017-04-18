namespace PodNET.BL4
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;

    /// <summary>
    /// Represents an event on a track.
    /// </summary>
    public class TrackEvent
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackEvent"/> class.
        /// </summary>
        public TrackEvent(BinaryReader reader)
        {
            Name = reader.ReadPodString();
            
            uint paramSize = reader.ReadUInt32();
            int paramCount = reader.ReadInt32();
            Params = new byte[paramCount][];
            for (int i = 0; i < paramCount; i++)
            {
                Params[i] = reader.ReadBytes((int)paramSize);
            }
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the name of the event.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets an array of parameters which each has a byte array of data.
        /// </summary>
        public byte[][] Params
        {
            get;
            private set;
        }
    }
}
