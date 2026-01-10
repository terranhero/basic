using System.ComponentModel.DataAnnotations;
using Basic.MvcLibrary;
using Xunit.Abstractions;

namespace StandardTest.MvcLibrary
{
	internal sealed class PropertyProviderModel
	{
		public string StringNotNull { get; set; } = "CESHI ";
		public string StringIsNull { get; set; }

		public bool Boolean { get; set; } = false;

		public DateTime DateTime { get; set; } = DateTime.UtcNow;
		public DateTime? DateTime1 { get; set; } = DateTime.UtcNow;
		public DateTime? DateTime2 { get; set; }

		[DisplayFormat(DataFormatString = "{0:hh\\:mm\\:ss\\.fff}")]
		public TimeSpan TimeSpan { get; set; } = DateTime.UtcNow.TimeOfDay;
		public TimeSpan? TimeSpan1 { get; set; } = DateTime.UtcNow.TimeOfDay;
		public TimeSpan? TimeSpan2 { get; set; }

		public bool? NullableBool1 { get; set; } = false;
		public bool? NullableBool2 { get; set; }

		public string[] StringArray { get; set; } = ["数组1", "数组2"];

		public bool[] BoolArray { get; set; } = [true, false];

		public int[] Int32Array { get; set; } = [1, 3, 7, 21, 7];
	}

	public class IPropertyProvidersTester
	{
		private readonly ITestOutputHelper _output;

		public IPropertyProvidersTester(ITestOutputHelper output) { _output = output; }

		[Fact]
		public void PropertyFor()
		{
			PropertyProviderModel model = new PropertyProviderModel();
			IPropertyProviders<PropertyProviderModel> provider = new PropertyProviders<PropertyProviderModel>(model);
			_output.WriteLine("Default = {0}", provider.PropertyFor(m => m.StringNotNull));
			_output.WriteLine("withComma is true = {0}", provider.PropertyFor(m => m.StringNotNull, true));
			_output.WriteLine("withComma is false = {0}", provider.PropertyFor(m => m.StringNotNull, false));

			_output.WriteLine("Default = {0}", provider.PropertyFor(m => m.StringIsNull));
			_output.WriteLine("withComma is true = {0}", provider.PropertyFor(m => m.StringIsNull, true));
			_output.WriteLine("withComma is false = {0}", provider.PropertyFor(m => m.StringIsNull, false));

			_output.WriteLine(" ======= boolean =======");
			_output.WriteLine("Default = {0}", provider.PropertyFor(m => m.Boolean));
			_output.WriteLine("withComma is true = {0}", provider.PropertyFor(m => m.Boolean, "", "{0}", true));
			_output.WriteLine("withComma is false = {0}", provider.PropertyFor(m => m.Boolean, false));

			_output.WriteLine("\r\n ======= boolean? =======");
			_output.WriteLine("Default = {0}", provider.PropertyFor(m => m.NullableBool1));
			_output.WriteLine("withComma is true = {0}", provider.PropertyFor(m => m.NullableBool1, "", "{0}", true));
			_output.WriteLine("withComma is false = {0}", provider.PropertyFor(m => m.NullableBool1, false));
			_output.WriteLine("Default = {0}", provider.PropertyFor(m => m.NullableBool2));
			_output.WriteLine("withComma is true = {0}", provider.PropertyFor(m => m.NullableBool2, "", "{0}", true));
			_output.WriteLine("withComma is false = {0}", provider.PropertyFor(m => m.NullableBool2, false));


			_output.WriteLine("\r\n ======= string array =======");
			_output.WriteLine("Default = {0}", provider.PropertyFor(m => m.StringArray));
			_output.WriteLine("withComma is true = {0}", provider.PropertyFor(m => m.StringArray, "", "{0}", true));
			_output.WriteLine("withComma is false = {0}", provider.PropertyFor(m => m.StringArray, false));

			_output.WriteLine("\r\n ======= bool array =======");
			_output.WriteLine("Default = {0}", provider.PropertyFor(m => m.BoolArray));
			_output.WriteLine("withComma is true = {0}", provider.PropertyFor(m => m.BoolArray, "", "{0}", true));
			_output.WriteLine("withComma is false = {0}", provider.PropertyFor(m => m.BoolArray, false));

			_output.WriteLine("\r\n ======= int32 array =======");
			_output.WriteLine("Default = {0}", provider.PropertyFor(m => m.Int32Array));
			_output.WriteLine("withComma is true = {0}", provider.PropertyFor(m => m.Int32Array, "", "{0}", true));
			_output.WriteLine("withComma is false = {0}", provider.PropertyFor(m => m.Int32Array, false));

			_output.WriteLine("\r\n ======= datetime array =======");
			_output.WriteLine("Default = {0}", provider.PropertyFor(m => m.DateTime));
			_output.WriteLine("withComma is true = {0}", provider.PropertyFor(m => m.DateTime, "", "{0}", true));
			_output.WriteLine("withComma is false = {0}", provider.PropertyFor(m => m.DateTime, false));

			_output.WriteLine("Default = {0}", provider.PropertyFor(m => m.DateTime1));
			_output.WriteLine("withComma is true = {0}", provider.PropertyFor(m => m.DateTime1, "", "{0}", true));
			_output.WriteLine("withComma is false = {0}", provider.PropertyFor(m => m.DateTime1, false));

			_output.WriteLine("Default = {0}", provider.PropertyFor(m => m.DateTime2));
			_output.WriteLine("withComma is true = {0}", provider.PropertyFor(m => m.DateTime2, "", "{0}", true));
			_output.WriteLine("withComma is false = {0}", provider.PropertyFor(m => m.DateTime2, false));

			_output.WriteLine("\r\n ======= TimeSpan array =======");
			_output.WriteLine("Default = {0}", provider.PropertyFor(m => m.TimeSpan));
			_output.WriteLine("withComma is true = {0}", provider.PropertyFor(m => m.TimeSpan, "", "{0}", true));
			_output.WriteLine("withComma is false = {0}", provider.PropertyFor(m => m.TimeSpan, false));

			_output.WriteLine("Default = {0}", provider.PropertyFor(m => m.TimeSpan1));
			_output.WriteLine("withComma is true = {0}", provider.PropertyFor(m => m.TimeSpan1, "", "{0}", true));
			_output.WriteLine("withComma is false = {0}", provider.PropertyFor(m => m.TimeSpan1, false));

			_output.WriteLine("Default = {0}", provider.PropertyFor(m => m.TimeSpan2));
			_output.WriteLine("withComma is true = {0}", provider.PropertyFor(m => m.TimeSpan2, "", "{0}", true));
			_output.WriteLine("withComma is false = {0}", provider.PropertyFor(m => m.TimeSpan2, false));
		}
	}
}