using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PuzzleBoy
{
    abstract class Entity
    {
        protected World _world; // Every object holds a reference to the world

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }
        protected Vector2 _position;

        List<IComponent> _components;

        public Entity(World world, Vector2 absPos)
        {
            _world = world;
            _position = absPos;
            _components = new List<IComponent>();
        }

        /**
         * Overridables
         */
        public abstract void Init();
        public abstract void LoadContent(ContentManager content);
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);

        public void AttachComponent(String componentId, IComponent newComponent)
        {
            _components.Add(newComponent);
        }

        public void DetachComponent(String componentId)
        {
            foreach (IComponent component in _components)
                if (component.GetComponentId() == componentId)
                    _components.Remove(component);
        }

        public IComponent GetComponent(String componentId)
        {
            foreach (IComponent component in _components)
                if (component.GetComponentId() == componentId)
                    return component;
            return null;
        }
    }
}
