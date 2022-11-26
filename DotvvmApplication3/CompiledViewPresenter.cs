using DotVVM.Framework.Hosting;
using Microsoft.Extensions.ObjectPool;
using System.Reflection;
using System.Text;
using RouteBase = DotVVM.Framework.Routing.RouteBase;

namespace DotvvmApplication3
{
	internal class CompiledViewPresenter : IDotvvmPresenter
	{
		readonly static Dictionary<string, Action<StringWriter>?> _viewsCache = new Dictionary<string, Action<StringWriter>?>();
		readonly static ObjectPool<StringBuilder> _sbPool = new DefaultObjectPoolProvider().CreateStringBuilderPool();
		public async Task ProcessRequest(IDotvvmRequestContext context)
		{
			RouteBase? route = context.Route;
			if (route != null)
			{
				string name = Path.GetFileNameWithoutExtension(route.VirtualPath);
				if (!_viewsCache.TryGetValue(name, out Action<StringWriter>? renderAction))
					lock(_viewsCache)
						if (!_viewsCache.TryGetValue(name, out renderAction))
						{
							Type? type = Type.GetType("GeneratedViews.View" + name);
							if (type != null)
							{
								MethodInfo? mi = type.GetMethod("Render", BindingFlags.NonPublic | BindingFlags.Static);
								if (mi != null)
									renderAction = mi.CreateDelegate<Action<StringWriter>>();
							}
							_viewsCache.Add(name, renderAction);
						}
				if (renderAction != null)
				{
					StringBuilder sb = _sbPool.Get();
					renderAction(new StringWriter(sb));
					context.HttpContext.Response.ContentType = "text/html";
					string s = sb.ToString();
					_sbPool.Return(sb);
					await context.HttpContext.Response.WriteAsync(s);
				}
			}
		}
	}
}
