/*
    Little Registry Cleaner
    Copyright (C) 2008-2011 Little Apps (http://www.little-apps.org/)

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
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Win32;

namespace Little_Registry_Cleaner
{
    public class RegEditGo : IDisposable
    {
        public RegEditGo()
        {
            uint processId;

            // Checks if access is disabled to regedit, and adds access to it
            CheckAccess();

            Process[] processes = Process.GetProcessesByName("RegEdit");
            if (processes.Length == 0)
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "RegEdit.exe";
                    process.Start();

                    process.WaitForInputIdle();

                    wndApp = process.MainWindowHandle;
                    processId = (uint)process.Id;

                }
            }
            else
            {
                wndApp = processes[0].MainWindowHandle;
                processId = (uint)processes[0].Id;

                Interop.SetForegroundWindow(wndApp);
            }

            if (wndApp == IntPtr.Zero)
            {
                ShowErrorMessage(new SystemException("no app handle"));
            }

            // get handle to treeview
            wndTreeView = Interop.FindWindowEx(wndApp, IntPtr.Zero, "SysTreeView32", null);
            if (wndTreeView == IntPtr.Zero)
            {
                ShowErrorMessage(new SystemException("no treeview"));
            }

            // get handle to listview
            wndListView = Interop.FindWindowEx(wndApp, IntPtr.Zero, "SysListView32", null);
            if (wndListView == IntPtr.Zero)
            {
                ShowErrorMessage(new SystemException("no listview"));
            }


            // allocate buffer in local process
            lpLocalBuffer = Marshal.AllocHGlobal(dwBufferSize);
            if (lpLocalBuffer == IntPtr.Zero)
                ShowErrorMessage(new SystemException("Failed to allocate memory in local process"));

            hProcess = Interop.OpenProcess(Interop.PROCESS_ALL_ACCESS, false, processId);
            if (hProcess == IntPtr.Zero)
                ShowErrorMessage(new ApplicationException("Failed to access process"));

            // Allocate a buffer in the remote process
            lpRemoteBuffer = Interop.VirtualAllocEx(hProcess, IntPtr.Zero, dwBufferSize, Interop.MEM_COMMIT, Interop.PAGE_READWRITE);
            if (lpRemoteBuffer == IntPtr.Zero)
                ShowErrorMessage(new SystemException("Failed to allocate memory in remote process"));
        }

        ~RegEditGo()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        public void Close()
        {
            Dispose();
        }

        #region public


        /// <summary>
        /// Opens RegEdit.exe and navigates to given registry path and value 
        /// </summary>
        /// <param name="keyPath">path of registry key</param>
        /// <param name="valueName">name of registry value (can be null)</param>
        public static void GoTo(string keyPath, string valueName)
        {
            using (RegEditGo locator = new RegEditGo())
            {
                bool hasValue = !string.IsNullOrEmpty(valueName);
                locator.OpenKey(keyPath, hasValue);

                if (hasValue)
                {
                    System.Threading.Thread.Sleep(200);
                    locator.OpenValue(valueName);
                }
            }
        }

        public void OpenKey(string path, bool select)
        {
            if (string.IsNullOrEmpty(path)) return;

            const int TVGN_CARET = 0x0009;

            if (path.StartsWith("HKLM"))
            {
                path = "HKEY_LOCAL_MACHINE" + path.Remove(0, 4);
            }
            else if (path.StartsWith("HKCU"))
            {
                path = "HKEY_CURRENT_USER" + path.Remove(0, 4);
            }
            else if (path.StartsWith("HKCR"))
            {
                path = "HKEY_CLASSES_ROOT" + path.Remove(0, 4);
            }

            Interop.SendMessage(wndTreeView, Interop.WM_SETFOCUS, IntPtr.Zero, IntPtr.Zero);

            IntPtr tvItem = Interop.SendMessage(wndTreeView, Interop.TVM_GETNEXTITEM, (IntPtr)Interop.TVGN_ROOT, IntPtr.Zero);
            foreach (string key in path.Split('\\'))
            {
                if (key.Length == 0) continue;

                tvItem = FindKey(tvItem, key);
                if (tvItem == IntPtr.Zero)
                {
                    return;
                }
                Interop.SendMessage(wndTreeView, Interop.TVM_SELECTITEM, (IntPtr)TVGN_CARET, tvItem);

                // expand tree node 
                const int VK_RIGHT = 0x27;
                Interop.SendMessage(wndTreeView, Interop.WM_KEYDOWN, (IntPtr)VK_RIGHT, IntPtr.Zero);
                Interop.SendMessage(wndTreeView, Interop.WM_KEYUP, (IntPtr)VK_RIGHT, IntPtr.Zero);
            }

            Interop.SendMessage(wndTreeView, Interop.TVM_SELECTITEM, (IntPtr)TVGN_CARET, tvItem);

            if (select)
            {
                Interop.BringWindowToTop(wndApp);
            }
            else
            {
                SendTabKey(false);
            }
        }

        public void OpenValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return;

            Interop.SendMessage(wndListView, Interop.WM_SETFOCUS, IntPtr.Zero, IntPtr.Zero);

            if (value.Length == 0)
            {
                SetLVItemState(0);
                return;
            }

            int item = 0;
            for (; ; )
            {
                string itemText = GetLVItemText(item);
                if (itemText == null)
                {
                    return;
                }
                if (string.Compare(itemText, value, true) == 0)
                {
                    break;
                }
                item++;
            }

            SetLVItemState(item);


            const int LVM_FIRST = 0x1000;
            const int LVM_ENSUREVISIBLE = (LVM_FIRST + 19);
            Interop.SendMessage(wndListView, LVM_ENSUREVISIBLE, (IntPtr)item, IntPtr.Zero);

            Interop.BringWindowToTop(wndApp);

            SendTabKey(false);
            SendTabKey(true);
        }
        #endregion


        #region private
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
            }

            if (lpLocalBuffer != IntPtr.Zero)
                Marshal.FreeHGlobal(lpLocalBuffer);
            if (lpRemoteBuffer != IntPtr.Zero)
                Interop.VirtualFreeEx(hProcess, lpRemoteBuffer, 0, Interop.MEM_RELEASE);
            if (hProcess != IntPtr.Zero)
                Interop.CloseHandle(hProcess);
        }

        private const int dwBufferSize = 1024;

        private IntPtr wndApp;
        private IntPtr wndTreeView;
        private IntPtr wndListView;

        private IntPtr hProcess = IntPtr.Zero;
        private IntPtr lpRemoteBuffer = IntPtr.Zero;
        private IntPtr lpLocalBuffer = IntPtr.Zero;

        private void SendTabKey(bool shiftPressed)
        {
            const int VK_TAB = 0x09;
            const int VK_SHIFT = 0x10;
            if (!shiftPressed)
            {
                Interop.PostMessage(wndApp, Interop.WM_KEYDOWN, VK_TAB, 0x1f01);
                Interop.PostMessage(wndApp, Interop.WM_KEYUP, VK_TAB, 0x1f01);
            }
            else
            {
                Interop.PostMessage(wndApp, Interop.WM_KEYDOWN, VK_SHIFT, 0x1f01);
                Interop.PostMessage(wndApp, Interop.WM_KEYDOWN, VK_TAB, 0x1f01);
                Interop.PostMessage(wndApp, Interop.WM_KEYUP, VK_TAB, 0x1f01);
                Interop.PostMessage(wndApp, Interop.WM_KEYUP, VK_SHIFT, 0x1f01);
            }
        }


        private string GetTVItemTextEx(IntPtr wndTreeView, IntPtr item)
        {
            const int TVIF_TEXT = 0x0001;
            const int MAX_TVITEMTEXT = 512;

            Interop.TVITEM tvi = new Interop.TVITEM();
            tvi.mask = TVIF_TEXT;
            tvi.hItem = item;
            tvi.cchTextMax = MAX_TVITEMTEXT;
            // set address to remote buffer immediately following the tvItem
            tvi.pszText = (IntPtr)(lpRemoteBuffer.ToInt32() + Marshal.SizeOf(typeof(Interop.TVITEM)));

            // copy local tvItem to remote buffer
            bool bSuccess = Interop.WriteProcessMemory(hProcess, lpRemoteBuffer, ref tvi, Marshal.SizeOf(typeof(Interop.TVITEM)), IntPtr.Zero);
            if (!bSuccess)
                ShowErrorMessage(new SystemException("Failed to write to process memory"));


            Interop.SendMessage(wndTreeView, Interop.TVM_GETITEMW, IntPtr.Zero, lpRemoteBuffer);

            // copy tvItem back into local buffer (copy whole buffer because we don't yet know how big the string is)
            bSuccess = Interop.ReadProcessMemory(hProcess, lpRemoteBuffer, lpLocalBuffer, dwBufferSize, IntPtr.Zero);
            if (!bSuccess)
                ShowErrorMessage(new SystemException("Failed to read from process memory"));

            return Marshal.PtrToStringUni((IntPtr)(lpLocalBuffer.ToInt32() + Marshal.SizeOf(typeof(Interop.TVITEM))));
        }

        private IntPtr FindKey(IntPtr itemParent, string key)
        {
            IntPtr itemChild = Interop.SendMessage(wndTreeView, Interop.TVM_GETNEXTITEM, (IntPtr)Interop.TVGN_CHILD, itemParent);
            while (itemChild != IntPtr.Zero)
            {
                if (string.Compare(GetTVItemTextEx(wndTreeView, itemChild), key, true) == 0)
                {
                    return itemChild;
                }
                itemChild = Interop.SendMessage(wndTreeView, Interop.TVM_GETNEXTITEM, (IntPtr)Interop.TVGN_NEXT, itemChild);
            }
            ShowErrorMessage(new SystemException(string.Format("TVM_GETNEXTITEM failed... key '{0}' not found!", key)));
            return IntPtr.Zero;
        }

        private void SetLVItemState(int item)
        {
            const int LVM_FIRST = 0x1000;
            const int LVM_SETITEMSTATE = (LVM_FIRST + 43);
            const int LVIF_STATE = 0x0008;

            const int LVIS_FOCUSED = 0x0001;
            const int LVIS_SELECTED = 0x0002;

            Interop.LVITEM lvItem = new Interop.LVITEM();
            lvItem.mask = LVIF_STATE;
            lvItem.iItem = item;
            lvItem.iSubItem = 0;

            lvItem.state = LVIS_FOCUSED | LVIS_SELECTED;
            lvItem.stateMask = LVIS_FOCUSED | LVIS_SELECTED;

            // copy local lvItem to remote buffer
            bool bSuccess = Interop.WriteProcessMemory(hProcess, lpRemoteBuffer, ref lvItem, Marshal.SizeOf(typeof(Interop.LVITEM)), IntPtr.Zero);
            if (!bSuccess)
                ShowErrorMessage(new SystemException("Failed to write to process memory"));

            // Send the message to the remote window with the address of the remote buffer
            if (Interop.SendMessage(wndListView, LVM_SETITEMSTATE, (IntPtr)item, lpRemoteBuffer) == IntPtr.Zero)
                ShowErrorMessage(new SystemException("LVM_GETITEM Failed "));
        }

        private string GetLVItemText(int item)
        {
            const int LVM_GETITEM = 0x1005;
            const int LVIF_TEXT = 0x0001;

            Interop.LVITEM lvItem = new Interop.LVITEM();
            lvItem.mask = LVIF_TEXT;
            lvItem.iItem = item;
            lvItem.iSubItem = 0;
            // set address to remote buffer immediately following the lvItem 
            lvItem.pszText = (IntPtr)(lpRemoteBuffer.ToInt32() + Marshal.SizeOf(typeof(Interop.LVITEM)));
            lvItem.cchTextMax = 50;

            // copy local lvItem to remote buffer
            bool bSuccess = Interop.WriteProcessMemory(hProcess, lpRemoteBuffer, ref lvItem, Marshal.SizeOf(typeof(Interop.LVITEM)), IntPtr.Zero);
            if (!bSuccess)
                ShowErrorMessage(new SystemException("Failed to write to process memory"));

            // Send the message to the remote window with the address of the remote buffer
            if (Interop.SendMessage(wndListView, LVM_GETITEM, IntPtr.Zero, lpRemoteBuffer) == IntPtr.Zero)
                return null;

            // copy lvItem back into local buffer (copy whole buffer because we don't yet know how big the string is)
            bSuccess = Interop.ReadProcessMemory(hProcess, lpRemoteBuffer, lpLocalBuffer, dwBufferSize, IntPtr.Zero);
            if (!bSuccess)
                ShowErrorMessage(new SystemException("Failed to read from process memory"));

            return Marshal.PtrToStringAnsi((IntPtr)(lpLocalBuffer.ToInt32() + Marshal.SizeOf(typeof(Interop.LVITEM))));
        }

        private void CheckAccess()
        {
            using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", true))
            {
                if (regKey == null)
                    return;

                int? n = regKey.GetValue("DisableRegistryTools") as int?;

                // Value doesnt exists
                if (!n.HasValue)
                    return;

                // User has access
                if (n.Value == 0)
                    return;

                // Value is either 1 or 2 which means we cant access regedit.exe

                // So, lets enable access
                regKey.SetValue("DisableRegistryTools", (int)0, RegistryValueKind.DWord);
            }
        }

        private void ShowErrorMessage(Exception ex)
        {
#if (DEBUG)
            throw ex;
#endif
        }

        private class Interop
        {

            #region structs

            /// <summary>
            /// from 'http://dotnetjunkies.com/WebLog/chris.taylor/'
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct LVITEM
            {
                public uint mask;
                public int iItem;
                public int iSubItem;
                public uint state;
                public uint stateMask;
                public IntPtr pszText;
                public int cchTextMax;
                public int iImage;
            }

            /// <summary>
            /// from '.\PlatformSDK\Include\commctrl.h'
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            internal struct TVITEM
            {
                public uint mask;
                public IntPtr hItem;
                public uint state;
                public uint stateMask;
                public IntPtr pszText;
                public int cchTextMax;
                public uint iImage;
                public uint iSelectedImage;
                public uint cChildren;
                public IntPtr lParam;
            }
            #endregion

            internal const uint PROCESS_ALL_ACCESS = (uint)(0x000F0000L | 0x00100000L | 0xFFF);
            internal const uint MEM_COMMIT = 0x1000;
            internal const uint MEM_RELEASE = 0x8000;
            internal const uint PAGE_READWRITE = 0x04;

            internal const int WM_SETFOCUS = 0x0007;
            internal const int WM_KEYDOWN = 0x0100;
            internal const int WM_KEYUP = 0x0101;
            internal const int TVM_GETNEXTITEM = 0x1100 + 10;
            internal const int TVM_SELECTITEM = 0x1100 + 11;
            internal const int TVM_GETITEMW = 0x1100 + 62;
            internal const int TVGN_ROOT = 0x0000;
            internal const int TVGN_NEXT = 0x0001;
            internal const int TVGN_CHILD = 0x0004;


            [DllImport("user32.dll")]
            internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            internal static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

            [DllImport("kernel32")]
            internal static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

            [DllImport("user32.dll")]
            internal static extern uint WaitForInputIdle(IntPtr hProcess, uint dwMilliseconds);

            [DllImport("kernel32")]
            internal static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint flAllocationType, uint flProtect);

            [DllImport("kernel32")]
            internal static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint dwFreeType);

            [DllImport("kernel32")]
            internal static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref LVITEM buffer, int dwSize, IntPtr lpNumberOfBytesWritten);

            [DllImport("kernel32")]
            internal static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref TVITEM buffer, int dwSize, IntPtr lpNumberOfBytesWritten);

            [DllImport("kernel32")]
            internal static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int dwSize, IntPtr lpNumberOfBytesRead);

            [DllImport("kernel32")]
            internal static extern bool CloseHandle(IntPtr hObject);


            [DllImport("user32.dll")]
            internal static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll")]
            internal static extern int PostMessage(IntPtr hWnd, int msg, Int32 wParam, Int32 lParam);

            [DllImport("user32.dll")]
            internal static extern bool BringWindowToTop(IntPtr hWnd);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool SetForegroundWindow(IntPtr hWnd);
        }

        #endregion
    }
}
