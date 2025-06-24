using EcsR3.Blueprints;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using Game.Components;
using UnityEngine;

namespace Game.Blueprints
{
    public class LevelBlueprint : IBlueprint
    {
        private readonly int _minWalls = 4;
        private readonly int _maxWalls = 9;
        private readonly int _minFood = 1;
        private readonly int _maxFood = 5;
        private readonly int _level;

        public LevelBlueprint(int level = 1)
        {
            _level = level;
        }

        public void Apply(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            var levelComponent = new LevelComponent();
            UpdateLevel(levelComponent, _level);
            entityComponentAccessor.AddComponents(entity, levelComponent);
        }

        public void UpdateLevel(LevelComponent levelComponent, int level)
        {
            levelComponent.EnemyCount = (int) Mathf.Log(level, 2f);
            levelComponent.FoodCount = Random.Range(_minFood, _maxFood);
            levelComponent.WallCount = Random.Range(_minWalls, _maxWalls);
            levelComponent.Level.Value = level;
        }
    }
}