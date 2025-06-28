namespace CastleFight.Core.HealthSystem.Events
{
    public struct HitEvent
    {

        public float Damage { get; private set; }
        public object Damager { get; private set; }

        public HitEvent(float damage, object damager)
        {
            Damage = damage;
            Damager = damager;
        }
    }
}
