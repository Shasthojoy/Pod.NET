namespace Pod.NET.Test
{
    using PodNET.BL4;

    /// <summary>
    /// Main class of the program containing the application entry point.
    /// </summary>
    internal class Program
    {
        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private static void Main(string[] args)
        {
            //PodBinaryDataFile pbdf;
            BL4Track beltane = new BL4Track(@"C:\Games\Pod\DATA\BINARY\CIRCUITS\BELTANE.BL4");
            //pbdf = new PodBinaryDataFile(@"C:\Games\Pod\DATA\BINARY\VOITURES\SCORP.BV4");
        }
    }
}
