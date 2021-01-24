using NUnit.Framework;
using Guppy;
using System;

namespace GuppyTest
{
	public class MarlinStringHelperTests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void FiltersOutFullStringsAsExpected()
		{ 
			Assert.IsFalse(MarlinStringHelpers.CleanMarlinResponseAndRemoveTextAndLinesNotNeededForCommands("Unified Bed Leveling System v1.01 active").Item1);
		}

		[Test]
		public void DoesNotFilterOutCertainStrings()
		{
			Tuple<bool, string> r;
			r = MarlinStringHelpers.CleanMarlinResponseAndRemoveTextAndLinesNotNeededForCommands(string.Empty);
			Assert.IsTrue(r.Item1);
			Assert.IsTrue(r.Item2 == string.Empty);

			r = MarlinStringHelpers.CleanMarlinResponseAndRemoveTextAndLinesNotNeededForCommands(" ");
			Assert.IsTrue(r.Item1);
			Assert.IsTrue(r.Item2==string.Empty);
		}
		[Test]
		public void FiltersOutSubStringsAsExpected()
		{
			Tuple<bool, string> r;

			r = MarlinStringHelpers.CleanMarlinResponseAndRemoveTextAndLinesNotNeededForCommands("echo:; Driver stepping mode:");
			Assert.IsTrue(r.Item1);
			Assert.IsTrue(r.Item2 == "; Driver stepping mode:");

			r = MarlinStringHelpers.CleanMarlinResponseAndRemoveTextAndLinesNotNeededForCommands("echo: M569 S1 X Y Z");
			Assert.IsTrue(r.Item1);
			Assert.IsTrue(r.Item2 == "M569 S1 X Y Z");
		}

		[Test]
		public void MultiLinesFilterProperly()
		{
			string i =
@"echo:; Driver stepping mode:
echo: M569 S1 X Y Z
echo:  M569 S1 T0 E
echo:; Linear Advance:
echo: M900 K0.00
echo:; Filament load/ unload lengths:
echo: M603 L350.00 U400.00";

			string o =
@"; Driver stepping mode:
M569 S1 X Y Z
M569 S1 T0 E
; Linear Advance:
M900 K0.00
; Filament load/ unload lengths:
M603 L350.00 U400.00";

			Tuple<bool, string> r;

			r = MarlinStringHelpers.CleanMarlinResponseAndRemoveTextAndLinesNotNeededForCommands(i);
			Assert.IsTrue(r.Item1);
			Assert.IsTrue(r.Item2 == o);
		}

	}
}