using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Oscetch.MonoGame.Input.Managers;
using Oscetch.MonoGame.Input.Services;
using Oscetch.MonoGame.Math.Helpers;
using Oscetch.MonoGame.Textures;
using Oscetch.MonoGame.Textures.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2025.Challenges.Day1
{
    internal class SecretEntranceExample : Scene
    {
        private const int distanceToNumberFromCenter = 350;
        private readonly string _example = @"L68
L30
R48
L5
R60
L55
L1
L99
R14
L82";
        private List<int> _data;
        private int _currentIndex;
        private int _previous = 50;
        private int _wantedValue = 50;
        private int _rotatedOverZero = 0;
        private float _currentValue = MathHelper.Pi;
        private float _speed;
        private string _lastData;
        private Delay _delay;
        private Color _pointerColor = Color.White;
        private SpriteFont _font;
        private Texture2D _pointer;
        private Texture2D _centerDot;
        private KeyboardStateService _keyboard;
        private readonly Dictionary<int, int> _distributionMap = [];
        private int _maxDistribution;
        private Vector2 _centerDotPosition;
        private Color _centerDotColor;

        protected virtual double DelayTimeSeconds => 1;


        private readonly List<Vector2> _dialNumbers = [];
        private readonly List<float> _pointers = [];


        protected virtual List<int> ParseData() => [.. _example.Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => 
        {
            var n = x[0];
            var m = int.Parse(x[1..]);
            return n == 'L' ? -m : m;
        })];

        public override void Initialize(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _keyboard = KeyboardManager.GetGeneral();
            _delay = new(() => {
                if (_currentIndex >= _data.Count) return;
                _currentIndex++;
                if (_currentIndex >= _data.Count) return;
                NextWanted();
            }, DelayTimeSeconds);
            _data = ParseData();

            var center = new Vector2(Game1.Width / 2f, Game1.Height / 2f);
            _centerDotPosition = center;

            for (var i = 0; i < 100; i++) 
            {
                var angle = MathHelper.TwoPi * (i / 100f);
                var n = AngleHelper.MoveObjectInDirection(center, angle, distanceToNumberFromCenter);
                _dialNumbers.Add(n);
            }
            _font = content.Load<SpriteFont>("Font");

            var builder = new CustomTextureParameters.CustomTextureParametersBuilder()
                .WithShape(ShapeType.RectangleCornerRadius)
                .WithCornerRadius(2)
                .WithFillColor(Color.White)
                .WithWidth(distanceToNumberFromCenter - 20)
                .WithHeight(5);
            _pointer = CustomTextureManager.GetCustomTexture(builder.Build(), graphicsDevice);

            var centerDotBuilder = new CustomTextureParameters.CustomTextureParametersBuilder()
                .WithShape(ShapeType.Circle)
                .WithFillColor(Color.White)
                .WithSize(50);

            _centerDot = CustomTextureManager.GetCustomTexture(centerDotBuilder.Build(), graphicsDevice);

            NextWanted();
        }

        private void NextWanted()
        {
            var nextData = _data[_currentIndex];
            var next = _wantedValue + nextData;
            var nextWanted = next.Mod(100);
            if (next < 0)
            {
                var negativeNext = next + ((_wantedValue == 0 || nextWanted == 0) ? 0 : -100);
                var rotations = (int)Math.Floor((float)Math.Abs(negativeNext / 100));
                _rotatedOverZero += rotations;
            }
            else if (next > 100)
            {
                var rotations = (int)Math.Floor(next / 100f);
                _rotatedOverZero += rotations - (nextWanted == 0 ? 1 : 0);
            }

            _previous = _wantedValue;
            _currentValue = _previous / 100f * MathHelper.TwoPi;
            _wantedValue = nextWanted;
            if (_currentIndex != 0)
            {
                _pointers.Add(_currentValue);
                _distributionMap.TryGetValue(_previous, out var val);
                var n = val + 1;
                if (_previous != 0)
                {
                    _maxDistribution = Math.Max(n, _maxDistribution);
                }
                _distributionMap[_previous] = n;
            }
            UpdateSpeed();
        }

        private void Previous() => SetToIndex(_currentIndex - 1);
        private void Next() => SetToIndex(_currentIndex + 1);

        private void Faster() 
        {
            const float toRemove = 0.1f;
            _delay.TargetTime = MathHelper.Clamp((float)_delay.TargetTime - toRemove, 0, 10);
            if (_currentIndex >= _data.Count) return;
            UpdateSpeed();
        }

        private void Slower() 
        {
            const float toAdd = 0.1f;
            _delay.TargetTime = MathHelper.Clamp((float)_delay.TargetTime + toAdd, 0, 10);
            if (_currentIndex >= _data.Count) return;
            UpdateSpeed();
        }

        private void SetToIndex(int index) 
        {
            var n = MathHelper.Clamp(index, 0, _data.Count - 1);
            _pointers.Clear();
            _distributionMap.Clear();
            _delay.ElapsedTime = 0;
            _currentIndex = 0;
            _previous = 50;
            _wantedValue = 50;
            _rotatedOverZero = 0;
            _currentValue = MathHelper.Pi;
            _speed = 0;
            _maxDistribution = 0;
            NextWanted();

            for (var i = 0; i < n; i++)
            {
                _currentIndex++;
                NextWanted();
            }
        }

        private void DrawPointerAtAngle(SpriteBatch spriteBatch, float angle, Color color)
        {
            var center = new Vector2(Game1.Width / 2f, Game1.Height / 2f);
            var pointerPosition = AngleHelper.MoveObjectInDirection(center, angle, distanceToNumberFromCenter / 2f);
            var pointerOrigin = _pointer.Bounds.Center.ToVector2();

            spriteBatch.Draw(_pointer, pointerPosition, null, color, angle, pointerOrigin, 1f, SpriteEffects.None, 0);
        }

        private void UpdateSpeed()
        {
            int direction = _data[_currentIndex] > 0 ? 1 : -1;

            float current = _currentValue;
            float target = _wantedValue / 100f * MathHelper.TwoPi;

            float rawDelta = target - current;
            float twoPi = MathHelper.TwoPi;

            float delta;
            if (direction > 0)
            {
                if (rawDelta < 0) rawDelta += twoPi;
                delta = rawDelta;
            }
            else
            {
                if (rawDelta > 0) rawDelta -= twoPi;
                delta = rawDelta;
            }

            float duration = (float)_delay.TargetTime;
            _speed = delta / duration;
        }

        public override void Update(GameTime gameTime)
        {
            if (_keyboard.IsKeyClicked(Keys.Right)) 
            {
                Next();
            }
            if (_keyboard.IsKeyClicked(Keys.Left)) 
            {
                Previous();
            }
            if (_keyboard.IsKeyClicked(Keys.Up)) 
            {
                Faster();
            }
            if (_keyboard.IsKeyClicked(Keys.Down)) 
            {
                Slower();
            }
            if (_keyboard.IsKeyClicked(Keys.Back)) 
            {
                SetToIndex(0);
            }
            _delay.Update(gameTime);
            _pointerColor = Color.Lerp(Color.Yellow, Color.Purple, (float)(_delay.ElapsedTime / _delay.TargetTime));
            _centerDotColor = Color.Lerp(Color.Purple, Color.Blue, (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds));
            if (_currentIndex >= _data.Count) return;
            _currentValue = (_currentValue + _speed * (float)gameTime.ElapsedGameTime.TotalSeconds) % MathHelper.TwoPi;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            for (var i = 0; i < _dialNumbers.Count; i++) 
            {
                var distribution = _distributionMap.TryGetValue(i, out var val);
                var color = Color.Lerp(Color.White, Color.Red, (float)val / _maxDistribution);
                var s = i.ToString();
                var size = _font.MeasureString(s);
                var origin = size / 2f;
                spriteBatch.DrawString(_font, i.ToString(), _dialNumbers[i], color, 0f, origin, 1f, SpriteEffects.None, 0);
            }

            for (var i = 0; i < _pointers.Count; i++)
            {
                DrawPointerAtAngle(spriteBatch, _pointers[i], Color.Lerp(Color.Blue, Color.Purple, (float)i / _pointers.Count));
            }

            DrawPointerAtAngle(spriteBatch, _currentValue, _pointerColor);


            spriteBatch.Draw(_centerDot, _centerDotPosition, null, _centerDotColor, 0f, _centerDot.Bounds.Center.ToVector2(), 1f, SpriteEffects.None, 0);
            if (_currentIndex < _data.Count)
            {
                var points = _pointers.Count(x => x == 0f);
                var data = _data[_currentIndex];
                var allPoints = points + _rotatedOverZero;

                var direction = data > 0 ? "Right" : "Left";
                var time = (_delay.TargetTime - _delay.ElapsedTime).ToString("0.00");
                _lastData = $@"
Index: {_currentIndex}
Target: {_wantedValue}
Previous: {_previous}
All Points: {allPoints}
Regular points: {points}
Rotated over zero: {_rotatedOverZero}
Data point: {data} {direction}
Rotation time: {time}
Points left: {_data.Count - _currentIndex - 1}
";
            }
            spriteBatch.DrawString(_font, _lastData, Vector2.Zero, Color.White);
        }
    }
}
