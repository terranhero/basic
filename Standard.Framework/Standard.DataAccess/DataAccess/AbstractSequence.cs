using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Basic.EntityLayer;

namespace Basic.DataAccess
{
    /// <summary>
    /// 数据库表关键字抽象序列
    /// </summary>
    internal abstract class AbstractSequence
    {
        private readonly string propertyName = null;
        /// <summary>
        /// 初始化 AbstractSequence 类实例，此构造函数仅供子类使用。
        /// </summary>
        protected AbstractSequence(string name) { propertyName = name; }

        /// <summary>
        /// 获取序列中下一个值。
        /// 当类型是int，long， decima时获取序列中下一个值，并返回当前值。
        /// 当类型是string, Guid时获取序列中新值，System.Guid的表示形式。
        /// </summary>
        /// <returns>返回当前序列的下一个值。</returns>
        public abstract object NextValue { get; }

        /// <summary>
        /// 获取序列中下一个值，并返回。
        /// </summary>
        /// <returns>返回当前序列的下一个值。</returns>
        public virtual string PropertyName { get { return propertyName; } }

        /// <summary>
        /// 创建强类型序列生成器
        /// </summary>
        /// <param name="name">生成序列的属性字段名称</param>
        /// <param name="value">序列生成器的开始值。</param>
        /// <returns>返回创建成功的一个 AbstractSequence 子类实例，表示一个强类型的序列生成器。</returns>
        public static AbstractSequence CreateSequence(string name, object value)
        {
            if (value is int)
                return new Int32Sequence(name, (int)value);
            else if (value is long)
                return new Int64Sequence(name, (long)value);
            else if (value is decimal)
                return new DecimalSequence(name, (decimal)value);
            return null;
        }

        /// <summary>
        /// 创建强类型序列生成器
        /// </summary>
        /// <returns>返回创建成功的一个 AbstractSequence 子类实例，表示一个强类型的序列生成器。</returns>
        public static AbstractSequence CreateGuidStringSequence(string name)
        {
            return new GuidStringSequence(name);
        }

        /// <summary>
        /// 创建强类型序列生成器
        /// </summary>
        /// <returns>返回创建成功的一个 AbstractSequence 子类实例，表示一个强类型的序列生成器。</returns>
        public static AbstractSequence CreateGuidSequence(string name)
        {
            return new GuidSequence(name);
        }

        /// <summary>
        /// String类型字段的序列构造器
        /// </summary>
        private class GuidStringSequence : AbstractSequence
        {
            /// <summary>
            /// 初始化 GuidStringSequence 类实例。
            /// </summary>
            public GuidStringSequence(string name) : base(name) { }

            /// <summary>
            /// 获取序列中下一个值，并返回。
            /// </summary>
            /// <returns>返回当前序列的下一个值。</returns>
            public override object NextValue
            {
                get { return GuidConverter.GuidString; }
            }
        }

        /// <summary>
        /// Guid类型字段的序列构造器
        /// </summary>
        private class GuidSequence : AbstractSequence
        {
            /// <summary>
            /// 初始化 GuidSequence 类实例。
            /// </summary>
            public GuidSequence(string name) : base(name) { }

            /// <summary>
            /// 获取序列中下一个值，并返回。
            /// </summary>
            /// <returns>返回当前序列的下一个值。</returns>
            public override object NextValue
            {
                get { return GuidConverter.NewGuid; }
            }
        }

        /// <summary>
        /// Int32类型字段的序列构造器
        /// </summary>
        private class Int32Sequence : AbstractSequence
        {
            private int currentValue = 0;
            public Int32Sequence(string name, int value) : base(name) { currentValue = value; }
            /// <summary>
            /// 获取序列中下一个值，并返回。
            /// </summary>
            /// <returns>返回当前序列的下一个值。</returns>
            public override object NextValue
            {
                get { return currentValue++; }
            }
        }

        /// <summary>
        /// Int64类型字段的序列构造器
        /// </summary>
        private class Int64Sequence : AbstractSequence
        {
            private long currentValue = 0;
            public Int64Sequence(string name, long value) : base(name) { currentValue = value; }
            /// <summary>
            /// 获取序列中下一个值，并返回。
            /// </summary>
            /// <returns>返回当前序列的下一个值。</returns>
            public override object NextValue
            {
                get { return currentValue++; }
            }
        }

        /// <summary>
        /// decimal类型字段的序列构造器
        /// </summary>
        private class DecimalSequence : AbstractSequence
        {
            private decimal currentValue = 0;
            public DecimalSequence(string name, decimal value) : base(name) { currentValue = value; }
            /// <summary>
            /// 获取序列中下一个值，并返回。
            /// </summary>
            /// <returns>返回当前序列的下一个值。</returns>
            public override object NextValue
            {
                get { return currentValue++; }
            }
        }
    }
}
