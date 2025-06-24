using System;
using EcsR3.Components;
using R3;

namespace Game.Components
{
    public class LevelComponent : IComponent, IDisposable
    {
        public ReactiveProperty<bool> HasLoaded { get; set; } 
        public ReactiveProperty<int> Level { get; set; }
        public int WallCount { get; set; }
        public int FoodCount { get; set; }
        public int EnemyCount { get; set; }

        public LevelComponent()
        {
            HasLoaded = new ReactiveProperty<bool>(false);
            Level = new ReactiveProperty<int>(1);
        }

        public void Dispose()
        {
            HasLoaded.Dispose();
            Level.Dispose();
        }
    }
}