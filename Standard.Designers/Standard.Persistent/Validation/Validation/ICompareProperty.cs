
namespace Basic.DataEntities
{
	/// <summary>
	/// 属性比较接口
	/// </summary>
	internal interface ICompareProperty
	{
		/// <summary>
		/// 需要比较的属性
		/// </summary>
		string OtherProperty { get; set; }

		/// <summary>
		/// 设置 OtherProperty 属性值。
		/// </summary>
		/// <param name="property"></param>
		void SetOtherProperty(DataEntityPropertyElement property);
	}
}
