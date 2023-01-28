using Basic.Builders;
using Basic.Collections;
using Basic.DataEntities;
using Basic.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic.Views
{
	/// <summary>
	/// Index视图
	/// </summary>
	internal class MvcIndexView : MvcView
	{
		/// <summary>
		/// 初始化 MvcIndexView 类实例。
		/// </summary>
		/// <param name="builder">视图构建器</param>
		/// <param name="templateFiles">模版列表</param>
		/// <param name="name">视图名称</param>
		internal protected MvcIndexView(AbstractMvcViewBuilder builder, ObservableCollection<DropDownFile> templateFiles,
			AbstractEntityColllection entities)
			: base(builder, templateFiles, entities, "Index", ViewTypeEnum.None, true)
		{
			base.Template = "~/Master/_DataGrid.cshtml";
		}

		/// <summary>
		/// 生成客户端代码
		/// </summary>
		/// <param name="writer">需要输入代码的 System.IO.TextWriter 类实例。</param>
		public override void WriteCode(System.IO.TextWriter writer)
		{
			if (base.Entity != null && Entity is DataEntityElement)
			{
				DataEntityElement dataEntity = Entity as DataEntityElement;
				writer.WriteLine("@model {0}", dataEntity.Condition.FullName);
			}
			if (!string.IsNullOrEmpty(base.Template))
				writer.WriteLine("@{{ ViewBag.Title = \"Index\"; Layout = \"{0}\"; }}", base.Template);
			writer.WriteLine("@section ToolBar{");
			writer.WriteLine("\t<ul class=\"toolbar\">");
			writer.WriteLine("\t\t<li>@Html.EasyCreate(0, new ButtonOptions() { width = 640 })</li>");
			writer.WriteLine("\t\t<li>@Html.EasyEdit(0, new ButtonOptions() { width = 640, enabledState = PageStatus.SelectedRows | PageStatus.SelectedRow })</li>");
			writer.WriteLine("\t\t<li>@Html.EasyDelete(0, new ButtonOptions() { enabledState = PageStatus.SelectedRows | PageStatus.SelectedRow })</li>");
			writer.WriteLine("\t\t<li class=\"right\">@Html.AntiForgeryToken()</li>");
			writer.WriteLine("\t\t<li class=\"right\">@Html.WebWaterMark(0, \"CodeName\", \"AbstractEntity_CodeName\")</li>");
			writer.WriteLine("\t\t<li class=\"right\">@Html.EasySearch(0, new ButtonOptions(){ searchfor=\"#CodeName\" })</li>");
			writer.WriteLine("\t\t<li class=\"right\">@Html.EasyAdSearch(0, new ButtonOptions() { width = 550, enabledState = PageStatus.Normal })</li>");
			writer.WriteLine("\t</ul>");
			writer.WriteLine("}");
			writer.WriteLine("@Html.Partial(\"Grid\")");
			writer.WriteLine("@section Script{");
			writer.WriteLine("\t<script type=\"text/javascript\">");
			writer.WriteLine("\t\tfunction page_load(target) {");
			writer.WriteLine("\t\t\treturn $(target).pageOptions().getOptions();");
			writer.WriteLine("\t\t}");
			writer.WriteLine("\t</script>");
			writer.WriteLine("}");
		}
	}
}
