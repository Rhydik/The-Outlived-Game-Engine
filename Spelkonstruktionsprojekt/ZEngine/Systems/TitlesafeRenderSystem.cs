﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Spelkonstruktionsprojekt.ZEngine.Components;
using ZEngine.Managers;
using ZEngine.Wrappers;

namespace Spelkonstruktionsprojekt.ZEngine.Systems
{
    // This system is used for rendering the HUD, those entities
    // that have instances ofcomponents like health or ammo will 
    // be able to se the component data on the screen when playing.
    // This can be used to that the player always can see the status
    // of his player. (everything drawn here is and should be titlesafe) 
    class TitlesafeRenderSystem : ISystem
    {
        public static string SystemName = "TitlesafeRender";
        private GameDependencies _gameDependencies;

        // This draw method is used to start the system process.
        // it uses DrawTitleSafe to draw the components.
        public void Draw(GameDependencies gameDependencies)
        {
            this._gameDependencies = gameDependencies;

            _gameDependencies.SpriteBatch.Begin(SpriteSortMode.FrontToBack);
            DrawMenu();
            DrawTitleSafe();
            _gameDependencies.SpriteBatch.End();
        }

        private void DrawMenu()
        {
            var graphics = _gameDependencies.GraphicsDeviceManager.GraphicsDevice;
            var fontComponents = ComponentManager.Instance.GetEntitiesWithComponent(typeof(FontComponent));
            //should have MenuComponent? and ButtonComponent?
            foreach (var font in fontComponents)
            {
                var fontComponent = font.Value as FontComponent;
                string text = fontComponent.text;
                
                var xPosition = 500;
                var yPosition = 500;

                var position = new Vector2(xPosition, yPosition);
                _gameDependencies.SpriteBatch.DrawString(fontComponent.font, text, position, Color.White);
            }
        }
        // DrwaTitleSafe gets all the components and draws them in a
        // correct format, so the data will sortet so that each entity will
        // have it's own row, and each component will be sorted by column.
        private void DrawTitleSafe()
        {
            var graphics = _gameDependencies.GraphicsDeviceManager.GraphicsDevice;
            var titlesafearea = graphics.Viewport.TitleSafeArea;

            var playerComponents = ComponentManager.Instance.GetEntitiesWithComponent(typeof(PlayerComponent));

            // We save the previous text height so we can stack
            // them (the text for every player) on top of eachother.
            var previousHeight = 5f;

            var contentManager = _gameDependencies.GameContent as ContentManager;         
            // Maybe let the user decide?
            var spriteFont = contentManager.Load<SpriteFont>("ZEone");

            foreach (var playerInstance in playerComponents)
            {
                var playerComponent = playerInstance.Value as PlayerComponent;
                var text = playerComponent.Name;
                // Adding the health component to text.
                if (ComponentManager.Instance.EntityHasComponent<HealthComponent>(playerInstance.Key))
                {
                    var health = ComponentManager.Instance.GetEntityComponentOrDefault<HealthComponent>(playerInstance.Key);

                    if (health.Alive)
                    {
                        var currentHealth = health.MaxHealth - health.Damage.Sum();
                        text = text + ": " + currentHealth + "HP";
                    }
                    else
                    {
                        text = text + ": Rest in peace";                      
                    }

                    // adding ammo here the same way.
                    if (ComponentManager.Instance.EntityHasComponent<AmmoComponent>(playerInstance.Key) && health.Alive)
                    {
                        var ammo = ComponentManager.Instance.GetEntityComponentOrDefault<AmmoComponent>(playerInstance.Key);
                        text = text + " Ammo: " + ammo.Amount;
                    }
                }

                // this call gives us the height of the text,
                // so now we are able to stack them on top of each other.
                var textHeight = spriteFont.MeasureString(text).Y;

                var xPosition = titlesafearea.Width - spriteFont.MeasureString(text).X - 10;
                var yPosition = titlesafearea.Height - (textHeight + previousHeight);
                previousHeight += textHeight;

                var position = new Vector2(xPosition, yPosition);
                _gameDependencies.SpriteBatch.DrawString(spriteFont, text, position, Color.BlueViolet);

            }
        }
    }
}
