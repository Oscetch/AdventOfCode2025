using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2025.Challenges.Day3
{
    internal class Lobby : LobbyExample
    {
        protected override double Speed => 0;

        protected override List<string> ParseData()
        {
            return [.. File.ReadAllLines("Challenges\\Day3\\data.txt").Where(x => !string.IsNullOrWhiteSpace(x))];
        }
    }
}
