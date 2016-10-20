using System;

namespace Heroes
{
    internal class Program
    {
        #region Static Fields

        private static readonly Bootstrap BootstrapInstance = new Bootstrap();

        #endregion

        #region Methods

        private static void Main()
        {
            while (true)
            {
                Console.Out.WriteLine("OMGOMG");
            }
            BootstrapInstance.Initialize();
        }

        #endregion
    }
}