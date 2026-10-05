using System;
using UnityEngine;

namespace ClayForge.Systems
{
    public abstract class CraftingStage : MonoBehaviour
    {
        public event Action Started;
        public event Action Completed;

        public bool IsRunning { get; protected set; }
        public float Progress { get; protected set; }

        public virtual void Begin()
        {
            IsRunning = true;
            Progress = 0f;
            Started?.Invoke();
        }

        protected void Finish()
        {
            IsRunning = false;
            Progress = 1f;
            Completed?.Invoke();
        }
    }
}
