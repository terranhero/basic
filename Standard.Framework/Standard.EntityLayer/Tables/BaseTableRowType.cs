using Basic.EntityLayer;
using Basic.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Basic.Tables
{
	/// <summary>
	/// 表示强类型的抽象的 DataRow 类
	/// </summary>
	public abstract class BaseTableRowType :
#if (NET35)
 DataRow, INotifyPropertyChanging, INotifyPropertyChanged,IEntityInfo
#else
 DataRow, INotifyPropertyChanging, INotifyPropertyChanged, IEntityInfo, IDataErrorInfo
#endif
	{
		#region Data Command Events
		//private Action<BaseTableRowType> _CreateAction;
		///// <summary>
		///// Created 方法的回调
		///// </summary>
		///// <param name="action">Created 事件的回调函数。</param>
		//public void SetCreate(Action<BaseTableRowType> action) { _CreateAction = action; }

		///// <summary>
		///// 引发 Created 事件的方法
		///// </summary>
		//[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
		//internal void RaiseCreate() { if (_CreateAction != null) { _CreateAction(this); } }

		//private Action<BaseTableRowType> _UpdateAction;
		///// <summary>
		///// Update 方法的回调
		///// </summary>
		///// <param name="action">Update 事件的回调函数。</param>
		//public void SetUpdate(Action<BaseTableRowType> action) { _UpdateAction = action; }

		///// <summary>
		///// 引发 Update 事件的方法
		///// </summary>
		//[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
		//internal void RaiseUpdate() { if (_UpdateAction != null) { _UpdateAction(this); } }

		//private Action<BaseTableRowType> _DeleteAction;
		///// <summary>
		///// Delete 方法的回调
		///// </summary>
		///// <param name="action">Delete 事件的回调函数。</param>
		//public void SetDelete(Action<BaseTableRowType> action) { _DeleteAction = action; }

		///// <summary>
		///// 引发 Delete 事件的方法
		///// </summary>
		//[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
		//internal void RaiseDelete() { if (_DeleteAction != null) { _DeleteAction(this); } }

		private Func<BaseTableRowType, string, string, string> _CheckedAction;
		/// <summary>
		/// Checked 方法的回调
		/// </summary>
		/// <param name="action">Delete 事件的回调函数。</param>
		public void SetChecked(Func<BaseTableRowType, string, string, string> action) { _CheckedAction = action; }

		/// <summary>
		/// 引发 Checked 事件的方法
		/// </summary>
		/// <param name="converter"></param>
		/// <param name="errorCode"></param>
		[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
		internal string RaiseChecked(string converter, string errorCode)
		{
			if (_CheckedAction != null) { return _CheckedAction(this, converter, errorCode); }
			return errorCode;
		}

		private Action<BaseTableRowType> _CreatedValueAction;
		/// <summary>
		/// CreatedValue 方法的回调
		/// </summary>
		/// <param name="action">CreatedValue 事件的回调函数。</param>
		public void SetCreatedValue(Action<BaseTableRowType> action) { _CreatedValueAction = action; }

		/// <summary>
		/// 引发 CreatedValue 事件的方法
		/// </summary>
		[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
		internal void RaiseCreatedValue() { if (_CreatedValueAction != null) { _CreatedValueAction(this); } }
		#endregion

		/// <summary>
		/// 初始化 AbstractRow 的新实例。从生成器中构造行。仅供内部使用
		/// </summary>
		/// <param name="rb">生成器</param>
		protected BaseTableRowType(DataRowBuilder rb) : base(rb) { }

		/// <summary>
		/// 根据属性格式化错误信息。
		/// </summary>
		/// <param name="propertyName">属性名称</param>
		/// <param name="errorMessage">需要格式化的错误信息。</param>
		/// <returns>返回格式化的才错误信息。</returns>
		internal string FormatMessage(string propertyName, string errorMessage)
		{
			if (string.IsNullOrEmpty(propertyName)) { return errorMessage; }
			if (string.IsNullOrEmpty(errorMessage)) { return errorMessage; }
			if (errorMessage.IndexOf("{0}") >= 0)
			{
				PropertyInfo propertyInfo = this.GetType().GetProperty(propertyName);
				if (propertyInfo == null) { return errorMessage; }
				MethodInfo getMethodInfo = propertyInfo.GetGetMethod(true);
				if (getMethodInfo == null) { throw new System.ArgumentException("属性 get 访问器无效。", propertyName); }
				object value = getMethodInfo.Invoke(this, new object[] { });
				return string.Format(errorMessage, value);
			}
			return errorMessage;
		}

		/// <summary>
		/// 获取或设置存储在由名称指定的列中的数据。
		/// </summary>
		/// <param name="columnName">列的名称。</param>
		/// <returns> System.Object，包含该数据。</returns>
		/// <exception cref="System.ArgumentException">该列不属于此表。</exception>
		/// <exception cref="System.ArgumentNullException">column 为 null。</exception>
		/// <exception cref="System.Data.DeletedRowInaccessibleException">尝试对已删除的行设置值。</exception>
		/// <exception cref="System.InvalidCastException">值与列的数据类型不匹配。</exception>
		public new object this[string columnName]
		{
			get { return base[columnName]; }
			set
			{
				if (base[columnName] != value)
				{
					OnPropertyChanging(columnName);
					base[columnName] = value;
					OnPropertyChanged(columnName);
				}
			}
		}

		/// <summary>
		/// 获取或设置存储在指定的 System.Data.DataColumn 中的数据。
		/// </summary>
		/// <param name="column">一个包含数据的 System.Data.DataColumn。</param>
		/// <returns> System.Object，包含该数据。</returns>
		/// <exception cref="System.ArgumentException">该列不属于此表。</exception>
		/// <exception cref="System.ArgumentNullException">column 为 null。</exception>
		/// <exception cref="System.Data.DeletedRowInaccessibleException">尝试对已删除的行设置值。</exception>
		/// <exception cref="System.InvalidCastException">值与列的数据类型不匹配。</exception>
		public new object this[DataColumn column]
		{
			get { return base[column]; }
			set
			{
				if (base[column] != value)
				{
					OnPropertyChanging(column.ColumnName);
					base[column] = value;
					OnPropertyChanged(column.ColumnName);
				}
			}
		}

		#region INotifyPropertyChanging 成员, INotifyPropertyChanged 成员
		/// <summary>
		/// 在属性值更改时发生。
		/// </summary>
		public event PropertyChangingEventHandler PropertyChanging;
		/// <summary>
		/// 引发PropertyChanging事件
		/// </summary>
		/// <param name="propertyName">其值将更改的属性的名称。</param>
		protected virtual void OnPropertyChanging(string propertyName)
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
			}
		}

		/// <summary>
		/// 在更改属性值时发生。
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		/// <summary>
		/// 引发PropertyChanged事件
		/// </summary>
		/// <param name="propertyName">其值将更改的属性的名称。</param>
		protected virtual void OnPropertyChanged(string propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion

#if (NET35)
#else
		/// <summary>
		/// 获取指示对象何处出错的错误信息。
		/// </summary>
		/// <value>指示对象何处出错的错误信息。默认值为空字符串 ("")。</value>
		string IDataErrorInfo.Error { get { return string.Empty; } }

		/// <summary>
		/// 获取具有给定名称的属性的错误信息。
		/// </summary>
		/// <param name="propertyName">要获取其错误信息的属性的名称。</param>
		/// <returns>该属性的错误信息。默认值为空字符串 ("")。</returns>
		string IDataErrorInfo.this[string propertyName]
		{
			get
			{
				List<ValidationResult> res = new List<ValidationResult>();
				if (!ValidationProperty(propertyName, res) && res.Count > 0)
				{
					return string.Join(Environment.NewLine, res.Select(r => r.ErrorMessage));
				}
				return string.Empty;
			}
		}

		/// <summary>
		/// 验证所有属性的值，确定指定的对象是否有效。
		/// </summary>
		/// <returns>如果对象有效，则为 true；否则为 false。</returns>
		public bool Validation()
		{
			ICollection<ValidationResult> validationResults = new List<ValidationResult>();
			return Validation(validationResults);
		}

		/// <summary>
		/// 通过使用验证结果集合验证所有属性的值，确定指定的对象是否有效。
		/// </summary>
		/// <param name="validationResults">用于包含每个失败的验证的集合。</param>
		/// <returns>如果对象有效，则为 true；否则为 false。</returns>
		public bool Validation(ICollection<ValidationResult> validationResults)
		{
			ValidationContext validationContext = new ValidationContext(this, null, null);
			return Validator.TryValidateObject(this, validationContext, validationResults, true);
		}

		/// <summary>
		/// 验证属性的值，确定指定的对象是否有效。
		/// </summary>
		/// <param name="propertyName">当前对象的包含的属性名称。</param>
		/// <returns>如果对象有效，则为 true；否则为 false。</returns>
		public bool ValidationProperty(string propertyName)
		{
			ICollection<ValidationResult> validationResults = new List<ValidationResult>();
			return ValidationProperty(propertyName, validationResults);
		}

		/// <summary>
		/// 通过使用验证结果集合验证属性的值，确定指定的对象是否有效。
		/// </summary>
		/// <param name="propertyName">当前对象的包含的属性名称。</param>
		/// <param name="validationResults">用于包含每个失败的验证的集合。</param>
		/// <returns>如果对象有效或属性不存在，则为 true；否则为 false。</returns>
		public bool ValidationProperty(string propertyName, ICollection<ValidationResult> validationResults)
		{
			PropertyInfo propertyInfo = GetType().GetProperty(propertyName);
			if (propertyInfo == null) { return true; }
			MethodInfo getMethodInfo = propertyInfo.GetGetMethod(true);
			if (getMethodInfo == null) { throw new System.ArgumentException("属性 get 访问器无效。", propertyName); }
			object value = getMethodInfo.Invoke(this, new object[] { });
			ValidationContext validationContext = new ValidationContext(this, null, null);
			validationContext.MemberName = propertyName;
			return Validator.TryValidateProperty(value, validationContext, validationResults);
		}
#endif
	}
}
