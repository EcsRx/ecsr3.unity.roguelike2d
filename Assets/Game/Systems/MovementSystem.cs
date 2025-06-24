using System;
using System.Collections;
using System.Threading.Tasks;
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
using SystemsR3.Extensions;
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
        { return Observable.EveryUpdate().Select(x => entity); }

        public void Process(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            var viewGameObject = entityComponentAccessor.GetGameObject(entity);
            var movementComponent = entityComponentAccessor.GetComponent<MovementComponent>(entity);

            if (movementComponent.IsMoving || movementComponent.Movement.Value == Vector2.zero)
            { return; }

            var isPlayer = entityComponentAccessor.HasComponent<PlayerComponent>(entity);
            Vector2 currentPosition = viewGameObject.transform.position;
            var destination = currentPosition + movementComponent.Movement.Value;
            
            var collidedObject = CheckForCollision(viewGameObject, currentPosition, destination);
            var canMove = collidedObject == null;
            if (canMove)
            {
                movementComponent.IsMoving = true;
                
                var rigidBody = viewGameObject.GetComponent<Rigidbody2D>();
                MoveTowardsDestination(viewGameObject, rigidBody, destination, movementComponent)
                    .ToObservable()
                    .SubscribeOnce(x =>
                    {
                        movementComponent.IsMoving = false;
                        _eventSystem.Publish(new EntityMovedEvent(isPlayer));
                    
                        if (!isPlayer) { return; }
                    
                        var playerComponent = entityComponentAccessor.GetComponent<PlayerComponent>(entity);
                        playerComponent.Food.Value--;
                        _eventSystem.Publish(new PlayerTurnOverEvent());
                    });
            }
            else
            {
                movementComponent.Movement.Value = Vector2.zero;
                movementComponent.IsMoving = false;

                var entityView = collidedObject.GetComponent<EntityView>();
                if(!entityView) { return; }

                if (isPlayer && collidedObject.tag.Contains("Wall"))
                { WallHit(entityView.Entity, entity); }
                else if (isPlayer && collidedObject.tag.Contains("Enemy"))
                { EnemyHit(entityView.Entity, entity); }
                else if(!isPlayer && collidedObject.tag.Contains("Player"))
                { PlayerHit(entityView.Entity, entity); }
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

        protected async Task MoveTowardsDestination(GameObject mover, Rigidbody2D rigidBody, Vector3 destination, MovementComponent movementComponent)
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
                await Task.Yield();
            }

            if(mover != null)
            { mover.transform.position = destination; }

            movementComponent.Movement.Value = Vector2.zero;
        }

        private void WallHit(Entity wall, Entity player)
        {
            _eventSystem.Publish(new WallHitEvent(wall, player));
            _eventSystem.Publish(new PlayerTurnOverEvent());
        }
        
        private void PlayerHit(Entity player, Entity enemy)
        { _eventSystem.Publish(new PlayerHitEvent(player, enemy)); }

        private void EnemyHit(Entity enemy, Entity player)
        {
            _eventSystem.Publish(new EnemyHitEvent(enemy, player));
            _eventSystem.Publish(new PlayerTurnOverEvent());
        }
    }
}