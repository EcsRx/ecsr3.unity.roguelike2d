using System;
using System.Collections.Generic;
using System.Linq;
using EcsR3.Computeds.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Plugins.GroupBinding.Attributes;
using EcsR3.Systems;
using EcsR3.Unity.Extensions;
using Game.Components;
using Game.Events;
using R3;
using SystemsR3.Attributes;
using SystemsR3.Events;
using SystemsR3.Extensions;
using SystemsR3.Systems.Conventional;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Systems
{
    [Priority(10)]
    public class FoodTextUpdateSystem : IManualSystem, IGroupSystem
    {
        public IGroup Group { get; } = new Group(typeof(PlayerComponent));

        [FromGroup]
        public IComputedEntityGroup PlayerEntityGroup  { get; set; }
        
        public IEntityComponentAccessor EntityComponentAccessor { get; }
        
        private readonly IEventSystem _eventSystem;
        private PlayerComponent _playerComponent;
        private Text _foodText;
        private readonly IList<IDisposable> _subscriptions = new List<IDisposable>();

        public FoodTextUpdateSystem(IEventSystem eventSystem, IEntityComponentAccessor entityComponentAccessor)
        {
            _eventSystem = eventSystem;
            EntityComponentAccessor = entityComponentAccessor;
        }

        public void StartSystem()
        {
            this.WaitForScene().SubscribeOnce(x =>
            {
                var player = PlayerEntityGroup.First();
                _playerComponent = EntityComponentAccessor.GetComponent<PlayerComponent>(player);
                _foodText = GameObject.Find("FoodText").GetComponent<Text>();
                SetupSubscriptions();
            });
        }

        private void SetupSubscriptions()
        {
            _playerComponent.Food.DistinctUntilChanged()
                .Subscribe(foodAmount => { _foodText.text = $"Food: {foodAmount}"; })
                .AddTo(_subscriptions);

            _eventSystem.Receive<FoodPickupEvent>()
                .Subscribe(x =>
                {
                    var foodComponent = EntityComponentAccessor.GetComponent<FoodComponent>(x.Food);
                    var foodPoints = foodComponent.FoodAmount;
                    _foodText.text = $"+{foodPoints} Food: {_playerComponent.Food.Value}";
                })
                .AddTo(_subscriptions);

            _eventSystem.Receive<PlayerHitEvent>()
                .Subscribe(x =>
                {
                    var attackScore = EntityComponentAccessor.GetComponent<EnemyComponent>(x.Enemy).EnemyPower;
                    _foodText.text = $"-{attackScore} Food: {_playerComponent.Food.Value}";
                })
                .AddTo(_subscriptions);
        }

        public void StopSystem()
        { _subscriptions.DisposeAll(); }
    }
}