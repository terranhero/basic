using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basic.EntityLayer;

namespace Standard.EntityLayer.Test
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
