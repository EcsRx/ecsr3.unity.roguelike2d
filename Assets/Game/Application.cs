using System;
using EcsR3.Extensions;
using EcsR3.Collections.Entities;
using EcsR3.Plugins.GroupBinding;
using EcsR3.Plugins.Views.Components;
using EcsR3.Zenject;
using Game.Blueprints;
using Game.Components;
using Game.Configuration;
using Game.Events;
using Game.Modules;
using R3;
using SystemsR3.Extensions;
using SystemsR3.Infrastructure.Extensions;
using UnityEngine;
using Zenject;

namespace Game
{
    public class Application : EcsR3ApplicationBehaviour
    {
        [Inject]
        private GameConfiguration _gameConfiguration;
        
        protected override void LoadModules()
        {
            base.LoadModules();
            DependencyRegistry.LoadModule<GameModule>();
            DependencyRegistry.LoadModule<SceneCollectionsModule>();
            DependencyRegistry.LoadModule<ComputedModule>();
        }

        protected override void LoadPlugins()
        {
            base.LoadPlugins();
            RegisterPlugin(new GroupBindingsPlugin());
        }

        protected override void ApplicationStarted()
        {
            var levelBlueprint = new LevelBlueprint();
            var levelEntity = EntityCollection.Create(EntityComponentAccessor, levelBlueprint);
            var player = EntityCollection.Create(EntityComponentAccessor, new PlayerBlueprint(_gameConfiguration.StartingFoodPoints));
            var playerView = EntityComponentAccessor.GetComponent<ViewComponent>(player);
            var playerComponent = EntityComponentAccessor.GetComponent<PlayerComponent>(player);
            var levelComponent = EntityComponentAccessor.GetComponent<LevelComponent>(levelEntity);

            levelComponent.Level.DistinctUntilChanged()
                .Subscribe(x =>
                {
                    var gameObject = playerView.View as GameObject;
                    gameObject.transform.position = Vector3.zero;
                    SetupLevel(levelComponent);
                });

            EventSystem.Receive<PlayerKilledEvent>()
                .Delay(TimeSpan.FromSeconds(_gameConfiguration.IntroLength))
                .Subscribe(x =>
                {
                    levelBlueprint.UpdateLevel(levelComponent, 1);
                    playerComponent.Food.Value = _gameConfiguration.StartingFoodPoints;
                    SetupLevel(levelComponent);
                });
        }

        private void SetupLevel(LevelComponent levelComponent)
        {
            levelComponent.HasLoaded.Value = false;

            EntityCollection.RemoveEntitiesContaining(EntityComponentAccessor, typeof(GameBoardComponent),
                typeof(FoodComponent), typeof(WallComponent),
                typeof(EnemyComponent), typeof(ExitComponent));

            Observable.Interval(TimeSpan.FromSeconds(_gameConfiguration.IntroLength))
                .SubscribeOnce(x => levelComponent.HasLoaded.Value = true);
            
            EntityCollection.Create(EntityComponentAccessor, new GameBoardBlueprint());

            for (var i = 0; i < levelComponent.FoodCount; i++)
            { EntityCollection.Create(EntityComponentAccessor, new FoodBlueprint()); }

            for (var i = 0; i < levelComponent.WallCount; i++)
            { EntityCollection.Create(EntityComponentAccessor, new WallBlueprint()); }

            for (var i = 0; i < levelComponent.EnemyCount; i++)
            { EntityCollection.Create(EntityComponentAccessor, new EnemyBlueprint()); }

            EntityCollection.Create(EntityComponentAccessor, new ExitBlueprint());
        }
    }
}
