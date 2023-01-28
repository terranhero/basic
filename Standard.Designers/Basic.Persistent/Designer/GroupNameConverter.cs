using System;
using System.Collections.Generic;
using System.ComponentModel;
using Basic.Configuration;
using Basic.Localizations;
using Microsoft;
using Microsoft.VisualStudio.Shell.Interop;

namespace Basic.Designer
{
	/// <summary>
	/// 提供消息转换器选择
	/// </summary>
	public sealed class GroupNameConverter : TypeConverter
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
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			List<string> strings = new List<string>();
			PersistentDescriptor persistentDescriptor = context.Instance as PersistentDescriptor;
			PersistentConfiguration persistent = persistentDescriptor.DefinitionInfo;
			MessageInfo messageInfo = persistent.MessageConverter;
			if (messageInfo.IsEmpty) { return new StandardValuesCollection(strings); }
			EnvDTE.DTE dteClass = (EnvDTE.DTE)context.GetService(typeof(EnvDTE.DTE));
			Assumes.Present(dteClass);
			System.IO.FileInfo fileSolution = new System.IO.FileInfo(dteClass.Solution.FullName);
			string fullName = string.Concat(fileSolution.DirectoryName, messageInfo.FileName);
			System.IO.FileInfo fileItem = new System.IO.FileInfo(dteClass.Solution.FullName);
			if (!fileItem.Exists)
			{
				string message = string.Concat("本地化资源文件\"", messageInfo.FileName, "\"已经不存在，请重新选择。");
				if (context.GetService(typeof(SVsUIShell)) is IVsUIShell iVsUIShell)
				{
					Guid tempGuid = Guid.Empty;
					_ = iVsUIShell.ShowMessageBox(0, ref tempGuid, "Basic.Persistent", message, null, 0, OLEMSGBUTTON.OLEMSGBUTTON_OK,
						OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_WARNING, 0, out _);
				}
				return new StandardValuesCollection(strings);
			}
			LocalizationCollection resourceCollection = new LocalizationCollection();
			resourceCollection.LoadGroups(fullName);
			return new StandardValuesCollection(resourceCollection.GroupNames);
		}
	}
}
