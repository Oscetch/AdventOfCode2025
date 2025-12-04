using AdventOfCode2025.Challenges.Day2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2025.Challenges.Day3
{
    internal class LobbyExample : Scene
    {
        private readonly string _example = @"987654321111111
811111111111119
234234234234278
818181911112111";

        private SpriteFont _font;
        private Delay _checkDelay;
        private Delay _loadingDotDelay;
        private readonly List<string> _data = [];
        private readonly List<DrawableText> _rawTexts = [];
        private readonly List<DrawableText> _checkTexts = [];
        private DrawableText _calcText;
        private DrawableText _sumText;
        private DrawableText _percentText;
        private DrawableText _loadingText;

        private int _loadingDots;
        private int _currentTextIndex;
        private int _checkLength = 1;
        private ulong _currentSum = 0;

        protected virtual List<string> ParseData() => [.. _example.Split('\n').Select(x => x.Replace("\r", ""))];

        protected virtual double Speed { get; } = .1;
        protected virtual double Amount { get; } = 2;

        public override void Initialize(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            _font = contentManager.Load<SpriteFont>("Science");
            _checkDelay = new Delay(CheckToIndex, Speed);
            _data.AddRange(ParseData());
            foreach (var text in _data) 
            {
                var drawableText = new DrawableText(_font, text) 
                {
                    Position = new Vector2(Game1.Width / 2f, 200),
                };
                _rawTexts.Add(drawableText);
            }
            _calcText = new DrawableText(_font, "")
            {
                Tint = Color.GreenYellow,
                Position = new Vector2(Game1.Width / 2f, 300),
            };
            _sumText = new DrawableText(_font, "")
            {
                Tint = Color.HotPink,
                Position = new Vector2(Game1.Width / 2f, 400),
            };
            _percentText = new DrawableText(_font, "0%")
            {
                Position = new Vector2(Game1.Width / 2f, 100),
                Tint = Color.Yellow,
            };
            _loadingText = new DrawableText(_font, "Loading")
            {
                Position = new Vector2(Game1.Width / 2f, 70),
                Tint = Color.Yellow,
            };
            _loadingDotDelay = new Delay(UpdateLoadingDots, 1);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _percentText.Draw(spriteBatch);
            if (_checkTexts.Count > 0)
            {
                _rawTexts[_currentTextIndex].Draw(spriteBatch);
                _calcText.Draw(spriteBatch);
            }
            foreach (var checkText in _checkTexts) 
            {
                checkText.Draw(spriteBatch);
            }
            _sumText.Draw(spriteBatch);
            _loadingText.Draw(spriteBatch);
        }

        private void UpdateLoadingDots()
        {
            if (_percentText.Text == "100%")
            {
                _loadingText = new DrawableText(_font, $"Complete.")
                {
                    Position = new Vector2(Game1.Width / 2f, 70),
                    Tint = Color.Yellow,
                };
                return;
            }

            _loadingDots = (_loadingDots + 1) % 3;
            var dots = string.Join("", Enumerable.Range(0, _loadingDots).Select(_ => "."));
            _loadingText = new DrawableText(_font, $"Loading{dots}")
            {
                Position = new Vector2(Game1.Width / 2f, 70),
                Tint = Color.Yellow,
            };
        }

        public override void Update(GameTime gameTime)
        {
            _checkDelay.Update(gameTime);
            _loadingDotDelay.Update(gameTime);
        }

        private void CheckToIndex() 
        {
            var drawableText = _rawTexts[_currentTextIndex];
            var fullText = drawableText.Text;
            if (_checkLength > fullText.Length)
            {
                FinishCheck();

                if (_currentTextIndex + 1 >= _rawTexts.Count)
                {
                    _checkTexts.Clear();
                    return;
                }
                _checkLength = 1;
                _currentTextIndex++;
                CheckToIndex();
                return;
            }

            var partPercent = (float)_checkLength / fullText.Length * (1f / _rawTexts.Count);
            var wholePercents = (((float)_currentTextIndex / _rawTexts.Count) + partPercent) * 100;
            _percentText = new DrawableText(_font, $"{wholePercents:#.##}%")
            {
                Position = new Vector2(Game1.Width / 2f, 100),
                Tint = Color.Yellow,
            };
            _checkTexts.Clear();

            var indices = new List<int>();
            var values = new List<ulong>();
            var maxColor = Color.Purple;
            var minColor = Color.Blue;

            var startX = drawableText.Position.X - (drawableText.Size.X / 2f);
            var offsetX = 0;
            for (var i = 0; i < _checkLength; i++) 
            {
                var valueString = fullText[i].ToString();
                var actualValue = ulong.Parse(valueString);

                var remainingInLength = _checkLength - i;
                var wasCleared = false;
                for (var j = 0; j < indices.Count; j++)
                {
                    var storedValue = values[j];
                    var amountRequiredToClear = Amount - j;
                    if (storedValue >= actualValue || amountRequiredToClear > remainingInLength)
                    {
                        continue;
                    }
                    wasCleared = true;
                    values[j] = actualValue;
                    indices[j] = i;
                    for (var k = j + 1; k < indices.Count; k++)
                    {
                        indices.RemoveAt(k);
                        values.RemoveAt(k);
                        k--;
                    }
                    break;
                }
                if (!wasCleared)
                {
                    if (values.Count < Amount)
                    {
                        values.Add(actualValue);
                        indices.Add(i);
                    }
                    else 
                    {
                        var lastValue = values.Last();
                        if (actualValue > lastValue) 
                        {
                            values[^1] = actualValue;
                            indices[^1] = i;
                        }
                    }
                }

                var newText = new DrawableText(_font, valueString) { Tint = Color.Red };
                Vector2 position;
                position.Y = drawableText.Position.Y + 25;
                position.X = startX + offsetX + (newText.Size.X / 2);
                newText.Position = position;
                offsetX += newText.Size.X;
                _checkTexts.Add(newText);
            }
            foreach (var index in indices) 
            {
                _checkTexts[index].Tint = Color.Lerp(minColor, maxColor, (float)index / indices.Count);
            }

            _checkLength++;
        }

        private void FinishCheck()
        {
            var valueString = string.Join("", _checkTexts.Where(x => x.Tint != Color.Red).Select(x => x.Text));
            if (valueString.Length > 0)
            {
                var newText = valueString;

                if (_calcText.Text.Length != 0)
                {
                    newText = $"{_calcText.Text} + {valueString}";
                }

                _calcText = _font.MeasureString(newText).X >= Game1.Width
                    ? new DrawableText(_font, $"{_currentSum} + {valueString}") { Tint = Color.GreenYellow, Position = _calcText.Position }
                    : new DrawableText(_font, newText) { Tint = Color.GreenYellow, Position = _calcText.Position };
                _currentSum += ulong.Parse(valueString);
                _sumText = new DrawableText(_font, _currentSum.ToString())
                {
                    Tint = Color.HotPink,
                    Position = _sumText.Position,
                };
            }
        }
    }
}
