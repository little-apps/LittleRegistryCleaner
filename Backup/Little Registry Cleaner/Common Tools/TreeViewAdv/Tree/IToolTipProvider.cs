using System;
using System.Collections.Generic;
using System.Text;
using Common_Tools.TreeViewAdv.Tree.NodeControls;

namespace Common_Tools.TreeViewAdv.Tree
{
	public interface IToolTipProvider
	{
		string GetToolTip(TreeNodeAdv node, NodeControl nodeControl);
	}
}
