﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using AspNet.Mvc.Tests.CodeTemplates.Models;
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc.AspxCSharp;
using NUnit.Framework;

namespace AspNet.Mvc.Tests.CodeTemplates
{
	[TestFixture]
	public class AspxCSharpEditViewTemplateTests
	{
		Edit templatePreprocessor;
		TestableMvcTextTemplateHost mvcHost;
		
		void CreateViewTemplatePreprocessor()
		{
			mvcHost = new TestableMvcTextTemplateHost();
			templatePreprocessor = new Edit();
			templatePreprocessor.Host = mvcHost;
		}
		
		IEnumerable<Edit.ModelProperty> GetModelProperties()
		{
			return templatePreprocessor.GetModelProperties();
		}
		
		Edit.ModelProperty GetFirstModelProperty()
		{
			return GetModelProperties().First();
		}
		
		[Test]
		public void GetViewPageType_HostViewDataTypeNameIsMyAppMyModel_ReturnsMyAppMyModelSurroundedByAngleBrackets()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataTypeName = "MyApp.MyModel";
			
			string viewPageType = templatePreprocessor.GetViewPageType();
			
			Assert.AreEqual("<MyApp.MyModel>", viewPageType);
		}
		
		[Test]
		public void GetViewPageType_HostViewDataTypeNameIsNull_ReturnsEmptyString()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataTypeName = null;
			
			string viewPageType = templatePreprocessor.GetViewPageType();
			
			Assert.AreEqual(String.Empty, viewPageType);
		}
		
		[Test]
		public void GetViewPageType_HostViewDataTypeNameIsEmptyString_ReturnsEmptyString()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataTypeName = String.Empty;
			
			string viewPageType = templatePreprocessor.GetViewPageType();
			
			Assert.AreEqual(String.Empty, viewPageType);
		}
		
		[Test]
		public void TransformText_ModelHasNoPropertiesAndNoMasterPage_ReturnsFullHtmlPageWithFormAndFieldSetForModel()
		{
			CreateViewTemplatePreprocessor();
			Type modelType = typeof(ModelWithNoProperties);
			mvcHost.ViewDataType = modelType;
			mvcHost.ViewDataTypeName = modelType.FullName;
			mvcHost.ViewName = "MyView";
			
			string output = templatePreprocessor.TransformText();
		
			string expectedOutput = 
@"<%@ Page Language=""C#"" Inherits=""System.Web.Mvc.ViewPage<AspNet.Mvc.Tests.CodeTemplates.Models.ModelWithNoProperties>"" %>

<!DOCTYPE html>
<html>
	<head runat=""server"">
		<title>MyView</title>
	</head>
	<body>
		<% using (Html.BeginForm()) { %>
			<%: Html.ValidationSummary(true) %>
			<fieldset>
				<legend>ModelWithNoProperties</legend>
				
				<p>
					<input type=""submit"" value=""Save""/>
				</p>
			</fieldset>
		<% } %>
		<div>
			<%: Html.ActionLink(""Back"", ""Index"") %>
		</div>
	</body>
</html>
";
			Assert.AreEqual(expectedOutput, output);
		}
		
		[Test]
		public void TransformText_ModelHasNoPropertiesAndIsContentPage_ReturnsContentPageWithFormAndFieldSetForModel()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.IsContentPage = true;
			Type modelType = typeof(ModelWithNoProperties);
			mvcHost.ViewDataType = modelType;
			mvcHost.ViewDataTypeName = modelType.FullName;
			mvcHost.ViewName = "MyView";
			mvcHost.MasterPageFile = "~/Views/Shared/Site.master";
			mvcHost.PrimaryContentPlaceHolderID = "Main";
			
			string output = templatePreprocessor.TransformText();
		
			string expectedOutput = 
@"<%@ Page Language=""C#"" MasterPageFile=""~/Views/Shared/Site.master"" Inherits=""System.Web.Mvc.ViewPage<AspNet.Mvc.Tests.CodeTemplates.Models.ModelWithNoProperties>"" %>

<asp:Content ID=""Content1"" ContentPlaceHolderID=""Title"" runat=""server"">
MyView
</asp:Content>

<asp:Content ID=""Content2"" ContentPlaceHolderID=""Main"" runat=""server"">
	<% using (Html.BeginForm()) { %>
		<%: Html.ValidationSummary(true) %>
		<fieldset>
			<legend>ModelWithNoProperties</legend>
			
			<p>
				<input type=""submit"" value=""Save""/>
			</p>
		</fieldset>
	<% } %>
	<div>
		<%: Html.ActionLink(""Back"", ""Index"") %>
	</div>
</asp:Content>
";
			Assert.AreEqual(expectedOutput, output);
		}
		
		[Test]
		public void TransformText_ModelHasNoPropertiesAndIsPartialView_ReturnsControlWithFormAndFieldSetForModel()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.IsPartialView = true;
			Type modelType = typeof(ModelWithNoProperties);
			mvcHost.ViewDataType = modelType;
			mvcHost.ViewDataTypeName = modelType.FullName;
			mvcHost.ViewName = "MyView";
			
			string output = templatePreprocessor.TransformText();
		
			string expectedOutput = 
@"<%@ Control Language=""C#"" Inherits=""System.Web.Mvc.ViewUserControl<AspNet.Mvc.Tests.CodeTemplates.Models.ModelWithNoProperties>"" %>

<% using (Html.BeginForm()) { %>
	<%: Html.ValidationSummary(true) %>
	<fieldset>
		<legend>ModelWithNoProperties</legend>
		
		<p>
			<input type=""submit"" value=""Save""/>
		</p>
	</fieldset>
<% } %>
<div>
	<%: Html.ActionLink(""Back"", ""Index"") %>
</div>
";
			Assert.AreEqual(expectedOutput, output);
		}
		
		[Test]
		public void GetModelProperties_ModelHasOnePropertyCalledName_ReturnsModelPropertyCalledName()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.ViewDataType = typeof(ModelWithOneProperty);
			
			Edit.ModelProperty modelProperty = GetFirstModelProperty();
			
			Assert.AreEqual("Name", modelProperty.Name);
		}
		
		[Test]
		public void TransformText_ModelHasOnePropertyAndIsPartialView_ReturnsControlWithFormAndHtmlHelpersForModelProperty()
		{
			CreateViewTemplatePreprocessor();
			mvcHost.IsPartialView = true;
			Type modelType = typeof(ModelWithOneProperty);
			mvcHost.ViewDataType = modelType;
			mvcHost.ViewDataTypeName = modelType.FullName;
			mvcHost.ViewName = "MyView";
			
			string output = templatePreprocessor.TransformText();
		
			string expectedOutput = 
@"<%@ Control Language=""C#"" Inherits=""System.Web.Mvc.ViewUserControl<AspNet.Mvc.Tests.CodeTemplates.Models.ModelWithOneProperty>"" %>

<% using (Html.BeginForm()) { %>
	<%: Html.ValidationSummary(true) %>
	<fieldset>
		<legend>ModelWithOneProperty</legend>
		
		<div class=""editor-label"">
			<%: Html.LabelFor(model => model.Name) %>
		</div>
		<div class=""editor-field"">
			<%: Html.EditorFor(model => model.Name) %>
			<%: Html.ValidationMessageFor(model => model.Name) %>
		</div>
		
		<p>
			<input type=""submit"" value=""Save""/>
		</p>
	</fieldset>
<% } %>
<div>
	<%: Html.ActionLink(""Back"", ""Index"") %>
</div>
";
			Assert.AreEqual(expectedOutput, output);
		}
	}
}