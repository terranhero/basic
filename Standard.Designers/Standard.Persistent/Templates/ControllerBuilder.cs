using Basic.Configuration;
using Basic.DataEntities;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.Interfaces;
using Microsoft.VisualStudio.TextTemplating;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic.Builders
{
	/// <summary>
	/// 表示控制器生成类方法
	/// </summary>
	[System.Serializable()]
	public sealed class ControllerBuilder : AbstractViewBuilder
	{
		[System.NonSerialized()]
		private readonly EnvDTE.Project _Project;
		[System.NonSerialized()]
		private readonly EnvDTE.ProjectItem _ProjectItem;
		private readonly string _DefaultNamespance;
		private readonly ControllerBuilderData _Data = new ControllerBuilderData();
		/// <summary>
		/// 初始化 ControllerBuilder 类实例
		/// </summary>
		internal ControllerBuilder(PersistentService commandService, EnvDTE.Project project, EnvDTE.ProjectItem item)
			: base(commandService, project)
		{
			_Project = project; _ProjectItem = item;
			EnvDTE.Property nsProperty = _Project.Properties.Item("DefaultNamespace");
			if (nsProperty != null) { _DefaultNamespance = (string)nsProperty.Value; }
			if (_ProjectItem != null)
			{
				nsProperty = _ProjectItem.Properties.Item("DefaultNamespace");
				if (nsProperty != null) { _DefaultNamespance = (string)nsProperty.Value; }
			}
		}

		/// <summary>
		/// 模版名称
		/// </summary>
		protected override string TemplateName { get { return "Controller"; } }

		/// <summary>
		/// 当前项的默认命名空间
		/// </summary>
		public override string DefaultNamespance { get { return _DefaultNamespance; } }

		/// <summary>
		/// 表示控制器名称。
		/// </summary>
		public string ControllerClass { get { return string.Concat(base.Controller, "Controller"); } }

		/// <summary>
		/// 表示控制器名称。
		/// </summary>
		public string BaseController
		{
			get { return _Data.BaseController; }
			set
			{
				if (_Data.BaseController != value)
				{
					_Data.BaseController = value;
					OnPropertyChanged("BaseController");
				}
			}
		}

		/// <summary>
		/// 表示控制器名称文本框是否显示。
		/// </summary>
		public bool IndexEnabled
		{
			get { return _Data.IndexEnabled; }
			set
			{
				if (_Data.IndexEnabled != value)
				{
					_Data.IndexEnabled = value;
					OnPropertyChanged("IndexEnabled");
				}
			}
		}

		/// <summary>
		/// 表示控制器名称文本框是否显示。
		/// </summary>
		public bool CreateEnabled
		{
			get { return _Data.CreateEnabled; }
			set
			{
				if (_Data.CreateEnabled != value)
				{
					_Data.CreateEnabled = value;
					OnPropertyChanged("CreateEnabled");
				}
			}
		}

		/// <summary>
		/// 表示控制器名称文本框是否显示。
		/// </summary>
		public bool EditEnabled
		{
			get { return _Data.EditEnabled; }
			set
			{
				if (_Data.EditEnabled != value)
				{
					_Data.EditEnabled = value;
					OnPropertyChanged("EditEnabled");
				}
			}
		}

		/// <summary>
		/// 表示控制器名称文本框是否显示。
		/// </summary>
		public bool DeleteEnabled
		{
			get { return _Data.DeleteEnabled; }
			set
			{
				if (_Data.DeleteEnabled != value)
				{
					_Data.DeleteEnabled = value;
					OnPropertyChanged("DeleteEnabled");
				}
			}
		}

		/// <summary>
		/// 表示控制器名称文本框是否显示。
		/// </summary>
		public bool SearchAsyncEnabled
		{
			get { return _Data.SearchAsyncEnabled; }
			set
			{
				if (_Data.SearchAsyncEnabled != value)
				{
					_Data.SearchAsyncEnabled = value;
					OnPropertyChanged("SearchAsyncEnabled");
				}
			}
		}

		/// <summary>
		/// 表示控制器名称文本框是否显示。
		/// </summary>
		public bool ComplexSearchAsyncEnabled
		{
			get { return _Data.ComplexSearchAsyncEnabled; }
			set
			{
				if (_Data.ComplexSearchAsyncEnabled != value)
				{
					_Data.ComplexSearchAsyncEnabled = value;
					OnPropertyChanged("ComplexSearchAsyncEnabled");
				}
			}
		}

		/// <summary>
		/// 表示控制器名称文本框是否显示。
		/// </summary>
		public bool SearchEnabled
		{
			get { return _Data.SearchEnabled; }
			set
			{
				if (_Data.SearchEnabled != value)
				{
					_Data.SearchEnabled = value;
					OnPropertyChanged("SearchEnabled");
				}
			}
		}

		/// <summary>
		/// 表示控制器名称文本框是否显示。
		/// </summary>
		public bool ComplexSearchEnabled
		{
			get { return _Data.ComplexSearchEnabled; }
			set
			{
				if (_Data.ComplexSearchEnabled != value)
				{
					_Data.ComplexSearchEnabled = value;
					OnPropertyChanged("ComplexSearchEnabled");
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileName"></param>
		protected override void ReadPersistent(string fileName)
		{
			base.ReadPersistent(fileName);
			_Data.ContextName = _Persistent.ContextName;
			_Data.Namespace = _Persistent.Namespace;
			_Data.EntityNamespace = _Persistent.EntityNamespace;
			_Data.ControllerDescription = _Persistent.TableInfo.Description ?? _Persistent.TableInfo.TableName;
			_Data.Connection = _Persistent.ApplyConnection;
			if (_Persistent.NewEntity != null)
			{
				_Data.NewEntityName = _Persistent.NewEntity.ClassName;
				foreach (DataEntityPropertyElement property in _Persistent.NewEntity.Properties)
				{
					if (property.PrimaryKey && property.Type != null)
						_Data.NewEntityKeys.Add(new BuilderParameterData(property.Name, property.Type.Name));
					else if (property.PrimaryKey && property.Type == null)
						_Data.NewEntityKeys.Add(new BuilderParameterData(property.Name, property.TypeName));
				}
			}
			if (_Persistent.SearchEntity != null)
			{
				_Data.SearchEntityName = _Persistent.SearchEntity.ClassName;
				_Data.SearchConditionName = _Persistent.SearchEntity.Condition.ClassName;
				foreach (DataEntityPropertyElement property in _Persistent.SearchEntity.Properties)
				{
					if (property.PrimaryKey && property.Type != null)
						_Data.SearchEntityKeys.Add(new BuilderParameterData(property.Name, property.Type.Name));
					else if (property.PrimaryKey && property.Type == null)
						_Data.SearchEntityKeys.Add(new BuilderParameterData(property.Name, property.TypeName));
				}
			}
			if (_Persistent.EditEntity != null)
			{
				_Data.EditEntityName = _Persistent.EditEntity.ClassName;
				foreach (DataEntityPropertyElement property in _Persistent.EditEntity.Properties)
				{
					if (property.PrimaryKey && property.Type != null)
						_Data.EditEntityKeys.Add(new BuilderParameterData(property.Name, property.Type.Name));
					else if (property.PrimaryKey && property.Type == null)
						_Data.EditEntityKeys.Add(new BuilderParameterData(property.Name, property.TypeName));
				}
			}
			if (_Persistent.DeleteEntity != null)
			{
				_Data.DeleteEntityName = _Persistent.DeleteEntity.ClassName;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="defaultNamespace"></param>
		public override void GenerateCode(StreamWriter writer, string defaultNamespace)
		{
			string fileName = string.Concat(TemplateName, ".", _Provider.FileExtension, ".tt");
			TemplateFile = Path.Combine(AssemblyPath, TemplateFolder, fileName);
			//TemplateFile = @"D:\BASIC\PD_04_Trunk Code\Basic.Designers\Basic.Persistent\Templates\Controller.cs.tt";
			if (!File.Exists(TemplateFile)) { _CommandService.ShowMessage("模版文件不存在！" + TemplateFile); return; }
			ITextTemplatingSession session = CreateSession();
			session.Clear();
			session.Add("defaultNamespance", defaultNamespace);

			_Data.ControllerClass = this.ControllerClass;
			session.Add("builder", _Data);
			string input = File.ReadAllText(TemplateFile);
			string output = new Engine().ProcessTemplate(input, this);
			if (errorsValue.Count > 0)
			{
				foreach (System.CodeDom.Compiler.CompilerError error in errorsValue)
				{
					writer.WriteLine(error.ErrorText);
				}
				return;
			}
			writer.Write(output);
		}
	}
}
