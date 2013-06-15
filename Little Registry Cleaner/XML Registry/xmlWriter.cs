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
using System.IO;
using System.Windows.Forms;

namespace Little_Registry_Cleaner.Xml
{
	/// <summary>
	/// Summary description for xmlWriter.
	/// </summary>
	public class xmlWriter
	{
		StreamWriter _sw = null;
		bool _bFileOpen = false, _bPrototypeWritten;
		string _strFilename;
		int _nLevel;

		public xmlWriter()
		{
			init();
		}

		public void setIndentLevel(int n) 
		{ 
			_nLevel = n; 
		}
		
		public int getIndentLevel() 
		{ 
			return _nLevel; 
		}
		
		public string getFilename() 
		{ 
			return _strFilename; 
		}

		public bool mustBeClosed() 
		{ 
			return _bFileOpen; 
		}

		void init()
		{
			setIndentLevel(0);
			_bFileOpen = false;
			_bPrototypeWritten = false;
		}

		public bool open(string strFilename)
		{
			if (_bFileOpen)
				return true;

			_strFilename = strFilename;

			return true;
		}

		public bool writeString(string strData)
		{
			if (!_bFileOpen) // open file
			{
				try
				{
					_sw = File.CreateText(_strFilename);
					_bFileOpen = true;
				}
				catch (Exception ) 
				{
					_bFileOpen = false;
				}
			}
			
			if (!_bFileOpen)
				return false;

			if (!_bPrototypeWritten)
			{
				_bPrototypeWritten = true; // make sure to set this flag to true before we call a reentrant method such like .WritePInstruction

				// write XML prototype, once and only once
				//
				int nOldIndent = getIndentLevel();
				setIndentLevel(0);

				xmlElement xml = new xmlElement("xml");
				xml.addAttrib( "version", "1.0" );
				xml.addAttrib( "encoding", "UTF-8" );
				xml.writePInstruction(this,0);
					
				setIndentLevel(nOldIndent);
			}

			// actual write
			_sw.Write(strData);

			return true;
		}

		public bool close()
		{
			if (_sw!=null && _bFileOpen)
			{
				_sw.Close();
			}
			
			init();

			return true;
		}
	}
}
