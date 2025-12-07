using AdventOfCode2025.Challenges.Day1;
using AdventOfCode2025.Challenges.Day2;
using AdventOfCode2025.Challenges.Day3;
using AdventOfCode2025.Challenges.Day4;
using AdventOfCode2025.Challenges.Day5;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Oscetch.MonoGame.Input.Managers;
using Oscetch.MonoGame.Input.Services;
using Oscetch.MonoGame.Textures;
using Oscetch.MonoGame.Textures.Enums;
using System;
using System.Collections.Generic;

namespace AdventOfCode2025
{
    public class Game1 : Game
    {
        public static readonly int Width = 1280;
        public static readonly int Height = 720;

        public static Texture2D White { get; private set; }

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private KeyboardStateService _keyboard;
        private static bool _isPaused;

        private readonly IReadOnlyList<Func<Scene>> _scenes = [
            () => new Cafeteria(),
            () => new CafeteriaExample(),
            () => new PrintingDepartmentPart2(),
            () => new PrintingDepartmentPart2Example(),
            () => new PrintingDepartment(),
            () => new PrintingDepartmentExample(),
            () => new LobbyPart2(),
            () => new Lobby(),
            () => new LobbyExample(),
            () => new GiftShopPart2(),
            () => new GiftShopPart2Example(),
            () => new GiftShop(),
            () => new GiftShopExample(),
            () => new SecretEntrance(),
            () => new SecretEntranceExample(),
        ];
        private int _currentSceneIndex;
        private Scene _currentScene;

        public static void TogglePause() 
        {
            _isPaused = !_isPaused;
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this) 
            {
                PreferredBackBufferWidth = Width,
                PreferredBackBufferHeight = Height,
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = false;
            TargetElapsedTime = TimeSpan.FromMilliseconds(1);
        }

        protected override void Initialize()
        {
            base.Initialize();
            _keyboard = KeyboardManager.GetGeneral();
            MouseManager.SetScreenSize(Width, Height);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            var parameters = new CustomTextureParameters.CustomTextureParametersBuilder().WithShape(ShapeType.Rectangle).WithSize(1).WithFillColor(Color.White).Build();
            White = CustomTextureManager.GetCustomTexture(parameters, GraphicsDevice);
            _currentScene = _scenes[0]();
            _currentScene.Initialize(Content, GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            KeyboardManager.Update();
            MouseManager.Update();
            if (_keyboard.IsKeyClicked(Keys.Space)) 
            {
                _currentSceneIndex = (_currentSceneIndex + 1) % _scenes.Count;
                _currentScene = _scenes[_currentSceneIndex]();
                _currentScene.Initialize(Content, GraphicsDevice);
            }
            if (_keyboard.IsKeyClicked(Keys.Enter))
            {
                _isPaused = !_isPaused;
            }
            if (_keyboard.AreKeysClicked(Keys.LeftAlt, Keys.Enter)) 
            {
                _graphics.ToggleFullScreen();
            }

            if (!_isPaused)
            {
                _currentScene.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            _currentScene.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
