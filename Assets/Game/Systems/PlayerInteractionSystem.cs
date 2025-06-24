using System;
using System.Collections.Generic;
using EcsR3.Computeds.Entities;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Groups;
using EcsR3.Plugins.GroupBinding.Attributes;
using EcsR3.Plugins.Views.Components;
using EcsR3.Systems;
using EcsR3.Unity.Extensions;
using EcsR3.Unity.MonoBehaviours;
using Game.Components;
using Game.Events;
using R3;
using R3.Triggers;
using SystemsR3.Events;
using SystemsR3.Extensions;
using SystemsR3.Systems.Conventional;

namespace Game.Systems
{
    public class PlayerInteractionSystem : IManualSystem, IGroupSystem
    {
        public IGroup Group { get; } = new Group(typeof (PlayerComponent), typeof (ViewComponent));

        [FromGroup]
        public IComputedEntityGroup PlayerEntityGroup { get; set; }
        
        public IEventSystem EventSystem { get; }
        public IEntityComponentAccessor EntityComponentAccessor { get; }
        
        private readonly IList<IDisposable> _foodTriggers = new List<IDisposable>();
        private readonly IList<IDisposable> _exitTriggers = new List<IDisposable>();

        public PlayerInteractionSystem(IEventSystem eventSystem, IEntityComponentAccessor entityComponentAccessor)
        {
            EventSystem = eventSystem;
            EntityComponentAccessor = entityComponentAccessor;
        }

        public void StartSystem()
        {
            this.WaitForScene().Subscribe(x =>
            {
                foreach(var player in PlayerEntityGroup)
                { CheckForInteractions(player); }
            });
        }

        public void StopSystem()
        {
            _foodTriggers.DisposeAll();
            _exitTriggers.DisposeAll();
        }

        private void CheckForInteractions(Entity player)
        {
            var currentPlayer = player;
            var playerView = EntityComponentAccessor.GetGameObject(currentPlayer);
            var triggerObservable = playerView.OnTriggerEnter2DAsObservable();
            
            var foodTrigger = triggerObservable
                .Where(x => x.gameObject.CompareTag("Food") || x.gameObject.CompareTag("Soda"))
                .Subscribe(x =>
                {
                    var entityView = x.gameObject.GetComponent<EntityView>();
                    var isSoda = x.gameObject.CompareTag("Soda");
                    HandleFoodPickup(entityView.Entity, currentPlayer, isSoda);
                });

            _foodTriggers.Add(foodTrigger);

            var exitTrigger = triggerObservable
                .Where(x => x.gameObject.CompareTag("Exit"))
                .Subscribe(x =>
                {
                    var entityView = x.gameObject.GetComponent<EntityView>();
                    HandleExit(entityView.Entity, currentPlayer);
                });

            _exitTriggers.Add(exitTrigger);
        }

        private void HandleFoodPickup(Entity food, Entity player, bool isSoda)
        { EventSystem.Publish(new FoodPickupEvent(food, player, isSoda)); }

        private void HandleExit(Entity exit, Entity player)
        { EventSystem.Publish(new ExitReachedEvent(exit, player)); }
    }
}