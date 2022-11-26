#region using
using DotVVM.Framework.Compilation.ControlTree;
using DotVVM.Framework.Compilation.ControlTree.Resolved;
using DotVVM.Framework.Compilation.Parser.Dothtml.Parser;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Controls.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion using

namespace Generator
{
	internal class SimpleControlTreeResolver : IControlTreeResolver
	{
		public IAbstractTreeRoot ResolveTree(DothtmlRootNode root, string fileName)
		{
			ResolvedTreeRoot result = new ResolvedTreeRoot(new ControlResolverMetadata(typeof(DotvvmView)), root, DataContextStack.Create(typeof(object)), new Dictionary<string, IReadOnlyList<IAbstractDirective>>(), null);
			result.FileName = fileName;
			result.ResolveContentAction = () => result.Content.AddRange(root.Content.Select(ResolveNode));
			return result;
		}

		static ResolvedControl ResolveNode(DothtmlNode node)
		{
			Type metadataType;
			if (node is DothtmlLiteralNode lit)
				metadataType = typeof(RawLiteral);
			else if (node is DothtmlElementNode el)
				metadataType = typeof(HtmlGenericControl);
			else
				throw new NotImplementedException();

			List<ResolvedControl> content = new List<ResolvedControl>(node.EnumerateChildNodes().Where(x => !(x is DothtmlNameNode)).Where(x => { bool isElement = x is DothtmlElementNode; return !isElement || !((DothtmlElementNode)x).IsClosingTag; }).Select(ResolveNode));

			return new ResolvedControl(new ControlResolverMetadata(metadataType), node, content, DataContextStack.Create(typeof(object)));
		}
	}
}