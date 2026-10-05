using ClayForge.Core.Physics;
using UnityEngine;

namespace ClayForge.Core
{
    public sealed class HandController : MonoBehaviour
    {
        [SerializeField] private HandSide side;
        [SerializeField] private ClayBody target;
        [SerializeField] private float basePressure = 0.55f;
        [SerializeField] private float influenceRadius = 0.16f;

        private Vector3 _previousWorld;
        private bool _hasPrev;

        public HandSide Side => side;
        public bool IsPressing { get; private set; }
        public Vector3 WorldVelocity { get; private set; }

        public void Bind(HandSide handSide, ClayBody clay)
        {
            side = handSide;
            target = clay;
        }

        public void SetTarget(ClayBody clay) => target = clay;

        public void Tick(bool pressing, bool strong, bool fine, ClayToolMode mode)
        {
            var now = transform.position;
            WorldVelocity = _hasPrev ? (now - _previousWorld) / Mathf.Max(Time.deltaTime, 0.0001f) : Vector3.zero;
            _previousWorld = now;
            _hasPrev = true;
            IsPressing = pressing;
            if (!pressing || target == null)
                return;

            var pressure = basePressure;
            if (strong)
                pressure *= 2.1f;
            if (fine)
                pressure *= 0.35f;

            var influence = fine ? influenceRadius * 0.55f : influenceRadius;
            var towardAxis = -Vector3.ProjectOnPlane(transform.position - target.transform.position, Vector3.up).normalized;
            if (towardAxis.sqrMagnitude < 0.01f)
                towardAxis = side == HandSide.Left ? Vector3.right : Vector3.left;

            var contact = new ClayContact
            {
                WorldPoint = transform.position,
                WorldNormal = towardAxis,
                WorldVelocity = WorldVelocity,
                Pressure = pressure,
                InfluenceRadius = influence,
                Mode = mode,
                FromLeftHand = side == HandSide.Left
            };
            target.ApplyContact(contact);
        }
    }
}
