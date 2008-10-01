/*
    Little Registry Cleaner
    Copyright (C) 2008 Nick H.

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
using System.Threading;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Diagnostics;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Security.AccessControl;
using System.IO;

namespace Little_Registry_Cleaner.Xml
{
    /// <summary>
    /// xmlRegistry is the class that saves and load registry trees to/from xml files.
    /// It needs to directly access the API because Win32 classes do not allow good control over key type being created
    /// </summary>
    public class xmlRegistry
    {
        public struct InternalKeyLoad // used in LoadAsXml
        {
            public string strKeyname;
            public RegistryKey reg;
        };

        private const int ERROR_SUCCESS = 0;

        private const uint HKEY_CLASSES_ROOT = 0x80000000;
        private const uint HKEY_CURRENT_USER = 0x80000001;
        private const uint HKEY_LOCAL_MACHINE = 0x80000002;
        private const uint HKEY_USERS = 0x80000003;
        private const uint HKEY_PERFORMANCE_DATA = 0x80000004;
        private const uint HKEY_CURRENT_CONFIG = 0x80000005;
        private const uint HKEY_DYN_DATA = 0x80000006;

        private const int REG_NONE = 0;   // No value type
        private const int REG_SZ = 1;   // Unicode nul terminated string
        private const int REG_EXPAND_SZ = 2;   // Unicode nul terminated string
        private const int REG_BINARY = 3;   // Free form binary
        private const int REG_DWORD = 4;   // 32-bit number
        private const int REG_DWORD_LITTLE_ENDIAN = 4;   // 32-bit number =same as REG_DWORD)
        private const int REG_DWORD_BIG_ENDIAN = 5;   // 32-bit number
        private const int REG_LINK = 6;   // Symbolic Link = unicode
        private const int REG_MULTI_SZ = 7;   // Multiple Unicode strings
        private const int REG_RESOURCE_LIST = 8;   // Resource list in the resource map
        private const int REG_FULL_RESOURCE_DESCRIPTOR = 9;  // Resource list in the hardware description
        private const int REG_RESOURCE_REQUIREMENTS_LIST = 10;

        private const string FAKEDITEM = "faked item";
        private const string IDS_DEFAULTVALUENAME = "(DEFAULT)";
        private const string IDS_DEFAULTVALUEVALUE = "(value not set)";

        // standard XML
        public const string XML_ROOT = "registry";
        public const string XML_KEY = "k";
        public const string XML_NAME = "name";
        public const string XML_VALUE = "v";
        public const string XML_VALUE2 = "value";
        public const string XML_TYPE = "type";
        public const string XML_PROBLEM = "problem";
        public const string XML_PATH = "path";

        int _nSaveCounter = 0;

        [DllImport("advapi32.dll", EntryPoint = "RegOpenKey")]
        public static extern int RegOpenKeyA(int hKey, string lpSubKey, ref int phkResult);
        [DllImport("advapi32.dll")]
        public static extern int RegCloseKey(int hKey);
        [DllImport("advapi32.dll", EntryPoint = "RegQueryInfoKey")]
        public static extern int RegQueryInfoKeyA(int hKey, string lpClass, ref int lpcbClass, int lpReserved, ref int lpcSubKeys, ref int lpcbMaxSubKeyLen, ref int lpcbMaxClassLen, ref int lpcValues, ref int lpcbMaxValueNameLen, ref int lpcbMaxValueLen, ref int lpcbSecurityDescriptor, ref FILETIME lpftLastWriteTime);
        [DllImport("advapi32.dll", EntryPoint = "RegEnumValue")]
        public static extern int RegEnumValueA(int hKey, int dwIndex, ref byte lpValueName, ref int lpcbValueName, int lpReserved, ref int lpType, ref byte lpData, ref int lpcbData);
        [DllImport("advapi32.dll", EntryPoint = "RegEnumKeyEx")]
        public static extern int RegEnumKeyExA(int hKey, int dwIndex, ref byte lpName, ref int lpcbName, int lpReserved, string lpClass, ref int lpcbClass, ref FILETIME lpftLastWriteTime);
        [DllImport("advapi32.dll", EntryPoint = "RegSetValueEx")]
        public static extern int RegSetValueExA(int hKey, string lpSubKey, int reserved, int dwType, ref byte lpData, int cbData);
        [DllImport("advapi32.dll", EntryPoint = "RegDeleteValue")]
        public static extern int RegDeleteValueA(int hKey, string lpValueName);
        [DllImport("advapi32.dll", EntryPoint = "RegDeleteKey")]
        public static extern int RegDeleteKeyA(int hKey, string lpSubKey);
        [DllImport("advapi32.dll", EntryPoint = "RegDeleteTree")]
        public static extern int RegDeleteTreeA(int hKey, string lpSubKey);

        public xmlRegistry()
        {
        }

        /// <summary>
        /// Gets the Win32 handle for a given RegistryKey.
        /// </summary>
        /// <param name="registryKey">registry key you want the handle for</param>
        /// <returns>Desired handle</returns>
        public static int getRegistryHandle(RegistryKey registryKey)
        {
            Type type = registryKey.GetType();
            FieldInfo fieldInfo = type.GetField("hkey", BindingFlags.Instance | BindingFlags.NonPublic);
            System.Runtime.InteropServices.SafeHandle i = (System.Runtime.InteropServices.SafeHandle)fieldInfo.GetValue(registryKey);
            return ((IntPtr)i.DangerousGetHandle()).ToInt32();
        }

        public static bool keyExists(string strInPath)
        {
            string strPath = strInPath;

            if (strPath.Length == 0) return false;

            string strMainKeyname = strPath;

            int nSlash = strPath.IndexOf("\\");
            if (nSlash > -1)
            {
                strMainKeyname = strPath.Substring(0, nSlash);
                strPath = strPath.Substring(nSlash + 1);
            }
            else
                strPath = "";

            return keyExists(strMainKeyname, strPath);
        }

        public static bool keyExists(string strMainKey, string strPath)
        {
            bool bKeyExists = false;
            RegistryKey reg = openKey(strMainKey, strPath, false);

            if (reg != null)
            {
                bKeyExists = true;
                reg.Close();
            }

            return bKeyExists;
        }

        public static RegistryKey openKey(string strMainKey, string strPath, bool bWritable)
        {
            RegistryKey reg = null;

            try
            {
                if (strMainKey.ToUpper().CompareTo("HKEY_CLASSES_ROOT") == 0)
                {
                    reg = Registry.ClassesRoot.OpenSubKey(strPath, bWritable);
                }
                else if (strMainKey.ToUpper().CompareTo("HKEY_CURRENT_USER") == 0)
                {
                    reg = Registry.CurrentUser.OpenSubKey(strPath, bWritable);
                }
                else if (strMainKey.ToUpper().CompareTo("HKEY_LOCAL_MACHINE") == 0)
                {
                    reg = Registry.LocalMachine.OpenSubKey(strPath, bWritable);
                }
                else if (strMainKey.ToUpper().CompareTo("HKEY_USERS") == 0)
                {
                    reg = Registry.Users.OpenSubKey(strPath, bWritable);
                }
                else if (strMainKey.ToUpper().CompareTo("HKEY_CURRENT_CONFIG") == 0)
                {
                    reg = Registry.CurrentConfig.OpenSubKey(strPath, bWritable);
                }
                else
                    return null; // break here
            }
            catch (Exception e)
            {
                ShowErrorMessage(e, "Error opening registry key");
                return null;
            }

            return reg;
        }

        ArrayList addRegistryValues(int hKey)
        {
            ArrayList arrValues = new ArrayList();

            int classLength = 0;
            int cSubKeys = 0;                 // number of subkeys 
            int cbMaxSubKey = 0;              // longest subkey size 
            int cchMaxClass = 0;              // longest class string 
            int cValues = 0;              // number of values for key 
            int cchMaxValue = 0;          // longest value name 
            int cbMaxValueData = 0;       // longest value data 
            int cbSecurityDescriptor = 0; // size of security descriptor 
            FILETIME ftLastWriteTime = new FILETIME();      // last write time 

            int j;
            int retValue;

            // Get the class name and the value count. 
            retValue = RegQueryInfoKeyA(hKey,        // key handle 
                null,					// buffer for class name 
                ref classLength,					// length of class string 
                0,                    // reserved 
                ref cSubKeys,               // number of subkeys 
                ref cbMaxSubKey,            // longest subkey size 
                ref cchMaxClass,            // longest class string 
                ref cValues,                // number of values for this key 
                ref cchMaxValue,            // longest value name 
                ref cbMaxValueData,         // longest value data 
                ref cbSecurityDescriptor,   // security descriptor 
                ref ftLastWriteTime);       // last write time 

            // Enumerate the child keys, until RegEnumKeyEx fails.
            byte[] achValueName = new byte[cchMaxValue + 1];
            byte[] achValueData = new byte[cbMaxValueData + 1];

            string strDefaultName = IDS_DEFAULTVALUENAME; // (Default)
            string strDefaultValue = IDS_DEFAULTVALUEVALUE; // (value not set)

            // Enumerate the key values. 
            if (cValues != 0 && cValues != -1 && retValue == ERROR_SUCCESS)
            {
                for (j = 0, retValue = ERROR_SUCCESS; j < cValues; j++)
                {
                    int cchValueName = cchMaxValue + 1;
                    int cchValueData = cbMaxValueData + 1;
                    int dwType = 0;

                    achValueName[0] = 0;
                    achValueData[0] = 0;

                    string sValueName = "";

                    retValue = RegEnumValueA(hKey, j, ref achValueName[0],
                        ref cchValueName,
                        0,
                        ref dwType,
                        ref achValueData[0],
                        ref cchValueData);

                    if (retValue == ERROR_SUCCESS)
                    {
                        keyValue p = new keyValue();

                        if (cchValueName == 0 && sValueName.Length == 0)
                        {
                            if (cchValueData == 0 && achValueData[0] == 0)
                            {
                                p.setKeyValue(strDefaultName,
                                    strDefaultValue,
                                    dwType);
                            }
                            else
                            {
                                p.setKeyValue(strDefaultName,
                                    convertToString(REG_SZ, achValueData, cchValueData),
                                    dwType);
                            }
                        }
                        else
                        {
                            Encoding ascii = Encoding.ASCII;

                            // Convert the new byte[] into a char[] and then into a string.
                            char[] asciiChars = new char[ascii.GetCharCount(achValueName, 0, cchValueName)];
                            ascii.GetChars(achValueName, 0, cchValueName, asciiChars, 0);
                            string asciiString = new string(asciiChars);

                            sValueName = asciiString;

                            p.setKeyValue(sValueName,
                                            convertToString(dwType, achValueData, cchValueData),
                                            dwType);
                        }

                        arrValues.Add(p);
                    }
                }
            } // end if (cValues && cValues!=-1)

            // now sort all this
            int nNbItems = arrValues.Count;

            // make sure we have at least a default value to play with
            bool bFound = false;
            int i = 0;

            while (!bFound && i < nNbItems)
            {
                keyValue p = (keyValue)arrValues[i++];
                bFound = (p.getName().CompareTo(strDefaultName) == 0);
            }

            if (!bFound)
            {
                // this is a fake add, just to visually match what's shown in regedit.exe
                keyValue p = new keyValue();
                p.setKeyValue(strDefaultName,
                                strDefaultValue,
                                REG_SZ);

                arrValues.Insert(0, p);
            }

            return arrValues;
        }

        string getEscapedXmlString(string strInput)
        {
            string s = "";
            if (strInput.Length == 0) return s;

            int nLength = strInput.Length;

            for (int i = 0; i < nLength; i++)
            {
                char c = strInput[i];

                if (c == '&')
                    s += "&amp;";
                else if (c == '"')
                    s += "&quot;";
                else if (c == '<')
                    s += "&lt;";
                else
                    s += c;
            }

            return UTF8Conversion(s);
        }

        string UTF8Conversion(string s) // local charset ==> UTF8
        {
            // Create two different encodings.
            Encoding ascii = Encoding.ASCII;
            Encoding utf8 = Encoding.UTF8;

            // Convert the string into a byte[].
            byte[] asciiBytes = ascii.GetBytes(s);

            // Perform the conversion from one encoding to the other.
            byte[] utf8Bytes = Encoding.Convert(ascii, utf8, asciiBytes);

            // Convert the new byte[] into a char[] and then into a string.
            char[] utf8Chars = new char[utf8.GetCharCount(utf8Bytes, 0, utf8Bytes.Length)];
            utf8.GetChars(utf8Bytes, 0, utf8Bytes.Length, utf8Chars, 0);
            string utf8String = new string(utf8Chars);

            return utf8String;
        }

        string getUnescapedXmlString(string strInput) // Xml escape chars (&quot; ==> ", &lt; ==> <, &amp; ==> &);
        {
            string s = "";
            if (strInput.Length == 0) return s;

            s = fromUTF8Conversion(strInput);

            int nLt;
            while ((nLt = s.IndexOf("&lt;", 0)) > -1)
                s = s.Substring(0, nLt) + "<" + s.Substring(nLt + 4);

            int nQuot;
            while ((nQuot = s.IndexOf("&quot;", 0)) > -1)
                s = s.Substring(0, nQuot) + "\"" + s.Substring(nQuot + 6);

            int nAmp;
            while ((nAmp = s.IndexOf("&amp;", 0)) > -1)
                s = s.Substring(0, nAmp) + "&" + s.Substring(nAmp + 5);

            return s;
        }

        string fromUTF8Conversion(string s) // UTF8 ==> local charset
        {
            // Create two different encodings.
            Encoding ascii = Encoding.ASCII;
            Encoding utf8 = Encoding.UTF8;

            // Convert the string into a byte[].
            byte[] utf8Bytes = utf8.GetBytes(s);

            // Perform the conversion from one encoding to the other.
            byte[] asciiBytes = Encoding.Convert(utf8, ascii, utf8Bytes);

            // Convert the new byte[] into a char[] and then into a string.
            char[] asciiChars = new char[ascii.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
            ascii.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
            string asciiString = new string(asciiChars);

            return asciiString;
        }

        string convertToString(int dwType, byte[] bRawBuffer, int nLen)
        {
            string s = "";

            // conversion from number to string
            if ((dwType >= REG_BINARY && dwType <= REG_DWORD_BIG_ENDIAN) ||
                dwType == 11 ||
                dwType == REG_RESOURCE_LIST ||
                dwType == REG_RESOURCE_REQUIREMENTS_LIST)
            {
                switch (dwType)
                {
                    case REG_BINARY:
                    case REG_RESOURCE_LIST:
                    case REG_RESOURCE_REQUIREMENTS_LIST:
                        {
                            string sByte = "";
                            for (int i = 0; i < nLen; i++)
                            {
                                byte c = bRawBuffer[i];
                                sByte = string.Format("{0:x2}", c);
                                if (s.Length > 0) s += " ";
                                s += sByte;
                            }
                        }
                        break;
                    case REG_DWORD: // == REG_DWORD_LITTLE_ENDIAN
                        {
                            byte a = bRawBuffer[3];
                            byte b = bRawBuffer[2];
                            byte c = bRawBuffer[1];
                            byte d = bRawBuffer[0];
                            s = string.Format("0x{0:x2}{1:x2}{2:x2}{3:x2}", a, b, c, d);
                            uint n = (uint)((a << 24) | (b << 16) | (c << 8) | d);
                            string sDword = "";
                            sDword = string.Format(" {0}", n);
                            s += sDword;
                        }
                        break;
                    case REG_DWORD_BIG_ENDIAN:
                        {
                            byte a = bRawBuffer[0];
                            byte b = bRawBuffer[1];
                            byte c = bRawBuffer[2];
                            byte d = bRawBuffer[3];
                            s = string.Format("0x{0:x2}{1:x2}{2:x2}{3:x2}", a, b, c, d);
                            uint n = (uint)((a << 24) | (b << 16) | (c << 8) | d);
                            string sDword;
                            sDword = string.Format(" {0}", n);
                            s += sDword;
                        }
                        break;
                    case 11: // QWORD, QWORD_LITTLE_ENDIAN (64-bit integer)
                        {
                            byte a = bRawBuffer[7];
                            byte b = bRawBuffer[6];
                            byte c = bRawBuffer[5];
                            byte d = bRawBuffer[4];
                            byte e = bRawBuffer[3];
                            byte f = bRawBuffer[2];
                            byte g = bRawBuffer[1];
                            byte h = bRawBuffer[0];
                            s = string.Format("0x{0:x2}{1:x2}{2:x2}{3:x2}{4:x2}{5:x2}{6:x2}{7:x2}", a, b, c, d, e, f, g, h);
                        }
                        break;
                }
            }
            else
            {
                if (dwType == REG_LINK)
                {
                    // convert the Unicode string to local charset string
                    // Create two different encodings.
                    Encoding ascii = Encoding.ASCII;
                    Encoding unicode = Encoding.Unicode;

                    // Perform the conversion from one encoding to the other.
                    byte[] asciiBytes = Encoding.Convert(unicode, ascii, bRawBuffer);

                    // Convert the new byte[] into a char[] and then into a string.
                    char[] asciiChars = new char[ascii.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
                    ascii.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
                    string asciiString = new string(asciiChars);

                    s = asciiString;
                }
                else if (dwType == REG_MULTI_SZ)
                {
                    // a MULTI_str value is a set of strings separated by a 0 char, and
                    // finishes with a double 0
                    for (int i = 0; i < nLen - 2; i++) // nLen-1 instead of nLen, because we don't care the second 0 of the double 0
                    {
                        if (bRawBuffer[i] == 0)
                            s += "\r\n";
                        else
                            s += (char)bRawBuffer[i];
                    }
                }
                else
                {
                    Encoding ascii = Encoding.ASCII;

                    // Convert the new byte[] into a char[] and then into a string.
                    char[] asciiChars = new char[ascii.GetCharCount(bRawBuffer, 0, nLen)];
                    ascii.GetChars(bRawBuffer, 0, nLen, asciiChars, 0);
                    string asciiString = new string(asciiChars);

                    s = asciiString;
                }
            }

            return s;
        }

        // this function is the dual of convertToString
        byte[] convertFromString(string strValue, int dwType)
        {
            int nLen = 0;
            byte[] bRawBuffer = null;

            // real conversion starts here
            if (dwType == REG_SZ || dwType == REG_EXPAND_SZ)
            {
                Encoding ascii = Encoding.ASCII;

                // Convert the string into a byte[].
                bRawBuffer = ascii.GetBytes(strValue.TrimEnd() + " ");
                bRawBuffer[bRawBuffer.Length - 1] = 0;
            }
            else if (dwType == REG_BINARY || dwType == REG_RESOURCE_LIST || dwType == REG_RESOURCE_REQUIREMENTS_LIST)
            {
                if (strValue.Length == 0) // stupid case handling
                {
                    bRawBuffer = new byte[1];
                    bRawBuffer[0] = 0;
                }
                else
                {
                    int n = (strValue.Length + 1) / 3;
                    byte[] buffer = new byte[n];
                    if (buffer != null)
                    {
                        nLen = n;
                        int ncur = 0;

                        for (int i = 0; i < n; i++)
                        {
                            buffer[i] = convertToDecimal(strValue[ncur], strValue[ncur + 1]);
                            ncur += 3;
                        }

                        bRawBuffer = buffer;
                    }
                }
            }
            else if (dwType == REG_DWORD)
            {
                if (strValue.Length == 0) // stupid case handling
                {
                    byte[] buffer = new byte[4];
                    if (buffer != null)
                    {
                        buffer[0] = 0; buffer[1] = 0; buffer[2] = 0; buffer[3] = 0;
                        nLen = 4;
                        bRawBuffer = buffer;
                    }
                }
                else
                {
                    int n = 4;
                    byte[] buffer = new byte[n];
                    if (buffer != null)
                    {
                        nLen = n;
                        int ncur = 2; // starts of offset 2 since we have 0x

                        for (int i = 0; i < n; i++)
                        {
                            buffer[n - 1 - i] = convertToDecimal(strValue[ncur], strValue[ncur + 1]);
                            ncur += 2;
                        }

                        bRawBuffer = buffer;
                    }
                }
            }
            else if (dwType == 11)
            {
                if (strValue.Length == 0) // stupid case handling
                {
                    byte[] buffer = new byte[8];
                    if (buffer != null)
                    {
                        buffer[0] = 0; buffer[1] = 0; buffer[2] = 0; buffer[3] = 0;
                        buffer[4] = 0; buffer[5] = 0; buffer[6] = 0; buffer[7] = 0;
                        nLen = 8;
                        bRawBuffer = buffer;
                    }
                }
                else
                {
                    int n = 8;
                    byte[] buffer = new byte[n];
                    if (buffer != null)
                    {
                        nLen = n;
                        int ncur = 2; // starts of offset 2 since we have 0x

                        for (int i = 0; i < n; i++)
                        {
                            buffer[n - 1 - i] = convertToDecimal(strValue[ncur], strValue[ncur + 1]);
                            ncur += 2;
                        }

                        bRawBuffer = buffer;
                    }
                }
            }
            else if (dwType == REG_LINK)
            {
                // convert the local charset string to Unicode string
                // Create two different encodings.
                Encoding ascii = Encoding.ASCII;
                Encoding unicode = Encoding.Unicode;

                // Perform the conversion from one encoding to the other.
                bRawBuffer = Encoding.Convert(ascii, unicode, bRawBuffer);
            }
            else if (dwType == REG_MULTI_SZ)
            {
                // strValue : the MULTI_str value is strings separated by \r\n (0x0A0x0D)

                // buffer : a MULTI_str value is a set of strings separated by a 0 char, and
                // finishes with a double 0

                // strValue has a greater length than buffer since \r\n is 2 chars, while a 0-EOL is only one
                // buf buffer has also a trailing EOL
                byte[] buffer = new byte[strValue.Length + 2];
                if (buffer != null)
                {
                    int n = 0;

                    int nstrvaluelen = strValue.Length;
                    for (int i = 0; i < nstrvaluelen; i++)
                    {
                        char c = strValue[i];
                        if (c != '\r' && c != '\n')
                        {
                            buffer[n++] = (byte)c;
                        }
                        else if (c == '\n')
                        {
                            buffer[n++] = 0;
                        }
                    }

                    buffer[n++] = 0;
                    buffer[n++] = 0;

                    if (nstrvaluelen == 0) n = 1;

                    nLen = n;
                    bRawBuffer = buffer; // -SD, may need to account for actual buffer length in nLen
                }
            }

            return bRawBuffer;
        }

        byte convertToDecimal(char c1, char c2)
        {
            byte n1 = 0, n2 = 0;

            if (c1 >= '0' && c1 <= '9')
                n1 = (byte)(c1 - '0');
            else if (c1 >= 'A' && c1 <= 'F')
                n1 = (byte)(c1 - 'A' + 10);
            else if (c1 >= 'a' && c1 <= 'f')
                n1 = (byte)(c1 - 'a' + 10);

            if (c2 >= '0' && c2 <= '9')
                n2 = (byte)(c2 - '0');
            else if (c2 >= 'A' && c2 <= 'F')
                n2 = (byte)(c2 - 'A' + 10);
            else if (c2 >= 'a' && c2 <= 'f')
                n2 = (byte)(c2 - 'a' + 10);

            return (byte)(n1 * 16 + n2);
        }

        string stringFromValueType(int nType)
        {
            string s = "";

            switch (nType) // see winnt.h
            {
                case REG_BINARY: s = "REG_BINARY"; break;
                case REG_DWORD: s = "REG_DWORD"; break;
                case REG_DWORD_BIG_ENDIAN: s = "REG_DWORD"; break;
                case REG_EXPAND_SZ: s = "REG_EXPAND_SZ"; break;
                case REG_LINK: s = "REG_SZ"; break;
                case REG_MULTI_SZ: s = "REG_MULTI_SZ"; break;
                case REG_NONE: s = "REG_SZ"; break;
                case 11: s = "REG_QWORD"; break; // QWORD (64-bit integer)
                case REG_RESOURCE_LIST: s = "REG_RESOURCE_LIST"; break;
                case REG_RESOURCE_REQUIREMENTS_LIST: s = "REG_RESOURCE_REQUIREMENTS_LIST"; break;
                case REG_SZ: s = "REG_SZ"; break;
            }

            return s;
        }

        // this function is the dual of StringFromValueType
        int typeFromString(string strValueType)
        {
            int nType = REG_SZ; // default type

            if (strValueType.ToUpper().CompareTo("REG_BINARY") == 0)
                nType = REG_BINARY;
            else if (strValueType.ToUpper().CompareTo("REG_DWORD") == 0)
                nType = REG_DWORD;
            else if (strValueType.ToUpper().CompareTo("REG_SZ") == 0)
                nType = REG_SZ;
            else if (strValueType.ToUpper().CompareTo("REG_EXPAND_SZ") == 0)
                nType = REG_EXPAND_SZ;
            else if (strValueType.ToUpper().CompareTo("REG_MULTI_SZ") == 0)
                nType = REG_MULTI_SZ;
            else if (strValueType.ToUpper().CompareTo("REG_QWORD") == 0)
                nType = 11; // QWORD (64-bit integer)
            else if (strValueType.ToUpper().CompareTo("REG_RESOURCE_LIST") == 0)
                nType = REG_RESOURCE_LIST;
            else if (strValueType.ToUpper().CompareTo("REG_RESOURCE_REQUIREMENTS_LIST") == 0)
                nType = REG_RESOURCE_REQUIREMENTS_LIST;

            return nType;
        }

        public bool saveAsXml(xmlWriter w, bool bFakedXml, string strInPath, string strLimitValue, string strProblem)
        {
            string strPath = strInPath;

            if (strPath.Length == 0) return false;

            string strMainKeyname = strPath;

            int nSlash = strPath.IndexOf("\\");
            if (nSlash > -1)
            {
                strMainKeyname = strPath.Substring(0, nSlash);
                strPath = strPath.Substring(nSlash + 1);
            }
            else
                strPath = "";

            // open the key now
            RegistryKey reg = openKey(strMainKeyname, strPath, false);

            if (reg != null) // it's ok
            {
                // write the main key here
                ArrayList arKeyPath = new ArrayList();

                int nIndexSlash = 0, i;
                string strTmpPath = strInPath;

                do
                {
                    nIndexSlash = strTmpPath.IndexOf("\\", 0);
                    if (nIndexSlash > -1)
                    {
                        arKeyPath.Add(strTmpPath.Substring(0, nIndexSlash++));
                        strTmpPath = strTmpPath.Substring(nIndexSlash);
                    }
                    else
                        arKeyPath.Add(strTmpPath);
                }
                while (nIndexSlash > -1);

                int nSize = arKeyPath.Count;
                bool bResult = true;

                if (!bFakedXml)
                {
                    for (i = 0; i < nSize; i++)
                    {
                        xmlElement wkey = new xmlElement(XML_KEY);
                        wkey.addAttrib(getEscapedXmlString(XML_NAME), getEscapedXmlString((string)arKeyPath[i]));
                        if (i == 0)
                        {
                            wkey.addAttrib(getEscapedXmlString(XML_PROBLEM), getEscapedXmlString(strProblem));
                            if (string.IsNullOrEmpty(strLimitValue))
                                wkey.addAttrib(getEscapedXmlString(XML_PATH), getEscapedXmlString(strInPath));
                        }
                        wkey.write(w, 1, false, true);
                    }

                    bResult = saveAsXml(w, bFakedXml, "", getRegistryHandle(reg), strLimitValue);

                    for (i = 0; i < nSize; i++)
                    {
                        xmlElement wkey = new xmlElement(XML_KEY);
                        wkey.writeClosingTag(w, -1, false, true);
                    }
                }
                else
                {

                    nSize--;

                    for (i = 0; i < nSize; i++)
                    {
                        xmlElement wkey = new xmlElement((string)arKeyPath[i]);
                        wkey.write(w, 1, false, true);
                    }

                    bResult = saveAsXml(w, bFakedXml, (string)arKeyPath[nSize], getRegistryHandle(reg), strLimitValue);

                    for (i = 0; i < nSize; i++)
                    {
                        xmlElement wkey = new xmlElement((string)arKeyPath[nSize - 1 - i]);
                        wkey.writeClosingTag(w, -1, false, true);
                    }
                }

                reg.Close();

                return bResult;
            }

            return false;
        }

        bool saveAsXml(xmlWriter w, bool bFakedXml, string strKeyname, int hKey, string strLimitValue)
        {
            // write key name
            int classLength = 0;
            int cSubKeys = 0;                 // number of subkeys 
            int cbMaxSubKey = 0;              // longest subkey size 
            int cchMaxClass = 0;              // longest class string 
            int cValues = 0;              // number of values for key 
            int cchMaxValue = 0;          // longest value name 
            int cbMaxValueData = 0;       // longest value data 
            int cbSecurityDescriptor = 0; // size of security descriptor 
            FILETIME ftLastWriteTime = new FILETIME();      // last write time 

            int i;
            int retCode;


            // Get the class name and the value count. 
            retCode = RegQueryInfoKeyA(hKey,        // key handle 
                null,					// buffer for class name 
                ref classLength,					// length of class string 
                0,                    // reserved 
                ref cSubKeys,               // number of subkeys 
                ref cbMaxSubKey,            // longest subkey size 
                ref cchMaxClass,            // longest class string 
                ref cValues,                // number of values for this key 
                ref cchMaxValue,            // longest value name 
                ref cbMaxValueData,         // longest value data 
                ref cbSecurityDescriptor,   // security descriptor 
                ref ftLastWriteTime);       // last write time 


            if (retCode != ERROR_SUCCESS) return false;

            // Enumerate the child keys, until RegEnumKeyEx fails. 
            byte[] achKey = new byte[cbMaxSubKey + 1];

            xmlElement wkey = new xmlElement(XML_KEY);

            if (strKeyname.Length > 0)
            {
                if (!bFakedXml) // standard xml
                {
                    wkey.addAttrib(getEscapedXmlString(XML_NAME),
                                    getEscapedXmlString(strKeyname));

                    if (cSubKeys > 0 || cValues > 0)
                        wkey.write(w, 1, false, true);
                    else
                        wkey.writeEmpty(w, 1, false, true);
                }
                else // faked xml
                {
                    wkey.setName(getEscapedXmlString(strKeyname));
                }
            }


            // each 50 values, we pump a window message, to check out whether the user hit ESCAPE
            if ((_nSaveCounter++ % 50) == 0)
            {
                // this could be done....
            }

            // save values
            ArrayList arrValues = addRegistryValues(hKey);

            int nbItems = arrValues.Count;
            for (int j = 0; j < nbItems; j++)
            {
                keyValue p = (keyValue)arrValues[j];

                if (strLimitValue.Length != 0 && p.getName().CompareTo(strLimitValue) != 0) continue;

                if (p.getName().CompareTo(IDS_DEFAULTVALUENAME) != 0 || p.getValue().CompareTo(IDS_DEFAULTVALUEVALUE) != 0)
                {
                    int dwType = p.getType();

                    if (!bFakedXml) // standard xml
                    {
                        xmlElement wvalue = new xmlElement(XML_VALUE);

                        wvalue.addAttrib(XML_NAME, getEscapedXmlString(p.getName()));
                        wvalue.addAttrib(XML_VALUE2, getEscapedXmlString(p.getValue()));

                        if (dwType != REG_SZ && dwType != REG_NONE)
                        {
                            wvalue.addAttrib(XML_TYPE, getEscapedXmlString(stringFromValueType(dwType)));
                        }

                        wvalue.writeEmpty(w, 1, false, true);
                    }
                    else // faked xml
                    {
                        wkey.addAttrib(getEscapedXmlString(p.getName()), getEscapedXmlString(p.getValue()));
                    }

                }
            }

            if (strKeyname.Length != 0)
            {
                if (bFakedXml)
                {
                    if (cSubKeys > 0)
                        wkey.write(w, 1, false, true);
                    else
                        wkey.writeEmpty(w, 1, false, true);
                }
            }

            for (i = 0, retCode = ERROR_SUCCESS; retCode == ERROR_SUCCESS; i++)
            {
                int achKeyMaxLength = cbMaxSubKey + 1;

                retCode = RegEnumKeyExA(hKey,
                            i,
                            ref achKey[0],
                            ref achKeyMaxLength,
                            0,
                            null,
                            ref cchMaxClass,
                            ref ftLastWriteTime);

                if (retCode == ERROR_SUCCESS && achKeyMaxLength > 0)
                {
                    achKey[achKeyMaxLength] = 0; // force EOL

                    Encoding ascii = Encoding.ASCII;

                    // Convert the new byte[] into a char[] and then into a string.
                    char[] asciiChars = new char[ascii.GetCharCount(achKey, 0, achKeyMaxLength)];
                    ascii.GetChars(achKey, 0, achKeyMaxLength, asciiChars, 0);
                    string sKeyName = new string(asciiChars);

                    // open sub keys
                    int hSubkey = 0;
                    if (RegOpenKeyA(hKey, sKeyName, ref hSubkey) == ERROR_SUCCESS)
                    {
                        if (!saveAsXml(w, bFakedXml, sKeyName, hSubkey, strLimitValue))
                        {
                            return false;
                        }

                        RegCloseKey(hSubkey);
                    }
                }
            }  // end for

            if (strKeyname.Length != 0)
            {
                if (!bFakedXml)
                {
                    if (cSubKeys > 0 || cValues > 0)
                        wkey.writeClosingTag(w, -1, false, true);
                }
                else
                {
                    // with faked xml, we only need to actually close the tag when there
                    // are keys under it, otherwise, we did a WriteEmpty above.
                    if (cSubKeys > 0)
                        wkey.writeClosingTag(w, -1, false, true);
                }
            }

            return true;
        }

        RegistryKey LoadAsXml_OpenKey(ArrayList arrInternalKeyLoads)
        {
            RegistryKey reg = null;

            if (arrInternalKeyLoads.Count == 0) return null; // come on!

            // build the real path from the array of keynames
            // open the key now
            string strMainKey = "", strPath = "";
            int nSize = arrInternalKeyLoads.Count;

            for (int i = 0; i < nSize; i++)
            {
                InternalKeyLoad p = (InternalKeyLoad)arrInternalKeyLoads[i];
                if (i == 0)
                {
                    strMainKey = p.strKeyname;
                }
                else
                {
                    if (strPath.Length > 0)
                        strPath += "\\";

                    strPath += p.strKeyname;
                }
            } // end for

            try
            {
                // open the key now
                reg = openKey(strMainKey, strPath, true);

                if (reg != null) // it's ok
                    return reg;

                // key does not exist, so let's create it !
                if (strMainKey.ToUpper().CompareTo("HKEY_CLASSES_ROOT") == 0)
                {
                    reg = Registry.ClassesRoot.CreateSubKey(strPath);
                }
                else if (strMainKey.ToUpper().CompareTo("HKEY_CURRENT_USER") == 0)
                {
                    reg = Registry.CurrentUser.CreateSubKey(strPath);
                }
                else if (strMainKey.ToUpper().CompareTo("HKEY_LOCAL_MACHINE") == 0)
                {
                    reg = Registry.LocalMachine.CreateSubKey(strPath);
                }
                else if (strMainKey.ToUpper().CompareTo("HKEY_USERS") == 0)
                {
                    reg = Registry.Users.CreateSubKey(strPath);
                }
                else if (strMainKey.ToUpper().CompareTo("HKEY_CURRENT_CONFIG") == 0)
                {
                    reg = Registry.CurrentConfig.CreateSubKey(strPath);
                }
                else
                    return null; // break here
            }
            catch (Exception e)
            {
                ShowErrorMessage(e, "Error Creating Sub Key");
                return null;
            }

            if (reg != null) // it's ok
                return reg;

            MessageBox.Show(string.Format("couldn't create key {0}\\{1}", strMainKey, strPath), Application.ProductName,
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            return null;
        }

        void LoadAsXml_SetValue(int hKey, string strName, string strValue, string strType)
        {
            if (hKey == 0) return;

            // handle the (Default) value
            strName = strName.ToUpper().CompareTo(IDS_DEFAULTVALUENAME) == 0 ? "" : strName;

            int nType = typeFromString(strType);
            byte[] buffer = null;
            int hr = 0;

            buffer = convertFromString(strValue, nType);

            if (buffer != null && buffer.Length > 0)
            {
                hr = RegSetValueExA(
                    hKey,           // handle to key
                    strName,		// value name
                    0,				// reserved
                    nType,			// value type
                    ref buffer[0],	// value data
                    buffer.Length   // size of value data
                );

                if (hr == ERROR_SUCCESS) // it's ok
                {
                    buffer = null;
                    return;
                }
            }

            MessageBox.Show(string.Format("couldn't set value {0}", strName), Application.ProductName,
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        void DeleteAsXml_DeleteKey(int hKey, string strName)
        {
            if (hKey == 0) return;

            int hr = 0;

            hr = RegDeleteKeyA(hKey, // handle to key
                strName); // key name

            if (hr == ERROR_SUCCESS)
                return;

            MessageBox.Show(string.Format("couldn't delete key {0}, returned: {1}", strName, hr), Application.ProductName,
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        void DeleteAsXml_DeleteValue(int hKey, string strName)
        {
            if (hKey == 0) return;

            // handle the (Default) value
            strName = strName.ToUpper().CompareTo(IDS_DEFAULTVALUENAME) == 0 ? "" : strName;

            int hr = 0;

            hr = RegDeleteValueA(hKey,  // handle to key
                strName); // value name

            if (hr == ERROR_SUCCESS) // it's ok
                return;

            MessageBox.Show(string.Format("couldn't delete value {0}, returned: {1}", strName, hr), Application.ProductName,
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        void DeleteAsXml_DeleteTree(int hKey)
        {
            if (hKey == 0) return;

            int hr = 0;

            hr = RegDeleteTreeA(hKey,  // handle to key
                null); // deletes values and keys recursively

            if (hr == ERROR_SUCCESS) // it's ok
                return;

            MessageBox.Show(string.Format("couldn't delete tree, returned: {0}", hr), Application.ProductName,
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        public bool loadAsXml(xmlReader r, string strFilename)
        {
            bool bReturn = true;
            ArrayList arrInternalKeyLoads = new ArrayList();
            string strValueName = "", strValueValue = "", strValueType = "";

            r.open(strFilename);

            while (r.readString() && bReturn)
            {
                switch (r.getNodeType())
                {
                    case NODETYPE.NODETYPE_BEGINELEMENT:
                        {
                            string strElement = r.getNodeName();
                            strElement = getUnescapedXmlString(strElement);
                        }
                        break;
                    case NODETYPE.NODETYPE_ENDELEMENT:
                        {
                            string strElement = r.getNodeName();
                            strElement = getUnescapedXmlString(strElement);

                            // close the latest key now
                            if (strElement.CompareTo(XML_KEY) == 0 && arrInternalKeyLoads.Count > 0)
                            {
                                // retrieve latest key
                                InternalKeyLoad p = (InternalKeyLoad)arrInternalKeyLoads[arrInternalKeyLoads.Count - 1];
                                p.reg.Close();
                                arrInternalKeyLoads.RemoveAt(arrInternalKeyLoads.Count - 1);
                            }
                            else if (strElement.CompareTo(XML_VALUE) == 0 && arrInternalKeyLoads.Count > 0)  // <v .../>
                            {
                                // now set/create the value
                                InternalKeyLoad p = (InternalKeyLoad)arrInternalKeyLoads[arrInternalKeyLoads.Count - 1];
                                LoadAsXml_SetValue(getRegistryHandle(p.reg), strValueName, strValueValue, strValueType);

                                strValueName = "";
                                strValueValue = "";
                                strValueType = "";
                            }
                        }
                        break;
                    case NODETYPE.NODETYPE_ATTRIB:
                        {
                            string strName, strValue;

                            strName = r.getAttribName();
                            strValue = r.getAttribValue();

                            strName = getUnescapedXmlString(strName);
                            strValue = getUnescapedXmlString(strValue);

                            string strCurrentElement = r.getNodeName();

                            // <k name="..."> ?
                            if (strName.CompareTo(XML_NAME) == 0 && strCurrentElement.CompareTo(XML_KEY) == 0)
                            {
                                InternalKeyLoad p = new InternalKeyLoad();
                                p.strKeyname = strValue;

                                arrInternalKeyLoads.Add(p);

                                // open the key, and store the handle
                                p.reg = LoadAsXml_OpenKey(arrInternalKeyLoads);

                                arrInternalKeyLoads.RemoveAt(arrInternalKeyLoads.Count - 1);
                                arrInternalKeyLoads.Add(p);

                                if (p.reg == null)
                                    bReturn = false; // abort the process
                            }
                            else if (strCurrentElement.CompareTo(XML_VALUE) == 0) // <v name="..." value="..." type="..." />
                            {
                                if (strName.CompareTo(XML_NAME) == 0) // name="..."
                                {
                                    strValueName = strValue;
                                    strValueValue = "";
                                    strValueType = "";
                                }
                                else if (strName.CompareTo(XML_VALUE2) == 0) // value="..."
                                {
                                    strValueValue = strValue;
                                    strValueType = "";
                                }
                                else if (strName.CompareTo(XML_TYPE) == 0) // type="..."
                                {
                                    strValueType = strValue;
                                }
                            }
                        }
                        break;
                    // other nodetypes : we don't care
                }
            } // end read lines one by one

            r.close();

            return bReturn;
        }

        /// <summary>
        /// Parses XML File and deletes keys and values
        /// </summary>
        /// <param name="r">XML Reader Class</param>
        /// <param name="listView">Results list view</param>
        /// <returns></returns>
        public bool deleteAsXml(ListView listView, string strBackupFile)
        {
            int i;
            xmlWriter w = new xmlWriter();

            // Write opening tags to Backup File
            if (!w.open(strBackupFile))
                return false;
            xmlElement wroot = new xmlElement(xmlRegistry.XML_ROOT);
            wroot.write(w, 1, false, true);

            for (i = 0; i < listView.Items.Count; i++)
            {
                if (listView.Items[i].Checked)
                {
                    string strValueName = string.Empty;

                    string strProblem = listView.Items[i].SubItems[0].Text;
                    string strKey = listView.Items[i].SubItems[1].Text;
                    if (listView.Items[i].SubItems.Count > 2)
                        strValueName = listView.Items[i].SubItems[2].Text;

                    if (xmlRegistry.keyExists(strKey))
                        this.saveAsXml(w, false, strKey, strValueName, strProblem);
                } 
            }

            // Write Closing Tag to Backup File
            wroot.writeClosingTag(w, -1, false, true);
            w.close();

            // Remove problems from registry
            for (i = 0; i < listView.Items.Count; i++)
            {
                if (listView.Items[i].Checked)
                {
                    string strValueName = string.Empty;

                    string strProblem = listView.Items[i].SubItems[0].Text;
                    string strKey = listView.Items[i].SubItems[1].Text;
                    if (listView.Items[i].SubItems.Count > 2)
                        strValueName = listView.Items[i].SubItems[2].Text;

                    deleteRegistryKey(strKey.Substring(0, strKey.IndexOf('\\')), strKey.Substring(strKey.IndexOf('\\') + 1), strValueName);
                }
            }

            return true;
        }

        bool deleteRegistryKey(string strBaseKey, string strSubKey, string strLimitValue)
        {
            RegistryKey regKey = openKey(strBaseKey, strSubKey, true);

            if (regKey == null)
                return false;

            if (!string.IsNullOrEmpty(strLimitValue))
                DeleteAsXml_DeleteValue(getRegistryHandle(regKey), strLimitValue);
            else
            {
                string strMainSubKey = strSubKey.Substring(0, strSubKey.LastIndexOf('\\'));
                string strDelSubKey = strSubKey.Substring(strSubKey.LastIndexOf('\\') + 1);

                RegistryKey regKey2 = openKey(strBaseKey, strMainSubKey, true);
                if (regKey2 != null)
                {
                    regKey2.DeleteSubKeyTree(strDelSubKey);
                    regKey2.Close();
                }
            }

            regKey.Close();

            return true;
        }

        /// <summary>
        /// Shows error dialog
        /// </summary>
        /// <param name="e">Exception class</param>
        /// <param name="strTitle">Title of dialog</param>
        private static void ShowErrorMessage(Exception e, string strTitle)
        {
#if (DEBUG)
            System.Diagnostics.Debug.WriteLine(e.Message);
            ErrorDlg dlgError = new ErrorDlg(e, strTitle);
            dlgError.ShowDialog();
#endif
            return;
        }
    }
}
