using ClayForge.Core;
using ClayForge.Input;
using UnityEngine;

namespace ClayForge.Core
{
    public sealed class HandRig : MonoBehaviour
    {
        [SerializeField] private Transform clay;
        [SerializeField] private HandController left;
        [SerializeField] private HandController right;
        [SerializeField] private GameplayInput input;
        [SerializeField] private float orbitRadius = 0.34f;

        public void Bind(Transform clayTransform, HandController leftHand, HandController rightHand, GameplayInput gameplayInput)
        {
            clay = clayTransform;
            left = leftHand;
            right = rightHand;
            input = gameplayInput;
        }

        private void LateUpdate()
        {
            if (clay == null || input == null)
                return;

            var height = Mathf.Lerp(0.04f, Mathf.Max(0.18f, clay.GetComponent<ClayBody>()?.Height ?? 0.4f), input.Height01);
            var origin = clay.position;
            var angle = input.OrbitAngle * Mathf.Deg2Rad;
            var nudge = new Vector3(input.Nudge.x, 0f, input.Nudge.y) * 0.12f;
            Place(left, origin, height, angle, orbitRadius, nudge);
            Place(right, origin, height, angle + Mathf.PI, orbitRadius, -nudge);

            left.Tick(input.LeftPress, input.Strong, input.Fine, input.ToolMode);
            right.Tick(input.RightPress, input.Strong, input.Fine, input.ToolMode);
        }

        private static void Place(HandController hand, Vector3 origin, float height, float angle, float radius, Vector3 nudge)
        {
            if (hand == null)
                return;

            var pos = origin + new Vector3(Mathf.Cos(angle) * radius, height - origin.y + origin.y, Mathf.Sin(angle) * radius);
            pos.y = origin.y + height;
            hand.transform.position = pos + nudge;
        }
    }
}
