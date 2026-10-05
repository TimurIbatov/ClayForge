using ClayForge.Core.Physics;
using UnityEngine;

namespace ClayForge.Core
{
    /// <summary>
    /// Factory + quality switch for future particle/compute backends.
    /// </summary>
    public sealed class ClayPhysics : MonoBehaviour
    {
        public enum DetailLevel
        {
            Low = 0,
            High = 1
        }

        [SerializeField] private DetailLevel detail = DetailLevel.Low;

        public DetailLevel Detail => detail;

        public IClaySolver CreateSolver(int ringCount, int radialSegments)
        {
            if (detail == DetailLevel.High)
            {
                ringCount = Mathf.Max(ringCount, 40);
                radialSegments = Mathf.Max(radialSegments, 48);
            }

            return new LatheClaySolver(ringCount, radialSegments);
        }
    }
}
