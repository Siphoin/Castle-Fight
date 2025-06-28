namespace CastleFight.Core.HealthSystem.Events
{
    public struct RegenEvent
    {
        public float Amount { get; private set; }

        public RegenEvent(float amount)
        {
            Amount = amount;
        }
    }
}
