using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZEngine.Components;

namespace Spelkonstruktionsprojekt.ZEngine.Components
{
    class FontComponent : IComponent
    {
        public string fontName;
        public SpriteFont font;
        public string text;
        public bool FontIsLoaded { get; set; } = false;
    }
}
