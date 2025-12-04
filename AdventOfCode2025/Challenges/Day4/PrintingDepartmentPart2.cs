using Microsoft.Xna.Framework;
using QTree.MonoGame.Common;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2025.Challenges.Day4
{
    internal class PrintingDepartmentPart2 : PrintingDepartment
    {
        private bool _hasReturnedNull;
        protected override Sprite Next(List<Sprite> currentRolls)
        {
            if (_hasReturnedNull) return null;
            var quadTree = new DynamicQuadTree<Sprite>();
            var positionOffset = new Vector2(32f);
            foreach (var item in currentRolls)
            {
                if (item.Position == Forklift.DROP_OFF_POSITION) continue;
                var position = item.Position - positionOffset;
                var bounds = new Rectangle(position.ToPoint(), new Point(64));
                quadTree.Add(bounds, item);
            }
            foreach (var item in currentRolls)
            {
                if (item.Position == Forklift.DROP_OFF_POSITION) continue;
                var neighbors = 0;
                for (var x = -64; x < 128; x += 64) 
                {
                    for (var y = -64; y < 128; y += 64) 
                    {
                        if (x == 0 && y == 0) continue;
                        var testPoint = item.Position + new Vector2(x, y);
                        if (quadTree.FindNode(testPoint.ToPoint()).Any())
                        {
                            neighbors++;
                        }
                    }
                }
                if (neighbors < 4) return item;
            }
            _hasReturnedNull = true;
            return null;
        }
    }
}
