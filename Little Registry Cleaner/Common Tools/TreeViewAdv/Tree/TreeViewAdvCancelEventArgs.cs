using System;
using System.Collections.Generic;
using System.Text;

namespace Common_Tools.TreeViewAdv.Tree
{
	public class TreeViewAdvCancelEventArgs : TreeViewAdvEventArgs
	{
		private bool _cancel;

		public bool Cancel
		{
			get { return _cancel; }
			set { _cancel = value; }
		}

		public TreeViewAdvCancelEventArgs(TreeNodeAdv node)
			: base(node)
		{
		}

	}
}
