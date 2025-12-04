using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2025.Challenges.Day2
{
    internal class DrawableText
    {
        private readonly SpriteFont _font;
        private float _scale = 1f;
        public Vector2 Position { get; set; }
        public float Scale 
        { 
            get => _scale;
            set 
            {
                _scale = value;
                Size = (_font.MeasureString(Text) * Scale).ToPoint();
            }
        }
        public Color Tint { get; set; } = Color.White;
        public float Rotation { get; set; }
        public Vector2 Origin { get; set; }
        public SpriteEffects SpriteEffects { get; set; }

        public string Text { get; }
        public Point Size { get; private set; }

        public DrawableText(SpriteFont font, string text)
        {
            _font = font;
            Text = text;
            Size = (_font.MeasureString(Text) * Scale).ToPoint();
            Origin = Size.ToVector2() / 2;
        }

        public DrawableText WithNewText(string text) 
        {
            return new DrawableText(_font, text)
            {
                Position = Position,
                Scale = Scale,
                Tint = Tint,
                Rotation = Rotation,
                Origin = Origin,
                SpriteEffects = SpriteEffects,
            };
        }

        public void ScaleToSize(float size)
        {
            var s = Size;
            Scale = Math.Min(size / s.X, size / s.Y);
            Size = (_font.MeasureString(Text) * Scale).ToPoint();
        }

        public void ScaleToSize(Vector2 targetSize)
        {
            var size = _font.MeasureString(Text);
            var scale = targetSize / size;
            Scale = Math.Min(scale.X, scale.Y);
            Size = (_font.MeasureString(Text) * Scale).ToPoint();
        }

        public void ScaleToHeight(float targetHeight)
        {
            var height = _font.MeasureString("|").Y;
            Scale = targetHeight / height;
            Size = (_font.MeasureString(Text) * Scale).ToPoint();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_font, Text, Position, Tint, Rotation, Origin, Scale, SpriteEffects, 0);
        }

        public IEnumerable<string> SplitTextForWidth(string text, float width)
        {
            var remainingText = string.Join("", text.Where(x => x != '\n'));

            while (remainingText.Length > 0)
            {
                var length = _font.MeasureString(remainingText).X * Scale;
                if (length < width)
                {
                    yield return remainingText;
                    break;
                }

                var words = remainingText.Split(' ');

                if (words.Length <= 1) break;

                for (var i = 1; i < words.Length - 2; i++)
                {
                    var newWords = string.Join(' ', words.SkipLast(i));
                    if (_font.MeasureString(newWords).X * Scale < width)
                    {
                        yield return newWords;
                        remainingText = string.Join(' ', words.TakeLast(i));
                        break;
                    }
                }

            }
        }
    }
}
