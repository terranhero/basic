using Basic.Builders;
using Basic.Collections;
using Basic.DataEntities;
using Basic.Enums;
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
    internal class MvcGridView : MvcView
    {
        /// <summary>
        /// 初始化 MvcIndexView 类实例。
        /// </summary>
        /// <param name="builder">视图构建器</param>
        /// <param name="templateFiles">模版列表</param>
        /// <param name="name">视图名称</param>
        internal protected MvcGridView(AbstractMvcViewBuilder builder, ObservableCollection<DropDownFile> templateFiles,
            AbstractEntityColllection entities)
            : base(builder, templateFiles, entities, "Grid", ViewTypeEnum.None, true) { }

        /// <summary>
        /// 生成客户端代码
        /// </summary>
        /// <param name="writer">需要输入代码的 System.IO.TextWriter 类实例。</param>
        public override void WriteCode(System.IO.TextWriter writer)
        {
            if (base.Entity != null)
            {
                writer.WriteLine("@model {0}<{1}>", typeof(IPagination).FullName, base.Entity.FullName);
                if (!string.IsNullOrEmpty(base.Template))
                    writer.WriteLine("@{{ ViewBag.Title = \"Grid\"; Layout = \"{0}\"; }}", base.Template);
                writer.WriteLine("@using (IEasyGrid<{0}> grid = Html.EasyGrid<{0}>(\"gvTable\"))", base.Entity.FullName);
                writer.WriteLine("{");
                if (base.Entity is DataEntityElement)
                {
                    DataEntityElement entity = base.Entity as DataEntityElement;
                    foreach (DataEntityPropertyElement property in entity.Properties)
                    {
                        if (property.PrimaryKey)
                        {
                            writer.WriteLine("\tgrid.IdFieldFor(m => m.{0});", property.Name);
                            writer.WriteLine("\tgrid.LabelFor(m => m.{0}).AllowCheck();", property.Name);
                            continue;
                        }
                        if (property.DbType == DbTypeEnum.Timestamp)
                        {
                            writer.WriteLine("\tgrid.TsFieldFor(m => m.{0});", property.Name);
                            writer.WriteLine("\tgrid.DateFor(m => m.{0}).SetWidth(160);", property.Name);
                        }
                        else if (property.DbType == DbTypeEnum.Date) { writer.WriteLine("\tgrid.DateFor(m => m.{0}).SetWidth(100);", property.Name); }
                        else if (property.DbType == DbTypeEnum.DateTime) { writer.WriteLine("\tgrid.DateFor(m => m.{0}).SetWidth(150);", property.Name); }
                        else if (property.DbType == DbTypeEnum.Boolean) { writer.WriteLine("\tgrid.BooleanFor(m => m.{0});", property.Name); }
                        else { writer.WriteLine("\tgrid.LabelFor(m => m.{0});", property.Name); }
                    }
                }
                else if (base.Entity is DataConditionElement)
                {
                    DataConditionElement condition = base.Entity as DataConditionElement;
                    foreach (DataConditionPropertyElement property in condition.Arguments)
                    {
                        if (property.PrimaryKey) { writer.WriteLine("\tgrid.IdFieldFor(m => m.{0});", property.Name); continue; }
                        if (property.DbType == DbTypeEnum.Timestamp) { writer.WriteLine("\tgrid.TsFieldFor(m => m.{0});", property.Name); }
                        else if (property.DbType == DbTypeEnum.Date) { writer.WriteLine("\tgrid.DateFor(m => m.{0}).SetWidth(100);", property.Name); }
                        else if (property.DbType == DbTypeEnum.DateTime) { writer.WriteLine("\tgrid.DateFor(m => m.{0}).SetWidth(150);", property.Name); }
                        else if (property.DbType == DbTypeEnum.Boolean) { writer.WriteLine("\tgrid.BooleanFor(m => m.{0});", property.Name); }
                        else { writer.WriteLine("\tgrid.LabelFor(m => m.{0});", property.Name); }
                    }
                }
                writer.WriteLine("\tgrid.Render(Model);");
                writer.Write("}");
            }
        }
    }
}
