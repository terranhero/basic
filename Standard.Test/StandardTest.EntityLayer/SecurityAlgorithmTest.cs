using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basic.EntityLayer;

namespace StandardTest.EntityLayer
{
	[TestClass]
	public class SecurityAlgorithmTest
	{
		[TestMethod]
		public void OutputAssemblyKey()
		{
			Guid guid = new Guid("8EDED30C-4C25-4254-9BEC-3DCE2C75A371");
			string pwd0 = SecurityAlgorithm.SHA512ToStr("123@abcd", guid);
			//string pwd1 = SecurityAlgorithm.SHA512ToStr1("123@abcd", guid);
			Console.WriteLine(pwd0); 
			
			//string dis = string.Join(",", SecurityAlgorithm.AssemblyKey);
			//string source = "0,36,0,0,4,128,0,0,148,0,0,0,6,2,0,0,0,36,0,0,82,83,65,49,0,4,0,0,1,0,1,0,15,73,2,158,148,199,75,42,138,99,157,80,253,47,54,192,186,160,27,93,211,67,99,221,122,185,247,78,208,32,163,253,96,213,144,186,244,84,69,37,8,60,104,249,84,77,31,217,52,164,197,80,129,226,165,87,88,159,62,239,190,116,223,86,223,175,152,232,253,178,239,51,69,197,233,225,27,140,22,205,6,55,156,59,172,234,40,119,85,148,94,59,127,151,7,210,88,26,239,62,116,244,35,148,151,74,215,138,153,177,107,29,169,255,178,254,186,133,58,93,79,209,103,117,121,109,133,145";
			//Assert.IsTrue(pwd0 == pwd1, "SecurityAlgorithm.AssemblyKey");
		}
	}
}
