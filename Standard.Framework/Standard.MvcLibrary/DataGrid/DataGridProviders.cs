using System.Collections.Generic;
using Basic.Enums;

namespace Basic.EasyLibrary
{
	/// <summary>DataGrid 输出提供者</summary>
	public static class DataGridProviders
	{
		private static IRenderBuilder _RenderBuilder;

		/// <summary>获取 IRenderBuilder 实例</summary>
		public static IRenderBuilder Creator { set { _RenderBuilder = value; } }

		/// <summary>判断系统是否存在此模式的输出</summary>
		/// <param name="mode">DataGrid输出模式(Template/Xlsx/Json/)</param>
		/// <returns>如果存在则返回True，否则返回False。</returns>
		internal static bool ContainMode(RenderMode mode)
		{
			if (_RenderBuilder == null) { return false; }
			return _RenderBuilder.ContainMode(mode);
		}

		/// <summary></summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="mode"></param>
		/// <param name="grid"></param>
		/// <returns></returns>
		internal static IRender<T> Create<T>(RenderMode mode, DataGrid<T> grid) where T : class
		{
			return _RenderBuilder.Create<T>(mode, grid);
		}
	}

	/// <summary>
	/// 表示 Excel 导出程序构建器
	/// </summary>
	public interface IRenderBuilder
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mode"></param>
		/// <returns></returns>
		bool ContainMode(RenderMode mode);

		/// <summary><![CDATA[创建 IRenderBuilder<T> 类型实例]]></summary>
		/// <typeparam name="T">表示实体模型类型</typeparam>
		/// <param name="mode"></param>
		/// <param name="grid"></param>
		/// <returns></returns>
		IRender<T> Create<T>(RenderMode mode, DataGrid<T> grid) where T : class;
	}
}
