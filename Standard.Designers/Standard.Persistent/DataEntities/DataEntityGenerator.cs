using Basic.Designer;
using Basic.Enums;
using System.Data;
using System.Drawing.Design;

namespace Basic.DataEntities
{
	/// <summary>
	/// 
	/// </summary>
	[Basic.Designer.PersistentCategory("PersistentCategory_CodeGenerator")]
	[Basic.Designer.PersistentDescription("PropertyDescription_DataEntityGenerator")]
	[System.ComponentModel.TypeConverter(typeof(ConditionTypeConverter))]
	public sealed class DataEntityGenerator
	{
		private readonly DataEntityElement _DataEntity;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dataEntity"></param>
		public DataEntityGenerator(DataEntityElement dataEntity) { _DataEntity = dataEntity; }
		/// <summary>
		/// 获取或设置一个值，表示实体类型Guid。
		/// </summary>
		[System.ComponentModel.DefaultValue(""), System.ComponentModel.Bindable(true)]
		[Basic.Designer.PersistentCategory("PersistentCategory_EntityCodeGenerator")]
		[Basic.Designer.PersistentDescription("PropertyDescription_Guid")]
		[System.ComponentModel.Editor(typeof(GuidTypeEditor), typeof(UITypeEditor))]
		public System.Guid Guid
		{
			get { return _DataEntity.Guid; }
			set { _DataEntity.Guid = value; }
		}

		/// <summary>
		/// 获取或设置当前实体类的基类类型.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_BaseClass")]
		[Basic.Designer.PersistentCategory("PersistentCategory_EntityCodeGenerator")]
		[System.ComponentModel.DefaultValue("Basic.EntityLayer.AbstractEntity")]
		[System.ComponentModel.Editor(typeof(BaseClassSelector), typeof(UITypeEditor))]
		public string BaseClass
		{
			get { return _DataEntity.BaseClass; }
			set { _DataEntity.BaseClass = value; }
		}

		/// <summary>
		/// Gets or sets the EntityDefinition Name.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_IsAbstract")]
		[Basic.Designer.PersistentCategory("PersistentCategory_EntityCodeGenerator")]
		[System.ComponentModel.DefaultValue(false)]
		public bool IsAbstract
		{
			get { return _DataEntity.IsAbstract; }
			set { _DataEntity.IsAbstract = value; }
		}

		/// <summary>
		/// Gets or sets the EntityDefinition Name.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[System.ComponentModel.DefaultValue(typeof(ClassModifierEnum), "Public"), System.ComponentModel.Bindable(true)]
		[Basic.Designer.PersistentCategory("PersistentCategory_EntityCodeGenerator")]
		[Basic.Designer.PersistentDescription("PersistentDescription_Modifier")]
		public ClassModifierEnum Modifier
		{
			get { return _DataEntity.Modifier; }
			set { _DataEntity.Modifier = value; }
		}

		/// <summary>
		/// Gets or sets the EntityDefinition ClassName.
		/// </summary>
		/// <value>The string value assigned to the Name property</value>
		[Basic.Designer.PersistentDescription("PersistentDescription_Modifier")]
		[Basic.Designer.PersistentCategory("PersistentCategory_EntityCodeGenerator")]
		public string ClassName { get { return _DataEntity.ClassName; } }

		/// <summary>
		/// 当前对象的字符串表示形式。
		/// </summary>
		/// <returns>表示当前对象的字符串。</returns>
		public override string ToString()
		{
			return _DataEntity.BaseClass;
		}
	}
}
