using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Netcode;

namespace CastleFight.Networking.Models
{
    public struct NetworkDictionary<TKey, TValue> : INetworkSerializable, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> _dictionary;
        private string _json;

        public NetworkDictionary(Dictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary ?? new Dictionary<TKey, TValue>();
            _json = JsonConvert.SerializeObject(_dictionary);
        }

        

        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set
            {
                _dictionary[key] = value;
                ToJson();
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
            ToJson();
        }

        // Добавляем метод для удобного обновления
        public void SetValue(TKey key, TValue value)
        {
            _dictionary[key] = value;
            ToJson();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _dictionary.Add(item.Key, item.Value);
            ToJson();
        }

        public void Clear()
        {
            _dictionary.Clear();
            ToJson();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.TryGetValue(item.Key, out var value) && EqualityComparer<TValue>.Default.Equals(value, item.Value);
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
            if (result) ToJson();
            return result;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            bool result = _dictionary.Remove(item.Key);
            if (result) ToJson();
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
                serializer.SerializeValue(ref _json);
                _dictionary = JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(_json) ?? new Dictionary<TKey, TValue>();
            }
            else
            {
                ToJson();
                serializer.SerializeValue(ref _json);
            }
        }

        private void ToJson()
        {
            _json = JsonConvert.SerializeObject(_dictionary);
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