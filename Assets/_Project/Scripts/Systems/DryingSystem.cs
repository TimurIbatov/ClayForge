using ClayForge.Core;
using UnityEngine;

namespace ClayForge.Systems
{
    public sealed class DryingSystem : CraftingStage
    {
        [SerializeField] private ClayBody clay;
        [SerializeField] private float duration = 6f;

        public void Bind(ClayBody body) => clay = body;

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
            clay.AddWater(-Time.deltaTime * 0.12f);
            if (Progress < 1f)
                return;

            clay.ShrinkFromProcess(1f - clay.ClayType.Shrinkage * 0.35f);
            clay.SetProcessColor(clay.ClayType.LeatherHardColor);
            Finish();
        }
    }
}
