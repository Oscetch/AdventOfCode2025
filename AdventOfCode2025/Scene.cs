using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AdventOfCode2025
{
    internal abstract class Scene
    {
        public virtual void Initialize(ContentManager contentManager, GraphicsDevice graphicsDevice) { }

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
