using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2025.Challenges.Day2
{
    internal class GiftShop : GiftShopExample
    {
        protected override List<(ulong min, ulong max)> ParseValues()
        {
            return [.. File.ReadAllText(@"Challenges\Day2\data.txt").Split(',').Select(x => 
            {
                var n = x.Split('-');
                return (ulong.Parse(n[0]), ulong.Parse(n[1]));
            })];
        }
    }
}
