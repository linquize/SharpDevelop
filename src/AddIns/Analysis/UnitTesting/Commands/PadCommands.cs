﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TreeView;

namespace ICSharpCode.UnitTesting
{
	public class UnitTestFrameworkListBuilder : IMenuItemBuilder
	{
		public IEnumerable<object> BuildItems(Codon codon, object parameter)
		{
			List<object> items = new List<object>();
			ITestService testService = SD.GetRequiredService<ITestService>();
			foreach (var framework in testService.TestFrameworks) {
				MenuItem menuItem = new MenuItem();
				menuItem.Header = framework.Id;
				// copy in local variable so that lambda refers to correct loop iteration
				var fw = framework.TestFramework;
				menuItem.Click += (o, e) => fw.AddReference(ProjectService.CurrentProject);
				items.Add(menuItem);
			}
			return items;
		}
	}
	
	public class GotoDefinitionCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ITestTreeView treeView = Owner as ITestTreeView;
			if (treeView != null) {
				ITest test = treeView.SelectedTests.FirstOrDefault();
				if (test != null && test.GoToDefinition.CanExecute(null))
					test.GoToDefinition.Execute(null);
			}
		}
	}
	
	public class ExpandAllTestsCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (!(this.Owner is SharpTreeView))
				return;
			
			var treeView = (SharpTreeView)this.Owner;
			if (treeView.Root != null) {
				foreach (var n in treeView.Root.Descendants())
					n.IsExpanded = true;
			}
		}
	}
	
	public class CollapseAllTestsCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (!(this.Owner is SharpTreeView))
				return;
			
			var treeView = (SharpTreeView)this.Owner;
			if (treeView.Root != null) {
				foreach (var n in treeView.Root.Descendants())
					n.IsExpanded = false;
			}
		}
	}
}
