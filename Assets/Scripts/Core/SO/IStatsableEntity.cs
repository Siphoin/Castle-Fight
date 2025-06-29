namespace CastleFight.Core.SO
{
    public interface IStatsableEntity<T> where T : ScriptableLiveEntity
    {
        T Stats { get; }
    }
}
