namespace RazorEngineCore
{
	public abstract class RazorEngineTemplateBase<T> : RazorEngineTemplateBase, IRazorEngineTemplate<T>
	{
		private T innerModel;
		/// <summary></summary>
		public new T Model { get { return innerModel; } set { base.Model = innerModel = (T)value; } }

		T IRazorEngineTemplate<T>.Model { get { return innerModel; } set { base.Model = innerModel = (T)value; } }

		/// <summary>设置模型</summary>
		/// <param name="model"></param>
		public override void SetModel(object model) { Model = innerModel = (T)model; }

		/// <summary>获取模型布尔属性</summary>
		/// <param name="expression">读取布尔属性的 Lambda 表达式</param>
		/// <returns>返回属性的布尔值，如果模型为空则返回 false。</returns>
		public bool Bool(System.Linq.Expressions.Expression<System.Func<T, bool>> expression)
		{
			if (innerModel == null) { return false; }
			return expression.Compile().Invoke(innerModel);
		}

		/// <summary>获取属性值</summary>
		/// <param name="expression">读取属性的 Lambda 表达式</param>
		/// <returns>返回属性的值，如果模型为空则返回 空字符串。</returns>
		public string Raw(System.Linq.Expressions.Expression<System.Func<T, object>> expression)
		{
			if (innerModel == null) { return string.Empty; }
			object obj = expression.Compile().Invoke(innerModel);
			return System.Convert.ToString(obj, System.Globalization.CultureInfo.CurrentCulture);
		}

		/// <summary>获取属性值</summary>
		/// <param name="expression">读取属性的 Lambda 表达式</param>
		/// <param name="format">格式化字符串</param>
		/// <returns>返回属性的值，如果模型为空则返回 空字符串。</returns>
		public string Raw(System.Linq.Expressions.Expression<System.Func<T, object>> expression, string format)
		{
			if (innerModel == null) { return string.Empty; }
			object obj = expression.Compile().Invoke(innerModel);
			return string.Format(System.Globalization.CultureInfo.CurrentCulture, format, obj);
		}
	}
}