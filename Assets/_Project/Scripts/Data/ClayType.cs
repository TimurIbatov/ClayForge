using UnityEngine;

namespace ClayForge.Data
{
    [CreateAssetMenu(menuName = "ClayForge/Clay Type", fileName = "ClayType_")]
    public sealed class ClayType : ScriptableObject
    {
        [SerializeField] private string id = "red";
        [SerializeField] private string displayNameEn = "Red clay";
        [SerializeField] private string displayNameRu = "Красная глина";
        [SerializeField, Range(0.15f, 1f)] private float plasticity = 0.72f;
        [SerializeField, Range(0.1f, 1f)] private float strength = 0.5f;
        [SerializeField, Range(0f, 0.4f)] private float shrinkage = 0.08f;
        [SerializeField, Range(0.2f, 1.4f)] private float density = 1f;
        [SerializeField] private Color wetColor = new Color(0.55f, 0.22f, 0.12f);
        [SerializeField] private Color leatherHardColor = new Color(0.62f, 0.32f, 0.2f);
        [SerializeField] private Color firedColor = new Color(0.72f, 0.38f, 0.24f);

        public string Id => id;
        public float Plasticity => plasticity;
        public float Strength => strength;
        public float Shrinkage => shrinkage;
        public float Density => density;
        public Color WetColor => wetColor;
        public Color LeatherHardColor => leatherHardColor;
        public Color FiredColor => firedColor;

        public string DisplayName(bool russian) => russian ? displayNameRu : displayNameEn;

        public void Configure(
            string clayId,
            string en,
            string ru,
            float clayPlasticity,
            float clayStrength,
            float clayShrinkage,
            float clayDensity,
            Color wet,
            Color leather,
            Color fired)
        {
            id = clayId;
            displayNameEn = en;
            displayNameRu = ru;
            plasticity = clayPlasticity;
            strength = clayStrength;
            shrinkage = clayShrinkage;
            density = clayDensity;
            wetColor = wet;
            leatherHardColor = leather;
            firedColor = fired;
        }

        public static ClayType CreateRuntime(
            string clayId,
            string en,
            string ru,
            float clayPlasticity,
            float clayStrength,
            float clayShrinkage,
            float clayDensity,
            Color wet,
            Color leather,
            Color fired)
        {
            var asset = CreateInstance<ClayType>();
            asset.Configure(clayId, en, ru, clayPlasticity, clayStrength, clayShrinkage, clayDensity, wet, leather, fired);
            asset.name = "ClayType_" + clayId;
            return asset;
        }
    }
}
