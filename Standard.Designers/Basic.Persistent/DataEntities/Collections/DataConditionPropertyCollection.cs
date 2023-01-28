using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.DataEntities;
using System.Collections.Specialized;
using Basic.Configuration;
using System.ComponentModel;
using Basic.Designer;

namespace Basic.Collections
{
	/// <summary>
	/// 
	/// </summary>
	[System.ComponentModel.DisplayName("Condition Properties")]
	[PersistentDescription("PersistentDescription_DataConditionProperties")]
	[PersistentCategoryAttribute(PersistentCategoryAttribute.CategoryCondition)]
	[Editor(typeof(DataConditionPropertyEditor), typeof(System.Drawing.Design.UITypeEditor))]
	public sealed class DataConditionPropertyCollection : Basic.Collections.AbstractCollection<DataConditionPropertyElement>,
		ICollection<DataConditionPropertyElement>, IEnumerable<DataConditionPropertyElement>, INotifyCollectionChanged
	{
		/// <summary>
		/// 数据库表中所有列配置节名称
		/// </summary>
		internal const string XmlElementName = "ConditionProperties";
		internal readonly DataConditionElement _DataCondition;
		/// <summary>
		/// 初始化 DataEntityPropertyCollection 类的新实例。
		/// </summary>
		internal DataConditionPropertyCollection(DataConditionElement dataCondition)
			: base(dataCondition) { _DataCondition = dataCondition; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			_DataCondition.Persistent.OnFileContentChanged(System.EventArgs.Empty);
			base.OnCollectionChanged(e);
		}

		/// <summary>
		/// 获取集合的键属性
		/// </summary>
		/// <param name="item">需要获取键的集合子元素</param>
		/// <returns>返回元素的键</returns>
		protected internal override string GetKey(DataConditionPropertyElement item)
		{
			if (!string.IsNullOrWhiteSpace(item.Name))
				return item.Name;
			return item.Column;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		protected override void InsertItem(int index, DataConditionPropertyElement item)
		{
			if (string.IsNullOrWhiteSpace(item.Name) && string.IsNullOrWhiteSpace(item.Column))
				item.Name = string.Concat("Property_", index);
			string name = this.GetKey(item);
			if (base.ContainsKey(name)) { item.Name = string.Concat(item.Name, "_", this.Count); }
			base.InsertItem(index, item);
		}

		/// <summary>
		/// 获取当前节点元素名称
		/// </summary>
		protected internal override string ElementName
		{
			get { return XmlElementName; }
		}

		/// <summary>
		/// 从对象的 XML 表示形式读取属性。
		/// </summary>
		/// <param name="name">属性名称。</param>
		/// <param name="value">属性值</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		protected internal override bool ReadAttribute(string name, string value)
		{
			if (name == DataConditionElement.BaseClassAttribute) { _DataCondition.BaseClass = value; return true; }
			else if (name == AbstractEntityElement.ExpandedAttribute)
			{
				if (bool.TryParse(value, out bool _Expanded))
				{ _DataCondition.Expanded = _Expanded; return true; }
				else { return false; }
			}
			else if (name == AbstractEntityElement.GuidAttribute)
			{
				Guid guid = Guid.Empty;
				if (Guid.TryParse(value, out guid)) { _DataCondition.Guid = guid; return true; }
				return false;
			}
			return base.ReadAttribute(name, value);
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		/// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
		protected internal override bool ReadChildContent(System.Xml.XmlReader reader)
		{
			DataConditionPropertyElement element = new DataConditionPropertyElement(_DataCondition);
			element.ReadXml(reader);
			base.Add(element);
			return false;
		}
	}
}
