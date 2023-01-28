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
    /// 
    /// </summary>
    internal abstract class MvcView : AbstractView
    {
        /// <summary>
        /// 初始化 AbstractMvcView 类实例。
        /// </summary>
        /// <param name="builder">视图构建器</param>
        /// <param name="templateFiles">模版列表</param>
        /// <param name="name">视图名称</param>
        internal protected MvcView(AbstractMvcViewBuilder builder, ObservableCollection<DropDownFile> templateFiles,
            AbstractEntityColllection entities, string name)
            : this(builder, templateFiles, entities, name, ViewTypeEnum.None, true) { }

        /// <summary>
        /// 初始化 AbstractMvcView 类实例。
        /// </summary>
        /// <param name="builder">视图构建器</param>
        /// <param name="name">视图名称</param>
        /// <param name="viewType">视图类型</param>
        /// <param name="isCreated">默认是否创建视图</param>
        internal protected MvcView(AbstractMvcViewBuilder builder, ObservableCollection<DropDownFile> templateFiles,
           AbstractEntityColllection entities, string name, ViewTypeEnum viewType, bool isCreated)
            : base(builder, entities, name, viewType, isCreated) { _TemplateFiles = templateFiles; }

        private string _Template = null;
        /// <summary>
        /// 获取或设置视图模版名称。
        /// </summary>
        public virtual string Template
        {
            get { return _Template; }
            set { if (_Template != value) { _Template = value; OnPropertyChanged("Template"); } }
        }

        private readonly ObservableCollection<DropDownFile> _TemplateFiles = null;
        /// <summary>
        /// 获取或设置视图模版名称。
        /// </summary>
        public ObservableCollection<DropDownFile> TemplateFiles { get { return _TemplateFiles; } }
    }
}
