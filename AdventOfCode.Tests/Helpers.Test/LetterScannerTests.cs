using Xunit;
using AdventOfCode.Helpers;

namespace AdventOfCode.Helpers.UnitTests
{
	public class LetterScannerTests
	{
		[Fact]
		public void CanScanAllKnownLetters()
		{
			// See https://www.reddit.com/r/adventofcode/comments/enxuuw/unusual_request_looking_for_ascii_art_letters/
			var letters = new string[]
			{
				" ██  ███   ██  ███  ████ ████  ██  █  █  ███   ██ █  █ █     ██  ███  ███   ███  ███ █  █ █   █████",
				"█  █ █  █ █  █ █  █ █    █    █  █ █  █   █     █ █ █  █    █  █ █  █ █  █ █      █  █  █ █   █   █",
				"█  █ ███  █    █  █ ███  ███  █    ████   █     █ ██   █    █  █ █  █ █  █ █      █  █  █  █ █   █ ",
				"████ █  █ █    █  █ █    █    █ ██ █  █   █     █ █ █  █    █  █ ███  ███   ██    █  █  █   █   █  ",
				"█  █ █  █ █  █ █  █ █    █    █  █ █  █   █  █  █ █ █  █    █  █ █    █ █     █   █  █  █   █  █   ",
				"█  █ ███   ██  ███  ████ █     ███ █  █  ███  ██  █  █ ████  ██  █    █  █ ███    █   ██    █  ████"
			};
			var scan = LetterScanner.Scan(letters);
			Assert.Equal("ABCDEFGHIJKLOPRSTUYZ", scan);
		}
	}
}