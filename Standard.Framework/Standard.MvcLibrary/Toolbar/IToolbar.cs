using System;
using System.Linq.Expressions;
using System.Web;
using Basic.Collections;
using Basic.EntityLayer;
using BM = Basic.MvcLibrary;
namespace Basic.MvcLibrary
{
	/// <summary>表示工具条接口</summary>
	public interface IToolbar : IDisposable
	{
		/// <summary></summary>
		IBasicContext Basic { get; }

		/// <summary>表示按钮</summary>
		Button Button();

		/// <summary>表示按钮</summary>
		/// <param name="show">是否显示按钮</param>
		Button Button(bool show);

		/// <summary>表示按钮</summary>
		/// <param name="code">表示权限编码</param>
		Button Button(int code);

		/// <summary>表示新增按钮</summary>
		Button Create();

		/// <summary>表示新增按钮</summary>
		/// <param name="show">是否显示按钮</param>
		Button Create(bool show);

		/// <summary>表示新增按钮</summary>
		/// <param name="code">表示权限编码</param>
		Button Create(int code);

		/// <summary>表示更新按钮</summary>
		Button Update();

		/// <summary>表示更新按钮</summary>
		/// <param name="show">是否显示按钮</param>
		Button Update(bool show);

		/// <summary>表示更新按钮</summary>
		/// <param name="code">表示权限编码</param>
		Button Update(int code);

		/// <summary>表示删除按钮</summary>
		Button Delete();

		/// <summary>表示删除按钮</summary>
		/// <param name="show">是否显示按钮</param>
		Button Delete(bool show);

		/// <summary>表示删除按钮</summary>
		/// <param name="code">表示权限编码</param>
		Button Delete(int code);

		/// <summary>表示查询按钮</summary>
		Button Search();

		/// <summary>表示查询按钮</summary>
		/// <param name="show">是否显示按钮</param>
		Button Search(bool show);

		/// <summary>表示查询按钮</summary>
		/// <param name="code">表示权限编码</param>
		Button Search(int code);

		/// <summary>表示高级查询按钮</summary>
		Button ComplexSearch();

		/// <summary>表示高级查询按钮</summary>
		/// <param name="show">是否显示按钮</param>
		Button ComplexSearch(bool show);

		/// <summary>表示高级查询按钮</summary>
		/// <param name="code">表示权限编码</param>
		Button ComplexSearch(int code);

		/// <summary>表示导入按钮</summary>
		Button Import();

		/// <summary>表示导入按钮</summary>
		/// <param name="show">是否显示按钮</param>
		Button Import(bool show);

		/// <summary>表示导入按钮</summary>
		/// <param name="code">表示权限编码</param>
		Button Import(int code);

		/// <summary>表示导出按钮</summary>
		Button Export();

		/// <summary>表示导出按钮</summary>
		/// <param name="show">是否显示按钮</param>
		Button Export(bool show);

		/// <summary>表示导出按钮</summary>
		/// <param name="code">表示权限编码</param>
		Button Export(int code);

		/// <summary>表示打印按钮</summary>
		Button Print();

		/// <summary>表示打印按钮</summary>
		/// <param name="show">是否显示按钮</param>
		Button Print(bool show);

		/// <summary>表示打印按钮</summary>
		/// <param name="code">表示权限编码</param>
		Button Print(int code);

		/// <summary>检测授权码是否成功</summary>
		/// <param name="authorizationCode">功能授权码</param>
		/// <returns>如果</returns>
		bool CheckAuthorizeCode(int authorizationCode);

		/// <summary>检测授权码是否成功</summary>
		/// <param name="beginCode">功能授权码开始</param>
		/// <param name="endCode">功能授权码结束</param>
		/// <returns>如果</returns>
		bool CheckAuthorizeCode(int beginCode, int endCode);
	}

	/// <summary>表示工具条接口</summary>
	public interface IToolbar<TM> : IToolbar
	{

		/// <summary>表格数据源时间戳列名。</summary>
		/// <param name="value">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		IToolbar<TM> Model(string value);

		/// <summary>生成 label 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		Label LabelFor<TP>(Expression<Func<TM, TP>> expression);

