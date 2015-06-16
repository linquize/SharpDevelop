// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public class NUnitConsoleApplication
	{
		public NUnitConsoleApplication(IEnumerable<ITest> selectedTests, UnitTestingOptions options)
		{
			Initialize(selectedTests);
			InitializeOptions(options);
		}
		
		public NUnitConsoleApplication(IEnumerable<ITest> selectedTests)
		{
			Initialize(selectedTests);
		}
		
		void Initialize(IEnumerable<ITest> selectedTests)
		{
			var testList = new List<string>();
			this.TestList = testList;
			foreach (ITest test in selectedTests)
			{
				string name;
				this.project = test.ParentProject.Project;
				if (!Assemblies.Contains(project.OutputAssemblyFullPath))
					Assemblies.Add(project.OutputAssemblyFullPath);
				if (test is TestNamespace) {
					name = ((TestNamespace)test).NamespaceName;
				} else if (test is NUnitTestClass) {
					var testClass = (NUnitTestClass)test;
					name = testClass.ReflectionName;
				} else if (test is NUnitTestMethod) {
					var testMethod = (NUnitTestMethod)test;
					name = testMethod.FixtureReflectionName + "." + testMethod.MethodNameWithDeclaringTypeForInheritedTests;
				} else
					continue;
				testList.Add(name);
			}
		}
		
		void InitializeOptions(UnitTestingOptions options)
		{
			NoThread = options.NoThread;
			NoLogo = options.NoLogo;
			NoDots = options.NoDots;
			Labels = options.Labels;
			ShadowCopy = !options.NoShadow;
			NoXmlOutputFile = !options.CreateXmlOutputFile;
			
			if (options.CreateXmlOutputFile) {
				GenerateXmlOutputFileName();
			}
		}
		
		void GenerateXmlOutputFileName()
		{
			string directory = Path.GetDirectoryName(project.OutputAssemblyFullPath);
			string fileName = project.AssemblyName + "-TestResult.xml";
			XmlOutputFile = Path.Combine(directory, fileName);
		}
		
		/// <summary>
		/// returns full/path/to/Tools/NUnit
		/// </summary>
		string WorkingDirectory {
			get { return Path.Combine(FileUtility.ApplicationRootPath, @"bin\Tools\NUnit"); }
		}
				
		/// <summary>
		/// returns full/path/to/Tools/NUnit/nunit-console.exe or nunit-console-x86.exe if the
		/// project platform target is x86.
		/// </summary>
		public string FileName {
			get {
				string exe = "nunit-console";
				if (ProjectUsesDotnet20Runtime(project)) {
					exe += "-dotnet2";
				}
				// As SharpDevelop can't debug 64-bit applications yet, use
				// 32-bit NUnit even for AnyCPU test projects.
				if (project.IsPlatformTarget32BitOrAnyCPU()) {
					exe += "-x86";
				}
				exe += ".exe";
				return Path.Combine(WorkingDirectory, exe);
			}
		}
		
		public readonly List<string> Assemblies = new List<string>();
		
		/// <summary>
		/// Use shadow copy assemblies. Default = true.
		/// </summary>
		public bool ShadowCopy = true;
		
		/// <summary>
		/// Disables the use of a separate thread to run tests on separate thread. Default = false;
		/// </summary>
		public bool NoThread = false;
		
		/// <summary>
		/// Use /nologo directive.
		/// </summary>
		public bool NoLogo = false;
		
		/// <summary>
		/// Use /labels directive.
		/// </summary>
		public bool Labels = false;
		
		/// <summary>
		/// Use /nodots directive.
		/// </summary>
		public bool NoDots = false;
		
		/// <summary>
		/// File to write xml output to. Default = null.
		/// </summary>
		public string XmlOutputFile;
		
		/// <summary>
		/// Use /noxml.
		/// </summary>
		public bool NoXmlOutputFile = true;
		
		/// <summary>
		/// Selected tests appended to a list
		/// </summary>
		public IEnumerable<string> TestList;
		
		/// <summary>
		/// Pipe to write test results to.
		/// </summary>
		public string ResultsPipe;
		
		IProject project;
		
		public IProject Project {
			get { return project; }
		}
		
		public ProcessStartInfo GetProcessStartInfo()
		{
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.FileName = FileName;
			startInfo.Arguments = GetArguments();
			startInfo.WorkingDirectory = WorkingDirectory;
			return startInfo;
		}
		
		/// <summary>
		/// Gets the full command line to run the unit test application.
		/// This is the combination of the UnitTestApplication and
		/// the command line arguments.
		/// </summary>
		public string GetCommandLine()
		{
			return String.Format("\"{0}\" {1}", FileName, GetArguments());
		}
		
		/// <summary>
		/// Gets the arguments to use on the command line to run NUnit.
		/// </summary>
		public string GetArguments()
		{
			StringBuilder b = new StringBuilder();
			foreach (string assembly in Assemblies) {
				if (b.Length > 0)
					b.Append(' ');
				b.Append('"');
				b.Append(assembly);
				b.Append('"');
			}
			if (!ShadowCopy)
				b.Append(" /noshadow");
			if (NoThread)
				b.Append(" /nothread");
			if (NoLogo)
				b.Append(" /nologo");
			if (Labels) 
				b.Append(" /labels");
			if (NoDots) 
				b.Append(" /nodots");
			if (NoXmlOutputFile) {
				b.Append(" /noxml");
			} else if (XmlOutputFile != null) {
				b.Append(" /xml=\"");
				b.Append(XmlOutputFile);
				b.Append('"');
			}
			if (ResultsPipe != null) {
				b.Append(" /pipe=\"");
				b.Append(ResultsPipe);
				b.Append('"');
			}
			if (this.TestList.Any()) {
				if (this.TestList.Count() > 100) {
					string tempFile = Path.GetTempFileName();
					b.Append(" /runlist=\"");
					b.Append(tempFile);
					b.Append('"');
					File.WriteAllLines(tempFile, this.TestList);
				}
				else {
					b.Append(" /run=\"");
					bool first = true;
					foreach (string run in this.TestList) {
						if (!first)
							b.Append(',');
						else
							first = false;
						b.Append(run);
					}
					b.Append('"');
				}
			}
			return b.ToString();
		}
		
		static bool ProjectUsesDotnet20Runtime(IProject project)
		{
			var p = project as ICSharpCode.SharpDevelop.Project.Converter.IUpgradableProject;
			if (p != null && p.CurrentTargetFramework != null) {
				return p.CurrentTargetFramework.SupportedRuntimeVersion == "v2.0.50727";
			}
			return false;
		}
	}
}
