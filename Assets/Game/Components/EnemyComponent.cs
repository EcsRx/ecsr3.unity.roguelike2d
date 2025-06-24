using System;
using EcsR3.Components;
using Game.Enums;
using R3;

namespace Game.Components
{
    public class EnemyComponent : IComponent, IDisposable
    {
        public ReactiveProperty<int> Health { get; set; }
        public EnemyTypes EnemyType { get; set; }
        public int EnemyPower { get; set; }
        public bool IsSkippingNextTurn { get; set; }

        public EnemyComponent()
        {
            Health = new ReactiveProperty<int>();
        }

        public void Dispose()
        {
            Health.Dispose();
        }
    }
}