using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Spelkonstruktionsprojekt.ZEngine.Components;
using System;
using Microsoft.Xna.Framework.Content;
using ZEngine.Components;
using ZEngine.Managers;
using System.Drawing;

using static Spelkonstruktionsprojekt.ZEngine.GameTest.TestChaseGame;

namespace Spelkonstruktionsprojekt.Zenu
{
    public class Button
    {

        public Button(String text)
        {

            var fontComponent = new FontComponent()
            {
                fontName = "Score"
            };

            var entityId = EntityManager.GetEntityManager().NewEntity();
            var renderComponent = new RenderComponentBuilder()
                .Position(500, 500, 20)
                .Dimensions(300, 60)
                .Fixed(true)
                .Build();
            var spriteComponent = new SpriteComponent()
            {
                SpriteName = "dot"
            };

            ComponentManager.Instance.AddComponentToEntity(renderComponent, entityId);
            ComponentManager.Instance.AddComponentToEntity(spriteComponent, entityId);
            ComponentManager.Instance.AddComponentToEntity(fontComponent, entityId);
            // ComponentManager.Instance.AddComponentToEntity(textRenderComponent, entityId);
        }
    }
}