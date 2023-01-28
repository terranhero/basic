using System;
using System.Collections;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using Basic.Enums;

namespace Basic.Designer
{
    /// <summary>
    /// 表示类型信息获取的抽象类实现
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public abstract class AbstractCustomTypeDescriptor : ICustomDescriptor, IPropertySelectionContainer,
        IXmlSerializable, INotifyPropertyChanging, INotifyPropertyChanged, ICloneable, IDisposable
    {
        private readonly AbstractCustomTypeDescriptor _NotifyObject;
        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public virtual AbstractCustomTypeDescriptor NotifyObject { get { return _NotifyObject; } }
        /// <summary>
        /// 初始化 AbstractCustomTypeDescriptor 类实例。
        /// </summary>
        protected AbstractCustomTypeDescriptor() { }

        /// <summary>
        /// 初始化 AbstractCustomTypeDescriptor 类实例。
        /// </summary>
        /// <param name="nofity">需要通知 AbstractCustomTypeDescriptor 类实例当前类的属性已更改。</param>
        protected AbstractCustomTypeDescriptor(AbstractCustomTypeDescriptor nofity) { _NotifyObject = nofity; }
        /// <summary>
        /// 当内容更改时引发的事件。
        /// </summary>
        public event EventHandler FileContentChanged;

        /// <summary>
        /// 引发 FileContentChanged 事件
        /// </summary>
        /// <param name="e">引发事件的 EventArgs 类实例参数</param>
        protected internal virtual void OnFileContentChanged(EventArgs e)
        {
            if (_NotifyObject != null)
                _NotifyObject.OnFileContentChanged(e);
            FileContentChanged?.Invoke(this, e);
        }

        #region 接口 ICustomDescriptor 默认实现

        /// <summary>
        /// 返回此组件实例的类名。
        /// </summary>
        /// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
        public abstract string GetClassName();

        /// <summary>
        /// 返回此组件实例的名称。
        /// </summary>
        /// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
        public abstract string GetComponentName();

        /// <summary>
        /// 返回此组件实例的类名。
        /// </summary>
        /// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
        string ICustomDescriptor.GetClassName()
        {
            return GetClassName();
        }

        /// <summary>
        /// 返回此组件实例的名称。
        /// </summary>
        /// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
        string ICustomDescriptor.GetComponentName()
        {
            return GetComponentName();
        }

        /// <summary>
        /// 返回包含指定的属性描述符所描述的属性的对象。
        /// </summary>
        /// <param name="pd">表示要查找其所有者的属性的 System.ComponentModel.PropertyDescriptor。</param>
        /// <returns>表示指定属性所有者的 System.Object。</returns>
        object ICustomDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
        #endregion

        #region 接口 IXmlSerializable 默认实现
        /// <summary>
        /// 获取当前节点元素名称
        /// </summary>
        protected internal abstract string ElementName { get; }

        /// <summary>
        /// 获取当前节点元素命名空间
        /// </summary>
        protected internal virtual string ElementNamespace { get { return null; } }

        /// <summary>
        /// 获取当前节点元素前缀
        /// </summary>
        protected internal virtual string ElementPrefix { get { return null; } }

        /// <summary>
        /// 从对象的 XML 表示形式读取属性。
        /// </summary>
        /// <param name="name">属性名称。</param>
        /// <param name="value">属性值</param>
        /// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
        protected internal abstract bool ReadAttribute(string name, string value);

        /// <summary>
        /// 将对象转换为其 XML 表示形式中属性部分。
        /// </summary>
        /// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
        protected internal abstract void WriteAttribute(System.Xml.XmlWriter writer);

        /// <summary>
        /// 从对象的 XML 表示形式生成该对象扩展信息。
        /// </summary>
        /// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
        /// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
        protected internal abstract bool ReadContent(System.Xml.XmlReader reader);

        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
        protected internal abstract void WriteContent(System.Xml.XmlWriter writer);

        /// <summary>
        /// 将对象转换为其 XML 表示形式,共SQL SERVER/ORACLE使用
        /// </summary>
        /// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
        /// <param name="connectionType">表示数据库连接类型</param>
        protected internal virtual void GenerateConfiguration(XmlWriter writer, ConnectionTypeEnum connectionType) { }

        /// <summary>
        /// 从对象的 XML 表示形式生成该对象。
        /// </summary>
        /// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
        protected internal virtual void ReadXml(System.Xml.XmlReader reader)
        {
            reader.MoveToContent();
            if (reader.HasAttributes)
            {
                for (int index = 0; index < reader.AttributeCount; index++)
                {
                    reader.MoveToAttribute(index);
                    ReadAttribute(reader.LocalName, reader.GetAttribute(index));
                }
            }
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Whitespace) { continue; }
                else if (ReadContent(reader)) { break; }
            }
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
        protected internal virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            if (!string.IsNullOrWhiteSpace(ElementPrefix) && !string.IsNullOrWhiteSpace(ElementNamespace))
                writer.WriteStartElement(ElementPrefix, ElementName, ElementNamespace);
            else
                writer.WriteStartElement(ElementName);
            WriteAttribute(writer);
            WriteContent(writer);
            writer.WriteEndElement();
        }

        /// <summary>
        /// 此方法是保留方法，请不要使用。在实现 IXmlSerializable 接口时，
        /// 应从此方法返回 null（在 Visual Basic 中为 Nothing），
        /// 如果需要指定自定义架构，应向该类应用 XmlSchemaProviderAttribute。
        /// </summary>
        /// <returns>XmlSchema，描述由 WriteXml 方法产生并由 ReadXml 方法使用的对象的 XML 表示形式。</returns>
        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// 从对象的 XML 表示形式生成该对象。
        /// </summary>
        /// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
        void IXmlSerializable.ReadXml(System.Xml.XmlReader reader) { ReadXml(reader); }

        /// <summary>
        /// 将对象转换为其 XML 表示形式。
        /// </summary>
        /// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer) { WriteXml(writer); }
        #endregion

        #region 接口 INotifyPropertyChanging, INotifyPropertyChanged 的默认实现
        /// <summary>
        /// 在更改属性值时发生。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 引发 PropertyChanged 事件
        /// </summary>
        /// <param name="propertyName">已更改的属性名。</param>
        internal protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 引发 PropertyChanged 事件,同时通知编辑器需要保存数据。
        /// </summary>
        /// <param name="propertyName">已更改的属性名。</param>
        internal protected void RaisePropertyChanged(string propertyName)
        {
            this.OnFileContentChanged(EventArgs.Empty);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 在属性值更改时发生。
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// 引发 PropertyChanging 事件
        /// </summary>
        /// <param name="propertyName">其值将更改的属性的名称。</param>
        internal protected void OnPropertyChanging(string propertyName)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }
        #endregion

        #region 接口 ICloneable 默认实现
        /// <summary>
        /// 创建作为当前实例副本的新对象。
        /// </summary>
        /// <returns>作为此实例副本的新对象。</returns>
        protected virtual AbstractCustomTypeDescriptor Clone() { return null; }

        /// <summary>
        /// 创建作为当前实例副本的新对象。
        /// </summary>
        /// <returns>作为此实例副本的新对象。</returns>
        object ICloneable.Clone()
        {
            return Clone();
        }
        #endregion

        #region 接口 IDisposable 默认实现
        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        protected virtual void Dispose() { }

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        void IDisposable.Dispose()
        {
            Dispose();
        }
        #endregion

        #region 接口 IPropertySelectionContainer 默认实现
        /// <summary>
        /// 返回当前实例的可选择对象。
        /// </summary>
        /// <value>当前实例的 System.Collections.ICollection 类可选择对象。</value>
        protected internal virtual System.Collections.ICollection GetSelectedObjects
        {
            get { return new object[] { new ObjectDescriptor<ICustomDescriptor>(this) }; }
        }

        /// <summary>
        /// 设置当前实例添加入 System.Collections.IList 的可选择对象中。
        /// </summary>
        /// <param name="selectionList">选择器可选择对象集合</param>
        protected internal virtual IList SetSelectedObjects(IList selectionList)
        {
            selectionList.Add(new ObjectDescriptor<ICustomDescriptor>(this));
            return selectionList;
        }
        /// <summary>
        /// 返回当前实例的可选择对象。
        /// </summary>
        /// <value>当前实例的 System.Collections.ICollection 类可选择对象。</value>
        System.Collections.ICollection IPropertySelectionContainer.GetSelectedObjects
        {
            get { return GetSelectedObjects; }
        }
        /// <summary>
        /// 设置当前实例添加入 System.Collections.IList 的可选择对象中。
        /// </summary>
        /// <param name="selectionList">选择器可选择对象集合</param>
        void IPropertySelectionContainer.SetSelectedObjects(System.Collections.IList selectionList)
        {
            SetSelectedObjects(selectionList);
        }
        #endregion
    }
}
