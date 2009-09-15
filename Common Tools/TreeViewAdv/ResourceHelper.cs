using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Common_Tools.TreeViewAdv
{
    public static class ResourceHelper
    {
        // VSpilt Cursor with Innerline (symbolisize hidden column)
        private static Cursor _dVSplitCursor = GetCursor(Properties.Resources.DVSplit);
        public static Cursor DVSplitCursor
        {
            get { return _dVSplitCursor; }
        }

        /// <summary>
        /// Help function to convert byte[] from resource into Cursor Type 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static Cursor GetCursor(byte[] data)
        {
            using (MemoryStream s = new MemoryStream(data))
                return new Cursor(s);
        }

    }
}
