using Basic.Collections;
using Basic.Designer;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Basic.Database
{
	/// <summary>
	/// 数据库表关系
	/// </summary>
	public sealed class RelationDesignerInfo : AbstractNotifyChangedDescriptor
	{
		private readonly RelationDesignerCollection tableRelations;
		/// <summary>
		/// 拥有此关系的数据库表
		/// </summary>
		private readonly TableDesignerInfo parentTableInfo;

		/// <summary>
		/// 拥有此关系的数据库表
		/// </summary>
		private readonly TableDesignerInfo childTableInfo;

		/// <summary>
		/// 拥有此关系的数据库表
		/// </summary>
		private readonly RelationColumnCollection relationColumns;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="relations"></param>
		/// <param name="parentTable"></param>
		/// <param name="childTable"></param>
		public RelationDesignerInfo(RelationDesignerCollection relations, TableDesignerInfo parentTable, TableDesignerInfo childTable)
			: base(parentTable)
		{
			tableRelations = relations;
			parentTableInfo = parentTable; childTableInfo = childTable;
			relationColumns = new RelationColumnCollection(this);
			relationColumns.CollectionChanged += new NotifyCollectionChangedEventHandler(relationColumns_CollectionChanged);
		}

		private void relationColumns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			base.OnPropertyChanged("Relation");
		}

		/// <summary>
		/// 关系父表
		/// </summary>
		public TableDesignerInfo Parent { get { return parentTableInfo; } }

		/// <summary>
		/// 关系父表
		/// </summary>
		public TableDesignerInfo Child { get { return childTableInfo; } }

		/// <summary>
		/// 关系父表
		/// </summary>
		public RelationColumnCollection Columns { get { return relationColumns; } }

		/// <summary>
		/// 添加关系
		/// </summary>
		/// <param name="parentColumn"></param>
		/// <param name="childColumn"></param>
		public void Add(ColumnDesignerInfo parentColumn, ColumnDesignerInfo childColumn)
		{
			foreach (RelationColumnInfo relationColumnInfo in relationColumns)
			{
				if (relationColumnInfo.Parent == parentColumn || relationColumnInfo.Child == childColumn) { return; }
			}
			relationColumns.Add(new RelationColumnInfo(this, parentColumn, childColumn));
		}

		private string _Join = "JOIN";
		/// <summary>
		/// 获取或设置数据库表中的列的名称。
		/// </summary>
		[Basic.Designer.PackageDescription("PersistentDescription_ColumnName")]
		[Basic.Designer.PackageCategory(PackageCategoryAttribute.CategoryContent)]
		[System.ComponentModel.DefaultValue("JOIN")]
		public string Join
		{
			get { return _Join; }
			set
			{
				if (_Join != value)
				{
					_Join = value;
					base.OnPropertyChanged("Join");
					tableRelations.OnRelationChanged(this);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string Relation
		{
			get
			{
				List<string> list = new List<string>();
				foreach (RelationColumnInfo ci in relationColumns)
				{
					list.Add(string.Concat(ci.Parent.SelectName, "=", ci.Child.SelectName));
				}
				return string.Join(" AND ", list.ToArray());
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string JoinTable()
		{
			int index = tableRelations.IndexOf(this);
			StringBuilder joinBuilder = new StringBuilder(500);
			if (index == 0)
			{
				StringBuilder tableBuilder = new StringBuilder(300);
				tableBuilder.Append(parentTableInfo.FromName).Append(" ").Append(_Join).Append(" ");
				tableBuilder.Append(childTableInfo.FromName).Append(" ON ");
				int length = tableBuilder.Length;
				foreach (RelationColumnInfo relationColumn in Columns)
				{
					if (length == tableBuilder.Length)
						tableBuilder.Append(relationColumn.Parent.SelectName).Append("=").Append(relationColumn.Child.SelectName);
					else
						tableBuilder.Append(" AND ").Append(relationColumn.Parent.SelectName).Append("=").Append(relationColumn.Child.SelectName);
				}
				if (joinBuilder.Length != 0)
					joinBuilder.AppendLine();
				joinBuilder.Append(tableBuilder.ToString());
			}
			else
			{
				StringBuilder tableBuilder = new StringBuilder(300);
				tableBuilder.Append(" ").Append(_Join).Append(" ").Append(childTableInfo.FromName).Append(" ON ");
				int length = tableBuilder.Length;
				foreach (RelationColumnInfo relationColumn in Columns)
				{
					if (length == tableBuilder.Length)
						tableBuilder.Append(relationColumn.Parent.SelectName).Append("=").Append(relationColumn.Child.SelectName);
					else
						tableBuilder.Append(" AND ").Append(relationColumn.Parent.SelectName).Append("=").Append(relationColumn.Child.SelectName);
				}
				if (joinBuilder.Length != 0)
					joinBuilder.AppendLine();
				joinBuilder.Append(tableBuilder.ToString());
			}
			return joinBuilder.ToString();
		}
	}
}
