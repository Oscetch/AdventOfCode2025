using AdventOfCode2025.Challenges.Day2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Oscetch.MonoGame.Input.Enums;
using Oscetch.MonoGame.Input.Managers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;

namespace AdventOfCode2025.Challenges.Day5
{
    internal class CafeteriaExample : Scene
    {
        protected static string ExampleData => @"3-5
10-14
16-20
12-18

1
5
8
11
17
32";

        private readonly List<RangeThing> _ranges = [];
        private readonly List<ulong> _testValues = [];

        private Delay _waitDelay;

        private DrawableText _currentValueText;

        private Rectangle _valueTextBoundsThing;

        private SpriteFont _font;

        private float _percent;

        private int _currentIndex;
        private int _score;

        protected virtual double WaitDelay => .5;
        protected virtual float PercentPerSecond => 1f;

        protected virtual (List<(ulong min, ulong max)> ranges, List<ulong> testValues) ParseData() 
        {
            var lines = ExampleData.Split("\r\n");
            var ranges = new List<(ulong min, ulong max)>();
            var testValues = new List<ulong>();
            var foundEmpty = false;
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    foundEmpty = true;
                    continue;
                }
                if (!foundEmpty) 
                {
                    var minMaxStrign = line.Split('-');
                    ranges.Add((ulong.Parse(minMaxStrign[0]), ulong.Parse(minMaxStrign[1])));
                    continue;
                }
                testValues.Add(ulong.Parse(line));
            }
            return (ranges, testValues);
        }

        private static bool IsInRange(ulong value, ulong min, ulong max) 
        {
            return value >= min && value <= max;
        }

        public static (bool mergedAny, List<(ulong min, ulong max)> data) MergeRanges(List<(ulong min, ulong max)> data) 
        {
            var newList = new List<(ulong min, ulong max)>();
            var wasMerged = false;

            foreach (var (min, max) in data) 
            {
                if (newList.Count == 0) 
                {
                    newList.Add((min, max));
                    continue;
                }
                var mergedInside = false;
                for (var i = 0; i < newList.Count; i++) 
                {
                    var (newListMin, newListMax) = newList[i];
                    if (IsInRange(min, newListMin, newListMax) || IsInRange(max, newListMin, newListMax) || IsInRange(newListMin, min, max) || IsInRange(newListMax, min, max)) 
                    {
                        mergedInside = true;
                        wasMerged = true;
                        newList[i] = (Math.Min(min, newListMin), Math.Max(max, newListMax));
                        break;
                    }
                }
                if (!mergedInside) 
                {
                    newList.Add((min, max));
                }
            }
            return (wasMerged, newList);
        }

        private static void SolvePart2(List<(ulong min, ulong max)> data) 
        {
            var merged = MergeRanges(data);
            while (merged.mergedAny) 
            {
                merged = MergeRanges(merged.data);
            }

            ulong sum = 0;
            foreach (var (min, max) in merged.data) 
            {
                sum += max - min + 1;
            }

            Debug.WriteLine(sum);
        }

        public override void Initialize(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            _font = contentManager.Load<SpriteFont>("Science");
            var (ranges, testValues) = ParseData();
            SolvePart2(ranges);
            _ranges.AddRange(ranges.Select((x, index) => new RangeThing(x.min, x.max, _font, PercentPerSecond) 
            {
                Position = new Vector2(Game1.Width / 2f, 200 + (50 * index) + (5 * index))
            }));
            _testValues.AddRange(testValues);
            SetTarget(_testValues[0]);

            _valueTextBoundsThing = new Rectangle(0, 0, Game1.Width, 150);

            _waitDelay = new Delay(() =>
            {
                WaitForReset();
                _currentIndex++;
            }, WaitDelay);

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (var range in _ranges) 
            {
                range.Draw(spriteBatch);
            }
            spriteBatch.Draw(Game1.White, _valueTextBoundsThing, Color.Black);
            _currentValueText.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            var scrollChange = 0f;
            if (MouseManager.CurrentMouseWheelStatus == MouseWheelStatus.Down)
            {
                scrollChange = -50f;
            }
            else if (MouseManager.CurrentMouseWheelStatus == MouseWheelStatus.Up)  
            {
                scrollChange = 50f;
            }
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var finishedAnimating = true;
            foreach (var range in _ranges) 
            {
                range.Position += new Vector2(0, scrollChange);
                range.Update(delta);
                finishedAnimating &= range.IsNotAnimating;
            }
            if (!finishedAnimating) return;
            if (_currentIndex % 2 == 0)
            {
                _waitDelay.Update(gameTime);
            }
            else 
            {
                _currentIndex++;
                var nextDataIndex = _currentIndex / 2;
                if (nextDataIndex < _testValues.Count)
                {
                    _percent = (float)nextDataIndex / _testValues.Count * 100f;
                    SetTarget(_testValues[nextDataIndex]);
                }
                else
                {
                    _currentValueText = new DrawableText(_font, $"Done: 100% | Points: {_score} | Testing value:{_testValues.Last()}")
                    {
                        Position = new Vector2(Game1.Width / 2f, 100),
                    };

                }
            }
        }

        private void WaitForReset() 
        {
            foreach (var range in _ranges) 
            {
                range.Reset();
            }
        }

        private void SetTarget(ulong testData) 
        {
            var anyValid = false;
            foreach (var range in _ranges) 
            {
                range.ShowForValue(testData);
                anyValid |= range.IsShownValueInRange;
            }
            if (anyValid) _score++;
            _currentValueText = new DrawableText(_font, $"Done: {_percent:0.##}% | Points: {_score} | Testing value: {testData}")
            {
                Position = new Vector2(Game1.Width / 2f, 100),
            };
        }
    }
}
