using System;

namespace AdventOfCode2025
{
    internal static class IntExtensions
    {
        public static int Mod(this int a, int otherInt) 
        {
            return a - otherInt * (int)Math.Floor((float)a / otherInt);
        }
    }
}
