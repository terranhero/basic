using Basic.Configuration;
using Basic.DataEntities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Basic.Designer
{
    /// <summary>
    /// 提供数据库当前命令所在实体模型的异常代码转换器。
    /// 提供在字符串对象与其他表示形式之间实现相互转换的类型转换器。
    /// </summary>
    public sealed class ErrorCodeConveter : StringConverter
    {      /// <summary>
        /// 使用指定的上下文返回此对象是否支持可以从列表中选取的标准值集。
        /// </summary>
        /// <param name="context">一个提供格式上下文的 System.ComponentModel.ITypeDescriptorContext。</param>
        /// <returns>如果应调用 System.ComponentModel.TypeConverter.GetStandardValues() 来查找对象支持的一组公共值，则为 true；否则，为 false。</returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }

        /// <summary>
        /// 当与格式上下文一起提供时，返回此类型转换器设计用于的数据类型的标准值集合。
        /// </summary>
        /// <param name="context">提供格式上下文的 System.ComponentModel.ITypeDescriptorContext，可用来提取有关从中调用此转换器的环境的附加信息。此参数或其属性可以为 null。</param>
        /// <returns>包含标准有效值集的 System.ComponentModel.TypeConverter.StandardValuesCollection；如果数据类型不支持标准值集，则为 null。</returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            StringCollection strings = new StringCollection();
            if (context.Instance is CommandParameter)
            {
                CommandParameter parameter = context.Instance as CommandParameter;
                AbstractCommandElement command = parameter.Command;
                if (command.NotifyObject is DataEntityElement)
                {
                    DataEntityElement dataEntityElement = command.NotifyObject as DataEntityElement;
                    string groupName = dataEntityElement.Persistent.GroupName;
                    foreach (DataEntityPropertyElement property in dataEntityElement.Properties)
                    {
                        strings.Add(string.Concat(groupName, "_", property.Name, "_Exist"));
                        strings.Add(string.Concat(groupName, "_", property.Name, "_NotExist"));
                    }
                }
                else if (command.NotifyObject is AbstractCommandElement)
                {
                    AbstractCommandElement ownerCommand = command.NotifyObject as AbstractCommandElement;
                    DataEntityElement dataEntityElement = ownerCommand.NotifyObject as DataEntityElement;
					string groupName = dataEntityElement.Persistent.GroupName;
                    foreach (DataEntityPropertyElement property in dataEntityElement.Properties)
                    {
                        strings.Add(string.Concat(groupName, "_", property.Name, "_Exist"));
                        strings.Add(string.Concat(groupName, "_", property.Name, "_NotExist"));
                    }
                }
            }
            else if (context.Instance is CheckedCommandElement)
            {
                CheckedCommandElement checkCommand = context.Instance as CheckedCommandElement;
                StaticCommandElement command = checkCommand.NotifyObject as StaticCommandElement;
                if (command.NotifyObject is DataEntityElement)
                {
                    DataEntityElement dataEntityElement = command.NotifyObject as DataEntityElement;
					string groupName = dataEntityElement.Persistent.GroupName;
                    foreach (DataEntityPropertyElement property in dataEntityElement.Properties)
                    {
                        strings.Add(string.Concat(groupName, "_", property.Name, "_Exist"));
                        strings.Add(string.Concat(groupName, "_", property.Name, "_NotExist"));
                    }
                }
            }

            return new StandardValuesCollection(strings);
        }
    }
}
