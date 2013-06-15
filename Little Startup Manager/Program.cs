/*
    Little Registry Cleaner
    Copyright (C) 2008 Little Apps (http://www.little-apps.org/)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;

namespace Little_Startup_Manager
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            bool bMutexCreated = false;
            Mutex mutexMain = new Mutex(true, "Little Startup Manager", out bMutexCreated);

            // If mutex isnt available, show message and exit...
            if (!bMutexCreated)
            {
                MessageBox.Show(Properties.Resources.programAlreadyRunning, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            if (args.Length > 0)
            {
                foreach (string arg in args)
                {
                    // Culture needs to be sent via arguments as LRC settings are inaccessible and a static variable doesnt seem to return the right culture
                    if (arg.StartsWith(@"/culture:"))
                    {
                        int lcid = 0;

                        if (Int32.TryParse(arg.Remove(0, @"/culture:".Length), out lcid))
                        {
                            Properties.Resources.Culture = Thread.CurrentThread.CurrentUICulture = Application.CurrentCulture = new CultureInfo(lcid);
                             
                        }
                        
                    }
                }
                
            }

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new StartupManager());

            // Release Mutex
            mutexMain.ReleaseMutex();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Little_Registry_Cleaner.CrashReporter ErrorDlg = new Little_Registry_Cleaner.CrashReporter((Exception)e.ExceptionObject);
            ErrorDlg.ShowDialog();
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Little_Registry_Cleaner.CrashReporter ErrorDlg = new Little_Registry_Cleaner.CrashReporter(e.Exception);
            ErrorDlg.ShowDialog();
        }
    }
}
