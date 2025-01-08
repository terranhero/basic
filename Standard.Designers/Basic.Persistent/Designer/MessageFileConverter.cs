using Basic.Configuration;
using Microsoft;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms.Design;

namespace Basic.Designer
{
	/// <summary>
	/// 提供消息转换器选择
	/// </summary>
	public sealed class MessageFileConverter : TypeConverter
	{
		/// <summary>
		/// 使用指定的上下文返回此对象是否支持可以从列表中选取的标准值集。
		/// </summary>
		/// <param name="context">一个提供格式上下文的 System.ComponentModel.ITypeDescriptorContext。</param>
		/// <returns>如果应调用 System.ComponentModel.TypeConverter.GetStandardValues() 来查找对象支持的一组公共值，则为 true；否则，为 false。</returns>
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }

		/// <summary>
		/// 当与格式上下文一起提供时，返回此类型转换器设计用于的数据类型的标准值集合。
		/// </summary>
		/// <param name="context">提供格式上下文的 System.ComponentModel.ITypeDescriptorContext，可用来提取有关从中调用此转换器的环境的附加信息。此参数或其属性可以为 null。</param>
		/// <returns>包含标准有效值集的 System.ComponentModel.TypeConverter.StandardValuesCollection；如果数据类型不支持标准值集，则为 null。</returns>
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			List<MessageInfo> strings = new List<MessageInfo>();
			PersistentConfiguration persistent = context.Instance as PersistentConfiguration;
			EnvDTE.DTE dteClass = (EnvDTE.DTE)context.GetService(typeof(EnvDTE.DTE));
			Assumes.Present(dteClass);
			EnvDTE.Solution solutionClass = dteClass.Solution;
			System.IO.FileInfo fileSolution = new System.IO.FileInfo(solutionClass.FullName);
			System.IO.FileInfo[] fileArray = fileSolution.Directory.GetFiles("*.localresx", System.IO.SearchOption.AllDirectories);
			char separator = System.IO.Path.PathSeparator;
			foreach (System.IO.FileInfo fileLocalResx in fileArray)
			{
				strings.Add(new MessageInfo(persistent, fileLocalResx.Name, fileLocalResx.FullName.Replace(fileSolution.DirectoryName, "")));
				// string fileName = fileLocalResx.FullName.Replace(fileSolution.DirectoryName, "");
			}
			//IVsSolution2 vsSolution = (IVsSolution2)context.GetService(typeof(SVsSolution));
			//IVsHierarchy hierarchy; Guid projectGuid = Guid.Empty;
			//vsSolution.GetProjectOfUniqueName(project.UniqueName, out hierarchy);
			//vsSolution.GetGuidOfProject(hierarchy, out projectGuid);
			//IVsHierarchy ivsh = VsShellUtilities.GetHierarchy(context, projectGuid);
			//DynamicTypeService typeService = (DynamicTypeService)context.GetService(typeof(DynamicTypeService));
			//ITypeDiscoveryService discovery = typeService.GetTypeDiscoveryService(ivsh);
			//ICollection types = discovery.GetTypes(typeof(Basic.Message.IMessageConverter), true);
			//foreach (Type type in types) { strings.Add(type.Name); }
			return new StandardValuesCollection(strings);
		}
	}

	/// <summary>
	/// 属性类型编辑器
	/// </summary>
	public sealed class MessageFileEditor : System.Drawing.Design.UITypeEditor
	{
		private MessageListBox listBox;
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
				if (editorService == null) { return value; }
				MessageInfo messageInfo = value as MessageInfo;
				if (messageInfo == null) { return value; }
				if (this.listBox == null)
				{
					EnvDTE.DTE dteClass = (EnvDTE.DTE)provider.GetService(typeof(EnvDTE.DTE));
					this.listBox = new MessageListBox(dteClass);
				}
				PersistentDescriptor objectDescriptor = context.Instance as PersistentDescriptor;
				PersistentConfiguration persistet = objectDescriptor.DefinitionInfo;
				this.listBox.BeginEdit(editorService, provider, persistet, messageInfo.ConverterName);
				editorService.DropDownControl(this.listBox);
				MessageInfo info = (MessageInfo)listBox.SelectedItem;
				if (info != null)
				{
					messageInfo.FileName = info.FileName;
					messageInfo.ConverterName = info.ConverterName;
				}
				return messageInfo;
			}
			return base.EditValue(context, provider, value);
		}

		private class MessageListBox : System.Windows.Forms.ListBox
		{
			private IWindowsFormsEditorService _editorService;
			private readonly EnvDTE.DTE dteClass;
			public MessageListBox(EnvDTE.DTE dte) { dteClass = dte; this.DisplayMember = "ConverterName"; this.ValueMember = "ConverterName"; }

			internal void BeginEdit(IWindowsFormsEditorService editorService, IServiceProvider provider, PersistentConfiguration persistent, string value)
			{
				_editorService = editorService;
				this.Items.Clear();
				this.Items.Add(new MessageInfo(persistent, null, null));
				EnvDTE.Solution solutionClass = dteClass.Solution;
				System.IO.FileInfo fileSolution = new System.IO.FileInfo(solutionClass.FullName);
				System.IO.FileInfo[] fileArray = fileSolution.Directory.GetFiles("*.localresx", System.IO.SearchOption.AllDirectories);
				char separator = System.IO.Path.PathSeparator;
				foreach (System.IO.FileInfo fileLocalResx in fileArray)
				{
					string fileName = Path.GetFileNameWithoutExtension(fileLocalResx.FullName);
					string resxPath = fileLocalResx.FullName.Replace(fileSolution.DirectoryName, "");
					this.Items.Add(new MessageInfo(persistent, fileName, resxPath));
					if (fileName == value) { SelectedIndex = this.Items.Count - 1; }
				}
				this.Height = this.ItemHeight * 12;
				if (this.PreferredHeight <= this.Height)
					this.Height = this.PreferredHeight;
				if (this.Height <= 39) { this.Height = 39; }
				//if (value != null) { SelectedValue = value; }
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			protected override void OnSelectedIndexChanged(EventArgs e)
			{
				base.OnSelectedIndexChanged(e);
				_editorService.CloseDropDown();
			}
		}
	}
}
