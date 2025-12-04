using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace AdventOfCode2025.Challenges.Day4
{
    internal class PrintingDepartmentPart2Example : PrintingDepartmentPart2
    {
        private readonly string _examplePaper = @"..@@.@@@@.
@@@.@.@.@@
@@@@@.@.@@
@.@@@@..@.
@@.@@@@.@@
.@@@@@@@.@
.@.@.@.@@@
@.@@@.@@@@
.@@@@@@@@.
@.@.@@@.@.";

        protected override IEnumerable<(Vector2 position, bool isValid)> ParseData()
        {
            var rows = _examplePaper.Split("\r\n");
            for (var y = 0; y < rows.Length; y++)
            {
                var text = rows[y];
                for (var x = 0; x < text.Length; x++)
                {
                    if (text[x] != '@') continue;
                    var neighbors = 0;
                    for (var nox = -1; nox < 2; nox++)
                    {
                        for (var noy = -1; noy < 2; noy++)
                        {
                            var isZero = noy == 0 && nox == 0;
                            if (isZero) continue;
                            var nx = x + nox;
                            var ny = y + noy;
                            if (nx >= 0 && nx < text.Length && ny >= 0 && ny < rows.Length && rows[ny][nx] == '@') neighbors++;
                        }
                    }

                    yield return (new Vector2(x, y), neighbors < 4);
                }
            }
        }
    }
}
