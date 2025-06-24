using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Unity.Extensions;
using Game.Components;
using Game.Events;
using R3;
using SystemsR3.Events;
using SystemsR3.Systems.Conventional;
using UnityEngine;

namespace Game.Systems
{
    public class PlayerAttackedSystem : IReactToEventSystem<PlayerHitEvent>
    {
        private static readonly int EnemyAttack = Animator.StringToHash("enemyAttack");
        
        public IEventSystem EventSystem { get; }
        public IEntityComponentAccessor EntityComponentAccessor { get; }

        public PlayerAttackedSystem(IEventSystem eventSystem, IEntityComponentAccessor entityComponentAccessor)
        {
            EventSystem = eventSystem;
            EntityComponentAccessor = entityComponentAccessor;
        }

        public Observable<PlayerHitEvent> ObserveOn(Observable<PlayerHitEvent> observable)
        { return observable.ObserveOnMainThread(); }

        public void Process(PlayerHitEvent eventData)
        {
            var enemyComponent = EntityComponentAccessor.GetComponent<EnemyComponent>(eventData.Enemy);
            var playerComponent = EntityComponentAccessor.GetComponent<PlayerComponent>(eventData.Player);
            playerComponent.Food.Value -= enemyComponent.EnemyPower;

            var animator = EntityComponentAccessor.GetUnityComponent<Animator>(eventData.Enemy);
            animator.SetTrigger(EnemyAttack);

            if (playerComponent.Food.Value <= 0)
            { EventSystem.Publish(new PlayerKilledEvent(eventData.Player)); }
        }
    }
}