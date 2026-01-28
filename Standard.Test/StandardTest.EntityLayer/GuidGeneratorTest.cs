using Basic.EntityLayer;

namespace StandardTest.EntityLayer
{
	[TestClass]
	public class GuidGeneratorTest
	{
		[TestMethod]
		public void NewGuidTest()
		{
			long hash = GuidGenerator.Fnv1a64Hash("SYS_EVENTLOGGER");

			Console.WriteLine(hash);
			Console.WriteLine(GuidGenerator.NewGuid(hash));
			Console.WriteLine(GuidGenerator.NewGuid("SYS_EVENTLOGGER"));
		}
	}
}
