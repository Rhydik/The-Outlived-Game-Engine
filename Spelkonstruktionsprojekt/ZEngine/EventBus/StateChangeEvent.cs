﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Spelkonstruktionsprojekt.ZEngine.Components;
using Spelkonstruktionsprojekt.ZEngine.Managers;
using ZEngine.Managers;

namespace ZEngine.EventBus
{
    public class StateChangeEvent
    {
        public const string EventName = "StateChanged";
        public readonly int EntityId;
        public readonly double EventTime;
        public readonly ImmutableList<State> NewState;

        public StateChangeEvent(int entityId, double eventTime, IEnumerable<State> newState)
        {
            EntityId = entityId;
            EventTime = eventTime;
            NewState = newState.ToImmutableList();
        }
    }

    public class StateChangeEventBuilder
    {
        private int _entityId = -1;
        private double _eventTime;

        public StateChangeEventBuilder Entity(int id)
        {
            _entityId = id;
            return this;
        }

        public StateChangeEventBuilder Time(double eventTime)
        {
            _eventTime = eventTime;
            return this;
        }

        public StateChangeEvent Build()
        {
            if (_entityId == -1) throw new Exception("EntityId must be set before build StateChangeEvent.");
            var state = ComponentManager.Instance.GetEntityComponentOrDefault<StateComponent>(_entityId).State;
            return new StateChangeEvent(
                _entityId,
                _eventTime,
                state
            );
        }
    }
}