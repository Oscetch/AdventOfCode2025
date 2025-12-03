using System;
using System.Collections.Generic;

namespace AdventOfCode2025.Challenges.Day2
{
    internal class GiftShopPart2 : GiftShop
    {
        protected override IEnumerable<ulong> CreatePossibleValues(ulong min, ulong max)
        {
            var maxLength = CountDigits(max);
            if (maxLength < 2) yield break;
            var minLength = Math.Max(CountDigits(min), 2);
            var maxPossibleChunk = maxLength / 2;
            var minByMaxChunk = TakeDigits(min, minLength - maxPossibleChunk);
            var maxByMaxChunk = TakeDigits(max, maxLength - maxPossibleChunk);

            var start = Math.Min(minByMaxChunk, maxByMaxChunk);
            var end = NextTenth(maxPossibleChunk) - 1;

            for (var i = start; i <= end; i++)
            {
                var digits = CountDigits(i);
                for (var chunkSize = 1; chunkSize <= digits; chunkSize++)
                {
                    for (var l = minLength; l <= maxLength; l++)
                    {
                        if (l % chunkSize != 0) continue;
                        var chunk = TakeDigits(i, digits - chunkSize);
                        var value = AddAmount(chunk, l, chunkSize);
                        if (value >= min && value <= max) yield return value;
                    }
                }
            }
        }

        /*protected override IEnumerable<ulong> CreatePossibleValues(ulong min, ulong max)
        {
            var asd = TestCreatePossibleValues(min, max).Distinct().ToList();
            var maxLength = CountDigits(max);
            var minLength = CountDigits(min);

            for (var i = min; i <= max; i++) 
            {
                var digits = CountDigits(i);
                var maxChunk = digits / 2;
                for (var chunkSize = 1; chunkSize <= maxChunk; chunkSize++) 
                {
                    if (digits % chunkSize != 0) continue;
                    var chunk = TakeDigits(i, digits - chunkSize);
                    var value = AddAmount(chunk, digits, chunkSize);
                    if (value >= min && value <= max) 
                    {
                        if (!asd.Contains(value)) 
                        {
                            Debug.WriteLine("?");
                        }
                        yield return value; 
                    }
                }
            }
        */
            /*
             
             */

            /*for (var i = minLength; i <= maxLength; i++) 
            {
                var maxChunk = i / 2;
                for (var chunkSize = 1; chunkSize <= maxChunk; chunkSize++) 
                {
                    if (chunkSize % i != 0) continue;

                    var minForChunk = TakeDigits(min, minLength - chunkSize);
                    var maxForChunk = TakeDigits(max, maxLength - chunkSize);

                }
            }



            var minFirstDigit = (ulong)((int)(Math.Floor(min / Math.Pow(10, minLength - 1)) - 1)).Mod(10);
            var maxFirstDigit = (ulong)Math.Floor(max / Math.Pow(10, maxLength - 1));
            do
            {
                minFirstDigit = (minFirstDigit + 1) % 10;
                for (var n = minLength; n <= maxLength; n++)
                {
                    var m = AddAmount(minFirstDigit, n, 1);
                    if (m >= min && m <= max) yield return m;
                }
            } while (minFirstDigit != maxFirstDigit);
            var halfMaxLength = maxLength / 2;
            for (var i = 2; i <= halfMaxLength; i++)
            {
                var iAmount = NextEven(i - 1);
                for (var length = minLength; length <= maxLength; length++)
                {
                    if (length % i != 0) continue;
                    var n = iAmount;
                    while (CountDigits(n) == i)
                    {
                        var a = length / i;
                        var test = AddAmount(n, a, i);
                        if (test > min && test <= max) {
                            yield return test;
                        }
                        n++;
                    }
                }
            }*/

        private static ulong AddAmount(ulong start, int amount, int digits)
        {
            ulong current = 0;
            for (var i = 0; i < amount; i += digits)
            {
                current += start * (ulong)Math.Pow(10, i);
            }
            return current;
        }
    }
}
