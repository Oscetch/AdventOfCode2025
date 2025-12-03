using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2025.Challenges.Day2
{
    internal class GiftShopPart2Example : GiftShopPart2
    {
        private readonly string _example = @"11-22,95-115,998-1012,1188511880-1188511890,222220-222224,
1698522-1698528,446443-446449,38593856-38593862,565653-565659,
824824821-824824827,2121212118-2121212124";

        protected override List<(ulong min, ulong max)> ParseValues()
        {
            return [.. string.Join("", _example.Split('\n')).Split(',').Select(x =>
            {
                var values = x.Split('-');
                return (ulong.Parse(values[0]), ulong.Parse(values[1]));
            })];
        }
    }
}