		/// <summary>生成 label 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="code">表示权限编码</param>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		Label LabelFor<TP>(int code, Expression<Func<TM, TP>> expression);

		/// <summary>生成 el-input 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		InputText InputTextFor<TP>(Expression<Func<TM, TP>> expression);

		/// <summary>生成 el-input 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="code">表示权限编码</param>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		InputText InputTextFor<TP>(int code, Expression<Func<TM, TP>> expression);

		/// <summary>生成 el-inputnumber 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		InputNumber InputNumberFor<TP>(Expression<Func<TM, TP>> expression);

		/// <summary>生成 el-inputnumber 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="code">表示权限编码</param>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		InputNumber InputNumberFor<TP>(int code, Expression<Func<TM, TP>> expression);

		/// <summary>生成 el-date-picker 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		InputDate InputDateFor<TP>(Expression<Func<TM, TP>> expression);

		/// <summary>生成 el-date-picker 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="code">表示权限编码</param>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		InputDate InputDateFor<TP>(int code, Expression<Func<TM, TP>> expression);

		/// <summary>生成 el-cascader 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		Cascader CascaderFor<TP>(Expression<Func<TM, TP>> expression);

		/// <summary>生成 el-cascader 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="code">表示权限编码</param>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		Cascader CascaderFor<TP>(int code, Expression<Func<TM, TP>> expression);

		/// <summary>生成 select 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		Select SelectFor<TP>(Expression<Func<TM, TP>> expression);

		/// <summary>生成 el-cascader 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="code">表示权限编码</param>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		Select SelectFor<TP>(int code, Expression<Func<TM, TP>> expression);

		/// <summary>生成 el-select 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		Select ElSelectFor<TP>(Expression<Func<TM, TP>> expression);

		/// <summary>生成 el-select 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="code">表示权限编码</param>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		Select ElSelectFor<TP>(int code, Expression<Func<TM, TP>> expression);

		/// <summary>生成 select 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		RadioList RadioListFor<TP>(Expression<Func<TM, TP>> expression);

		/// <summary>生成 el-cascader 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="code">表示权限编码</param>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		RadioList RadioListFor<TP>(int code, Expression<Func<TM, TP>> expression);

		/// <summary>生成 select 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		CheckList CheckListFor<TP>(Expression<Func<TM, TP>> expression);

		/// <summary>生成 el-cascader 标签</summary>
		/// <typeparam name="TP">属性类型</typeparam>
		/// <param name="code">表示权限编码</param>
		/// <param name="expression">Lambda表达式</param>
		/// <returns></returns>
		CheckList CheckListFor<TP>(int code, Expression<Func<TM, TP>> expression);
	}

	/// <summary>表示工具条接口</summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0052:删除未读的私有成员", Justification = "<挂起>")]
	public class Toolbar<TM> : InputProvider<TM>, IToolbar, IToolbar<TM>, IDisposable where TM : class
	{
		private readonly string buttonConverter = typeof(WebStrings).Name;
		private readonly ButtonCollection<TM> buttons;
		private readonly TagHtmlWriter tagBuilder;
		private readonly IBasicContext basicContext;
		/// <summary>初始化 Toolbar 类实例</summary>
		/// <param name="bh">基础开发框架扩展</param>
		protected Toolbar(IBasicContext bh) : base(bh)
		{
			tagBuilder = new TagHtmlWriter(ViewTags.ToolBar);
			basicContext = bh; buttons = new ButtonCollection<TM>(this);
		}

		/// <summary>初始化 Toolbar 类实例</summary>
		/// <param name="bh">基础开发框架扩展</param>
		/// <param name="opts">工具条特性</param>
		internal Toolbar(IBasicContext bh, IOptions opts) : base(bh)
		{
			tagBuilder = new TagHtmlWriter(ViewTags.ToolBar);
			basicContext = bh; buttons = new ButtonCollection<TM>(this);
			tagBuilder.MergeOptions(opts);
			tagBuilder.RenderBeginTag(basicContext.Writer);
		}

		/// <summary></summary>
		protected void Render()
		{
			foreach (Button btn in buttons)
			{
				btn.Render(basicContext.Writer);
			}
		}

