using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Resources;
using System.Xml;
using System.Xml.Serialization;

namespace Basic.Localizations
{
	/// <summary>
	/// 标识资源本地化集合
	/// </summary>
	public sealed class LocalizationCollection : Collection<LocalizationItem>, IXmlSerializable, INotifyCollectionChanged
	{
		#region Xml Element Definition
		private const string XmlElementName = "root";
		private const string CultureInfosElementName = "cultureInfos";
		/// <summary></summary>
		public const string ResxDatasElementName = "resxDatas";
		private readonly ObservableCollection<CultureInfo> cultureInfos;
		private readonly GroupNameCollection _GroupNames;
		private readonly SortedDictionary<string, LocalizationItem> resourceCollection;
		private readonly ObservableCollection<CultureInfo> enabledCultureInfos;

		#endregion

		#region 接口 INotifyCollectionChanged 的实现
		private bool isNotifyChanged = true;
		/// <summary>集合开始变更</summary>
		public void BeginChanging() { isNotifyChanged = false; }

		/// <summary>集合变更完成</summary>
		public void EndChanged() { isNotifyChanged = true; OnCollectionChanged(NotifyCollectionChangedAction.Reset); }

		/// <summary>
		/// 当文件内容更改后引发的事件。
		/// </summary>
		public event EventHandler ContentChanged;
		internal void OnContentChanged(EventArgs args)
		{
			ContentChanged?.Invoke(this, args);
		}

		/// <summary>
		/// 集合变更事件
		/// </summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>
		/// 引发带有提供的参数的 LocalizationCollection.CollectionChanged 事件。
		/// </summary>
		/// <param name="action">引起该事件的操作。 这可设置为 System.Collections.Specialized.NotifyCollectionChangedAction.Replace。</param>
		/// <param name="newItem">要替换原始项的新项。</param>
		/// <param name="oldItem">要替换的原始项。</param>
		/// <param name="index">要替换的项的索引。</param>
		/// <exception cref="System.ArgumentException">如果 action 不是 Replace。</exception>
		internal void OnCollectionChanged(NotifyCollectionChangedAction action, LocalizationItem newItem, LocalizationItem oldItem, int index)
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
		}

