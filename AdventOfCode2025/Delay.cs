using Microsoft.Xna.Framework;
using System;

namespace AdventOfCode2025
{
    internal class Delay(Action onElapsed, double targetTime)
    {
        public double TargetTime { get; set; } = targetTime;
        public double ElapsedTime { get; set; }

        public Action OnElapsed { get; set; } = onElapsed;

        public void Update(GameTime gameTime)
        {
            ElapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
            if (ElapsedTime >= TargetTime)
            {
                ElapsedTime = 0;
                OnElapsed();
            }
        }
    }
}
