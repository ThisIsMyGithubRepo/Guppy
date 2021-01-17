using NUnit.Framework;
using Guppy;

namespace GuppyTest
{
	public class MacroItemTests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void WorksWithBlankString()
		{
			string s = string.Empty;
			MacroItem m = new MacroItem(s);
			Assert.IsTrue(m.Label == string.Empty);
			Assert.IsTrue(m.CommandList.Count == 0);
			Assert.IsTrue(m.ToString() == ",");
		}

		[Test]
		public void WorksWithSingleValueString()
		{
			string s = "Hi";
			MacroItem m = new MacroItem(s);
			Assert.IsTrue(m.Label == "Hi");
			Assert.IsTrue(m.CommandList.Count == 0);
			Assert.IsTrue(m.ToString() == "Hi,");
		}

		[Test]
		public void WorksWithSingleCommandString()
		{
			string s = "Hi,Command1";
			MacroItem m = new MacroItem(s);
			Assert.IsTrue(m.Label == "Hi");
			Assert.IsTrue(m.CommandList.Count == 1);
			Assert.IsTrue(m.CommandList[0] == "Command1");
			Assert.IsTrue(m.ToString() == s);
		}
		[Test]
		public void WorksWithMultipleCommandString()
		{
			string s = "Hi,Command1,Command2,Command3";
			MacroItem m = new MacroItem(s);
			Assert.IsTrue(m.Label == "Hi");
			Assert.IsTrue(m.CommandList.Count == 3);
			Assert.IsTrue(m.CommandList[0] == "Command1");
			Assert.IsTrue(m.CommandList[1] == "Command2");
			Assert.IsTrue(m.CommandList[2] == "Command3");
			Assert.IsTrue(m.ToString() == s);
		}

		[Test]
		public void WhitespaceAndCRLFAreDropped()
		{
			string s = "Hi,Command1,, ,\r,\n,\r\n, \r\n,Command2,Command3";
			MacroItem m = new MacroItem(s);
			Assert.IsTrue(m.Label == "Hi");
			Assert.IsTrue(m.CommandList.Count == 3);
			Assert.IsTrue(m.CommandList[0] == "Command1");
			Assert.IsTrue(m.CommandList[1] == "Command2");
			Assert.IsTrue(m.CommandList[2] == "Command3");
			Assert.IsTrue(m.ToString() == "Hi,Command1,Command2,Command3");
		}

	}
}