		/// <summary>
		/// 引发带有提供的参数的 LocalizationCollection.CollectionChanged 事件。
		/// </summary>
		/// <param name="action">引起该事件的操作。 这可以设置为 NotifyCollectionChangedAction.Reset、
		/// NotifyCollectionChangedAction.Add 或 NotifyCollectionChangedAction.Remove。</param>
		/// <param name="changedItem">受更改影响的项。</param>
		/// <param name="index">发生更改处的索引。</param>
		/// <exception cref="System.ArgumentException">如果 action 不是 Reset、Add 或 Remove；或者如果 action 是 Reset，并且要么 changedItems 不是 null，要么 index 不是 -1。</exception>
		internal void OnCollectionChanged(NotifyCollectionChangedAction action, LocalizationItem changedItem, int index)
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItem, index));
		}

		/// <summary>
		/// 引发带有提供的参数的 LocalizationCollection.CollectionChanged 事件。
		/// </summary>
		/// <param name="action">引起该事件的操作。 这可以设置为 NotifyCollectionChangedAction.Reset、
		/// NotifyCollectionChangedAction.Add 或 NotifyCollectionChangedAction.Remove。</param>
		/// <param name="changedItems">受更改影响的各项。</param>
		internal void OnCollectionChanged(NotifyCollectionChangedAction action, IList changedItems)
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItems));
		}

		/// <summary>
		/// 引发带有提供的参数的 LocalizationCollection.CollectionChanged 事件。
		/// </summary>
		/// <param name="action">引起该事件的操作。 这必须设置为 NotifyCollectionChangedAction.Reset。</param>
		internal void OnCollectionChanged(NotifyCollectionChangedAction action)
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(action));
		}

		/// <summary>
		/// 引发带有提供的参数的 LocalizationCollection.CollectionChanged 事件。
		/// </summary>
		/// <param name="e">要引发的事件的参数。</param>
		private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (isNotifyChanged) { CollectionChanged?.Invoke(this, e); OnContentChanged(e); }
		}
		#endregion

		/// <summary>
		/// 资源组名称集合
		/// </summary>
		public GroupNameCollection GroupNames { get { return _GroupNames; } }

		/// <summary>
		/// 当前资源文件名，含路径。
		/// </summary>
		public string FileName { get; private set; }

		/// <summary>
		/// 判断当前资源集合是否已经包含指定名称的资源键名称。
		/// </summary>
		/// <param name="name">一个String类型的值，该值表示一个资源名称。</param>
		/// <returns>如果存在则返回true，否则返回false。</returns>
		public bool ContainsName(string name)
		{
			return resourceCollection.ContainsKey(name);
		}

		/// <summary>
		/// 初始化 LocalizationCollection 类实例。
		/// </summary>
		public LocalizationCollection()
			: base()
		{
			_GroupNames = new GroupNameCollection();
			cultureInfos = new ObservableCollection<CultureInfo>();
			cultureInfos.CollectionChanged += (sender, eventArgs) =>
			{
				OnContentChanged(EventArgs.Empty);
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			};
			StringComparer stringComparer = StringComparer.Create(CultureInfo.CurrentUICulture, true);
			resourceCollection = new SortedDictionary<string, LocalizationItem>(stringComparer);
			enabledCultureInfos = new ObservableCollection<CultureInfo>
			{
				new CultureInfo(0x0004),    //zh-Hans, 4
				new CultureInfo(0x7C04),    //zh-Hant, 31748
				new CultureInfo(0x0804),  //zh-CN, 2052
				new CultureInfo(0x0404),  //zh-TW, 1028
				new CultureInfo(0x0409)   //en-US, 1033
			};
		}

		/// <summary>
		/// 初始化 LocalizationResxResource 类实例
		/// </summary>
		/// <param name="group">表示拥有此对象的 LocalizationCollection 类实例。</param>
		/// <param name="name">资源名称</param>
		/// <param name="value">资源值</param>
		public LocalizationItem Add(string group, string name, string value)
		{
			LocalizationItem res = new LocalizationItem(this, group, name, value);
			foreach (CultureInfo cultureInfo in cultureInfos)
			{
				if (!res.ContainsKey(cultureInfo.Name))
					res.Add(cultureInfo.Name, value);
			}
			base.Add(res);
			return res;
		}

		/// <summary>
		/// 添加本地化资源信息
		/// </summary>
		/// <param name="culture">预定义的 System.Globalization.CultureInfo 标识符、现有 System.Globalization.CultureInfo 
		/// 对象的 System.Globalization.CultureInfo.LCID 属性或仅 Windows 区域性标识符。</param>
		public CultureInfo GetEnabledCultureInfo(int culture)
		{
			foreach (CultureInfo cultureInfo in enabledCultureInfos)
			{
				if (cultureInfo.LCID == culture) { return cultureInfo; }
			}
			return null;
		}

		/// <summary>
		/// 添加本地化资源信息
		/// </summary>
		/// <param name="culture">预定义的 System.Globalization.CultureInfo 标识符、现有 System.Globalization.CultureInfo 
		/// 对象的 System.Globalization.CultureInfo.LCID 属性或仅 Windows 区域性标识符。</param>
		public CultureInfo GetCultureInfo(int culture)
		{
			foreach (CultureInfo cultureInfo in cultureInfos)
			{
				if (cultureInfo.LCID == culture) { return cultureInfo; }
			}
			return null;
		}

		/// <summary>
		/// 添加本地化资源信息
		/// </summary>
		/// <param name="name">预定义的 System.Globalization.CultureInfo 名称、现有 System.Globalization.CultureInfo 的 
		/// System.Globalization.CultureInfo.Name 或仅 Windows 区域性名称。</param>
		public CultureInfo GetCultureInfo(string name)
		{
			foreach (CultureInfo cultureInfo in cultureInfos)
			{
				if (cultureInfo.Name == name) { return cultureInfo; }
			}
			return null;
		}

		/// <summary>
		/// 添加本地化资源信息
		/// </summary>
		/// <param name="culture"></param>
		public bool ContainCultureInfo(int culture)
		{
			foreach (CultureInfo cultureInfo in cultureInfos)
			{
				if (cultureInfo.LCID == culture) { return true; }
			}
			return false;
		}

		/// <summary>
		/// 添加本地化资源信息到有效列表
		/// </summary>
		public void AddEnabledCultureInfos()
		{
			foreach (CultureInfo culture in enabledCultureInfos)
			{
				cultureInfos.Add(culture);
			}
			enabledCultureInfos.Clear();
		}

		/// <summary>
		/// 添加本地化资源信息
		/// </summary>
		/// <param name="cultureInfo"></param>
		public bool AddCultureInfo(CultureInfo cultureInfo)
		{
			if (enabledCultureInfos.Contains(cultureInfo))
			{
				enabledCultureInfos.Remove(cultureInfo);
				cultureInfos.Add(cultureInfo);
				return true;
			}
			return false;
		}

		/// <summary>
		/// 添加本地化资源信息
		/// </summary>
		/// <param name="cultureInfo"></param>
		public bool RemoveCultureInfo(CultureInfo cultureInfo)
		{
			if (cultureInfos.Contains(cultureInfo))
			{
				enabledCultureInfos.Add(cultureInfo);
				foreach (LocalizationItem localResx in base.Items)
				{
					if (localResx.ContainsKey(cultureInfo.Name))
					{
						localResx.Remove(cultureInfo.Name);
					}
				}
				return cultureInfos.Remove(cultureInfo);
			}
			return false;
		}

		/// <summary>
		/// 表示当前本地化资源文件支持的本地化信息。
		/// </summary>
		public ObservableCollection<CultureInfo> CultureInfos { get { return cultureInfos; } }

		/// <summary>
		/// 表示当前本地化资源文件可用的区域信息。
		/// </summary>
		public ObservableCollection<CultureInfo> EnabledCultureInfos { get { return enabledCultureInfos; } }

		/// <summary>
		/// 将一项插入集合中指定索引处。
		/// </summary>
		/// <param name="index">从零开始的索引，应在该位置插入 item。</param>
		/// <param name="item">要插入的 LocalizationResource 对象。</param>
		protected override void InsertItem(int index, LocalizationItem item)
		{
			foreach (CultureInfo cultureInfo in cultureInfos)
			{
				if (!item.ContainsKey(cultureInfo.Name))
					item.Add(cultureInfo.Name, item.Value);
			}
			if (!_GroupNames.Contains(item.Group))
				_GroupNames.Add(item.Group);
			base.InsertItem(index, item);
			if (string.IsNullOrWhiteSpace(item.Name)) { return; }
			if (!resourceCollection.ContainsKey(item.Name))
				resourceCollection.Add(item.Name, item);

			this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
		}

		/// <summary>
		/// 使用默认比较器对整个 LocalizationCollection 中的元素进行排序。
		/// </summary>
		/// <exception cref="System.InvalidOperationException">
		/// <![CDATA[默认比较器 System.Collections.Generic.Comparer<LocalizationResource>.Default 找不到 T 类型的 
		/// System.IComparable<LocalizationResource> 泛型接口或 System.IComparable 接口的实现。]]>
		/// </exception>
		public void Sort() { (base.Items as List<LocalizationItem>).Sort(); }

		/// <summary>
		/// 移除集合中指定索引处的项。
		/// </summary>
		/// <param name="index">要移除的元素的从零开始的索引。</param>
		protected override void RemoveItem(int index)
		{
			LocalizationItem item = this[index];
			if (resourceCollection.ContainsKey(item.Name))
			{
				base.RemoveItem(index);
				IEnumerable<LocalizationItem> groups = base.Items.Where(m => m.Group == item.Group);
				if (groups == null || groups.Count() == 0) { _GroupNames.Remove(item.Group); }
				resourceCollection.Remove(item.Name);
			}
			this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
		}

		/// <summary>
		/// 从集合中移除所有项。
		/// </summary>
		protected override void ClearItems()
		{
			base.ClearItems();
			_GroupNames.Clear();
			resourceCollection.Clear();
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		/// <summary>
		/// 替换指定索引处的元素。
		/// </summary>
		/// <param name="index">待替换元素的从零开始的索引。</param>
		/// <param name="item">位于指定索引处的元素的新值。</param>
		protected override void SetItem(int index, LocalizationItem item)
		{
			LocalizationItem oldItem = this[index];
			base.SetItem(index, item);
			IEnumerable<LocalizationItem> groups = base.Items.Where(m => m.Group == oldItem.Group);
			if (groups == null || groups.Count() == 0) { _GroupNames.Remove(oldItem.Group); }
			if (!_GroupNames.Contains(item.Group))
				_GroupNames.Add(item.Group);
			if (oldItem.Name != item.Name)
			{
				resourceCollection.Remove(oldItem.Name);
			}
			if (!resourceCollection.ContainsKey(item.Name))
			{
				resourceCollection.Add(item.Name, item);
			}
			else
			{
				resourceCollection[item.Name] = item;
			}
			this.OnCollectionChanged(NotifyCollectionChangedAction.Replace, oldItem, item, index);
		}

		#region 接口 IXmlSerializable 的实现
		/// <summary>
		/// 加载本地化字符串资源信息。
		/// </summary>
		/// <param name="pszFilename">本地化资源文件全名，含路径。</param>
		/// <returns>如果加载成功，则返回True，否则返回False</returns>
		public bool LoadGroups(string pszFilename)
		{
			if (!File.Exists(pszFilename)) { return false; }
			using (XmlReader reader = XmlReader.Create(pszFilename))
			{
				this.ReadXml(reader);
			}
			return true;
		}

		/// <summary>加载本地化字符串资源信息。</summary>
		/// <param name="pszFilename">本地化资源文件全名，含路径。</param>
		/// <param name="funcCreator"></param>
		/// <returns>如果加载成功，则返回True，否则返回False</returns>
		public bool Load(string pszFilename, Func<string, Stream> funcCreator)
		{
			Stream stream = funcCreator(pszFilename);
			if (stream == null) { return false; }
			using (XmlReader reader = XmlReader.Create(stream))
			{
				this.ReadXml(reader);
			}
			FileName = Path.GetFileNameWithoutExtension(pszFilename);//.Replace(fileInfo.Extension, "");
			foreach (CultureInfo cultureInfo in cultureInfos)
			{
				string cultureFileName = string.Concat(pszFilename.Replace(".localresx", ""), ".", cultureInfo.Name, ".resx");
				Stream cStream = funcCreator(cultureFileName);
				if (cStream == null) { continue; }
				using (ResXResourceReader reader = new ResXResourceReader(cStream))
				{
					foreach (DictionaryEntry node in reader)
					{
						string name = (string)node.Key;
						if (resourceCollection.TryGetValue(name, out LocalizationItem localResx))
						{
							string value = node.Value as string;
							if (string.IsNullOrWhiteSpace(value)) { value = localResx.Value; };
							if (localResx.ContainsKey(cultureInfo.Name))
								localResx[cultureInfo.Name] = value;
							else
								localResx.Add(cultureInfo.Name, value);
						}
					}   // end foreach (DictionaryEntry node in reader)
				}   // end using (ResXResourceReader reader = new ResXResourceReader(cultureFileInfo.OpenRead()))
			}
			this.Sort();
			return true;
		}

		/// <summary>保存本地化字符串资源</summary>
		/// <param name="pszFilename">本地化资源文件全名，含路径。</param>
		/// <returns>如果保存成功则返回保存成功的所有资源(*.resx)文件集合</returns>
		public string[] Save(string pszFilename)
		{
			List<string> resxFiles = new List<string>(6);
			if (resxFiles is null) { throw new ArgumentNullException(nameof(resxFiles)); }

			XmlWriterSettings settings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "\t",
				NewLineChars = Environment.NewLine
			};
			using (XmlWriter writer = XmlWriter.Create(pszFilename, settings))
			{
				this.WriteXml(writer);
			}
			FileInfo itemInfo = new FileInfo(pszFilename);
			string resxName = itemInfo.FullName.Replace(itemInfo.Extension, ".resx");
			Func<Type, string> typeNameConverter = new Func<Type, string>((type) => { return type.ToString(); });
			using (ResXResourceWriter writer = new ResXResourceWriter(resxName))
			{
				foreach (LocalizationItem localResx in base.Items)
				{
					ResXDataNode node = new ResXDataNode(localResx.Name, localResx.Value, typeNameConverter)
					{
						Comment = localResx.Comment
					};
					writer.AddResource(node);
				}
			}
			resxFiles.Add(resxName);
			foreach (CultureInfo cultureInfo in cultureInfos)
			{
				string cultureFileName = itemInfo.FullName.Replace(itemInfo.Extension, string.Concat(".", cultureInfo.Name, ".resx"));
				using (ResXResourceWriter writer = new ResXResourceWriter(cultureFileName))
				{
					foreach (LocalizationItem localResx in this)
					{
						ResXDataNode node = new ResXDataNode(localResx.Name, localResx[cultureInfo.Name], typeNameConverter)
						{
							Comment = localResx.Comment
						};
						writer.AddResource(node);
					}
				}
				resxFiles.Add(cultureFileName);
			}
			return resxFiles.ToArray();
		}

		/// <summary>
		/// 此方法是保留方法，请不要使用。在实现 IXmlSerializable 接口时，
		/// 应从此方法返回 null（在 Visual Basic 中为 Nothing），
		/// 如果需要指定自定义架构，应向该类应用 XmlSchemaProviderAttribute。
		/// </summary>
		/// <returns>XmlSchema，描述由 WriteXml 方法产生并由 ReadXml 方法使用的对象的 XML 表示形式。</returns>
		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema() { return null; }

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		internal void ReadXml(System.Xml.XmlReader reader)
		{
			reader.MoveToContent();
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Whitespace) { continue; }
				else if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == CultureInfosElementName)
				{
					continue;
				}
				else if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == ResxDatasElementName)
				{
					continue;
				}
				else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == CustomCultureInfo.XmlElementName)
				{
					if (reader.HasAttributes)
					{
						for (int index = 0; index < reader.AttributeCount; index++)
						{
							reader.MoveToAttribute(index);
							string name = reader.LocalName;
							if (name == CustomCultureInfo.LcidAttribute)
							{
								int lcid = Convert.ToInt32(reader.GetAttribute(index));
								CultureInfo cultureInfo = new CultureInfo(lcid);
								foreach (CultureInfo enabledcci in enabledCultureInfos)
								{
									if (enabledcci.LCID == cultureInfo.LCID)
									{
										enabledCultureInfos.Remove(enabledcci);
										break;
									}
								}
								cultureInfos.Add(cultureInfo);
							}
						}
					}
				}
				else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == LocalizationItem.XmlElementName)
				{
					LocalizationItem localizationResource = new LocalizationItem(this);
					localizationResource.ReadXml(reader.ReadSubtree());
					base.Add(localizationResource);
				}
			}
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			ReadXml(reader);
		}
		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		internal void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement(XmlElementName);
			//Assembly assembly = typeof(LocalizationCollection).Assembly;
			//using (Stream stream = assembly.GetManifestResourceStream("Basic.Resource.LocalizationResource.xsd"))
			//{
			//    XmlTextReader schemaReader = new XmlTextReader(stream);
			//    writer.WriteNode(schemaReader, true);
			//}
			writer.WriteStartElement(CultureInfosElementName);
			foreach (CultureInfo cultureInfo in cultureInfos)
			{
				if (cultureInfo.LCID > 0)
				{
					writer.WriteStartElement(CustomCultureInfo.XmlElementName);
					writer.WriteStartAttribute(CustomCultureInfo.LcidAttribute);
					writer.WriteValue(cultureInfo.LCID);
					writer.WriteEndAttribute();
					writer.WriteAttributeString(CustomCultureInfo.NameAttribute, cultureInfo.Name);
					writer.WriteEndElement();
				}
			}
			writer.WriteEndElement();
			writer.WriteStartElement(ResxDatasElementName);
			IOrderedEnumerable<LocalizationItem> sortedList = base.Items.OrderBy(item => item.Name);
			foreach (LocalizationItem item in sortedList)
			{
				item.WriteXml(writer);
			}
			writer.WriteEndElement();
			writer.WriteEndElement();
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			WriteXml(writer);
		}
		#endregion
	}

	/// <summary>
	/// 表示本地化资源分组集合，支持集合变动通知。
	/// </summary>
	public sealed class GroupNameCollection : Collection<string>, INotifyCollectionChanged
	{
		/// <summary>
		/// 初始化 GroupNameCollection 类的新实例。
		/// </summary>
		public GroupNameCollection() : base() { }

		/// <summary>
		/// 初始化 GroupNameCollection 类的新实例，该实例包含从指定的可枚举集合中复制的元素。
		/// </summary>
		/// <param name="collection">要复制的可枚举集合。</param>
		public GroupNameCollection(IList<string> collection) : base(collection) { }

		/// <summary>
		/// 在添加、移除、更改或移动项或者在刷新整个列表时发生。
		/// </summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>
		/// 引发 CollectionChanged 事件的受保护方法。
		/// </summary>
		/// <param name="eventArgs">有关事件的信息。</param>
		internal void RaiseCollectionChanged(NotifyCollectionChangedEventArgs eventArgs)
		{
			CollectionChanged?.Invoke(this, eventArgs);
		}
	}
}
