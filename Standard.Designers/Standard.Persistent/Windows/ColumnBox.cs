using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using Basic.Database;
using Basic.Enums;

namespace Basic.Windows
{
	/// <summary>
	/// 表示TreeView节点上的表
	/// </summary>
	public class ColumnBox : CheckBox
	{
		static ColumnBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ColumnBox), new FrameworkPropertyMetadata(typeof(ColumnBox)));
		}

		#region 属性 PrimaryKey 定义
		public static readonly DependencyProperty PrimaryKeyProperty = DependencyProperty.Register("PrimaryKey",
			typeof(bool), typeof(ColumnBox), new PropertyMetadata(false));
		/// <summary>
		/// 判断当前控件是否已经选择。
		/// </summary>
		public bool PrimaryKey
		{
			get { return (bool)base.GetValue(PrimaryKeyProperty); }
			set { base.SetValue(PrimaryKeyProperty, value); }
		}
		#endregion

		#region 属性 IsWhere 定义
		public static readonly DependencyProperty IsWhereProperty = DependencyProperty.Register("IsWhere",
			typeof(bool), typeof(ColumnBox), new PropertyMetadata(false));
		/// <summary>
		/// 判断当前控件是否已经选择。
		/// </summary>
		public bool IsWhere
		{
			get { return (bool)base.GetValue(IsWhereProperty); }
			set { base.SetValue(IsWhereProperty, value); }
		}
		#endregion

		#region 属性 SortOrder 定义
		public static readonly DependencyProperty SortOrderProperty = DependencyProperty.Register("SortOrder",
			typeof(OrderEnum), typeof(ColumnBox), new PropertyMetadata(OrderEnum.None));
		/// <summary>
		/// 判断当前控件是否已经选择。
		/// </summary>
		public OrderEnum SortOrder
		{
			get { return (OrderEnum)base.GetValue(SortOrderProperty); }
			set { base.SetValue(SortOrderProperty, value); }
		}
		#endregion

		#region 属性 Group 定义
		public static readonly DependencyProperty GroupProperty = DependencyProperty.Register("Group",
			typeof(bool), typeof(ColumnBox), new PropertyMetadata(false));
		/// <summary>
		/// 判断当前控件是否已经选择。
		/// </summary>
		public bool Group
		{
			get { return (bool)base.GetValue(GroupProperty); }
			set { base.SetValue(GroupProperty, value); }
		}
		#endregion

		#region 属性 IsAggregate 定义
		public static readonly DependencyProperty HasAggregateProperty = DependencyProperty.Register("HasAggregate",
			typeof(bool), typeof(ColumnBox), new PropertyMetadata(false));
		/// <summary>
		/// 判断当前控件是否已经选择。
		/// </summary>
		public bool HasAggregate
		{
			get { return (bool)base.GetValue(HasAggregateProperty); }
			set { base.SetValue(HasAggregateProperty, value); }
		}
		#endregion

		#region 重载拖放操作的拖动源事件
		private Point dragStartPoint;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnPreviewMouseLeftButtonDown(e);
			dragStartPoint = e.MouseDevice.GetPosition(this);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreviewMouseMove(MouseEventArgs e)
		{
			base.OnPreviewMouseMove(e);
			ColumnDesignerInfo dropColumn = DataContext as ColumnDesignerInfo;
			if (AllowDrop && e.LeftButton == MouseButtonState.Pressed &&
				dragStartPoint != e.GetPosition(this) && !dropColumn.Table.MainTable)
			{
				DragDrop.DoDragDrop(this, this, DragDropEffects.Copy);
			}
		}

		/// <summary>
		///  此事件在拖放操作过程中持续发生，使放置源可以向用户提供反馈信息。 
		///  提供此反馈的方法通常是更改鼠标指针的外观，以指示放置目标允许的效果。
		/// </summary>
		/// <param name="e">包含事件数据的 System.Windows.GiveFeedbackEventArgs。</param>
		protected override void OnPreviewGiveFeedback(GiveFeedbackEventArgs e)
		{
			base.OnPreviewGiveFeedback(e);
		}

		/// <summary>
		///  当拖放操作期间键盘或鼠标按钮状态更改时发生此事件，
		///  它使放置源能够根据键/按钮状态取消拖放操作。 
		/// </summary>
		/// <param name="e">包含事件数据的 System.Windows.QueryContinueDragEventArgs。</param>
		protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e)
		{
			base.OnQueryContinueDrag(e);
		}
		#endregion

		#region 重载拖放操作的放置目标事件
		/// <summary>
		/// 将对象放到放置目标上时发生此事件。 
		/// </summary>
		/// <param name="e">包含事件数据的 System.Windows.DragEventArgs。</param>
		protected override void OnPreviewDrop(DragEventArgs e)
		{
			base.OnPreviewDrop(e);
			ColumnBox columnBox = e.Data.GetData(typeof(ColumnBox)) as ColumnBox;
			if (columnBox != this && columnBox != null)
			{
				ColumnDesignerInfo dropColumn = DataContext as ColumnDesignerInfo;
				ColumnDesignerInfo dragColumn = columnBox.DataContext as ColumnDesignerInfo;
				if (Background != Brushes.Transparent)
					Background = Brushes.Transparent;
				if (dropColumn.Table != dragColumn.Table)
				{
					TableDesignerInfo fkeyTable = dragColumn.Table;
					fkeyTable.Tables.AddRelation(dropColumn, dragColumn);
				}
			}
		}

		/// <summary>
		///  将对象拖至放置目标的边框内时发生此事件。
		/// </summary>
		/// <param name="e">包含事件数据的 System.Windows.DragEventArgs。</param>
		protected override void OnPreviewDragEnter(DragEventArgs e)
		{
			ColumnBox columnBox = e.Data.GetData(typeof(ColumnBox)) as ColumnBox;
			if (columnBox != this && columnBox != null)
			{
				ColumnDesignerInfo dropColumn = DataContext as ColumnDesignerInfo;
				ColumnDesignerInfo dragColumn = columnBox.DataContext as ColumnDesignerInfo;
				if (dropColumn.Table != dragColumn.Table)
					Background = Brushes.LightCyan;
			}
			base.OnPreviewDragEnter(e);
		}

		/// <summary>
		/// 将对象拖出放置目标的边框时发生此事件。
		/// </summary>
		/// <param name="e">包含事件数据的 System.Windows.DragEventArgs。</param>
		protected override void OnPreviewDragLeave(DragEventArgs e)
		{
			if (Background != Brushes.Transparent)
				Background = Brushes.Transparent;
			base.OnPreviewDragLeave(e);
		}
		#endregion
	}
}
