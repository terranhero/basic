using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic.Builders
{
	public class AbstractPropertyChanged : System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// 在更改属性值时发生。
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 引发 PropertyChanged 事件的方法
        /// </summary>
        /// <param name="propertyName">表示当前值变化的属性名称。</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName)); }
        }
    }
}
