using System;
using Basic.Designer;

namespace Basic.Configuration
{
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public sealed class DesignerInfoElement : AbstractCustomTypeDescriptor
	{
		internal const string XmlElementName = "DesignerInfo";
		internal const string ExpanderAttribute = "Expander";
		internal const string WidthAttribute = "Width";
		internal const string HeightAttribute = "Height";
		internal const string TopAttribute = "Top";
		internal const string LeftAttribute = "Left";

		/// <summary>
		/// 初始化 AbstractEntityElement 类实例。
		/// </summary>
		/// <param name="baseClass"></param>
		internal DesignerInfoElement(AbstractCustomTypeDescriptor nofity)
			: base(nofity)
		{
			_Left = new Random().Next(400);
			_Top = new Random().Next(200);
		}

		/// <summary>
		/// 返回此组件实例的名称。
		public override string GetComponentName() { return typeof(DesignerInfoElement).Name; }

		/// <summary>
		/// 获取当前节点元素命名空间
		/// </summary>
		protected internal override string ElementNamespace { get { return null; } }

		public override string GetClassName() { return typeof(DesignerInfoElement).Name; }

		protected internal override string ElementName { get { return XmlElementName; } }

		private double _Width = double.NaN;
		/// <summary>
		/// 获取或设置命令描述
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public double Width
		{
			get { return _Width; }
			set
			{
				if (_Width != value)
				{
					_Width = value;
					base.RaisePropertyChanged("Width");
				}
			}
		}

		private double _Height = double.NaN;
		/// <summary>
		/// 获取或设置命令描述
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public double Height
		{
			get { return _Height; }
			set
			{
				if (_Height != value)
				{
					_Height = value;
					base.RaisePropertyChanged("Height");
				}
			}
		}

		private double _Top = double.NaN;
		/// <summary>
		/// 获取或设置命令描述
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public double Top
		{
			get { return _Top; }
			set
			{
				if (_Top != value)
				{
					_Top = value;
					base.RaisePropertyChanged("Top");
				}
			}
		}

		private double _Left = double.NaN;
		/// <summary>
		/// 获取或设置命令描述
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public double Left
		{
			get { return _Left; }
			set
			{
				if (_Left != value)
				{
					_Left = value;
					base.RaisePropertyChanged("Left");
				}
			}
		}

		private bool _Expander = false;
		/// <summary>
		/// 获取或设置命令描述
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public bool Expander
		{
			get { return _Expander; }
			set
			{
				if (_Expander != value)
				{
					_Expander = value;
					base.RaisePropertyChanged("Expander");
				}
			}
		}

		/// <summary>
		/// 从对象的 XML 表示形式读取属性。
		/// </summary>
		/// <param name="name">属性名称。</param>
		/// <param name="value">属性值</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		protected internal override bool ReadAttribute(string name, string value)
		{
			if (name == ExpanderAttribute) { _Expander = Convert.ToBoolean(value); return true; }
			else if (name == WidthAttribute) { _Width = Convert.ToDouble(value); return true; }
			else if (name == HeightAttribute) { _Height = Convert.ToDouble(value); return true; }
			else if (name == LeftAttribute) { _Left = Convert.ToDouble(value); return true; }
			else if (name == TopAttribute) { _Top = Convert.ToDouble(value); return true; }
			return false;
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		protected internal override bool ReadContent(System.Xml.XmlReader reader)
		{
			return true;
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
		{
			if (_Expander)
				writer.WriteAttributeString(ExpanderAttribute, Convert.ToString(_Expander).ToLower());
			if (!double.IsNaN(_Width))
				writer.WriteAttributeString(WidthAttribute, Convert.ToString(_Width));
			if (!double.IsNaN(_Height))
				writer.WriteAttributeString(HeightAttribute, Convert.ToString(_Height));
			if (!double.IsNaN(_Left))
				writer.WriteAttributeString(LeftAttribute, Convert.ToString(_Left));
			if (!double.IsNaN(_Top))
				writer.WriteAttributeString(TopAttribute, Convert.ToString(_Top));
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <param name="connectionType">表示数据库连接类型</param>
		protected internal override void WriteContent(System.Xml.XmlWriter writer) { }

		protected internal override void GenerateConfiguration(System.Xml.XmlWriter writer, Enums.ConnectionTypeEnum connectionType)
		{
		}
	}
}
