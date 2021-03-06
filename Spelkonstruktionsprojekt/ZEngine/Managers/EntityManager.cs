﻿using System.Collections;
using System.Collections.Generic;
using Spelkonstruktionsprojekt.ZEngine.Components;
using Spelkonstruktionsprojekt.ZEngine.Managers;

namespace ZEngine.Managers
{
    public class EntityManager
    {
        // _____________________________________________________________________________________________________________________ //

        // An instance of this singleton EntityManager.
        // The next unique entity id that will be generated.
        // List with all the unique entities.
        private static EntityManager _entityManager;
        private int _nextEntityId;
        private List<int> _existingEntities;

        // _____________________________________________________________________________________________________________________ //

        // This constructor initializes the first unique entity id to 0, so when
        // the client calls NewEntity: the first entity id will be 0. The other thing
        // the construktor does is that it initializes the list that will contain
        // all the entities that will be used.
        private EntityManager()
        {
            _nextEntityId = 0;
            _existingEntities = new List<int>();

        }

        // _____________________________________________________________________________________________________________________ //

        // Singelton pattern, we will only have one instance
        // of our entity manager.
        public static EntityManager GetEntityManager()
        {
            if (_entityManager != null)
            {
                return _entityManager;
            }
            else
            {
                _entityManager = new EntityManager();
                return _entityManager;
            }
        }

        // This method generates a unique entity that can be used
        // to build a gameobject with components.
        // Add new entity id to the list of entites and return it
        // while generating a new id for the next call.
        public int NewEntity()
        {
            _existingEntities.Add(_nextEntityId);
            var entityId = _nextEntityId++;
            AddMandatoryComponents(entityId);
            return entityId;
        }

        private void AddMandatoryComponents(int entityId)
        {
            var stateComponent = new StateComponent();
            var tagComponent = new TagComponent();
            ComponentManager.Instance.AddComponentToEntity(stateComponent, entityId);
            ComponentManager.Instance.AddComponentToEntity(tagComponent, entityId);
        }

        // Completely deletes the entity and all components
        // that are associated with it, thats why it needs to
        // use the component manager instance.
        public void DeleteEntity(int entityId)
        {
            _existingEntities.Remove(entityId);
            ComponentManager.Instance.DeleteEntity(entityId);
        }

        // Returns the complete list with all of the existing 
        // Entities, that have been created.
        public List<int> GetListWithEntities()
        {
            return _existingEntities;
        }
    }
}
