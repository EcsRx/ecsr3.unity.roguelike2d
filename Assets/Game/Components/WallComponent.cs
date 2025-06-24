using System;
using EcsR3.Components;
using R3;

namespace Game.Components
{
    public class WallComponent : IComponent, IDisposable
    {
         public ReactiveProperty<int> Health { get; set; }

        public WallComponent()
        {
            Health = new ReactiveProperty<int>();
        }

        public void Dispose()
        {
            Health.Dispose();
        }
    }
}