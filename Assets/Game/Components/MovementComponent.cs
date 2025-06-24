using System;
using EcsR3.Components;
using R3;
using UnityEngine;

namespace Game.Components
{
    public class MovementComponent : IComponent, IDisposable
    {
        public ReactiveProperty<Vector2> Movement { get; set; }
        public bool StopMovement { get; set; }

        public MovementComponent()
        {
            Movement = new ReactiveProperty<Vector2>();
            StopMovement = false;
        }

        public void Dispose()
        {
            Movement.Dispose();
        }
    }
}