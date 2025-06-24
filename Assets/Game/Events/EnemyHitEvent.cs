using EcsR3.Entities;

namespace Game.Events
{
    public class EnemyHitEvent
    {
        public Entity Enemy { get; private set; }
        public Entity Player { get; private set; }

        public EnemyHitEvent(Entity enemy, Entity player)
        {
            Enemy = enemy;
            Player = player;
        }
    }
}