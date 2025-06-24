using System;
using EcsR3.Components;
using R3;

namespace Game.Components
{
    public class PlayerComponent : IComponent, IDisposable
    {
        public ReactiveProperty<int> Food { get; set; }

        public PlayerComponent()
        {
            Food = new ReactiveProperty<int>();
        }

        public void Dispose()
        {
            Food.Dispose();
        }
    }
}