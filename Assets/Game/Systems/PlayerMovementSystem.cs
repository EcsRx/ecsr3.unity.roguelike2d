using System;
using EcsR3.Computeds.Entities;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Systems.Reactive;
using Game.Components;
using Game.Events;
using R3;
using SystemsR3.Events;
using SystemsR3.Systems.Conventional;
using UnityEngine;

namespace Game.Systems
{
    public class PlayerMovementSystem : IReactToGroupSystem, IReactToEventSystem<PlayerTurnEvent>, IReactToEventSystem<PlayerTurnOverEvent>
    {
        public IGroup Group { get; } = new Group(typeof(MovementComponent), typeof(PlayerComponent));

        private readonly IEventSystem _eventSystem;
        private bool _isPlayersTurn = false;

        public Observable<IComputedEntityGroup> ReactToGroup(IComputedEntityGroup group)
        { return Observable.EveryUpdate().Select(x => group); }

        public PlayerMovementSystem(IEventSystem eventSystem)
        { _eventSystem = eventSystem; }

        public void Process(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            if(!_isPlayersTurn) { return; }
            
            var movementComponent = entityComponentAccessor.GetComponent<MovementComponent>(entity);

            if (movementComponent.Movement.Value != Vector2.zero)
            { return; }
            
            if (entityComponentAccessor.HasComponent<StandardInputComponent>(entity))
            {
                var inputComponent = entityComponentAccessor.GetComponent<StandardInputComponent>(entity);
                movementComponent.Movement.Value = inputComponent.PendingMovement;
                inputComponent.PendingMovement = Vector2.zero;
                return;
            }

            if (entityComponentAccessor.HasComponent<TouchInputComponent>(entity))
            {
                var inputComponent = entityComponentAccessor.GetComponent<TouchInputComponent>(entity);
                movementComponent.Movement.Value = inputComponent.PendingMovement;
                inputComponent.PendingMovement = Vector2.zero;
                return;
            }
        }

        public Observable<PlayerTurnEvent> ObserveOn(Observable<PlayerTurnEvent> observable)
        { return observable; }

        public Observable<PlayerTurnOverEvent> ObserveOn(Observable<PlayerTurnOverEvent> observable)
        { return observable; }
        
        public void Process(PlayerTurnEvent eventData)
        { _isPlayersTurn = true; }

        public void Process(PlayerTurnOverEvent eventData)
        { _isPlayersTurn = false; }
    }
}