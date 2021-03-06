﻿using Spelkonstruktionsprojekt.ZEngine.Components;
using Spelkonstruktionsprojekt.ZEngine.Components.PickupComponents;
using Spelkonstruktionsprojekt.ZEngine.Constants;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spelkonstruktionsprojekt.ZEngine.Managers;
using ZEngine.EventBus;
using ZEngine.Managers;
using ZEngine.Systems;

namespace Spelkonstruktionsprojekt.ZEngine.Systems.Collisions
{
    internal class PickupCollisionSystem : ISystem
    {
        private readonly ComponentManager ComponentManager = ComponentManager.Instance;
        private readonly EventBus EventBus = EventBus.Instance;


        //Pickup Values, should be moved to components later
        private int HealingAmount = 50;
        private int AmmoAmount = 10;


        public void Start()
        {
            EventBus.Subscribe<SpecificCollisionEvent>(EventConstants.PickupCollision, Handle);
        }
        

        /*
         * Target is the pickup, entity is the player
         * This function determines what kind of pickup type it is, and calls the appropriate method.
         * First it checks if it has already been used(has the delete tag), in case 2 players touched it in the same frame.
         */
        public void Handle(SpecificCollisionEvent collisionEvent)
        {
            if (ComponentManager.GetEntityComponentOrDefault<TagComponent>(collisionEvent.Target).Tags.Contains(Tag.Delete))
            {
                return;
            }
            if (ComponentManager.EntityHasComponent(typeof(HealthPickupComponent), collisionEvent.Target))
            {
                HandleHealthPickup(collisionEvent.Entity, collisionEvent.Target);
            }
            else if (ComponentManager.EntityHasComponent(typeof(AmmoPickupComponent), collisionEvent.Target))
            {
                HandleAmmoPickup(collisionEvent.Entity, collisionEvent.Target);
            }

        }

        private void HandleHealthPickup(int player, int pickup)
        {
            var HealthComponent = (HealthComponent)ComponentManager.GetEntityComponentOrDefault(typeof(HealthComponent), player);
            if (HealthComponent.CurrentHealth < HealthComponent.MaxHealth)
            {
                HealthComponent.Damage.Add(-HealingAmount);
                DeletePickup(pickup);
            }
        }


        private void HandleAmmoPickup(int player, int pickup)
        {
            var AmmoComponent = (AmmoComponent)ComponentManager.GetEntityComponentOrDefault(typeof(AmmoComponent), player);
            AmmoComponent.Amount += AmmoAmount;
            DeletePickup(pickup);
        }

        /*
         This function tags the pickup for deletion. 
         Should only be called when the pickup was actually used up.
        */
        private void DeletePickup(int pickup)
        {
            var tagComponent = ComponentManager.GetEntityComponentOrDefault<TagComponent>(pickup);

            if (tagComponent == null)
            {
                tagComponent = new TagComponent();
                ComponentManager.AddComponentToEntity(tagComponent, pickup);
            }
            tagComponent.Tags.Add(Tag.Delete);
        }

    }
}

