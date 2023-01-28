using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Basic.Designer
{
	/// <summary>
	/// 表示类型信息获取的抽象类实现
	/// </summary>
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public abstract class AbstractNotifyChangedDescriptor : ICustomDescriptor, INotifyPropertyChanged, IDisposable
	{
		private readonly AbstractNotifyChangedDescriptor notifyObject;
		/// <summary>
		/// 初始化 AbstractCustomTypeDescriptor 类实例。
		/// </summary>
		protected AbstractNotifyChangedDescriptor() { }

		/// <summary>
		/// 初始化 AbstractCustomTypeDescriptor 类实例。
		/// </summary>
		/// <param name="nofity">需要通知 AbstractCustomTypeDescriptor 类实例当前类的属性已更改。</param>
		protected AbstractNotifyChangedDescriptor(AbstractNotifyChangedDescriptor nofity) { notifyObject = nofity; }

		#region 接口 ICustomDescriptor 默认实现

		/// <summary>
		/// 返回此组件实例的类名。
		/// </summary>
		/// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
		public virtual string GetClassName() { return GetType().ToString(); }

		/// <summary>
		/// 返回此组件实例的名称。
		/// </summary>
		/// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
		public virtual string GetComponentName() { return GetType().ToString(); }

		/// <summary>
		/// 返回此组件实例的类名。
		/// </summary>
		/// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
		string ICustomDescriptor.GetClassName()
		{
			return GetClassName();
		}

		/// <summary>
		/// 返回此组件实例的名称。
		/// </summary>
		/// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
		string ICustomDescriptor.GetComponentName()
		{
			return GetComponentName();
		}

		/// <summary>
		/// 返回包含指定的属性描述符所描述的属性的对象。
		/// </summary>
		/// <param name="pd">表示要查找其所有者的属性的 System.ComponentModel.PropertyDescriptor。</param>
		/// <returns>表示指定属性所有者的 System.Object。</returns>
		object ICustomDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}
		#endregion

		#region 接口 INotifyPropertyChanged 的默认实现
		/// <summary>
		/// 在更改属性值时发生。
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// 引发 PropertyChanged 事件
		/// </summary>
		/// <param name="propertyName">已更改的属性名。</param>
		internal protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion

		#region 接口 IDisposable 默认实现
		/// <summary>
		/// 执行与释放或重置非托管资源相关的应用程序定义的任务。
		/// </summary>
		protected virtual void Dispose() { }

		/// <summary>
		/// 执行与释放或重置非托管资源相关的应用程序定义的任务。
		/// </summary>
		void IDisposable.Dispose()
		{
			Dispose();
		}
		#endregion
	}
}
