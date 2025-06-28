using System;
using Unity.Netcode;

namespace CastleFight.Core.Models
{
    public struct NetworkDateTime : INetworkSerializable, IEquatable<NetworkDateTime>
    {
        private long _ticks;
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public NetworkDateTime(DateTime dateTime)
        {
            _ticks = dateTime.ToUniversalTime().Ticks - Epoch.Ticks;
        }

        public DateTime DateTime
        {
            get => new DateTime(Epoch.Ticks + _ticks, DateTimeKind.Utc).ToLocalTime();
            set => _ticks = value.ToUniversalTime().Ticks - Epoch.Ticks;
        }

        public bool Equals(NetworkDateTime other)
        {
            return _ticks == other._ticks;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _ticks);
        }

        public override bool Equals(object obj)
        {
            return obj is NetworkDateTime other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _ticks.GetHashCode();
        }

        public static bool operator ==(NetworkDateTime left, NetworkDateTime right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(NetworkDateTime left, NetworkDateTime right)
        {
            return !(left == right);
        }

        public void Add(TimeSpan timeSpan)
        {
            _ticks += timeSpan.Ticks;
        }

        public void AddDays(double value)
        {
            _ticks += (long)(TimeSpan.TicksPerDay * value);
        }

        public void AddHours(double value)
        {
            _ticks += (long)(TimeSpan.TicksPerHour * value);
        }

        public void AddMinutes(double value)
        {
            _ticks += (long)(TimeSpan.TicksPerMinute * value);
        }

        public void AddSeconds(double value)
        {
            _ticks += (long)(TimeSpan.TicksPerSecond * value);
        }

        public void AddMilliseconds(double value)
        {
            _ticks += (long)(TimeSpan.TicksPerMillisecond * value);
        }

        public void AddTicks(long value)
        {
            _ticks += value;
        }

        public void AddYears(int value)
        {
            DateTime = DateTime.AddYears(value);
        }

        public void AddMonths(int value)
        {
            DateTime = DateTime.AddMonths(value);
        }

        public override string ToString()
        {
            return DateTime.ToString();
        }

        public string ToString(string format)
        {
            return DateTime.ToString(format);
        }

        public string ToString(IFormatProvider provider)
        {
            return DateTime.ToString(provider);
        }

        public string ToString(string format, IFormatProvider provider)
        {
            return DateTime.ToString(format, provider);
        }
    }
}