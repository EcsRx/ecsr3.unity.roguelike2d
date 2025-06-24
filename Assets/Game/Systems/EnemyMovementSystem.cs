using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Unity.Extensions;
using Game.Components;
using Game.Computeds;
using Game.Events;
using R3;
using SystemsR3.Systems.Conventional;
using UnityEngine;

namespace Game.Systems
{
    public class EnemyMovementSystem : IReactToEventSystem<EnemyTurnEvent>
    {
        public IEntityComponentAccessor EntityComponentAccessor { get; }
        public IComputedPlayerPosition ComputedPlayerPosition { get; }

        public EnemyMovementSystem(IComputedPlayerPosition computedPlayerPosition, IEntityComponentAccessor entityComponentAccessor)
        {
            ComputedPlayerPosition = computedPlayerPosition;
            EntityComponentAccessor = entityComponentAccessor;
        }

        public Observable<EnemyTurnEvent> ObserveOn(Observable<EnemyTurnEvent> observable)
        { return observable.ObserveOnMainThread(); }

        public void Process(EnemyTurnEvent eventData)
        {
            var movementComponent = EntityComponentAccessor.GetComponent<MovementComponent>(eventData.Enemy);
            if(movementComponent.Movement.Value != Vector2.zero) { return; }

            var enemyComponent = EntityComponentAccessor.GetComponent<EnemyComponent>(eventData.Enemy);
            if (enemyComponent.IsSkippingNextTurn)
            {
                enemyComponent.IsSkippingNextTurn = false;
                return;
            }

            enemyComponent.IsSkippingNextTurn = true;

            var playerLocation = ComputedPlayerPosition.Value;
            
            var gameObject = EntityComponentAccessor.GetGameObject(eventData.Enemy);
            var entityLocation = gameObject.transform.position;
            movementComponent.Movement.Value = CalculateMovement(entityLocation, playerLocation);
        }
        
        private Vector2 CalculateMovement(Vector3 currentPosition, Vector3 playerPosition)
        {
            var x = 0.0f;
            var y = 0.0f;

            if (Mathf.Abs(playerPosition.x - currentPosition.x) < float.Epsilon)
            { y = playerPosition.y > currentPosition.y ? 1 : -1; }
            else
            { x = playerPosition.x > currentPosition.x ? 1 : -1; }

            return new Vector2(x, y);
        }
    }
}