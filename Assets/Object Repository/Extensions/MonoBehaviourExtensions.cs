using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
namespace ObjectRepositories.Extensions
{
    public static class MonoBehaviourExtensions
    {
        public static IEnumerable<T> FindObjectsOfTypeOnRepository<T> (this MonoBehaviour _)
        {
            var repository = ObjectRepository.GetInstance<T>();
            return repository;
        }

        public static T FindFirstObjectOfTypeOnRepository<T>(this MonoBehaviour _)
        {
            return FindObjectsOfTypeOnRepository<T>(_).FirstOrDefault();
        }


        public static T FindLastObjectOfTypeOnRepository<T>(this MonoBehaviour _)
        {
            return FindObjectsOfTypeOnRepository<T>(_).LastOrDefault();
        }

        public static IEnumerable<T> FindObjectsByNameOnRepository<T>(this MonoBehaviour _, string name) where T : Component
        {
            return FindObjectsOfTypeOnRepository<T>(_).Where(x => x.name == name);
        }

        public static IEnumerable<T> FindObjectsByTagOnRepository<T>(this MonoBehaviour _, string tag) where T : Component
        {
            return FindObjectsOfTypeOnRepository<T>(_).Where(x => x.tag == tag);
        }

        public static T FindByConditionOnRepository<T> (this MonoBehaviour _, Func<T, bool> condition)
        {
            var repository = ObjectRepository.GetInstance<T>();
            return repository.FirstOrDefault(condition);
        }

        public static IEnumerable<T> FindManyByConditionOnRepository<T>(this MonoBehaviour _, Func<T, bool> condition)
        {
            var repository = ObjectRepository.GetInstance<T>();
            return repository.Where(condition);
        }
    }
}
