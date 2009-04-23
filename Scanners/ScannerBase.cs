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

        // Returns current thread of scanner
        public Thread CurrentThread
        {
            get { return Thread.CurrentThread; }
        }

        public override string ToString()
        {
            return ScannerName;
        }

        //abstract public void Scan();
    }
}
