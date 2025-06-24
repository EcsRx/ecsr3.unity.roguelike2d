using EcsR3.Computeds.Entities;
using EcsR3.Entities;
using EcsR3.Entities.Accessors;
using EcsR3.Extensions;
using EcsR3.Groups;
using EcsR3.Systems.Reactive;
using Game.Components;
using R3;
using UnityEngine;

namespace Game.Systems
{
    public class StandardInputSystem : IReactToGroupSystem
    {
        public IGroup Group { get; } = new Group(typeof(MovementComponent), typeof(StandardInputComponent));

        public Observable<IComputedEntityGroup> ReactToGroup(IComputedEntityGroup group)
        {  return Observable.EveryUpdate().Select(x => group); }

        public void Process(IEntityComponentAccessor entityComponentAccessor, Entity entity)
        {
            var movementComponent = entityComponentAccessor.GetComponent<MovementComponent>(entity);
            if(movementComponent.Movement.Value != Vector2.zero) { return; }

            var horizontal = (int)(Input.GetAxisRaw("Horizontal"));
            var vertical = (int)(Input.GetAxisRaw("Vertical"));
            
            if (horizontal != 0)
            {
                vertical = 0;
            }

            if (horizontal != 0 || vertical != 0)
            {
                var inputComponent = entityComponentAccessor.GetComponent<StandardInputComponent>(entity);
                inputComponent.PendingMovement = new Vector2(horizontal, vertical);
            }
        }
    }
}