using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Guppy.OutputItems;

namespace GuppyTest
{
	class MarlinOutputItemFactoryTests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void UBL_G29_T1_Creates_Correct_G29T_MeshMap_OBject()
		{
			List<string> cl = new List<string>(@"Bed Topography Report for CSV:

0.185	0.175	0.170	0.000	-0.070	-0.140	-0.120	-0.195	-0.205	-0.255
0.080	0.110	0.065	-0.005	-0.115	-0.110	-0.115	-0.185	-0.180	-0.195
0.085	0.005	0.055	-0.010	-0.095	-0.060	-0.065	-0.085	-0.145	-0.165
0.295	0.110	0.080	0.150	0.005	-0.050	-0.030	-0.115	-0.130	-0.150
0.295	0.210	0.030	-0.030	0.100	-0.015	-0.020	-0.095	-0.150	-0.085
0.355	0.350	0.210	0.165	0.025	0.085	0.005	-0.005	-0.045	-0.165
0.305	0.335	0.240	0.160	0.125	0.025	0.075	0.030	0.020	0.010
0.250	0.260	0.225	0.145	0.115	0.020	-0.030	-0.095	0.015	0.005
0.230	0.210	0.150	0.005	-0.010	-0.005	-0.130	-0.050	-0.145	-0.115
0.290	0.230	0.165	0.090	0.025	0.000	0.005	0.000	0.040	0.030".Split("\r\n"));

			IOutputItem o = MarlinOutputItemFactory.BuildProcessedResponseG29T(cl);
			Assert.IsTrue(o is pr_G29T_MeshMap);

			pr_G29T_MeshMap mm = o as pr_G29T_MeshMap;
			Assert.IsTrue(mm.MeshValues[0, 0] == 0.290f);
			Assert.IsTrue(mm.MeshValues[9, 9] == -0.255f);
			Assert.IsTrue(mm.MeshValues[9, 0] == 0.030f);
			Assert.IsTrue(mm.MeshValues[0, 9] == 0.185f);


			Assert.IsTrue(mm.MeshValues[4, 2] == 0.115f);
			Assert.IsTrue(mm.MeshValues[1, 2] == 0.260f);

		}
	}
}
