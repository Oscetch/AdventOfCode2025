using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace AdventOfCode2025.Challenges.Day4
{
    internal class Sprite
    {
        public bool ShouldRemove { get; set; }
        public Vector2 Position { get; set; }
        public float Scale { get; set; } = 1f;
        public Color Tint { get; set; } = Color.White;
        public float Rotation { get; set; }
        public Vector2 Origin { get; set; }
        public SpriteEffects SpriteEffects { get; set; }

        protected Texture2D Texture { get; }
        protected Rectangle SourceRectangle { get; }

        public Vector2 Size => SourceRectangle.Size.ToVector2() * Scale;

        public Sprite(Texture2D texture) : this(texture, new Rectangle(0, 0, texture.Width, texture.Height))
        {
        }

        public void ScaleToSize(float wantedSize) 
        {
            var max = Math.Max(SourceRectangle.Size.X, SourceRectangle.Size.Y);
            Scale = wantedSize / max;
        }

        public Sprite(Texture2D texture2D, Rectangle sourceRectangle)
        {
            Texture = texture2D;
            SourceRectangle = sourceRectangle;
            Origin = SourceRectangle.Size.ToVector2() / 2;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                Texture,
                Position,
                SourceRectangle,
                Tint,
                Rotation,
                Origin,
                Scale,
                SpriteEffects,
                0f
            );
        }
    }
}
