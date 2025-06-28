using System;
using Unity.Netcode;

namespace CastleFight.Core.Models
{
    public struct NetworkDateTime : INetworkSerializable, IEquatable<NetworkDateTime>
    {
        private byte[] _data;
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);


        public NetworkDateTime(DateTime dateTime)
        {
            _data = ConvertToBytes(dateTime);
        }


        public DateTime DateTime
        {
            get => ConvertFromBytes(_data);
            set => _data = ConvertToBytes(value);
        }

        public bool Equals(NetworkDateTime other)
        {
            if (_data == null || other._data == null)
                return false;

            if (_data.Length != other._data.Length)
                return false;

            for (int i = 0; i < _data.Length; i++)
            {
                if (_data[i] != other._data[i])
                    return false;
            }
            return true;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _data);
        }

        public override bool Equals(object obj)
        {
            return obj is NetworkDateTime other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _data != null ? BitConverter.ToInt32(_data, 0) : 0;
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
            DateTime = DateTime.Add(timeSpan);
        }

        public void AddDays(double value)
        {
            DateTime = DateTime.AddDays(value);
        }

        public void AddHours(double value)
        {
            DateTime = DateTime.AddHours(value);
        }

        public void AddMinutes(double value)
        {
            DateTime = DateTime.AddMinutes(value);
        }

        public void AddSeconds(double value)
        {
            DateTime = DateTime.AddSeconds(value);
        }

        public void AddMilliseconds(double value)
        {
            DateTime = DateTime.AddMilliseconds(value);
        }

        public void AddTicks(long value)
        {
            DateTime = DateTime.AddTicks(value);
        }

        public void AddYears(int value)
        {
            DateTime = DateTime.AddYears(value);
        }

        public void AddMonths(int value)
        {
            DateTime = DateTime.AddMonths(value);
        }

        private static byte[] ConvertToBytes(DateTime dateTime)
        {
            TimeSpan ts = dateTime.ToUniversalTime() - Epoch;
            int secondsSinceEpoch = (int)ts.TotalSeconds;
            return BitConverter.GetBytes(secondsSinceEpoch);
        }

        private static DateTime ConvertFromBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 4)
                return DateTime.MinValue;

            int secondsSinceEpoch = BitConverter.ToInt32(bytes, 0);
            return Epoch.AddSeconds(secondsSinceEpoch).ToLocalTime();
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