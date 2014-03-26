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
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public class XUnitTestFramework : ITestFramework
	{
		static readonly ITypeReference factAttributeRef = new GetClassTypeReference("Xunit", "FactAttribute", 0);
		
		/// <summary>
		/// Determines whether the project is a test project. A project
		/// is considered to be a test project if it contains a reference
		/// to the xunit assembly.
		/// </summary>
		public bool IsTestProject(IProject project)
		{
			if (project == null)
				return false;
			return factAttributeRef.Resolve(SD.ParserService.GetCompilation(project).TypeResolveContext).Kind != TypeKind.Unknown;
		}
		
		public ITestProject CreateTestProject(ITestSolution parentSolution, IProject project)
		{
			return new XUnitTestProject(project);
		}
		
		public static bool IsTestMethod(IMethod method)
		{
			if (method == null || method.SymbolKind != SymbolKind.Method)
				return false;
			var factAttribute = factAttributeRef.Resolve(method.Compilation);
			foreach (var attr in method.Attributes) {
				if (attr.AttributeType.Equals(factAttribute))
					return true;
			}
			return false;
		}
		
		public static bool IsTestClass(ITypeDefinition type)
		{
			if (type == null)
				return false;
			if (type.NestedTypes.Any(IsTestClass))
				return true;
			if (type.IsAbstract && !type.IsStatic)
				return false;
			return type.Methods.Any(IsTestMethod);
		}
	}
}
