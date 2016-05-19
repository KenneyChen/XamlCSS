﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XamlCSS.CssParsing;

namespace XamlCSS.Tests.CssParsing
{
	[TestFixture]
	public class CssParserTests
	{
		string test1 = @"
@namespace xamlcss ""XamlCss"";
.main .sub>div xamlcss|Button {
	background-color: red;
	background: #00ff00, solid, url('aaa');
	Grid.Row: 1;
}
";
		[Test]
		public void TestTokenize()
		{
			var tokens = CssParser.Tokenize(test1).ToList();
			Assert.Contains(new CssToken(CssTokenType.Identifier, "red"), tokens);
		}

		[Test]
		public void TestGetAst()
		{
			var doc = CssParser.GetAst(test1);

			var node = doc.Children.FirstOrDefault(x => x.Type == CssNodeType.StyleRule)
				?.Children.FirstOrDefault(x => x.Type == CssNodeType.StyleDeclarationBlock)
				?.Children.FirstOrDefault(x => x.Type == CssNodeType.StyleDeclaration)
				?.Children.FirstOrDefault(x => 
					x.Type == CssNodeType.Value &&
					x.Text == "red")
				;

			Assert.NotNull(node);
		}
		
		[Test]
		public void TestParseCss()
		{
			var styleSheet = CssParser.Parse(test1);

			Assert.AreEqual(1, styleSheet.Rules.Count);
		}

		[Test]
		public void TestParseCssWithoutSpaces()
		{
			var styleSheet = CssParser.Parse("Button{Foreground:Red;}");

			Assert.AreEqual(1, styleSheet.Rules.Count);
			Assert.AreEqual("Button", styleSheet.Rules[0].Selector);
			Assert.AreEqual("Foreground", styleSheet.Rules[0].DeclarationBlock[0].Property);
		}

		[Test]
		public void Test_can_set_attaced_property()
		{
			var styleSheet = CssParser.Parse("Button{Grid.Row:1;}");

			Assert.AreEqual(1, styleSheet.Rules.Count);
			Assert.AreEqual("Button", styleSheet.Rules[0].Selector);
			Assert.AreEqual("Grid.Row", styleSheet.Rules[0].DeclarationBlock[0].Property);
		}

		[Test]
		public void Test_can_parse_namespace()
		{
			var styleSheet = CssParser.Parse(@"@namespace ui ""System.Windows.Controls"";");

			Assert.AreEqual(1, styleSheet.Namespaces.Count());
			Assert.AreEqual("ui", styleSheet.Namespaces[0].Alias);
			Assert.AreEqual("System.Windows.Controls", styleSheet.Namespaces[0].Namespace);
		}

		[Test]
		public void Test_can_parse_namespace2()
		{
			var styleSheet = CssParser.Parse(@"@namespace ui ""System.Windows.Controls, PresentationFramework, Version = 4.0.0.0, Culture = neutral, PublicKeyToken = 31bf3856ad364e35"";");

			Assert.AreEqual(1, styleSheet.Namespaces.Count());
			Assert.AreEqual("ui", styleSheet.Namespaces[0].Alias);
			Assert.AreEqual("System.Windows.Controls, PresentationFramework, Version = 4.0.0.0, Culture = neutral, PublicKeyToken = 31bf3856ad364e35", styleSheet.Namespaces[0].Namespace);
		}

		[Test]
		public void Test_can_parse_namespace3()
		{
			var styleSheet = CssParser.Parse(@"@namespace ui ""System.Windows.Controls, PresentationFramework, Version = 4.0.0.0, Culture = neutral, PublicKeyToken = 31bf3856ad364e35"";
.test
{
	ui|Grid.Row: 0;
	ui|Grid.Column: 1;
}

");

			Assert.AreEqual(1, styleSheet.Namespaces.Count());
			Assert.AreEqual("ui", styleSheet.Namespaces[0].Alias);
			Assert.AreEqual("System.Windows.Controls, PresentationFramework, Version = 4.0.0.0, Culture = neutral, PublicKeyToken = 31bf3856ad364e35", styleSheet.Namespaces[0].Namespace);
			Assert.AreEqual("ui|Grid.Row", styleSheet.Rules[0].DeclarationBlock[0].Property);
			Assert.AreEqual("ui|Grid.Column", styleSheet.Rules[0].DeclarationBlock[1].Property);
		}
	}
}
