using EcsR3.Entities;

namespace Game.Events
{
    public class EnemyTurnEvent
    {
        public Entity Enemy { get; private set; }

        public EnemyTurnEvent(Entity enemy)
        {
            Enemy = enemy;
        }
    }
}