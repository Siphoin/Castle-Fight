using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CastleFight.Main
{
    public abstract class ScriptableObjectIdentity : ScriptableObject, IIdentity
    {
        [SerializeField, ReadOnly] private string _guidObject = Guid.NewGuid().ToString();

        public string GUID => _guidObject;
    }
}
