// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - Util.cs                                         //
//     Copyright (c) 2010-2011 DeskMetrics Limited                       //
//                                                                       //
//     http://deskmetrics.com                                            //
//     http://support.deskmetrics.com                                    //
//                                                                       //
//     support@deskmetrics.com                                           //
//                                                                       //
//     This code is provided under the DeskMetrics Modified BSD License  //
//     A copy of this license has been distributed in a file called      //
//     LICENSE with this source code.                                    //
//                                                                       //
// **********************************************************************//


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Common_Tools.DeskMetrics
{
    internal class Util
    {
        /// <summary>
        /// The method create a Base64 encoded string from a normal string.
        /// </summary>
        /// <param name="toEncode">The String containing the characters to encode.</param>
        /// <returns>The Base64 encoded string.</returns>
        public static string EncodeTo64(string toEncode)
        {
            try
            {
                byte[] toEncodeAsBytes = System.Text.Encoding.Unicode.GetBytes(toEncode);
                string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
                return returnValue;
            }
            catch
            {
                return "";
            }

        }

        /// <summary>
        /// The method to Decode your Base64 strings.
        /// </summary>
        /// <param name="encodedData">The String containing the characters to decode.</param>
        /// <returns>A String containing the results of decoding the specified sequence of bytes.</returns>
        public static string DecodeFrom64(string encodedData)
        {
            try
            {
                byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
                string returnValue = System.Text.Encoding.Unicode.GetString(encodedDataAsBytes);
                return returnValue;
            }
            catch
            {
                return "";
            }
        }


        /// <summary>
        /// Timestamp GMT +0
        /// </summary>
        public static int GetTimeStamp()
        {
            try
            {
                double _timeStamp = 0;
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                TimeSpan diff = DateTime.UtcNow - origin;
                _timeStamp = Math.Floor(diff.TotalSeconds);
                return Convert.ToInt32(_timeStamp);
            }
            catch
            {
                return 0;
            }
         
        }

        public static bool IsOnline()
        {
            Ping 
        }
    }
}
