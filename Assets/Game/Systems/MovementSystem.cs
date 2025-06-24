using System;
using System.Collections;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Plugins.Views.Components;
using EcsR3.Systems.Reactive;
using EcsR3.Unity.Extensions;
using EcsR3.Unity.MonoBehaviours;
using Game.Components;
using Game.Configuration;
using Game.Events;
using R3;
using SystemsR3.Events;
using UnityEngine;

namespace Game.Systems
{
    public class MovementSystem : IReactToEntitySystem
    {
        private readonly LayerMask _blockingLayer = LayerMask.GetMask("BlockingLayer");
        
        private readonly GameConfiguration _gameConfiguration;
        private readonly IEventSystem _eventSystem;
        
        public IGroup Group { get; } = new Group(typeof(ViewComponent), typeof(MovementComponent)); 

        public MovementSystem(GameConfiguration gameConfiguration, IEventSystem eventSystem)
        {
            _gameConfiguration = gameConfiguration;
            _eventSystem = eventSystem;
        }

        public Observable<Entity> ReactToEntity(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            var movementComponent = entityComponentAccessor.GetComponent<MovementComponent>(entity);
            
            return movementComponent.Movement.DistinctUntilChanged()
                .Where(x => x != Vector2.zero)
                .Select(x => entity);
        }

        public void Process(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            var viewGameObject = entityComponentAccessor.GetGameObject(entity);
            var movementComponent = entityComponentAccessor.GetComponent<MovementComponent>(entity);

            Vector2 currentPosition = viewGameObject.transform.position;
            var destination = currentPosition + movementComponent.Movement.Value;
            var collidedObject = CheckForCollision(viewGameObject, currentPosition, destination);
            var canMove = collidedObject == null;

            var isPlayer = entityComponentAccessor.HasComponent<PlayerComponent>(entity);

            if (!canMove)
            {
                movementComponent.Movement.Value = Vector2.zero;

                var entityView = collidedObject.GetComponent<EntityView>();
                if(!entityView) { return; }

                if (isPlayer && collidedObject.tag.Contains("Wall"))
                { WallHit(entityView.Entity, entity); }

                if (isPlayer && collidedObject.tag.Contains("Enemy"))
                { EnemyHit(entityView.Entity, entity); }

                if(!isPlayer && collidedObject.tag.Contains("Player"))
                { PlayerHit(entityView.Entity, entity); }
                
                return;
            }

            var rigidBody = viewGameObject.GetComponent<Rigidbody2D>();
            SmoothMovement(viewGameObject, rigidBody, destination, movementComponent);
            _eventSystem.Publish(new EntityMovedEvent(isPlayer));

            if (isPlayer)
            {
                var playerComponent = entityComponentAccessor.GetComponent<PlayerComponent>(entity);
                playerComponent.Food.Value--;
            }
        }

        private GameObject CheckForCollision(GameObject mover, Vector2 start, Vector2 destination)
        {
            var boxCollider = mover.GetComponent<BoxCollider2D>();
            boxCollider.enabled = false;
            var hit = Physics2D.Linecast(start, destination, _blockingLayer);
            boxCollider.enabled = true;

            if(!hit.collider) { return null; }
            return hit.collider.gameObject;
        }

        protected async void SmoothMovement(GameObject mover, Rigidbody2D rigidBody, Vector3 destination, MovementComponent movementComponent)
        {
            while (mover != null && Vector3.Distance(mover.transform.position, destination) > 0.1f)
            {
                if (movementComponent.StopMovement)
                {
                    movementComponent.Movement.Value = Vector2.zero;
                    movementComponent.StopMovement = false;
                    return;
                }

                var newPostion = Vector3.MoveTowards(rigidBody.position, destination, _gameConfiguration.MovementSpeed * Time.deltaTime);
                rigidBody.MovePosition(newPostion);
                await System.Threading.Tasks.Task.Yield();
            }

            if(mover != null)
            { mover.transform.position = destination; }

            movementComponent.Movement.Value = Vector2.zero;
        }

        private void WallHit(Entity wall, Entity player)
        {
            _eventSystem.Publish(new WallHitEvent(wall, player));
        }

        private void PlayerHit(Entity player, Entity enemy)
        {
            _eventSystem.Publish(new PlayerHitEvent(player, enemy));
        }

        private void EnemyHit(Entity enemy, Entity player)
        {
            _eventSystem.Publish(new EnemyHitEvent(enemy, player));
        }
    }
}