using System;
using UnityEngine;

namespace ClayForge.Core
{
    public sealed class PotteryWheel : MonoBehaviour
    {
        [SerializeField] private Transform wheelSurface;
        [SerializeField] private Transform clayAnchor;
        [SerializeField] private float maxSpeed = 240f;
        [SerializeField] private float acceleration = 160f;
        [SerializeField] private float defaultSpeed = 90f;

        public event Action<float> SpeedChanged;

        public float TargetSpeed { get; private set; }
        public float CurrentSpeed { get; private set; }
        public bool IsRunning { get; private set; } = true;
        public Transform ClayAnchor => clayAnchor;
        public float MaxSpeed => maxSpeed;

        public void Bind(Transform surface, Transform anchor)
        {
            wheelSurface = surface;
            clayAnchor = anchor;
            TargetSpeed = defaultSpeed;
        }

        public void SetSpeed(float speed)
        {
            TargetSpeed = Mathf.Clamp(speed, 0f, maxSpeed);
            SpeedChanged?.Invoke(TargetSpeed);
        }

        public void AddSpeed(float delta) => SetSpeed(TargetSpeed + delta);

        public void Toggle() => IsRunning = !IsRunning;

        private void Update()
        {
            var target = IsRunning ? TargetSpeed : 0f;
            CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, target, acceleration * Time.deltaTime);
            if (wheelSurface != null)
                wheelSurface.Rotate(Vector3.up, CurrentSpeed * Time.deltaTime, Space.Self);
        }
    }
}
