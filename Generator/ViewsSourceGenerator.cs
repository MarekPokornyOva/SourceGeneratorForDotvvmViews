#region using
using DotVVM.Framework.Compilation.ControlTree;
using DotVVM.Framework.Compilation.ControlTree.Resolved;
using DotVVM.Framework.Compilation.Parser.Dothtml.Parser;
using DotVVM.Framework.Compilation.Parser.Dothtml.Tokenizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.IO;
using System.Text;
#endregion using

namespace Generator
{
	[Generator]
	public class ViewsSourceGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{
		}

		public void Execute(GeneratorExecutionContext context)
		{
			DothtmlTokenizer tokenizer = new DothtmlTokenizer();
			DothtmlParser dothtmlParser = new DothtmlParser();
			ViewCodeGenerator viewCodeGenerator = new ViewCodeGenerator();
			IControlTreeResolver _controlTreeResolver = new SimpleControlTreeResolver();

			foreach (AdditionalText additionalText in context.AdditionalFiles)
			{
				SourceText sourceText = additionalText.GetText(context.CancellationToken);
				if (sourceText == null)
					continue;
				string name = "View" + Path.GetFileNameWithoutExtension(additionalText.Path);
				
				tokenizer.Tokenize(sourceText.ToString());
				DothtmlRootNode rootNode = dothtmlParser.Parse(tokenizer.Tokens);
				ResolvedTreeRoot resolvedView = (ResolvedTreeRoot)_controlTreeResolver.ResolveTree(rootNode, name);
				viewCodeGenerator.Clear();
				resolvedView.Accept(viewCodeGenerator);
				string csharpCode = viewCodeGenerator.GetCode();

				context.AddSource(name, SourceText.From(csharpCode, Encoding.UTF8));
			}

			#region simple version
			//StringBuilder sb = new StringBuilder();
			//foreach (AdditionalText additionalText in context.AdditionalFiles)
			//{
			//	SourceText sourceText = additionalText.GetText(context.CancellationToken);
			//	if (sourceText == null)
			//		continue;
			//	string name = "View" + Path.GetFileNameWithoutExtension(additionalText.Path);
			//
			//	sb.Clear();
			//	sb
			//		.AppendLine("namespace GeneratedViews")
			//		.AppendLine("{")
			//		.Append("static class ").AppendLine(name)
			//		.AppendLine("{")
			//		.Append("static void Render(StringWriter writer/*, ViewRenderContext").AppendLine(" context*/)")
			//		.AppendLine("{")
			//		;
			//	sb
			//		.Append("writer.Write(@\"")
			//		.Append(sourceText.ToString().Replace("\"", "\"\""))
			//		.AppendLine("\");");
			//	sb
			//		.AppendLine("}")
			//		.AppendLine("}")
			//		.AppendLine("}")
			//		;
			//
			//	string csharpCode = sb.ToString();
			//	context.AddSource(name, SourceText.From(csharpCode, Encoding.UTF8));
			//}
			#endregion simple version
		}
	}
}
