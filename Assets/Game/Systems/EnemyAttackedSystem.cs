using EcsR3.Collections.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Unity.Extensions;
using Game.Components;
using Game.Events;
using R3;
using SystemsR3.Systems.Conventional;
using UnityEngine;

namespace Game.Systems
{
    public class EnemyAttackedSystem : IReactToEventSystem<EnemyHitEvent>
    {
        private static readonly int PlayerChop = Animator.StringToHash("playerChop");
        
        public IEntityComponentAccessor EntityComponentAccessor { get; }
        public IEntityCollection EntityCollection { get; }

        public EnemyAttackedSystem(IEntityComponentAccessor entityComponentAccessor, IEntityCollection entityCollection)
        {
            EntityComponentAccessor = entityComponentAccessor;
            EntityCollection = entityCollection;
        }

        public Observable<EnemyHitEvent> ObserveOn(Observable<EnemyHitEvent> observable)
        { return observable.ObserveOnMainThread(); }

        public void Process(EnemyHitEvent eventData)
        {
            var enemyComponent = EntityComponentAccessor.GetComponent<EnemyComponent>(eventData.Enemy);
            enemyComponent.Health.Value--;

            var animator = EntityComponentAccessor.GetUnityComponent<Animator>(eventData.Player);
            animator.SetTrigger(PlayerChop);

            if (enemyComponent.Health.Value <= 0)
            { EntityCollection.Remove(eventData.Enemy); }
        }
    }
}