using EcsR3.Components;
using UnityEngine;

namespace Game.Components
{
    public class StandardInputComponent : IComponent
    {
        public Vector2 PendingMovement { get; set; }
    }
}