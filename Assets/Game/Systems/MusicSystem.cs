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
    public class MusicSystem : IManualSystem, IGroupSystem
    {
        public IGroup Group { get; } = new Group(typeof(LevelComponent));

        [FromGroup]
        public IComputedEntityGroup LevelEntityGroup { get; set; }
        
        public IEntityComponentAccessor EntityComponentAccessor { get; }
        
        private readonly IEventSystem _eventSystem;
        private readonly AudioSource _musicSource;
        private LevelComponent _levelComponent;
        private readonly IList<IDisposable> _subscriptions = new List<IDisposable>();

        public MusicSystem(IEventSystem eventSystem, IEntityComponentAccessor entityComponentAccessor)
        {
            var soundEffectObject = GameObject.Find("MusicSource");
            _musicSource = soundEffectObject.GetComponent<AudioSource>();
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
                    SetupSubscriptions();
                });
        }

        private void SetupSubscriptions()
        {
            _eventSystem.Receive<PlayerKilledEvent>()
                .Subscribe(x => _musicSource.Stop())
                .AddTo(_subscriptions);

            _levelComponent.HasLoaded
                .DistinctUntilChanged()
                .Subscribe(x =>
                {
                    if(!_musicSource.isPlaying)
                    { _musicSource.Play(); }
                })
                .AddTo(_subscriptions);
        }

        public void StopSystem()
        { _subscriptions.DisposeAll(); }
    }
    
}