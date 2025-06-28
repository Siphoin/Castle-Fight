using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Netcode;

namespace CastleFight.Networking.Models
{
    public struct NetworkDictionary<TKey, TValue> : INetworkSerializable, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> _dictionary;

        public NetworkDictionary(Dictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary ?? new Dictionary<TKey, TValue>();
        }

        

        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set
            {
                _dictionary[key] = value;
            }
        }

        public ICollection<TKey> Keys => _dictionary.Keys;
        public ICollection<TValue> Values => _dictionary.Values;
        public int Count => _dictionary.Count;
        public bool IsReadOnly => false;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
        }
        public void SetValue(TKey key, TValue value)
        {
            _dictionary[key] = value;
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _dictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return TryGetValue(item.Key, out var value) && EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public bool Remove(TKey key)
        {
            bool result = _dictionary.Remove(key);
            return result;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            bool result = _dictionary.Remove(item.Key);
            return result;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                byte[] data = null;
                serializer.SerializeValue(ref data);
                using (var memoryStream = new MemoryStream(data))
                {
                    var binaryFormatter = new BinaryFormatter();
                    _dictionary = (Dictionary<TKey, TValue>)binaryFormatter.Deserialize(memoryStream);
                }
            }
            else
            {
                byte[] data;
                using (var memoryStream = new MemoryStream())
                {
                    var binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(memoryStream, _dictionary);
                    data = memoryStream.ToArray();
                }
                serializer.SerializeValue(ref data);
            }
        }


        public static implicit operator Dictionary<TKey, TValue>(NetworkDictionary<TKey, TValue> networkDict)
        {
            return networkDict._dictionary;
        }

        public static implicit operator NetworkDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
        {
            return new NetworkDictionary<TKey, TValue>(dictionary);
        }
    }
}