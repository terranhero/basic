using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.DataEntities;
using System.Collections.Specialized;
using Basic.Configuration;

namespace Basic.Collections
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class DataEntityPropertyCollection : Basic.Collections.AbstractCollection<DataEntityPropertyElement>,
		ICollection<DataEntityPropertyElement>, IEnumerable<DataEntityPropertyElement>, INotifyCollectionChanged
	{
		/// <summary>
		/// 数据库表中所有列配置节名称
		/// </summary>
		internal const string XmlElementName = "Properties";
		internal readonly DataEntityElement dataEntityElement;
		/// <summary>
		/// 初始化 DataEntityPropertyCollection 类的新实例。
		/// </summary>
		internal DataEntityPropertyCollection(DataEntityElement entity) : base(entity) { dataEntityElement = entity; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			dataEntityElement.OnFileContentChanged(System.EventArgs.Empty);
			base.OnCollectionChanged(e);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		protected override void InsertItem(int index, DataEntityPropertyElement item)
		{
			if (string.IsNullOrWhiteSpace(item.Name) && string.IsNullOrWhiteSpace(item.Column))
				item.Name = string.Concat("Property_", index);
			string name = this.GetKey(item);
			if (base.ContainsKey(name)) { item.Name = string.Concat(item.Name, "_", this.Count); }
			base.InsertItem(index, item);
		}

		/// <summary>
		/// 获取集合的键属性
		/// </summary>
		/// <param name="item">需要获取键的集合子元素</param>
		/// <returns>返回元素的键</returns>
		protected internal override string GetKey(DataEntityPropertyElement item)
		{
			if (!string.IsNullOrWhiteSpace(item.Name))
				return item.Name;
			return item.Column;
		}

		/// <summary>
		/// 获取当前节点元素名称
		/// </summary>
		protected internal override string ElementName
		{
			get { return XmlElementName; }
		}

		protected internal override bool ReadChildContent(System.Xml.XmlReader reader)
		{
			DataEntityPropertyElement element = new DataEntityPropertyElement(dataEntityElement);
			element.ReadXml(reader);
			base.Add(element);
			return false;
		}
	}
}
