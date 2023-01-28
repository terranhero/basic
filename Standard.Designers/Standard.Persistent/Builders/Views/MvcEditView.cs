using Basic.Builders;
using Basic.Collections;
using Basic.DataEntities;
using Basic.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic.Views
{
	/// <summary>
	/// Edit视图
	/// </summary>
	internal class MvcEditView : MvcView
	{
		/// <summary>
		/// 初始化 MvcIndexView 类实例。
		/// </summary>
		/// <param name="builder">视图构建器</param>
		/// <param name="templateFiles">模版列表</param>
		/// <param name="name">视图名称</param>
		internal protected MvcEditView(AbstractMvcViewBuilder builder, ObservableCollection<DropDownFile> templateFiles,
			AbstractEntityColllection entities, string name)
			: base(builder, templateFiles, entities, name, ViewTypeEnum.X1EditView, true)
		{
			//base.Template = "~/Master/_EditLayout.cshtml";
		}

		/// <summary>
		/// 生成客户端代码
		/// </summary>
		/// <param name="writer">需要输入代码的 System.IO.TextWriter 类实例。</param>
		public override void WriteCode(System.IO.TextWriter writer)
		{
			if (base.Entity != null)
			{
				writer.WriteLine("@model {0}", base.Entity.FullName);
				if (!string.IsNullOrEmpty(base.Template))
					writer.WriteLine("@{{ ViewBag.Title = \"{0}\"; Layout = \"{1}\"; }}", base.Name, base.Template);
				writer.WriteLine("@using (Html.BeginForm())");
				writer.WriteLine("{");
                writer.WriteLine("@Html.AntiForgeryToken()");
				writer.WriteLine("\t<table class=\"edittable\" data-options=\"multiple:true\">");
				writer.WriteLine("\t\t<tr>");
				int index = 0;
				StringBuilder keyTimestamp = new StringBuilder(500);
				string fieldClassName = "editor-field";
				//if (ViewType == ViewTypeEnum.X2EditView) { fieldClassName = "editor-field2"; }
				//else if (ViewType == ViewTypeEnum.X3EditView) { fieldClassName = "editor-field3"; }
				if (base.Entity is DataEntityElement)
				{
					DataEntityElement entity = base.Entity as DataEntityElement;
					foreach (DataEntityPropertyElement property in entity.Properties)
					{
						if (property.PrimaryKey) { keyTimestamp.Append("@Html.WebKeyFor(m => m.").Append(property.Name).AppendLine(")"); continue; }
						else if (property.DbType == DbTypeEnum.Timestamp)
						{
							keyTimestamp.Append("@Html.WebTimeStampFor(m => m.").Append(property.Name).AppendLine(")"); continue;
						}
						index++;
						writer.WriteLine("\t\t\t<td class=\"editor-label\">@Html.WebLabelFor(m => m.{0})</td>", property.Name);
						if (property.DbType == DbTypeEnum.Date)
							writer.WriteLine("\t\t\t\t<td class=\"{0}\">@Html.WebDateFor(m => m.{1})</td>", fieldClassName, property.Name);
						else if (property.DbType == DbTypeEnum.DateTime)
							writer.WriteLine("\t\t\t\t<td class=\"{0}\">@Html.WebDateFor(m => m.{1})</td>", fieldClassName, property.Name);
						else if (property.DbType == DbTypeEnum.Boolean)
							writer.WriteLine("\t\t\t\t<td class=\"{0}\">@Html.WebYesOrNoFor(m => m.{1})</td>", fieldClassName, property.Name);
						else
							writer.WriteLine("\t\t\t<td class=\"{0}\">@Html.WebTextBoxFor(m => m.{1})</td>", fieldClassName, property.Name);
						writer.WriteLine("\t\t\t<td class=\"editor-msg\">@Html.WebValidationFor(m => m.{0})</td>", property.Name);

						if (ViewType == ViewTypeEnum.X1EditView) { writer.WriteLine("\t\t</tr>\r\n\t\t<tr>"); }
						else if (ViewType == ViewTypeEnum.X2EditView && index % 2 == 0) { writer.WriteLine("\t\t</tr>\r\n\t\t<tr>"); }
						else if (ViewType == ViewTypeEnum.X3EditView && index % 3 == 0) { writer.WriteLine("\t\t</tr>\r\n\t\t<tr>"); }
					}
				}
				else if (base.Entity is DataConditionElement)
				{
					DataConditionElement condition = base.Entity as DataConditionElement;
					foreach (DataConditionPropertyElement property in condition.Arguments)
					{
						if (property.PrimaryKey) { keyTimestamp.Append("@Html.HiddenFor(m => m.").Append(property.Name).AppendLine(")"); continue; }
						else if (property.DbType == DbTypeEnum.Timestamp)
						{
							keyTimestamp.Append("@Html.WebTimeStampFor(m => m.").Append(property.Name).AppendLine(")"); continue;
						}
						index++;
						writer.WriteLine("\t\t\t<td class=\"editor-label\">@Html.WebLabelFor(m => m.{0})</td>", property.Name);
						if (property.DbType == DbTypeEnum.Date)
							writer.WriteLine("\t\t\t\t<td class=\"{0}\">@Html.WebDateFor(m => m.{1})</td>", fieldClassName, property.Name);
						else if (property.DbType == DbTypeEnum.DateTime)
							writer.WriteLine("\t\t\t\t<td class=\"{0}\">@Html.WebDateFor(m => m.{1})</td>", fieldClassName, property.Name);
						else if (property.DbType == DbTypeEnum.Boolean)
							writer.WriteLine("\t\t\t\t<td class=\"{0}\">@Html.WebYesOrNoFor(m => m.{1})</td>", fieldClassName, property.Name);
						else
							writer.WriteLine("\t\t\t<td class=\"{0}\">@Html.WebTextBoxFor(m => m.{1})</td>", fieldClassName, property.Name);
						writer.WriteLine("\t\t\t<td class=\"editor-msg\">@Html.WebValidationFor(m => m.{0})</td>", property.Name);
						if (ViewType == ViewTypeEnum.X1EditView) { writer.WriteLine("\t\t</tr>\r\n\t\t<tr>"); }
						else if (ViewType == ViewTypeEnum.X2EditView && index % 2 == 0) { writer.WriteLine("\t\t</tr>\r\n\t\t<tr>"); }
						else if (ViewType == ViewTypeEnum.X3EditView && index % 3 == 0) { writer.WriteLine("\t\t</tr>\r\n\t\t<tr>"); }
					}
				}
				writer.WriteLine("\t\t</tr>");
				writer.WriteLine("\t\t<tr>");
				if (ViewType == ViewTypeEnum.X1EditView) { writer.Write("\t\t\t<td colspan=\"3\">"); }
				else if (ViewType == ViewTypeEnum.X2EditView) { writer.Write("\t\t\t<td colspan=\"6\">"); }
				else if (ViewType == ViewTypeEnum.X3EditView) { writer.Write("\t\t\t<td colspan=\"9\">"); }
				writer.WriteLine(keyTimestamp.ToString());
				writer.WriteLine("\t\t\t\t<div class=\"validation-summary-errors\"></div>\r\n\t\t</tr>");
				writer.WriteLine("\t</table>");
				writer.Write("}");
			}
		}
	}
}
