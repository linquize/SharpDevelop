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
using System.IO;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.UnitTesting
{
	public class XUnitTestResult : TestResult
	{
		public XUnitTestResult(TestResult testResult)
			: base(testResult.Name)
		{
			Message = testResult.Message;
			ResultType = testResult.ResultType;
			StackTrace = testResult.StackTrace;
		}

		protected override void OnStackTraceChanged()
		{
			// TODO:
			/*
			FileLineReference fileLineRef = OutputTextLineParser.GetXUnitOutputFileLineReference(StackTrace, true);
			if (fileLineRef != null) {
				StackTraceFilePosition = CreateFilePosition(fileLineRef);
			} else {
				StackTraceFilePosition = DomRegion.Empty;
			}
			*/
		}
		
		DomRegion CreateFilePosition(FileLineReference fileLineRef)
		{
			string fileName = Path.GetFullPath(fileLineRef.FileName);
			return new DomRegion(fileName, fileLineRef.Line, fileLineRef.Column + 1);
		}
	}
}
