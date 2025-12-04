using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Oscetch.MonoGame.Math.Helpers;
using System;

namespace AdventOfCode2025.Challenges.Day4
{
    internal class Forklift(Texture2D texture) : Sprite(texture)
    {
        public static readonly Vector2 DROP_OFF_POSITION = new (-32, 32);

        private bool _targetAquired;
        private Sprite _target;

        public Sprite Target 
        { 
            get => _target; 
            set
            {
                _target = value;
                _targetAquired = false;
                TargetLeft = false;
            }
        }

        public float MovementSpeed { get; set; } = 64f;

        public bool TargetLeft { get; private set; }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (_targetAquired) _target.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime) 
        {
            if (_target == null) return;
            if (_targetAquired)
            {
                MoveTarget(gameTime);
            }
            else 
            {
                AquireTarget(gameTime);
            }
        }

        private void AquireTarget(GameTime gameTime) 
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var distance = Vector2.Distance(Position, _target.Position);
            if (distance < 1f) 
            {
                _targetAquired = true;
                return;
            }
            var intendedMovement = MovementSpeed * delta;
            var actualMovement = Math.Min(distance, intendedMovement);
            var angle = AngleHelper.GetAngleBetweenVectorsInRadians(Position, _target.Position);
            Rotation = angle;
            Position = AngleHelper.MoveObjectInDirection(Position, angle, actualMovement);
        }

        private void MoveTarget(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var distance = Vector2.Distance(Position, DROP_OFF_POSITION);
            if (distance < 1f) 
            {
                Target.Position = DROP_OFF_POSITION;
                Target = null;
                TargetLeft = true;
                return;
            }
            var intendedMovement = MovementSpeed * delta;
            var actualMovement = Math.Min(distance, intendedMovement);
            var angle = AngleHelper.GetAngleBetweenVectorsInRadians(Position, DROP_OFF_POSITION);
            Rotation = angle;
            Position = AngleHelper.MoveObjectInDirection(Position, angle, actualMovement);
            _target.Position = Position;
        }
    }
}
