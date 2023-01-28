using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.Configuration;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Basic.Collections;
using Basic.EntityLayer;
using Basic.Interfaces;
using System.CodeDom;
using Basic.DataAccess;


namespace Basic.Functions
{
	/// <summary>
	/// 表示调用基类的 GetEntities 方法。
	/// </summary>
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public sealed class GetEntitiesMethod : AbstractExecutableMethod
	{
		internal const string XmlElementName = "GetEntities";
		private readonly DynamicCommandElement dynamicCommand;
		internal GetEntitiesMethod(DynamicCommandElement element) : base(element) { dynamicCommand = element; }

		#region 类 AbstractExecutableMethod 抽象方法
		/// <summary>
		/// 
		/// </summary>
		public override string MethodName
		{
			get { return XmlElementName; }
		}

		/// <summary>
		/// 将当前类写入代码
		/// </summary>
		/// <param name="method"></param>
		protected internal override void WriteCode(System.CodeDom.CodeMemberMethod method)
		{
			method.Name = dataCommand.Name;
			method.ReturnType = new CodeTypeReference(typeof(QueryEntities<>));
			method.Comments.Add(new CodeCommentStatement("<summary>", true));
			method.Comments.Add(new CodeCommentStatement(dataCommand.Comment, true));
			method.Comments.Add(new CodeCommentStatement("</summary>", true));
			CodeBaseReferenceExpression baseReference = new CodeBaseReferenceExpression();
			CodeMethodReferenceExpression callMethodReference = new CodeMethodReferenceExpression(baseReference, MethodName);
			CodeMethodInvokeExpression callMethod = new CodeMethodInvokeExpression(callMethodReference);
			callMethod.Parameters.Add(new CodePrimitiveExpression(dataCommand.Name));
			CodeMethodReturnStatement returnMethod = new CodeMethodReturnStatement(callMethod);
			//foreach (Argument argument in Arguments)
			//{
			//    CodeTypeReference parameterTypeReference = new CodeTypeReference();
			//    method.Parameters.Add(new CodeParameterDeclarationExpression(parameterTypeReference, argument.Name));
			//    callMethod.Parameters.Add(new CodeVariableReferenceExpression(argument.Name));
			//}
			method.Statements.Add(returnMethod);

		}

		private string _GenericsType = "";
		/// <summary>
		/// 获取或设置当前方法是否调用基类的泛型方法。
		/// </summary>
		/// <value>返回类型</value>
		[System.ComponentModel.Description("获取或设置当前方法是否调用基类的泛型方法")]
		[System.ComponentModel.DefaultValue(""), System.ComponentModel.Category("DataCommand")]
		public string GenericsType
		{
			get { return _GenericsType; }
			set
			{
				if (_GenericsType != value)
				{
					base.OnPropertyChanging("GenericsType");
					_GenericsType = value;
					base.RaisePropertyChanged("GenericsType");
				}
			}
		}

		private Type _ArgumentType = typeof(AbstractEntity);
		/// <summary>
		/// 获取或设置当前方法是否调用基类方法的参数类型。
		/// </summary>
		/// <value>返回类型</value>
		[System.ComponentModel.Description("获取或设置当前方法是否调用基类方法的参数类型")]
		[System.ComponentModel.DefaultValue(typeof(AbstractEntity), "AbstractEntity"), System.ComponentModel.Category("DataCommand")]
		[System.ComponentModel.Editor(typeof(ArgumentTypeEditor), typeof(UITypeEditor))]
		public Type ArgumentType
		{
			get { return _ArgumentType; }
			set
			{
				if (_ArgumentType != value)
				{
					base.OnPropertyChanging("ArgumentType");
					_ArgumentType = value;
					base.RaisePropertyChanged("ArgumentType");
				}
			}
		}

		private string _EntityType = null;
		/// <summary>
		/// 获取或设置当前方法是否调用基类方法的可实例化的实体类型。
		/// </summary>
		/// <value>返回类型</value>
		[System.ComponentModel.Description("获取或设置当前方法是否调用基类方法的可实例化的实体类型")]
		[System.ComponentModel.DefaultValue(""), System.ComponentModel.Category("DataCommand")]
		public string EntityType
		{
			get { return _EntityType; }
			set
			{
				if (_EntityType != value)
				{
					base.OnPropertyChanging("EntityType");
					_EntityType = value;
					base.RaisePropertyChanged("EntityType");
				}
			}
		}
		#endregion

		#region 接口 ICustomDescriptor 默认实现
		/// <summary>
		/// 返回此组件实例的名称。
		/// </summary>
		/// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
		public override string GetComponentName()
		{
			return XmlElementName;
		}

		/// <summary>
		/// 返回表示当前类的 System.String 表示形式。
		/// </summary>
		/// <returns>System.String，表示当前的类。</returns>
		public override string ToString()
		{
			return XmlElementName;
		}
		#endregion

		#region 接口 IXmlSerializabl 实现
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
			return false;
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <param name="connectionEnum">表示数据库连接类型</param>
		protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
		{
		}
		#endregion

		/// <summary>
		/// 属性类型编辑器
		/// </summary>
		private class ArgumentTypeEditor : UITypeEditor
		{
			private TypeListBox listBox;
			/// <summary>
			/// 获取由 EditValue 方法使用的编辑器样式。
			/// </summary>
			/// <param name="context"></param>
			/// <returns></returns>
			public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
			{
				//指定为模式窗体属性编辑器类型
				return UITypeEditorEditStyle.DropDown;
			}

			/// <summary>
			/// 使用 System.Drawing.Design.UITypeEditor.GetEditStyle() 方法所指示的编辑器样式编辑指定对象的值。
			/// </summary>
			/// <param name="context">可用于获取附加上下文信息的 System.ComponentModel.ITypeDescriptorContext。</param>
			/// <param name="provider">System.IServiceProvider，此编辑器可用其来获取服务。</param>
			/// <param name="value">要编辑的对象。</param>
			/// <returns>新的对象值。如果对象的值尚未更改，则它返回的对象应与传递给它的对象相同。</returns>
			public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
			{
				if (provider != null)
				{
					IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
					if (editorService == null)
					{
						return value;
					}
					if (this.listBox == null)
					{
						this.listBox = new TypeListBox(new Type[] { typeof(IStringObjectArray), typeof(AbstractEntity), typeof(AbstractCondition) });
					}
					this.listBox.BeginEdit(editorService, value);
					editorService.DropDownControl(this.listBox);
					return this.listBox.SelectedItem;
				}
				return value;
				//return base.EditValue(context, provider, value);
			}
		}
	}
}
