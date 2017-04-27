﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ZEngine.Components;
using ZEngine.Managers;
using ZEngine.EventBus;
using ZEngine.Wrappers;
using Spelkonstruktionsprojekt.ZEngine.Components;

namespace ZEngine.Systems
{
    class LoadContentSystem : ISystem
    {
        public void LoadContent(ContentManager contentManager)
        {
            var entities = ComponentManager.Instance
                .GetEntitiesWithComponent<SpriteComponent>()
                .Where(entity => !entity.Value.SpriteIsLoaded);

            foreach (var entity in entities)
            {
                if (string.IsNullOrEmpty(entity.Value.SpriteName)) continue;
                entity.Value.Sprite = contentManager.Load<Texture2D>(entity.Value.SpriteName);
                entity.Value.SpriteIsLoaded = true;
                entity.Value.Width = entity.Value.Sprite.Width;
                entity.Value.Height = entity.Value.Sprite.Height;
            }

            var fontEntities = ComponentManager.Instance
                .GetEntitiesWithComponent<FontComponent>()
                .Where(entity => !entity.Value.FontIsLoaded);
            foreach (var entity in fontEntities)
            {
                entity.Value.font = contentManager.Load<SpriteFont>(entity.Value.fontName);
                entity.Value.FontIsLoaded = true;
            }

            var soundEntities = ComponentManager.Instance.GetEntitiesWithComponent(typeof(SoundComponent));
            foreach (var entity in soundEntities)
            {
                SoundComponent soundComponent = (SoundComponent)entity.Value;
                soundComponent.SoundEffect = contentManager.Load<SoundEffect>(soundComponent.SoundEffectName);
            }
        }
    }
}
