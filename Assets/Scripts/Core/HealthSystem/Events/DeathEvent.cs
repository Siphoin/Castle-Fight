namespace CastleFight.Core.HealthSystem.Events
{
    public struct DeathEvent
    {
        public object Killer { get; private set; }

        public DeathEvent(object killer)
        {
            Killer = killer;
        }
    }
}