		void IDisposable.Dispose()
		{
			tagBuilder.RenderEndTag(basicContext.Writer);
		}
		private string vModel = string.Empty;
		/// <summary>数据源。</summary>
		/// <param name="value">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public new IToolbar<TM> Model(string value) { vModel = value; base.Model(value); return this; }

		/// <summary>表示按钮</summary>
		public Button Button()
		{
			return buttons.Add(new Button(this, ButtonType.Custom));
		}

		/// <summary>表示按钮</summary>
		/// <param name="code">表示权限编码</param>
		public Button Button(int code)
		{
			bool isAuthorization = AuthorizeContext.CheckAuthorizationCode(basicContext, code);
			if (isAuthorization == false) { return BM.Button.Empty(basicContext); }
			return Button();
		}

		/// <summary>表示按钮</summary>
		/// <param name="show">表示当前按钮是否显示</param>
		public Button Button(bool show)
		{
			if (show == false) { return BM.Button.Empty(basicContext); }
			return Button();
		}

		/// <summary>表示新增按钮</summary>
		public Button Create()
		{
			return buttons.Add(new Button(this, ButtonType.Create)).Action("Create", null)
				.Text(buttonConverter, "Button_Create").Icon("icon-create");
		}

		/// <summary>表示新增按钮</summary>
		/// <param name="show">表示当前按钮是否显示</param>
		public Button Create(bool show)
		{
			if (show == false) { return BM.Button.Empty(basicContext); }
			return Create();
		}

		/// <summary>表示新增按钮</summary>
		/// <param name="code">表示权限编码</param>
		public Button Create(int code)
		{
			bool isAuthorization = AuthorizeContext.CheckAuthorizationCode(basicContext, code);
			if (isAuthorization == false) { return BM.Button.Empty(basicContext); }
			return Create();
		}

		/// <summary>表示更新按钮</summary>
		public Button Update()
		{
			return buttons.Add(new Button(this, ButtonType.Update)).Text(buttonConverter, "Button_Edit")
				.Status(PageStatus.SelectedRow | PageStatus.SelectedRows).Icon("icon-edit").Action("Edit", null);
		}

		/// <summary>表示更新按钮</summary>
		/// <param name="show">表示当前按钮是否显示</param>
		public Button Update(bool show)
		{
			if (show == false) { return BM.Button.Empty(basicContext); }
			return Update();
		}

		/// <summary>表示更新按钮</summary>
		/// <param name="code">表示权限编码</param>
		public Button Update(int code)
		{
			bool isAuthorization = AuthorizeContext.CheckAuthorizationCode(basicContext, code);
			if (isAuthorization == false) { return BM.Button.Empty(basicContext); }
			return Update();
		}

		/// <summary>表示删除按钮</summary>
		public Button Delete()
		{
			return buttons.Add(new Button(this, ButtonType.Delete)).Text(buttonConverter, "Button_Delete")
.Status(PageStatus.SelectedRow | PageStatus.SelectedRows).Icon("icon-remove").Action("Delete", null);
		}

		/// <summary>表示删除按钮</summary>
		/// <param name="show">表示当前按钮是否显示</param>
		public Button Delete(bool show)
		{
			if (show == false) { return BM.Button.Empty(basicContext); }
			return Delete();
		}

		/// <summary>表示删除按钮</summary>
		/// <param name="code">表示权限编码</param>
		public Button Delete(int code)
		{
			bool isAuthorization = AuthorizeContext.CheckAuthorizationCode(basicContext, code);
			if (isAuthorization == false) { return BM.Button.Empty(basicContext); }
			return Delete();
		}

		/// <summary>表示查询按钮</summary>
		public Button Search()
		{
			return buttons.Add(new Button(this, ButtonType.Search)).Action("Search", null)
				.Text(buttonConverter, "Button_Search").Icon("icon-search");
		}

		/// <summary>表示查询按钮</summary>
		/// <param name="show">表示当前按钮是否显示</param>
		public Button Search(bool show)
		{
			if (show == false) { return BM.Button.Empty(basicContext); }
			return Search();
		}

