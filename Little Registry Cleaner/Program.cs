/*
    Little Registry Cleaner
    Copyright (C) 2008-2009 Little Apps (http://www.littleapps.co.cc/)

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
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Reflection;

namespace Little_Registry_Cleaner
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool bMutexCreated = false;
            Mutex mutexMain = new Mutex(true, "Little Registry Cleaner", out bMutexCreated);

            // If mutex isnt available, show message and exit...
            if (!bMutexCreated)
            {
                MessageBox.Show(Properties.Resources.programAlreadyRunning, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            // Create event log source
            if (!EventLog.SourceExists(Application.ProductName))
                EventLog.CreateEventSource(Application.ProductName, "Application");

            // If application is being ran for first time or is newer version, then upgrade settings
            if (Properties.Settings.Default.bUpgradeSettings || !Properties.Settings.Default.IsSynchronized)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.bUpgradeSettings = false;
            }

            //#if (!DEBUG)
            // Add event handler for thread exceptions
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            //#else

            //#endif

            // Check if admin, otherwise exit
            if (!Permissions.IsUserAdministrator)
            {
                MessageBox.Show(Properties.Resources.needAdmin, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            // Enable needed privileges
            Permissions.SetPrivileges(true);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());

            // Disable needed privileges
            Permissions.SetPrivileges(false);

            // Release Mutex
            mutexMain.ReleaseMutex();

            // Save settings
            Properties.Settings.Default.Save();

            return;
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            CrashReporter ErrorDlg = new CrashReporter((Exception)e.ExceptionObject);
            ErrorDlg.ShowDialog();
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            CrashReporter ErrorDlg = new CrashReporter(e.Exception);
            ErrorDlg.ShowDialog();
        }
    }
}
