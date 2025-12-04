using AdventOfCode2025.Challenges.Day2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Oscetch.MonoGame.Camera;
using Oscetch.MonoGame.Input.Managers;
using Oscetch.MonoGame.Input.Services;
using System.Collections.Generic;

namespace AdventOfCode2025.Challenges.Day4
{
    internal class PrintingDepartmentExample : Scene
    {
        private readonly string _examplePaper = @"..@@.@@@@.
@@@.@.@.@@
@@@@@.@.@@
@.@@@@..@.
@@.@@@@.@@
.@@@@@@@.@
.@.@.@.@@@
@.@@@.@@@@
.@@@@@@@@.
@.@.@@@.@.";

        private readonly CameraHandler _camera = new (Game1.Width, Game1.Height);
        private Texture2D _factoryFloor;
        private Forklift _forklift;
        private readonly List<Sprite> _validTargets = [];
        private readonly List<Sprite> _paperRolls = [];
        private DrawableText _scoreText;
        private KeyboardStateService _keyboard;
        private int _score;

        protected virtual IEnumerable<(Vector2 position, bool isValid)> ParseData() 
        {
            var rows = _examplePaper.Split("\r\n");
            for (var y = 0; y < rows.Length; y++) 
            {
                var text = rows[y];
                for (var x = 0; x < text.Length; x++) 
                {
                    if (text[x] != '@') continue;
                    var neighbors = 0;
                    for (var nox = -1; nox < 2; nox++) 
                    {
                        for (var noy = -1; noy < 2; noy++)
                        {
                            var isZero = noy == 0 && nox == 0;
                            if (isZero) continue;
                            var nx = x + nox;
                            var ny = y + noy;
                            if (nx >= 0 && nx < text.Length && ny >= 0 && ny < rows.Length && rows[ny][nx] == '@') neighbors++;
                        }
                    }

                    yield return (new Vector2(x, y), neighbors < 4);
                }
            }
        }

        protected virtual Sprite Next(List<Sprite> currentRolls)
        {
            if (_validTargets.Count > 0)
            {
                _validTargets.RemoveAt(0);
            }
            if (_validTargets.Count > 0) return _validTargets[0];
            return null;
        }

        public override void Initialize(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            var font = contentManager.Load<SpriteFont>("Science");
            _keyboard = KeyboardManager.GetGeneral();
            _scoreText = new DrawableText(font, "0")
            { 
                Tint = Color.Yellow,
                Position = new Vector2(Game1.Width / 2f, 50),
            };
            _factoryFloor = contentManager.Load<Texture2D>("factory_floor");
            _forklift = new Forklift(contentManager.Load<Texture2D>("forklift_no_shadow")) 
            {
                Position = new Vector2(32),
                MovementSpeed = 4096,
            };

            _forklift.ScaleToSize(64f);
            var paperRollsSprite = contentManager.Load<Texture2D>("paper_rolls");

            var offset = new Vector2(32);
            foreach (var (position, isValid) in ParseData()) 
            {
                var paperRoll = new Sprite(paperRollsSprite)
                {
                    Position = offset + (position * 64f)
                };
                paperRoll.ScaleToSize(64f);
                _paperRolls.Add(paperRoll);
                if (isValid) 
                {
                    _validTargets.Add(paperRoll);
                }
            }

            _forklift.Target = _validTargets[0];
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            for (var x = 0; x < Game1.Width; x += 64) 
            {
                for (var y = 0; y < Game1.Height; y += 64)
                {
                    spriteBatch.Draw(_factoryFloor, new Vector2(x, y), Color.White);
                }
            }
            spriteBatch.End();
            spriteBatch.Begin(transformMatrix: _camera.ViewMatrix);
            foreach (var paparRoll in _paperRolls) 
            {
                paparRoll.Draw(spriteBatch);
            }

            _forklift.Draw(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin();
            _scoreText.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            if (_keyboard.IsKeyClicked(Keys.Up)) 
            {
                _forklift.MovementSpeed *= 2;
            }
            if (_keyboard.IsKeyClicked(Keys.Down)) 
            {
                _forklift.MovementSpeed /= 2;
            }
            _camera.Update();
            _forklift.Update(gameTime);
            if (_forklift.TargetLeft)
            {
                _forklift.Target = Next(_paperRolls);
                _score++;
                _scoreText = _scoreText.WithNewText(_score.ToString());
            }
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var diff = _camera.Center - _forklift.Position;
            _camera.Camera2DPosition += diff * delta;
            _camera.Camera2DPosition = Vector2.Min(Vector2.Zero, _camera.Camera2DPosition);
        }
    }
}
