using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;

namespace AdventOfCode2025.Challenges.Day4
{
    internal class PrintingDepartment : PrintingDepartmentExample
    {
        protected override IEnumerable<(Vector2 position, bool isValid)> ParseData()
        {
            var rows = File.ReadAllLines(@"Challenges\Day4\data.txt");
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
