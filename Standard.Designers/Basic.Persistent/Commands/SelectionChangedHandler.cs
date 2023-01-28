using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Basic.Commands
{
    /// <summary>
    /// 表示 DesignerEntity 类型控件中项的选择更改事件委托。
    /// </summary>
    /// <param name="sender">事件发起者</param>
    /// <param name="e">包含 SelectionChanged 事件的参数。</param>
    public delegate void SelectionChangedHandler(object sender, SelectionChangedEventArgs e);

    /// <summary>
    /// SelectionChanged 事件的参数
    /// </summary>
    public class SelectionChangedEventArgs : RoutedEventArgs
    {
        // Fields
        private readonly object _oldValues;
        private readonly object _newValues;

        /// <summary>
        /// 初始化 SelectionChangedEventArgs 类新实例。
        /// </summary>
        /// <param name="routedEvent">系统中注册的路由事件，此处特指 SelectionChangedEvent 事件。</param>
        /// <param name="oldValue">选择事件新植。</param>
        /// <param name="newValue">选择事件原值。</param>
        public SelectionChangedEventArgs(RoutedEvent routedEvent, object oldValue, object newValue)
        {
            this._oldValues = oldValue;
            this._newValues = newValue;
            base.RoutedEvent = routedEvent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="genericHandler"></param>
        /// <param name="genericTarget"></param>
        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            SelectionChangedHandler handler = (SelectionChangedHandler)genericHandler;
            handler(genericTarget, (SelectionChangedEventArgs)this);
        }

        /// <summary>
        /// 
        /// </summary>
        public object NewValue { get { return this._newValues; } }

        /// <summary>
        /// 
        /// </summary>
        public object OldValue { get { return this._oldValues; } }
    }

}
