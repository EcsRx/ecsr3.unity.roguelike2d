using EcsR3.Entities;

namespace Game.Events
{
    public class ExitReachedEvent
    {
        public Entity Exit { get; private set; }
        public Entity Player { get; private set; }

        public ExitReachedEvent(Entity exit, Entity player)
        {
            Exit = exit;
            Player = player;
        }
    }
}