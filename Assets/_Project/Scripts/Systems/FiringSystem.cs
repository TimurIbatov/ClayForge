using ClayForge.Core;
using UnityEngine;

namespace ClayForge.Systems
{
    public sealed class FiringSystem : CraftingStage
    {
        [SerializeField] private ClayBody clay;
        [SerializeField] private float duration = 8f;
        [SerializeField] private float temperature = 980f;

        public float Temperature => temperature;

        public void Bind(ClayBody body) => clay = body;

        public void SetTemperature(float value) => temperature = Mathf.Max(0f, value);

        public override void Begin()
        {
            if (clay == null)
                return;
            base.Begin();
        }

        private void Update()
        {
            if (!IsRunning)
                return;

            Progress = Mathf.Min(1f, Progress + Time.deltaTime / duration);
            if (Progress < 1f)
                return;

            var shrink = 1f - clay.ClayType.Shrinkage * (temperature > 1100f ? 1f : 0.7f);
            clay.ShrinkFromProcess(shrink);
            clay.SetProcessColor(clay.ClayType.FiredColor);
            Finish();
        }
    }
}
