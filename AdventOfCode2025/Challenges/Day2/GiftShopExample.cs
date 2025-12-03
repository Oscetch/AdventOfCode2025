using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Oscetch.MonoGame.Math.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace AdventOfCode2025.Challenges.Day2
{
    internal enum GiftShopState 
    {
        SELECT_TEXT,
        MOVE_SELECT_TEXT,
        TAKE_VALUE,
        MOVE_VALUE,
        TAKE_VALIDATION_VALUE,
        VALIDATE_VALUE,
        WAIT,
        MOVE_SUM,
        SUM,
    }

    internal class GiftShopExample : Scene
    {
        private readonly string _example = @"11-22,95-115,998-1012,1188511880-1188511890,222220-222224,
1698522-1698528,446443-446449,38593856-38593862,565653-565659,
824824821-824824827,2121212118-2121212124";
        private SpriteFont _font;
        private List<(ulong min, ulong max)> _values;
        private readonly List<DrawableText> _rangeTexts = [];
        private readonly List<DrawableText> _valueTexts = [];
        private readonly List<DrawableText> _validatedValues = [];
        private readonly List<ulong> _validatedValuesN = [];
        private readonly List<ulong> _possibleValues = [];
        private GiftShopState _state = GiftShopState.SELECT_TEXT;
        private (ulong min, ulong max) _selectedValues;
        private DrawableText _selectedText;
        private DrawableText _selectedValue;
        private DrawableText _validationValue;
        private DrawableText _sumText;
        private Delay _moveTextDelay;
        private Delay _moveValueDelay;
        private Delay _validationDelay;
        private Delay _moveSumDelay;
        private int _possibleValueIndex;
        private ulong _sum;
        private int _maxHeight = 0;
        private int _maxWidth = 0;
        private int _validationIndex = 1;

        protected virtual List<(ulong min, ulong max)> ParseValues() 
        {
            return [.. string.Join("", _example.Split('\n')).Split(',').Select(x =>
            {
                var values = x.Split('-');
                return (ulong.Parse(values[0]), ulong.Parse(values[1]));
            })];
        }

        public override void Initialize(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            _font = contentManager.Load<SpriteFont>("Science");
            _values = ParseValues();
            foreach (var (min, max) in _values) 
            {
                var text = new DrawableText(_font, $"{min} - {max}");
                _maxWidth = Math.Max(text.Size.X, _maxWidth);
                _maxHeight = Math.Max(text.Size.Y, _maxHeight);
                _rangeTexts.Add(text);
            }
            for (int i = 0; i < _rangeTexts.Count; i++) 
            {
                var text = _rangeTexts[i];
                var y = (i * _maxHeight) + (_maxHeight / 2) + 20;
                text.Position = new Vector2(Game1.Width - 20 - _maxWidth + (text.Size.X / 2), y);
            }
            _sumText = new DrawableText(_font, "0") 
            {
                Position = new Vector2(Game1.Width / 2f, Game1.Height / 2f)
            };
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (var text in _rangeTexts) 
            {
                text.Draw(spriteBatch);
            }
            _selectedText?.Draw(spriteBatch);
            foreach (var valueText in _valueTexts) 
            {
                valueText.Draw(spriteBatch);
            }
            _selectedValue?.Draw(spriteBatch);
            foreach (var validationValue in _validatedValues) 
            {
                validationValue.Draw(spriteBatch);
            }
            _sumText.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            switch (_state) 
            {
                case GiftShopState.SELECT_TEXT:
                    UpdateSelectText();
                    break;
                case GiftShopState.MOVE_SELECT_TEXT:
                    MoveSelectText(gameTime);
                    break;
                case GiftShopState.TAKE_VALUE:
                    TakeValue();
                    break;
                case GiftShopState.MOVE_VALUE:
                    MoveValue(gameTime);
                    break;
                case GiftShopState.TAKE_VALIDATION_VALUE:
                    TakeValidationValue();
                    break;
                case GiftShopState.VALIDATE_VALUE:
                    ValidateValue();
                    break;
                case GiftShopState.WAIT:
                    _validationDelay.Update(gameTime);
                    break;
                case GiftShopState.MOVE_SUM:
                    MoveSum(gameTime);
                    break;
                case GiftShopState.SUM:
                    Sum();
                    break;
            }
        }

        private void Sum()
        {
            foreach (var validatedValue in _validatedValuesN) 
            {
                _sum += validatedValue;
            }
            _validatedValues.Clear();
            _validatedValuesN.Clear();
            _sumText = new DrawableText(_font, _sum.ToString()) 
            {
                Position = _sumText.Position,
            };
            _state = GiftShopState.SELECT_TEXT;
        }

        private void MoveSum(GameTime gameTime)
        {
            var target = new Vector2(Game1.Width / 2f, Game1.Height / 2f);
            var percentOfTheWay = (float)(_moveSumDelay.ElapsedTime / _moveSumDelay.TargetTime);
            foreach (var validatedValue in _validatedValues) 
            {
                var distance = Vector2.Distance(validatedValue.Position, target);
                var toMove = percentOfTheWay * distance;
                var angle = AngleHelper.GetAngleBetweenVectorsInRadians(validatedValue.Position, target);
                validatedValue.Position = AngleHelper.MoveObjectInDirection(validatedValue.Position, angle, toMove);
            }
            _moveSumDelay.Update(gameTime);
        }

        private void ValidateValue()
        {
            _validationValue.Tint = Color.Green;
            _validatedValues.Add(_validationValue);
            _validatedValuesN.Add(ulong.Parse(_validationValue.Text));
            _state = GiftShopState.WAIT;
        }

        private void TakeValidationValue()
        {
            if (_valueTexts.Count == 0)
            {
                _validationValue = null;
                _state = GiftShopState.MOVE_SUM;
                _moveSumDelay = new Delay(() => _state = GiftShopState.SUM, .5f);
                return;
            }

            _selectedValue = null;
            _validationDelay = new Delay(() => _state = GiftShopState.TAKE_VALIDATION_VALUE, .1);
            _validationValue = _valueTexts[0];
            _valueTexts.RemoveAt(0);
            _validationIndex = _validationValue.Text.Length / 2;
            _state = GiftShopState.VALIDATE_VALUE;
        }

        private void MoveValue(GameTime gameTime)
        {
            var start = _selectedText.Position.Y + _maxHeight;
            var end = start + _maxHeight * _valueTexts.Count;
            var distance = end - start;
            var pD = _moveValueDelay.ElapsedTime / _moveValueDelay.TargetTime;
            var p = double.IsNaN(pD) || pD < 0.001 ? 1f : (float)pD;
            _selectedValue.Position = new Vector2(_selectedValue.Position.X, start + distance * p);
            _moveValueDelay.Update(gameTime);
        }

        private void TakeValue()
        {
            var nextI = _possibleValueIndex++;
            if (nextI >= _possibleValues.Count)
            {
                _state = GiftShopState.TAKE_VALIDATION_VALUE;
                return;
            }
            var next = _possibleValues[nextI];
            var nextS = next.ToString();
            _selectedValue = new DrawableText(_font, nextS);
            var startPositionX = _selectedText.Position.X - (_selectedText.Size.X / 2f) + (_selectedValue.Size.X / 2f);
            _selectedValue.Position = new Vector2(startPositionX, _selectedText.Position.Y);
            _state = GiftShopState.MOVE_VALUE;
            var wantedTimeTotal = 2.0;
            var totalToProcess = _selectedValues.max - _selectedValues.min;
            var time = wantedTimeTotal / totalToProcess;
            _moveValueDelay = new Delay(() => {
                _valueTexts.Add(_selectedValue);
                _state = GiftShopState.TAKE_VALUE; 
            }, time);
        }

        private void UpdateSelectText()
        {
            if (_values.Count == 0) return;
            _selectedValues = _values[0];
            _selectedText = _rangeTexts[0];
            _values.RemoveAt(0);
            _rangeTexts.RemoveAt(0);
            _possibleValueIndex = 0;
            _state = GiftShopState.MOVE_SELECT_TEXT;
            _moveTextDelay = new Delay(() => _state = GiftShopState.TAKE_VALUE, 1f);

            _possibleValues.Clear();
            _possibleValues.AddRange(CreatePossibleValues(_selectedValues.min, _selectedValues.max).Distinct());
        }

        protected virtual IEnumerable<ulong> CreatePossibleValues(ulong min, ulong max)
        {
            var minS = min.ToString();
            var maxS = max.ToString();
            var smallestLength = minS.Length;
            var lengthDiff = maxS.Length - smallestLength;
            if (lengthDiff <= 1 && smallestLength % 2 != 0 && maxS.Length % 2 != 0)
            {
                yield break;
            }
            var smallestEven = min;
            if (smallestLength % 2 != 0)
            {
                smallestEven = NextTenth(smallestLength);
                smallestLength++;
            }
            smallestEven = SetFirstPartLastPart(smallestEven, smallestLength);

            while (smallestEven <= max)
            {
                if (smallestEven >= min)
                {
                    yield return smallestEven;
                }

                var firstPart = GetFirstPart(smallestEven, smallestLength);
                var next = firstPart + 1;
                smallestLength = CountDigits(next) * 2;
                smallestEven = DoublePart(next, smallestLength);
            }
        }

        protected static int CountDigits(ulong n) => (int)Math.Floor(Math.Log10(n) + 1);

        protected static ulong NextTenth(int currentLength) => (ulong)Math.Pow(10, currentLength);
        protected static ulong SetFirstPartLastPart(ulong n, int length) 
        {
            var firstPart = GetFirstPart(n, length);
            return DoublePart(firstPart, length);
        }

        protected static ulong DoublePart(ulong n, int length) 
        {
            var halfLength = length / 2d;
            return n * (ulong)Math.Pow(10, halfLength) + n;
        }

        protected static ulong TakeDigits(ulong n, int inverseAmount) 
        {
            return (ulong)Math.Floor(n / Math.Pow(10, inverseAmount));
        }

        protected static ulong GetFirstPart(ulong n, int length)
        {
            var halfLength = length / 2;
            return TakeDigits(n, halfLength);
        }

        private void MoveSelectText(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach (var text in _rangeTexts) 
            {
                text.Position -= new Vector2(0, _maxHeight * delta);
            }

            var startX = Game1.Width - 20 - _maxWidth + (_selectedText.Size.X / 2);
            var targetX = 20 + _selectedText.Size.X / 2;
            var distance = startX - targetX;

            _selectedText.Position -= new Vector2(distance * delta, 0);
            _moveTextDelay.Update(gameTime);
        }
    }
}
