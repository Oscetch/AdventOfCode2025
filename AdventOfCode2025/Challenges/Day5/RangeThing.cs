using AdventOfCode2025.Challenges.Day2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Oscetch.MonoGame.Extensions;
using System;

namespace AdventOfCode2025.Challenges.Day5
{
    internal class RangeThing(ulong min, ulong max, SpriteFont font, float percentPerSecond)
    {
        private float _shownPercent = 0.5f;
        private float _targetPercent;
        private readonly float _percentPerSecond = percentPerSecond;
        private readonly DrawableText _minText = new(font, min.ToString()) { Tint = Color.Black };
        private readonly DrawableText _maxText = new(font, max.ToString()) { Tint = Color.Black };
        private bool _isReseting;

        protected ulong Length { get; } = max - min;
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; } = new Vector2(Game1.Width - 200, 50);
        public ulong Min { get; } = min;
        public ulong Max { get; } = max;

        public ulong ShownValue { get; private set; }
        public bool IsShownValueInRange { get; private set; }
        public bool IsNotAnimating => _shownPercent == _targetPercent;

        public void ShowForValue(ulong value) 
        {
            IsShownValueInRange = value >= Min && value <= Max;
            ShownValue = value;
            var valueDiff = value < Min ? 0 : value - Min;
            _targetPercent = Length == 0 ? 0f : MathHelper.Clamp((float)valueDiff / Length, 0f, 1f);
            _isReseting = false;
        }

        public void Reset() 
        {
            var value = _shownPercent > 0.5f ? Min : Max;
            ShowForValue(value);
            _isReseting = true;
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            const int START_END_WIDTH = 5;
            const int LINE_HEIGHT = 25;
            var box = Size.ToRectangle(Position, true);

            var minTextPosition = new Vector2(Position.X - (Size.X / 2f) + (_minText.Size.X / 2f) + START_END_WIDTH, Position.Y);
            var maxTextPosition = new Vector2(Position.X + (Size.X / 2f) - (_maxText.Size.X / 2f) - START_END_WIDTH, Position.Y);

            _minText.Position = minTextPosition;
            _maxText.Position = maxTextPosition;

            var lineStart = _minText.Position.X + (_minText.Size.X / 2f) + START_END_WIDTH;
            var lineEnd = _maxText.Position.X - (_maxText.Size.X / 2f) - START_END_WIDTH;

            var lineLength = lineEnd - lineStart;

            var line = new Vector2(lineLength, LINE_HEIGHT).ToRectangle(new Vector2(lineStart, Position.Y - LINE_HEIGHT / 2f));
            var start = new Rectangle(line.Location, new Point(START_END_WIDTH, line.Height));
            var end = new Rectangle(line.Right - START_END_WIDTH, line.Top, START_END_WIDTH, line.Height);
            var cursorLeft = start.Left + ((line.Width - START_END_WIDTH) * _shownPercent);
            var cursor = new Rectangle((int)cursorLeft, start.Y, start.Width, start.Height);
            var color = IsNotAnimating ? Color.Lerp(Color.Blue, Color.Purple, _shownPercent) : _isReseting ? Color.Black : Color.Green;

            spriteBatch.Draw(Game1.White, box, IsShownValueInRange && !_isReseting ? Color.Green : Color.LightGray);
            spriteBatch.Draw(Game1.White, line, Color.White);
            spriteBatch.Draw(Game1.White, start, Color.Black);
            spriteBatch.Draw(Game1.White, end, Color.Black);
            spriteBatch.Draw(Game1.White, cursor, color);

            _minText.Draw(spriteBatch);
            _maxText.Draw(spriteBatch);
        }

        public void Update(float delta) 
        {
            const float min = 0.1f;

            var left = _targetPercent - _shownPercent;
            var change = _percentPerSecond * delta * (left >= 0 ? 1 : -1);
            var next = MathHelper.Clamp(_shownPercent + change, 0f, 1f);
            //var diffBetweenLeftAndNext = Math.Abs(left - next);
            if (Math.Abs(left) < min)
            {
                _shownPercent = _targetPercent;
                return;
            }
            _shownPercent = next;
        }
    }
}
