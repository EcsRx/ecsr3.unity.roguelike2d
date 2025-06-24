using System.Linq;
using EcsR3.Computeds.Entities;
using EcsR3.Computeds.Entities.Conventions;
using EcsR3.Entities.Accessors;
using EcsR3.Unity.Extensions;
using R3;
using UnityEngine;

namespace Game.Computeds
{
    public class ComputedPlayerPosition : ComputedFromEntityGroup<Vector3>, IComputedPlayerPosition
    {
        public IEntityComponentAccessor EntityComponentAccessor { get; }
        
        public ComputedPlayerPosition(IComputedEntityGroup dataSource, IEntityComponentAccessor entityComponentAccessor) : base(dataSource)
        {
            EntityComponentAccessor = entityComponentAccessor;
        }

        protected override Observable<Unit> RefreshWhen()
        { return Observable.EveryUpdate(); }

        protected override bool UpdateComputedData()
        {
            var player = DataSource.FirstOrDefault();
            if(player.Id == -1)
            { ComputedData = Vector3.zero; }
            
            var gameObject = EntityComponentAccessor.GetGameObject(player);
            ComputedData = gameObject.transform.position;
            return true;
        }
    }
}