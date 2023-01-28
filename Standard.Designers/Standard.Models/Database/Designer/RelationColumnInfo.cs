using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.Designer;

namespace Basic.Database
{
	/// <summary>
	/// 数据库表关系中对应的列定义
	/// </summary>
	public sealed class RelationColumnInfo : AbstractNotifyChangedDescriptor
	{
		private readonly ColumnDesignerInfo parentColumn;

		private readonly ColumnDesignerInfo childColumn;
		private readonly RelationDesignerInfo relationInfo;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="relation"></param>
		/// <param name="parent"></param>
		/// <param name="child"></param>
		public RelationColumnInfo(RelationDesignerInfo relation, ColumnDesignerInfo parent, ColumnDesignerInfo child)
			: base(relation)
		{
			relationInfo = relation;
			parentColumn = parent;
			childColumn = child;
		}

		/// <summary>
		/// 表示关系中父表所在列
		/// </summary>
		public ColumnDesignerInfo Parent { get { return parentColumn; } }

		/// <summary>
		/// 表示关系中子表所在列
		/// </summary>
		public ColumnDesignerInfo Child { get { return childColumn; } }

		/// <summary>
		/// 数据库表关系
		/// </summary>
		public RelationDesignerInfo Relation { get { return relationInfo; } }

	}
}
