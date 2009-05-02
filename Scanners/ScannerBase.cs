using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;

namespace Little_Registry_Cleaner.Scanners
{
    public abstract class ScannerBase
    {
        /// <summary>
        /// Returns the scanner name
        /// </summary>
        abstract public string ScannerName
        {
            get;
        }

        /// <summary>
        /// The root node (used for scan dialog)
        /// </summary>
        public BadRegistryKey RootNode = new BadRegistryKey();

        public override string ToString()
        {
            return ScannerName;
        }

        //abstract public void Scan();
    }
}
