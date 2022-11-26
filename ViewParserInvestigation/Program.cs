#region using
using DotVVM.Framework.Compilation;
using DotVVM.Framework.Compilation.ControlTree;
using DotVVM.Framework.Compilation.ControlTree.Resolved;
using DotVVM.Framework.Compilation.Parser.Dothtml.Parser;
using DotVVM.Framework.Compilation.Parser.Dothtml.Tokenizer;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Controls.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;
#endregion using

internal class Program
{
	static void Main(string[] args)
	{
		IHost host = Host.CreateDefaultBuilder(args)
			.ConfigureServices(services=>
			{
				services.AddDotVVM<DotvvmStartup>();
			})
			.Build();
		IServiceProvider sp = host.Services;

		DothtmlTokenizer tokenizer = new DothtmlTokenizer();
		tokenizer.Tokenize(@"<!DOCTYPE html>
<body>
	<h1>Hello world!</h1>
</body>
</html>
");
		DothtmlRootNode rootNode = new DothtmlParser().Parse(tokenizer.Tokens);

		IControlTreeResolver controlTreeResolver = sp.GetRequiredService<IControlTreeResolver>();
		IBindingCompiler bindingCompiler = sp.GetRequiredService<IBindingCompiler>();
		ResolvedTreeRoot resolvedView = (ResolvedTreeRoot)controlTreeResolver.ResolveTree(rootNode, "Index.dothtml");

		ViewCodeGenerator viewCodeGenerator = new ViewCodeGenerator();
		resolvedView.Accept(viewCodeGenerator);
		string csharpCode = viewCodeGenerator.GetCode();

		Console.Clear();
		Console.WriteLine(csharpCode);
	}

	class ViewCodeGenerator : ResolvedControlTreeVisitor
	{
		readonly StringBuilder _sb = new StringBuilder();

		public string GetCode() => _sb.ToString();

		public override void VisitControl(ResolvedControl control)
		{
			if (control.Metadata.Type == typeof(RawLiteral))
				_sb
					.Append("writer.Write(@\"")
					.Append(((DothtmlLiteralNode)control.DothtmlNode!).Value.Replace("\"", "\"\""))
					.AppendLine("\");");
			else if (control.Metadata.Type == typeof(HtmlGenericControl))
			{
				if (!((DothtmlElementNode)control.DothtmlNode!).IsClosingTag)
					_sb
						.Append("writer.Write(@\"")
						.Append('<')
						.Append(((DothtmlElementNode)control.DothtmlNode!).TagName)
						.Append('>')
						.AppendLine("\");");
			}
			else
				throw new NotImplementedException();

			base.VisitControl(control);

			if (control.Metadata.Type == typeof(HtmlGenericControl))
				_sb
					.Append("writer.Write(@\"")
					.Append("</")
					.Append(((DothtmlElementNode)control.DothtmlNode!).TagName)
					.Append('>')
					.AppendLine("\");");
		}

		public override void VisitView(ResolvedTreeRoot view)
		{
			_sb
				.AppendLine("namespace GeneratedViews")
				.AppendLine("{")
				.Append("static class ").AppendLine(Path.GetFileNameWithoutExtension(view.FileName))
				.AppendLine("{")
				.Append("static void Render(StringWriter writer/*, ViewRenderContext<").Append(view.Metadata.Type.FullName).AppendLine("> context*/)")
				.AppendLine("{")
				;
			base.VisitView(view);
			_sb
				.AppendLine("}")
				.AppendLine("}")
				.AppendLine("}")
				;
		}

		#region not implemented
		public override void VisitPropertyValue(ResolvedPropertyValue propertyValue)
		{
			throw new NotImplementedException();
		}

		public override void VisitPropertyBinding(ResolvedPropertyBinding propertyBinding)
		{
			throw new NotImplementedException();
		}

		public override void VisitPropertyTemplate(ResolvedPropertyTemplate propertyTemplate)
		{
			throw new NotImplementedException();
		}

		public override void VisitPropertyControlCollection(ResolvedPropertyControlCollection propertyControlCollection)
		{
			throw new NotImplementedException();
		}

		public override void VisitPropertyControl(ResolvedPropertyControl propertyControl)
		{
			throw new NotImplementedException();
		}

		public override void VisitBinding(ResolvedBinding binding)
		{
			throw new NotImplementedException();
		}

		public override void VisitDirective(ResolvedDirective directive)
		{
			throw new NotImplementedException();
		}

		//public void VisitImportDirective(ResolvedImportDirective importDirective)
		//{
		//	throw new NotImplementedException();
		//}
		#endregion not implemented
	}
}
