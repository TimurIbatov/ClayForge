using System;
using UnityEngine;

namespace ClayForge.Events
{
    [CreateAssetMenu(menuName = "ClayForge/Events/Float Channel", fileName = "FloatChannel_")]
    public sealed class FloatEventChannel : ScriptableObject
    {
        public event Action<float> Raised;

        public void Raise(float value) => Raised?.Invoke(value);
    }
}
