using System;
using System.ComponentModel;
using Spelkonstruktionsprojekt.ZEngine.Components;
using ZEngine.EventBus;
using ZEngine.Managers;
using ZEngine.Systems;
using ZEngine.Components;
using ZEngine.Managers;

namespace Spelkonstruktionsprojekt.ZEngine.Systems
{
    public class PickupSystem : ISystem
    {
        private EventBus EventBus = EventBus.Instance;
        private ComponentManager ComponentManager = ComponentManager.Instance;

        public ISystem Start()
        {
            EventBus.Subscribe<SpecificCollisionEvent>("entityCollideWithPickup", CollideWithPickup);
            return this;
        }

        public ISystem Stop()
        {
            return this;
        }

        public void SpawnPickup()
        {

        }

        public void CreatePickup(RenderComponent renderComponent)
        {
            var x = renderComponent.PositionComponent.Position.X;
            var y = renderComponent.PositionComponent.Position.Y;
            var z = renderComponent.PositionComponent.ZIndex;

            var pickupEntityId = EntityManager.GetEntityManager().NewEntity();
            var pickupRenderComponent = new RenderComponentBuilder()
                .Position(x, y, z)
                .Dimensions(50, 50)
                .Build();
            var pickupCollisionComponent = new CollisionComponent();

            var r = new Random();
            var pickupIndex = r.Next(1, 2);

            var pickupComponent = new PickupComponent();
            if (pickupIndex == 1)
            {

            }

            ComponentManager.AddComponentToEntity(pickupCollisionComponent, pickupEntityId);
            ComponentManager.AddComponentToEntity(pickupRenderComponent, pickupEntityId);
        }

        public void CollideWithPickup(SpecificCollisionEvent collisionEvent)
        {

        }
    }
}