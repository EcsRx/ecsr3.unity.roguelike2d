using EcsR3.Components;
using UnityEngine;

namespace Game.Components
{
    public class RandomlyPlacedComponent : IComponent
    {
        public Vector3 RandomPosition { get; set; }
    }
}