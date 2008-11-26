/*
    Little Registry Cleaner
    Copyright (C) 2008 Little Apps (http://www.littleapps.co.cc/)

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

public enum NODETYPE
{
	NODETYPE_NA = 0,
	NODETYPE_BEGINELEMENT = 1,
	NODETYPE_ENDELEMENT = 2,
	NODETYPE_ATTRIB = 3,
	NODETYPE_CONTENT = 4,
	NODETYPE_PI = 5,
	NODETYPE_COMMENT = 6,
	NODETYPE_CDATA = 7
};

namespace Little_Registry_Cleaner.Xml
{
	/// <summary>
	/// xmlReader reads and parses an XML file
	/// </summary>
	public class xmlReader
	{
		public const string IDS_EMPTYELEMENTNAME = "Empty element name";
		public const string IDS_BADBEGINNODESYMBOL = "Bad '<' symbol";
		public const string IDS_NOBEGINNODESYMBOLINEOL = "'<' symbol not allowed at the end of a line";
		public const string IDS_GENERICSYNTAXERROR = "Syntax error";
		public const string IDS_MISSINGATTRIBNAME = "Missing attribute name before '='";
		public const string IDS_MISSINGEQUALSYMBOL = "Missing '=' after attribute name";
		public const string IDS_NOEQUALSYMBOLINEOL = "There should not be a '=' symbol at the end of a line";
		public const string IDS_BADATTRIBUTEVALUESYNTAX = "There should not be a quote char at the end of a line";

		StreamReader _sr; // general file members
		bool		_bFileOpen;
		string		_strFilename;
        bool        _bShowMsgBoxOnError; // true if message boxes are allowed to display while parsing
		string		_strLastError; // filled with error description if ParseContent() returns false

		//
		string		_strContent; // internal use : parser buffer
		int		_nCursor, _nbLines; // internal cursors (horizontal and vertical directions)
		NODETYPE	_nCurNodeType; // returns where is the parser on at the moment
		string		_strCurNodeName, _strCurNodeContent; // returns the current node value, and the current node content value
		string		_strCurAttribName, _strCurAttribValue; // returns the current attrib name/value pair
		string		_strCurPInstruction; // returns the current PInstruction (for instance ?xml, !DOCTYPE, ...)
		bool		_bCurInsideComment; // true if the parser is inside a comment ( <!-- ... -->)
		bool		_bCurInsideCDATA; // true if hte parser is inside a CDATA secrtion ( <![CDATA[[ ... ]]> )
		bool		_bCurInsideNode; // true if the parser is inside a node begin tag
		bool		_bCurInsideAttrib; // true if _strCurAttribName is valid and _strCurAttribValue is pending
		bool		_bCurInsideContent; // true if the parser is inside content

		public xmlReader()
		{
			init();
		}

		void init()
		{
			_bFileOpen = false;
#if (DEBUG)
			showMsgBoxOnError(true);
#else
            showMsgBoxOnError(false);
#endif
		}

		public void showMsgBoxOnError(bool bShow)
		{
			_bShowMsgBoxOnError = bShow;
		}

		public bool open(string strFilename)
		{
			if (_bFileOpen)
				return true;

			_strFilename = strFilename;

			return true;
		}

		public bool readString()
		{
			if (!_bFileOpen) // open file for reading
			{
				try 
				{
					_sr = File.OpenText(_strFilename);
					_bFileOpen = _sr != null;
					_nCursor = -1;
					_nbLines = 0;
					_bCurInsideNode = _bCurInsideComment = _bCurInsideAttrib = _bCurInsideContent = _bCurInsideCDATA = false;
					_strLastError = "";
					_strCurNodeName = "";
					_strCurNodeContent = "";
					_strCurAttribName = "";
					_strCurAttribValue = "";
					_strCurPInstruction = "";
				}
				catch (Exception e) 
				{
                    if (_bShowMsgBoxOnError)
                        throw new Exception("Error opening file " + _strFilename, e);

					_bFileOpen = false;
				}
			}

			if (!_bFileOpen)
				return false;

			bool bResult = true;

			if (_nCursor==-1)
			{
				_strContent = _sr.ReadLine();
				_nCursor = 0;
				_nbLines++;
			}

			if (_strContent==null)
			{
				bResult = false;
			}

			if (_strContent!=null && !parseContent())
			{
				string s;
				s = "Parse error in line "+_nbLines+" : " + _strLastError;
				_strLastError = s;

				if (_bShowMsgBoxOnError)
                    throw new Exception("Error opening file " + _strFilename, new Exception(_strLastError));

				bResult = false;
			}

			return bResult;
		}
	
		public bool close()
		{
			if (_bFileOpen)
				_sr.Close();
			
			init();

			return true;
		}

		public NODETYPE getNodeType()
		{
			return _nCurNodeType;
		}

		public string getNodeName()
		{
			return _strCurNodeName;
		}

		public string getAttribName()
		{
			return _strCurAttribName;
		}

		public string getAttribValue()
		{
			return _strCurAttribValue;
		}
		
		string getNodeContent()
		{
			return _strCurNodeContent;
		}


		string getPInstruction()
		{
			return _strCurPInstruction;
		}

		public int getCurrentLine()
		{
			return _nbLines;
		}


		public string getLastError() // if any
		{
			return _strLastError;
		}

		public bool parseContent()
		{
			_nCurNodeType = NODETYPE.NODETYPE_NA;

			int i = (int)_nCursor;
			int imax = _strContent.Length-1;

			if (i>imax)
			{
				_nCursor = -1; // force next string to be read from file
				return true;
			}

			string strTemp = _strContent + i;

			// pass spaces if we are inside a <...> and not yet processing an attribute value
			while ( (i<=imax) && 
				(_bCurInsideNode && !_bCurInsideAttrib) && 
				(_strContent[i]==' ' || _strContent[i]==0x0A || _strContent[i]==0x0D) )
					i++;

			if (i>imax)
			{
				_nCursor = -1; // force next string to be read from file
				return true;
			}

			// are we inside a comment ?
			if (_bCurInsideComment)
			{
				while ( (i<=imax-2) && 
				!(_strContent[i]=='-' && _strContent[i+1]=='-' && _strContent[i+2]=='>') )
				i++;

				if (i<=imax-2) // found an end-comment
				{
					_nCurNodeType = NODETYPE.NODETYPE_NA; // tell user we have nothing to provide him with
					_nCursor = i+2+1;

					// after '-->' we are automatically within a content
					_bCurInsideNode = _bCurInsideAttrib = _bCurInsideComment = _bCurInsideCDATA = false;
					_bCurInsideContent = true;
					_strCurNodeContent = "";

					return true;
				}
				else // we still are inside an comment
				{
					_nCurNodeType = NODETYPE.NODETYPE_COMMENT;
					_nCursor = imax+1; // force next string to be read
					return true;
				}
			}


			// are we inside a CDATA section ?
			if (_bCurInsideCDATA)
			{
				while ( (i<=imax-2) && 
				!(_strContent[i]==']' && _strContent[i+1]==']' && _strContent[i+2]=='>') )
				i++;

				if (i<=imax-2) // found an end-comment
				{
					_nCurNodeType = NODETYPE.NODETYPE_NA; // tell user we have nothing to provide him with
					_nCursor = i+2+1;

					// after ']]>' we are automatically within a content
					_bCurInsideNode = _bCurInsideAttrib = _bCurInsideComment = _bCurInsideCDATA = false;
					_bCurInsideContent = true;
					_strCurNodeContent = "";

					return true;
				}
				else // we still are inside an CDATA section
				{
					_nCurNodeType = NODETYPE.NODETYPE_CDATA;
					_nCursor = imax+1; // force next string to be read
					return true;
				}
			}


			if (_bCurInsideAttrib) // extracting the attrib value, possibly in multiple passes
			{
				if ( _strCurAttribValue.Length==0 )
				{
					// pass EOL
					while ( (i<=imax) && (_strContent[i]==' ' || _strContent[i]==0x0A || _strContent[i]==0x0D) )
					i++;

					if (i>imax)
					{
						_nCurNodeType = NODETYPE.NODETYPE_NA;
						_nCursor = i;
						return true;
					}

					char quotechar = _strContent[i++];

					_strCurAttribValue += quotechar; // start with something whatsoever!
					// in fact, we don't check the quotechar is an actual quotechar, ie " or '

					_nCurNodeType = NODETYPE.NODETYPE_NA;
					_nCursor = i;
					return true;
				}
				else
				{
					long ibegin = i;

					// pass until we find spaces or EOL or >
					while ( (i<=imax) && _strContent[i]!='\"'
					&& _strContent[i]!='\''
					&& _strContent[i]!=0x0A
					&& _strContent[i]!=0x0D
					&& _strContent[i]!='>') 
					i++;

					// TODO : properly manage the case of a multiple-line attrib-value
					// (we should in this case return a N/A nodetype as long as we haven't
					// encountered the ending quotechar, while buffering all the chars in
					// the strAttribValue member).

					long iend = i;

					_strCurAttribValue += _strContent.Substring((int)ibegin, (int)(iend-ibegin));

					if (i>imax)
					{ // don't forget to add the EOL as well
						_strCurAttribValue += "\r\n";

						_nCurNodeType = NODETYPE.NODETYPE_NA;
						_nCursor = i;
						return true;
					}

					// and remove the prefixed quote char
					while ( _strCurAttribValue.Length!=0 && 
					(_strCurAttribValue[0]=='\"' || _strCurAttribValue[0]=='\'') )
					{
						_strCurAttribValue = _strCurAttribValue.Substring(1);
					}

					_nCurNodeType = NODETYPE.NODETYPE_ATTRIB;
					_bCurInsideAttrib = false;

					if ( _strContent[i]!='>' )
						i++; // pass ending quote char

					_nCursor = i;
					return true;
				}

			} // end if _bCurInsideAttrib==true


			if (_bCurInsideContent)
			{
				long ibegin = i;

				// pass until we find spaces or EOL or >
				while ( (i<=imax) && _strContent[i]!=0x0A
					&& _strContent[i]!=0x0D
					&& _strContent[i]!='<')
						i++;

				long iend = i;

				if ( (i<=imax) && _strContent[i]=='<')
					_bCurInsideContent = false;

				_strCurNodeContent = _strContent.Substring((int)ibegin, (int)(iend-ibegin));
				if (_strCurNodeContent.Length==0)
					_nCurNodeType = NODETYPE.NODETYPE_NA;	
				else
					_nCurNodeType = NODETYPE.NODETYPE_CONTENT;

				_nCursor = i;
				return true;
			} // end if (_bCurInsideContent)

			//
			char c = _strContent[i];

			// a node ?
			if (c=='<')
			{
				if (_bCurInsideNode) // error, we were already inside one
				{
					_strLastError = IDS_BADBEGINNODESYMBOL;
					return false;
				}

				_bCurInsideNode = true;
				_bCurInsideAttrib = _bCurInsideContent = _bCurInsideComment = _bCurInsideCDATA = false;

				i++;

				// pass spaces
				while ( _strContent[i]==' ' || _strContent[i]==0x0A || _strContent[i]==0x0D)
					i++;

				if (i>imax)
				{
					_strLastError = IDS_NOBEGINNODESYMBOLINEOL;
					return false;
				}

				// here we have either a node name, a PI, or a begin comment
				if (imax-i>=2) // is it a begin comment ? ( <!-- )
				{
					if ( _strContent[i+0]=='!' &&
					_strContent[i+1]=='-' &&
					_strContent[i+2]=='-')
					{
						_nCurNodeType = NODETYPE.NODETYPE_COMMENT;
						_bCurInsideComment = true;

						i+=3; // go to actual comment content

						_nCursor = i;
						return true;
					}
				}

				if (imax-i>=7) // is it a begin cdatasection ? ( <![CDATA[ )
				{
					if ( _strContent[i+0]=='!' &&
					_strContent[i+1]=='[' &&
					_strContent[i+2]=='C' &&
					_strContent[i+3]=='D' &&
					_strContent[i+4]=='A' &&
					_strContent[i+5]=='T' &&
					_strContent[i+6]=='A' &&
					_strContent[i+7]=='[')
					{
						_nCurNodeType = NODETYPE.NODETYPE_CDATA;
						_bCurInsideCDATA = true;

						i+=8; // go to actual cdata section content

						_nCursor = i;
						return true;
					}
				}


				// the node name begins at position i
				long ibegin = i;

				// pass until we find spaces or EOL or >
				while ( (i<=imax) && _strContent[i]!=' ' 
				&& _strContent[i]!=0x0A
				&& _strContent[i]!=0x0D
				&& (_strContent[i]!='/' || (i==ibegin)) // don't forget empty elements (for instance <br/>)
				&& _strContent[i]!='>')
				i++;

				long iend = i;
				
				_strCurNodeName = _strContent.Substring((int)ibegin, (int)(iend-ibegin));
				if (_strCurNodeName.Length==0)
				{
					_strLastError = IDS_EMPTYELEMENTNAME;
					return false;
				}


				if (_strCurNodeName[0]=='?' || _strCurNodeName[0]=='!')
				{
					_nCurNodeType = NODETYPE.NODETYPE_PI;
					_strCurPInstruction = _strCurNodeName;
					_strCurNodeName = ""; // erase the PI instruction so it does not appear as a node name
				}
				else if (_strCurNodeName[0]=='/')
				{
					_nCurNodeType = NODETYPE.NODETYPE_ENDELEMENT;
					_strCurNodeName = _strCurNodeName.Substring(1); // remove /
				}
				else
					_nCurNodeType = NODETYPE.NODETYPE_BEGINELEMENT;

				_nCursor = i;
				return true;
			}
			else // >, or ?, or content or attribute
			{
				if (c=='?')
				{
					_nCurNodeType = NODETYPE.NODETYPE_NA;
					_nCursor = i+1;
					return true;
				}
				else if (c=='/')
				{
					i++;

					// pass node name
					long ibegin = i;

					// pass until we find spaces or EOL or >
					while ( (i<=imax) && _strContent[i]!=' ' 
						&& _strContent[i]!=0x0A
						&& _strContent[i]!=0x0D
						&& _strContent[i]!='>')
							i++;

					long iend = i;
						
					_nCurNodeType = NODETYPE.NODETYPE_ENDELEMENT;

					_nCursor = i;
					return true;
				}
				else if (c=='>')
				{
					_bCurInsideNode = _bCurInsideAttrib = false;
					_bCurInsideContent = true;
					_strCurNodeContent = "";
					_nCurNodeType = NODETYPE.NODETYPE_NA;
					_nCursor = i+1;
					return true;
				}

				if (_bCurInsideNode) // attributes
				{

					if (!_bCurInsideAttrib)
					{
						if (c=='=')
						{
							_nCurNodeType = NODETYPE.NODETYPE_NA;
							_bCurInsideAttrib = true; // enable extraction of the associated attribute value

							i++; // pass '=' symbol

							_nCursor = i;
							return true;
						}

						// get attribute name
						long ibegin = i;

						// pass until we find spaces or EOL or >
						while ( (i<=imax) && _strContent[i]!=' '
							&& _strContent[i]!=0x0A
							&& _strContent[i]!=0x0D
							&& _strContent[i]!='='
							&& _strContent[i]!='>') // check against > is just for safety
								i++;

						long iend = i;

						_strCurAttribName = _strContent.Substring((int)ibegin, (int)(iend-ibegin));
						if (_strCurAttribName.Length==0)
						{
							_strLastError = IDS_MISSINGATTRIBNAME;
							return false;
						}

						_strCurAttribValue = ""; // make sure the attrib value is empty for the moment
						_nCurNodeType = NODETYPE.NODETYPE_NA;

						_nCursor = i;
						return true;
					}
				}
			}

			// this code never executes
			_strLastError = IDS_GENERICSYNTAXERROR;
			return false;
		}
	}
}