		/// <summary>表示查询按钮</summary>
		/// <param name="code">表示权限编码</param>
		public Button Search(int code)
		{
			bool isAuthorization = AuthorizeContext.CheckAuthorizationCode(basicContext, code);
			if (isAuthorization == false) { return BM.Button.Empty(basicContext); }
			return Search();
		}

		/// <summary>表示高级查询按钮</summary>
		public Button ComplexSearch()
		{
			return buttons.Add(new Button(this, ButtonType.ComplexSearch)).Action("ComplexSearch", null)
				.Text(buttonConverter, "Button_ComplexSearch").Icon("icon-search");
		}

		/// <summary>表示高级查询按钮</summary>
		/// <param name="show">表示当前按钮是否显示</param>
		public Button ComplexSearch(bool show)
		{
			if (show == false) { return BM.Button.Empty(basicContext); }
			return ComplexSearch();
		}

		/// <summary>表示高级查询按钮</summary>
		/// <param name="code">表示权限编码</param>
		public Button ComplexSearch(int code)
		{
			bool isAuthorization = AuthorizeContext.CheckAuthorizationCode(basicContext, code);
			if (isAuthorization == false) { return BM.Button.Empty(basicContext); }
			return ComplexSearch();
		}

		/// <summary>表示导入按钮</summary>
		public Button Import()
		{
			return buttons.Add(new Button(this, ButtonType.Import)).Action("Import", null)
				.Text(buttonConverter, "Button_Import").Icon("icon-import");
		}

		/// <summary>表示导入按钮</summary>
		/// <param name="show">表示当前按钮是否显示</param>
		public Button Import(bool show)
		{
			if (show == false) { return BM.Button.Empty(basicContext); }
			return Import();
		}

		/// <summary>表示导入按钮</summary>
		/// <param name="code">表示权限编码</param>
		public Button Import(int code)
		{
			bool isAuthorization = AuthorizeContext.CheckAuthorizationCode(basicContext, code);
			if (isAuthorization == false) { return BM.Button.Empty(basicContext); }
			return Import();
		}

		/// <summary>表示导出按钮</summary>
		public Button Export()
		{
			return buttons.Add(new Button(this, ButtonType.Export)).Action("Export", null)
				.Text(buttonConverter, "Button_Export").Icon("icon-export");
		}

		/// <summary>表示导出按钮</summary>
		/// <param name="show">表示当前按钮是否显示</param>
		public Button Export(bool show)
		{
			if (show == false) { return BM.Button.Empty(basicContext); }
			return Export();
		}
		/// <summary>表示导出按钮</summary>
		/// <param name="code">表示权限编码</param>
		public Button Export(int code)
		{
			bool isAuthorization = AuthorizeContext.CheckAuthorizationCode(basicContext, code);
			if (isAuthorization == false) { return BM.Button.Empty(basicContext); }
			return Export();
		}

		/// <summary>表示打印按钮</summary>
		public Button Print()
		{
			return buttons.Add(new Button(this, ButtonType.Print))
				.Text(buttonConverter, "Button_Print").Icon("icon-print");
		}

		/// <summary>表示打印按钮</summary>
		/// <param name="show">表示当前按钮是否显示</param>
		public Button Print(bool show)
		{
			if (show == false) { return BM.Button.Empty(basicContext); }
			return Print();
		}
		/// <summary>表示打印按钮</summary>
		/// <param name="code">表示权限编码</param>
		public Button Print(int code)
		{
			bool isAuthorization = AuthorizeContext.CheckAuthorizationCode(basicContext, code);
			if (isAuthorization == false) { return BM.Button.Empty(basicContext); }
			return Print();
		}

		/// <summary>检测授权码是否成功</summary>
		/// <param name="authorizationCode">功能授权码</param>
		/// <returns>如果</returns>
		public bool CheckAuthorizeCode(int authorizationCode) { return AuthorizeContext.CheckAuthorizationCode(basicContext, authorizationCode); }

		/// <summary>检测授权码是否成功</summary>
		/// <param name="beginCode">功能授权码开始</param>
		/// <param name="endCode">功能授权码结束</param>
		/// <returns>如果</returns>
		public bool CheckAuthorizeCode(int beginCode, int endCode) { return AuthorizeContext.CheckAuthorizationCode(basicContext, beginCode, endCode); }

	}
}
