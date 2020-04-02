using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PuzzleBoy
{
    class ObtainableItem : Entity, IInteractable
    {
        Texture2D texture = null;

        public ObtainableItem(World world, Vector2 absPos) : base(world, absPos)
        {

        }

        public override void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("Sprites/Item");
        }

        public override void Init()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, _position, Color.White);
        }

        public void Interact()
        {
            _world.Map.Entities.Remove(this);
        }
    }
}
