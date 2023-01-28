using System;
using System.Collections.Generic;

namespace RazorEngineCore
{
	/// <summary>表示Razor服务静态类</summary>
	public static class Razor
	{
		private static readonly SortedList<string, IRazorEngineCompiledTemplate> templates = new SortedList<string, IRazorEngineCompiledTemplate>();

		/// <summary>检查Razor模板是否已经缓存。</summary>
		/// <param name="key">缓存键</param>
		/// <returns>如果存在则返回true，否则返回false</returns>
		public static bool IsTemplateCached(string key) { return templates.ContainsKey(key); }

		/// <summary>获取缓存内已经编译的模板</summary>
		/// <param name="key">缓存键</param>
		/// <param name="template">已编译的Razor模板</param>
		/// <returns></returns>
		public static bool TryGetTemplate(string key, out IRazorEngineCompiledTemplate template)
		{
			return templates.TryGetValue(key, out template);
		}

		/// <summary>获取缓存内已经编译的模板</summary>
		/// <param name="key">缓存键</param>
		/// <param name="content">模板内容</param>
		/// <returns></returns>
		public static IRazorEngineCompiledTemplate CompileTemplate(string key, string content)
		{
			if (templates.TryGetValue(key, out IRazorEngineCompiledTemplate template)) { templates.Remove(key); }
			template = new RazorEngine().Compile(content, builder =>
			{
				builder.Inherits(typeof(RazorEngineTemplateBase<Type>));
				builder.AddAssemblyReference(typeof(Type).Assembly);
			});
			templates.Add(key, template);
			return template;
		}
	}
}