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
using SystemsR3.Events;
using SystemsR3.Extensions;
using SystemsR3.Systems.Conventional;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Systems
{
    public class LevelTextUpdateSystem : IManualSystem, IGroupSystem
    {
        public IGroup Group { get; } = new Group(typeof(LevelComponent));

        [FromGroup]
        public IComputedEntityGroup LevelEntityGroup { get; set; }
        
        public IEntityComponentAccessor EntityComponentAccessor { get; }
        
        private Text _levelText;
        private LevelComponent _levelComponent;
        private readonly IEventSystem _eventSystem;
        private readonly IList<IDisposable> _subscriptions = new List<IDisposable>();

        public LevelTextUpdateSystem(IEventSystem eventSystem, IEntityComponentAccessor entityComponentAccessor)
        {
            _eventSystem = eventSystem;
            EntityComponentAccessor = entityComponentAccessor;
        }

        public void StartSystem()
        {
            this.WaitForScene()
                .SubscribeOnce(x =>
                {
                    var level = LevelEntityGroup.First();
                    _levelComponent = EntityComponentAccessor.GetComponent<LevelComponent>(level);
                    _levelText = GameObject.Find("LevelText").GetComponent<Text>();
                    SetupSubscriptions();
                });
        }

        private void SetupSubscriptions()
        {
            _levelComponent.Level.DistinctUntilChanged()
                .Subscribe(levelNumber => _levelText.text = $"Day {levelNumber}")
                .AddTo(_subscriptions);

            _eventSystem.Receive<PlayerKilledEvent>()
                .Subscribe(eventData => _levelText.text = $"After {_levelComponent.Level.Value} days, you starved.")
                .AddTo(_subscriptions);
        }

        public void StopSystem()
        { _subscriptions.DisposeAll(); }
    }
}