using System.Collections.Generic;
using System.IO;

namespace AdventOfCode2025.Challenges.Day5
{
    internal class Cafeteria : CafeteriaExample
    {
        protected override double WaitDelay => 0.0;
        protected override float PercentPerSecond => 10f;

        protected override (List<(ulong min, ulong max)> ranges, List<ulong> testValues) ParseData()
        {
            var lines = File.ReadAllLines(@"Challenges\Day5\data.txt");
            var ranges = new List<(ulong min, ulong max)>();
            var testValues = new List<ulong>();
            var foundEmpty = false;
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    foundEmpty = true;
                    continue;
                }
                if (!foundEmpty)
                {
                    var minMaxStrign = line.Split('-');
                    ranges.Add((ulong.Parse(minMaxStrign[0]), ulong.Parse(minMaxStrign[1])));
                    continue;
                }
                testValues.Add(ulong.Parse(line));
            }
            return (ranges, testValues);
        }
    }
}
