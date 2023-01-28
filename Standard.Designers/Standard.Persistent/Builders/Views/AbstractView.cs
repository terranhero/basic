using Basic.Builders;
using Basic.Collections;
using Basic.DataEntities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic.Views
{
    /// <summary>
    /// 表示视图实例基类
    /// </summary>
    internal abstract class AbstractView : AbstractPropertyChanged
    {
        private readonly AbstractViewBuilder _Builder;
        /// <summary>
        /// 初始化 AbstractViewer 类实例。
        /// </summary>
        /// <param name="builder">视图构建器</param>
        protected AbstractView(AbstractViewBuilder builder, AbstractEntityColllection entities, string name)
            : this(builder, entities, name, ViewTypeEnum.None, true) { }

        /// <summary>
        /// 初始化 AbstractViewer 类实例。
        /// </summary>
        /// <param name="builder">视图构建器</param>
        /// <param name="entities">实体模型列表</param>
        /// <param name="name">默认视图名称</param>
        /// <param name="viewType">默认视图类型</param>
        /// <param name="isCreated">默认是否创建视图</param>
        protected AbstractView(AbstractViewBuilder builder, AbstractEntityColllection entities, string name, ViewTypeEnum viewType, bool isCreated)
        {
            _Builder = builder;
            _Entities = entities;
            _Name = name;
            _ViewType = viewType;
            _Created = isCreated;
        }

        private AbstractEntityColllection _Entities = null;
        /// <summary>
        /// 获取或设置视图的实体模型。
        /// </summary>
        public AbstractEntityColllection Entities { get { return _Entities; } }

        private AbstractEntityElement _Entity = null;
        /// <summary>
        /// 获取或设置视图的实体模型。
        /// </summary>
        public AbstractEntityElement Entity
        {
            get { return _Entity; }
            set { if (_Entity != value) { _Entity = value; OnPropertyChanged("Entity"); } }
        }

        private string _Name = null;
        /// <summary>
        /// 获取或设置视图名称。
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { if (_Name != value) { _Name = value; OnPropertyChanged("Name"); } }
        }

        private ViewTypeEnum _ViewType = ViewTypeEnum.None;
        /// <summary>
        /// 视图类型
        /// </summary>
        public ViewTypeEnum ViewType
        {
            get { return _ViewType; }
            set { if (_ViewType != value) { _ViewType = value; OnPropertyChanged("ViewType"); } }
        }

        private bool _Created = true;
        /// <summary>
        /// 是否创建视图
        /// </summary>
        public bool Created
        {
            get { return _Created; }
            set { if (_Created != value) { _Created = value; OnPropertyChanged("Created"); } }
        }

        /// <summary>
        /// 生成客户端文件代码
        /// </summary>
        /// <param name="writer">需要输入代码的 System.IO.TextWriter 类实例。</param>
        public abstract void WriteCode(TextWriter writer);
    }
}
