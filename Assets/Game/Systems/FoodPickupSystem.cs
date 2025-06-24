using EcsR3.Collections.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Unity.Extensions;
using Game.Components;
using Game.Events;
using R3;
using SystemsR3.Attributes;
using SystemsR3.Systems.Conventional;
using SystemsR3.Types;

namespace Game.Systems
{
    [Priority(PriorityTypes.Low)]
    public class FoodPickupSystem : IReactToEventSystem<FoodPickupEvent>
    {
        public IEntityCollection EntityCollection { get; }
        public IEntityComponentAccessor EntityComponentAccessor { get; }
        
        public FoodPickupSystem(IEntityCollection entityCollection, IEntityComponentAccessor entityComponentAccessor)
        {
            EntityCollection = entityCollection;
            EntityComponentAccessor = entityComponentAccessor;
        }

        public Observable<FoodPickupEvent> ObserveOn(Observable<FoodPickupEvent> observable)
        { return observable.ObserveOnMainThread(); }

        public void Process(FoodPickupEvent eventData)
        {
            var playerComponent = EntityComponentAccessor.GetComponent<PlayerComponent>(eventData.Player);
            var foodComponent = EntityComponentAccessor.GetComponent<FoodComponent>(eventData.Food);

            playerComponent.Food.Value += foodComponent.FoodAmount;

            this.AfterUpdateDo(() => EntityCollection.Remove(eventData.Food));
        }
    }
}