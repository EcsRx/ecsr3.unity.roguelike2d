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
    public class WallHitSystem : IReactToEventSystem<WallHitEvent>
    {
        private static readonly int PlayerChop = Animator.StringToHash("playerChop");
        
        public IEntityCollection EntityCollection { get; }
        public IEntityComponentAccessor EntityComponentAccessor { get; }

        public WallHitSystem(IEntityCollection entityCollection, IEntityComponentAccessor entityComponentAccessor)
        {
            EntityCollection = entityCollection;
            EntityComponentAccessor = entityComponentAccessor;
        }

        public Observable<WallHitEvent> ObserveOn(Observable<WallHitEvent> observable)
        { return observable.ObserveOnMainThread(); }

        public void Process(WallHitEvent eventData)
        {
            var wallComponent = EntityComponentAccessor.GetComponent<WallComponent>(eventData.Wall);
            wallComponent.Health.Value--;

            var animator = EntityComponentAccessor.GetUnityComponent<Animator>(eventData.Player);
            animator.SetTrigger(PlayerChop);

            if (wallComponent.Health.Value <= 0)
            { EntityCollection.Remove(eventData.Wall); }
        }
    }
}