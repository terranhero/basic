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
    /// Detail 视图
    /// </summary>
    internal class MvcDetailView : MvcView
    {
        /// <summary>
        /// 初始化 MvcDetailView 类实例。
        /// </summary>
        /// <param name="builder">视图构建器</param>
        /// <param name="templateFiles">模版列表</param>
        /// <param name="name">视图名称</param>
        internal protected MvcDetailView(AbstractMvcViewBuilder builder, ObservableCollection<DropDownFile> templateFiles,
            AbstractEntityColllection entities, string name)
            : base(builder, templateFiles, entities, name, ViewTypeEnum.X1EditView, true) { }

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
                writer.WriteLine("\t<table class=\"detailtable\">");
                writer.WriteLine("\t\t<tr>");
                int index = 0;
                StringBuilder keyTimestamp = new StringBuilder(500);
                string fieldClassName = "editor-field";
                if (ViewType == ViewTypeEnum.X2EditView) { fieldClassName = "editor-field2"; }
                else if (ViewType == ViewTypeEnum.X3EditView) { fieldClassName = "editor-field3"; }
                if (base.Entity is DataEntityElement)
                {
                    DataEntityElement entity = base.Entity as DataEntityElement;
                    foreach (DataEntityPropertyElement property in entity.Properties)
                    {
                        if (property.PrimaryKey) { continue; }
                        index++;
                        writer.WriteLine("\t\t\t<td class=\"editor-label\">@Html.WebLabelFor(m => m.{0})</td>", property.Name);
                        if (property.DbType == DbTypeEnum.Boolean)
                            writer.WriteLine("\t\t\t<td class=\"{0}\" data-field=\" {1}\">@Html.BooleanFor(m => m.{1})</td>", fieldClassName, property.Name);
                        else if (property.DbType == DbTypeEnum.Time)
                            writer.WriteLine("\t\t\t<td class=\"{0}\" data-field=\" {1}\">@Html.TimeFor(m => m.{1})</td>", fieldClassName, property.Name);
                        else
                            writer.WriteLine("\t\t\t<td class=\"{0}\" data-field=\" {1}\">@Html.DisplayFor(m => m.{1})</td>", fieldClassName, property.Name);

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
                        if (property.PrimaryKey) { continue; }
                        index++;
                        writer.WriteLine("\t\t\t<td class=\"editor-label\">@Html.WebLabelFor(m => m.{0})</td>", property.Name);
                        if (property.DbType == DbTypeEnum.Boolean)
                            writer.WriteLine("\t\t\t<td class=\"{0}\" data-field=\" {1}\">@Html.BooleanFor(m => m.{1})</td>", fieldClassName, property.Name);
                        else if (property.DbType == DbTypeEnum.Time)
                            writer.WriteLine("\t\t\t<td class=\"{0}\" data-field=\" {1}\">@Html.TimeFor(m => m.{1})</td>", fieldClassName, property.Name);
                        else
                            writer.WriteLine("\t\t\t<td class=\"{0}\" data-field=\" {1}\">@Html.DisplayFor(m => m.{1})</td>", fieldClassName, property.Name);

                        if (ViewType == ViewTypeEnum.X1EditView) { writer.WriteLine("\t\t</tr>\r\n\t\t<tr>"); }
                        else if (ViewType == ViewTypeEnum.X2EditView && index % 2 == 0) { writer.WriteLine("\t\t</tr>\r\n\t\t<tr>"); }
                        else if (ViewType == ViewTypeEnum.X3EditView && index % 3 == 0) { writer.WriteLine("\t\t</tr>\r\n\t\t<tr>"); }
                    }
                }
                writer.WriteLine("\t\t</tr>");
                writer.WriteLine("\t\t<tr>");
                if (ViewType == ViewTypeEnum.X1EditView) { writer.Write("\t\t\t<td colspan=\"2\">"); }
                else if (ViewType == ViewTypeEnum.X2EditView) { writer.Write("\t\t\t<td colspan=\"4\">"); }
                else if (ViewType == ViewTypeEnum.X3EditView) { writer.Write("\t\t\t<td colspan=\"6\">"); }
                writer.WriteLine(keyTimestamp.ToString());
                writer.WriteLine("\t\t\t</td>\r\n\t\t</tr>");
                writer.WriteLine("\t</table>");
            }
        }
    }
}
