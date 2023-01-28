using System;
using System.Linq.Expressions;

namespace Basic.MvcLibrary
{
	/// <summary></summary>
	public static class ToolbarExtension
	{
		///// <summary>初始化 Toolbar 类实例</summary>
		//public static IToolbar Toolbar<T>(this Basic<T> basic) where T : class { return new Toolbar<T>(basic, new Options()); }

		///// <summary>初始化 Toolbar 类实例</summary>
		//public static IToolbar Toolbar<T>(this Basic<T> basic, Expression<Action<Options>> expression) where T : class
		//{
		//	Options opts = new Options();
		//	if (expression != null) { expression.Compile().Invoke(opts); }
		//	return new Toolbar<T>(basic, opts);
		//}
	}
}
