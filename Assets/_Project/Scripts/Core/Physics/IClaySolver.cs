using UnityEngine;

namespace ClayForge.Core.Physics
{
    /// <summary>
    /// Swap-in deformation backend. Prototype uses a lathe profile; later particle/compute solvers implement the same contract.
    /// </summary>
    public interface IClaySolver
    {
        float Height { get; }
        float MaxRadius { get; }
        Vector2 CenterOffset { get; }
        float Moisture { get; set; }

        void ResetToLump(float radius, float height);
        void ApplyContact(in ClayContact contact, Transform clayTransform, Data.ClayType clayType);
        void NudgeCenter(Vector2 delta);
        void PullTowardCenter(float amount);
        void AddWater(float amount);
        void Shrink(float factor);
        Mesh RebuildMesh(Mesh mesh);
        ClayShapeSnapshot Capture();
        void Restore(ClayShapeSnapshot snapshot);
    }

    [System.Serializable]
    public struct ClayShapeSnapshot
    {
        public float Height;
        public float[] Radii;
        public Vector2 CenterOffset;
        public float Moisture;
        public string ClayTypeId;
    }
}
