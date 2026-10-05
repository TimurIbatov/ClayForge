using UnityEngine;

namespace ClayForge.Core.Physics
{
    public struct ClayContact
    {
        public Vector3 WorldPoint;
        public Vector3 WorldNormal;
        public Vector3 WorldVelocity;
        public float Pressure;
        public float InfluenceRadius;
        public ClayToolMode Mode;
        public bool FromLeftHand;
    }
}
