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

namespace Game.Systems
{
    public class LevelScreenVisibilitySystem : IManualSystem, IGroupSystem
    {
        public IGroup Group { get; } = new Group(typeof(LevelComponent));
        
        [FromGroup]
        public IComputedEntityGroup LevelEntityGroup { get; set; }
        
        public IEntityComponentAccessor EntityComponentAccessor { get; }
        
        private IEventSystem _eventSystem;
        private GameObject _levelImage;
        private LevelComponent _levelComponent;
        private IList<IDisposable> _subscriptions = new List<IDisposable>();

        public LevelScreenVisibilitySystem(IEventSystem eventSystem, IEntityComponentAccessor entityComponentAccessor)
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
                    _levelImage = GameObject.Find("LevelImage");
                    SetupSubscriptions();
                });
        }

        public void SetupSubscriptions()
        {
            _levelComponent.HasLoaded.DistinctUntilChanged()
                .Subscribe(x => _levelImage.SetActive(!_levelComponent.HasLoaded.Value))
                .AddTo(_subscriptions);

            _eventSystem.Receive<PlayerKilledEvent>()
                .Subscribe(x => _levelComponent.HasLoaded.Value = false)
                .AddTo(_subscriptions);
        }

        public void StopSystem()
        { _subscriptions.DisposeAll(); }
    }
}