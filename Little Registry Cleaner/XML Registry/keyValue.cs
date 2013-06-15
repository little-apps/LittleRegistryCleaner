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

namespace Little_Registry_Cleaner.Xml
{
	/// <summary>
	/// Summary description for keyValue.
	/// </summary>
	public class keyValue
	{
		string _strName = "";
		string _strValue = "";
		int _nType = 0;

		public keyValue()
		{
		}

		public void setKeyValue(string strName, string strValue, int nType)
		{
			_strName = strName;
			_strValue = strValue;
			_nType = nType;
		}

		public string getName() 
		{ 
			return _strName; 
		}
		
		public string getValue() 
		{ 
			return _strValue; 
		}
		
		public int getType() 
		{ 
			return _nType; 
		}
	}
}
