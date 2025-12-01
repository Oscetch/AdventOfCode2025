using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2025.Challenges.Day1
{
    internal class SecretEntrance : SecretEntranceExample
    {
        protected override List<int> ParseData()
        {
            return [.. File.ReadAllLines("Challenges\\Day1\\Day1_Part1.txt").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => {
                var n = x[0];
                var m = int.Parse(x[1..]);
                return n == 'L' ? -m : m;
            })];
        }
    }
}
