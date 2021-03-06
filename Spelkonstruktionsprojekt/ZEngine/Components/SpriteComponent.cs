﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZEngine.Components;

namespace ZEngine.Components
{
    class SpriteComponent : IComponent
    {
        public int TileWidth { get; set; } = 0;
        public int TileHeight { get; set; } = 0;
        public Point Position { get; set; } = new Point(0,0);

        public bool SpriteIsLoaded { get; set; } = false;
        public Texture2D Sprite { get; set; }
        public string SpriteName { get; set; }

        // Added to be used for gradient transparency when an entity
        // dies, but can be used in other cases also.
        public float Scale { get; set; } = 1;
        public float Alpha { get; set; } = 1;
    }
}
