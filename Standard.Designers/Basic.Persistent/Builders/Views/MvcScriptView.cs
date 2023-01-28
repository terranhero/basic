using Basic.Builders;
using Basic.Collections;
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
    internal class MvcScriptView : MvcView
    {
        /// <summary>
        /// 初始化 MvcIndexView 类实例。
        /// </summary>
        /// <param name="builder">视图构建器</param>
        /// <param name="templateFiles">模版列表</param>
        /// <param name="name">视图名称</param>
        internal protected MvcScriptView(AbstractMvcViewBuilder builder, ObservableCollection<DropDownFile> templateFiles,
            AbstractEntityColllection entities)
            : base(builder, templateFiles, entities, "Script", ViewTypeEnum.None, false) { }

        /// <summary>
        /// 生成客户端代码
        /// </summary>
        /// <param name="writer">需要输入代码的 System.IO.TextWriter 类实例。</param>
        public override void WriteCode(System.IO.TextWriter writer)
        {
        }
    }
}
