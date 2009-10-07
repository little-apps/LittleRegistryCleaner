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
using System.Collections;
using System.Xml;

namespace Little_Registry_Cleaner.Xml
{
	///	<summary>
	///	xmlElement is used by xmlReader and xmlWriter for processing.
	///	</summary>
	public class xmlElement
	{
		public string _strName;
		private	ArrayList _arrayAttribNames, _arrayAttribValues;

		public xmlElement()
		{
			_arrayAttribNames = new ArrayList();
			_arrayAttribValues = new ArrayList();
		}

		public xmlElement(string strName)
		{
			_arrayAttribNames = new ArrayList();
			_arrayAttribValues = new ArrayList();

			setName(strName);
		}

		string helperBuildIndent(int nLevel)
		{
			string strSpaces = "";
			
			for	(int i=0;i<nLevel;i++)
				strSpaces +=	" ";

			return strSpaces;
		}

		public void setName(string strName)
		{
			_strName = strName;
		}

		string getName()
		{
			return _strName;
		}
		
		string getAttribName(int nIndex)
		{
			string strAttribName = "";
			if (nIndex>-1 && nIndex<getAttribCount())
				strAttribName = (string)_arrayAttribNames[nIndex];

			return strAttribName;
		}

		string getAttribValue(int nIndex)
		{
			string strAttribValue = "";
			if (nIndex>-1 && nIndex<_arrayAttribValues.Count)
				strAttribValue = (string)_arrayAttribValues[nIndex];

			return strAttribValue;
		}

		int	getAttribCount()
		{
			return (int)_arrayAttribNames.Count;
		}

		bool findAttrib(string strAttribName)
		{
			bool bFound	= false;
			int	i=0;
			int	nSize =	(int) _arrayAttribNames.Count;
			
			while (i<nSize && !bFound)
			{
				bFound = ((string)_arrayAttribNames[i]==strAttribName);
				i++;
			}

			return bFound;
		}

		public void addAttrib(string strAttribName, string strAttribValue)
		{
			bool bFound	= false;
			int	i=0;
			int	nSize =	(int) _arrayAttribNames.Count;
			
			while (i<nSize && !bFound)
			{
				bFound = ((string)_arrayAttribNames[i]==strAttribName);
				i++;
			}
			
			if (bFound)	// already known
			{
				i--;
				_arrayAttribValues[i] = strAttribValue;
			}
			else
			{
				_arrayAttribNames.Add( strAttribName );
				_arrayAttribValues.Add( strAttribValue	);
			}
		}

		public void write(xmlWriter writer, int nDeltaLevel, bool bIndent, bool bEOL)	// for any kind	of open	tag
		{
			writer.setIndentLevel( writer.getIndentLevel()+nDeltaLevel );

			string	s = "";
			if (bIndent)
				s = helperBuildIndent(writer.getIndentLevel());
			
			s += "<";
			s += _strName;
			int	i;
			int	nCount = getAttribCount();
	
			for	(i=0;i<nCount;i++)
			{
				s += " "; // separator between attribute pairs
				s += _arrayAttribNames[i];
				s += "=\"";
				s += _arrayAttribValues[i];
				s += "\"";
			}

			s += ">";
			if (bEOL)
			s += "\r\n"; //	ENDL
			
			writer.writeString(	s );
		}

        public void write(XmlWriter writer)
        {
            writer.WriteStartElement(_strName);

            for (int i = 0; i < getAttribCount(); i++)
                writer.WriteAttributeString((string)_arrayAttribNames[i], (string)_arrayAttribValues[i]);
        }

        public void writeEmpty(xmlWriter writer, int nDeltaLevel, bool bIndent, bool bEOL)
		{
			writer.setIndentLevel( writer.getIndentLevel()+nDeltaLevel );

			string	s = "";
			if (bIndent)
				helperBuildIndent(writer.getIndentLevel());
			
			s += "<";
			s += _strName;
			int	i;
			int	nCount = getAttribCount();
			
			for	(i=0;i<nCount;i++)
			{
				s += " "; // separator between attribute pairs
				s += _arrayAttribNames[i];
				s += "=\"";
				s += _arrayAttribValues[i];
				s += "\"";
			}

			s += "></";
			s += _strName;
			s += ">";

			if (bEOL)
			s += "\r\n"; //	ENDL
			writer.writeString(	s );

			writer.setIndentLevel( writer.getIndentLevel()-nDeltaLevel );
		}

        public void writeEmpty(XmlWriter writer)
        {
            writer.WriteStartElement(_strName);

            for (int i = 0; i < getAttribCount(); i++)
                writer.WriteAttributeString((string)_arrayAttribNames[i], (string)_arrayAttribValues[i]);

            writer.WriteEndElement();
        }

		public void writePInstruction(xmlWriter writer, int nDeltaLevel)
		{
			writer.setIndentLevel( writer.getIndentLevel()+nDeltaLevel );

			string	s = "";
			
			s = helperBuildIndent(writer.getIndentLevel());
			
			s += "<?";
			s += _strName;
			
			int	i;
			int	nCount = getAttribCount();
			
			for	(i=0;i<nCount;i++)
			{
				s += " "; // separator between attribute pairs
				s += _arrayAttribNames[i];
				s += "=\"";
				s += _arrayAttribValues[i];
				s += "\"";
			}

			s += "?>";
			s += "\r\n"; //	ENDL
			
			writer.writeString(	s );
		}

        public void writePInstruction(XmlWriter writer)
        {
            string s = "";

            int i;
            int nCount = getAttribCount();

            for (i = 0; i < nCount; i++)
            {
                s += _arrayAttribNames[i];
                s += "=\"";
                s += _arrayAttribValues[i];
                s += "\" "; // separator between attribute pairs
            }

            writer.WriteProcessingInstruction(_strName, s);
        }

		public void writeClosingTag(xmlWriter writer, int nDeltaLevel, bool bIndent, bool bEOL)
		{
			string	s = "";
			
			if (bIndent)
				s = helperBuildIndent(writer.getIndentLevel());

			s += "</";
			s += _strName;
			s += ">";
			if (bEOL)
			s += "\r\n"; //	ENDL
			writer.writeString(	s );

			writer.setIndentLevel( writer.getIndentLevel()+nDeltaLevel );

		}

        public void writeClosingTag(XmlWriter writer)
        {
            writer.WriteEndElement();
        }
	}
}
