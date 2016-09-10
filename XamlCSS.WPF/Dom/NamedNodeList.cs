﻿using System.Collections.Generic;
using System.Windows;
using AngleSharp.Dom;
using XamlCSS.Dom;
using System.Linq;
using System.Windows.Controls;

namespace XamlCSS.WPF.Dom
{
	public class NamedNodeList : NamedNodeListBase<DependencyObject, DependencyProperty>
	{
		public NamedNodeList(DomElementBase<DependencyObject, DependencyProperty> node)
			: base(node)
		{

		}

		public NamedNodeList(IEnumerable<INode> nodes)
			: base(nodes)
		{

		}

		protected override INode CreateNode(DependencyObject dependencyObject, IDomElement<DependencyObject> parentNode)
		{
			return new LogicalDomElement(dependencyObject, parentNode);
		}
		protected override IEnumerable<DependencyObject> GetChildren(DependencyObject dependencyObject)
		{
			if (dependencyObject is Window)
			{
				return new List<DependencyObject>() { (dependencyObject as Window).Content as DependencyObject };
			}

			if (dependencyObject is Page)
			{
				return new List<DependencyObject>() { (dependencyObject as Page).Content as DependencyObject };
			}

			if (dependencyObject is Panel)
			{
				return new List<DependencyObject>((dependencyObject as Panel).Children.Cast<DependencyObject>());
			}

			var res = LogicalTreeHelper.GetChildren(dependencyObject);
			if (res.Cast<object>().Any() == false)
			{
				return Enumerable.Empty<DependencyObject>();
			}

			return res.Cast<DependencyObject>().ToList();
		}
	}
}
