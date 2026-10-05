using System;
using UnityEngine;

namespace ClayForge.Core
{
    public sealed class CenteringSystem : MonoBehaviour
    {
        [SerializeField] private ClayBody clay;
        [SerializeField] private PotteryWheel wheel;
        [SerializeField] private HandController leftHand;
        [SerializeField] private HandController rightHand;
        [SerializeField] private float wobbleScale = 0.55f;

        public event Action<float> CenteringChanged;

        public float Score { get; private set; } = 1f;

        public void Bind(ClayBody body, PotteryWheel potteryWheel, HandController left, HandController right)
        {
            clay = body;
            wheel = potteryWheel;
            leftHand = left;
            rightHand = right;
        }

        private void Update()
        {
            if (clay == null || clay.Solver == null || wheel == null)
                return;

            var offset = clay.Solver.CenterOffset;
            var error = offset.magnitude;
            var rpmFactor = Mathf.Clamp01(wheel.CurrentSpeed / 180f);
            Score = 1f - Mathf.Clamp01(error / 0.16f);
            CenteringChanged?.Invoke(Score);

            if (leftHand != null && rightHand != null && leftHand.IsPressing && rightHand.IsPressing)
            {
                var pull = (0.12f + clay.Moisture * 0.2f) * Time.deltaTime;
                clay.Solver.PullTowardCenter(pull);
            }
            else if (wheel.IsRunning && error > 0.01f)
            {
                clay.Solver.NudgeCenter(offset.normalized * (rpmFactor * 0.015f * Time.deltaTime));
            }

            if (clay.transform.parent != null)
            {
                var wobble = error * wobbleScale * (0.35f + rpmFactor);
                var angle = Time.time * (1f + wheel.CurrentSpeed * 0.04f);
                clay.transform.localPosition = new Vector3(
                    offset.x + Mathf.Cos(angle) * wobble,
                    clay.transform.localPosition.y,
                    offset.y + Mathf.Sin(angle) * wobble);
            }
        }
    }
}
