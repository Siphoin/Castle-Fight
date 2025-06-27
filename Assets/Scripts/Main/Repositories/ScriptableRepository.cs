using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CastleFight.Main.Repositories
{
    public abstract class ScriptableRepository : ScriptableObjectIdentity
    {
    }

    public abstract class ScriptableRepository<T> : ScriptableRepository, IEnumerable<T>, ICollection<T>
    {
        [SerializeField] private List<T> _elements = new();

        public int Count => _elements.Count;

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            _elements.Add(item);
        }

        public void AddRange (IEnumerable<T> items)
        {
            _elements.AddRange(items);
        }

        public void Clear()
        {
            _elements.Clear();
        }

        public bool Contains(T item)
        {
            return _elements.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _elements.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        public bool Remove(T item)
        {
            return _elements.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

    }
}