using Basic.Designer;
using Basic.Enums;
using System.Drawing.Design;

namespace Basic.DataEntities
{
	/// <summary>
	/// 
	/// </summary>
	[Basic.Designer.PersistentCategory("PersistentCategory_ConditionCodeGenerator")]
	[Basic.Designer.PersistentDescription("PropertyDescription_DataConditionGenerator")]
	[System.ComponentModel.TypeConverter(typeof(ConditionTypeConverter))]
	public sealed class DataConditionGenerator
	{
		private readonly DataEntityElement _DataEntity;
		private readonly DataConditionElement _Condition;
		/// <summary>
		/// 初始化 DataConditionGenerator 类实例。
		/// </summary>
		/// <param name="dataEntity"></param>
		public DataConditionGenerator(DataEntityElement dataEntity)
		{
			_DataEntity = dataEntity;
			_Condition = dataEntity.Condition;
		}

		/// <summary>
		/// 获取或设置一个值，表示实体类型Guid。
		/// </summary>
		[System.ComponentModel.DefaultValue(""), System.ComponentModel.Bindable(true)]
		[Basic.Designer.PersistentCategory("PersistentCategory_ConditionCodeGenerator")]
		[Basic.Designer.PersistentDescription("PropertyDescription_Guid")]
		[System.ComponentModel.Editor(typeof(GuidTypeEditor), typeof(UITypeEditor))]
		public System.Guid Guid
		{
			get { return _Condition.Guid; }
			set { _Condition.Guid = value; }
		}

		/// <summary>
		/// 获取或设置当前实体类的基类类型.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_BaseClass")]
		[Basic.Designer.PersistentCategory("PersistentCategory_ConditionCodeGenerator")]
		[System.ComponentModel.DefaultValue("Basic.EntityLayer.AbstractCondition")]
		[System.ComponentModel.Editor(typeof(BaseConditionSelector), typeof(UITypeEditor))]
		public string BaseClass
		{
			get { return _Condition.BaseClass; }
			set { _Condition.BaseClass = value; }
		}

		/// <summary>
		/// Gets or sets the EntityDefinition Name.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_IsAbstract")]
		[Basic.Designer.PersistentCategory("PersistentCategory_ConditionCodeGenerator")]
		[System.ComponentModel.DefaultValue(false)]
		public bool IsAbstract
		{
			get { return _Condition.IsAbstract; }
			set { _Condition.IsAbstract = value; }
		}

		/// <summary>
		/// Gets or sets the EntityDefinition Name.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[System.ComponentModel.DefaultValue(typeof(ClassModifierEnum), "Public"), System.ComponentModel.Bindable(true)]
		[Basic.Designer.PersistentCategory("PersistentCategory_ConditionCodeGenerator")]
		[Basic.Designer.PersistentDescription("PersistentDescription_Modifier")]
		public ClassModifierEnum Modifier
		{
			get { return _Condition.Modifier; }
			set { _Condition.Modifier = value; }
		}

		/// <summary>
		/// Gets or sets the EntityDefinition ClassName.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_ClassName")]
		[Basic.Designer.PersistentCategory("PersistentCategory_Content")]
		public string ClassName { get { return _Condition.ClassName; } }

		/// <summary>
		/// 当前对象的字符串表示形式。
		/// </summary>
		/// <returns>表示当前对象的字符串。</returns>
		public override string ToString()
		{
			return _Condition.BaseClass;
		}
	}
}
