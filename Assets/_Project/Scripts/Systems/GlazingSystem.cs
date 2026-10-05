using ClayForge.Core;
using UnityEngine;

namespace ClayForge.Systems
{
    public sealed class GlazingSystem : CraftingStage
    {
        [SerializeField] private ClayBody clay;

        public void Bind(ClayBody body) => clay = body;

        public void ApplyGlaze(string glazeId)
        {
            if (clay == null)
                return;

            Begin();
            var gloss = glazeId == "ash" ? new Color(0.35f, 0.42f, 0.32f) : new Color(0.22f, 0.28f, 0.45f);
            clay.SetProcessColor(Color.Lerp(clay.ClayType.FiredColor, gloss, 0.65f));
            Finish();
        }
    }
}
