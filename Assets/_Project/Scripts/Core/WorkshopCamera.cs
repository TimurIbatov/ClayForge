using ClayForge.Localization;
using UnityEngine;

namespace ClayForge.Core
{
    public sealed class WorkshopCamera : MonoBehaviour
    {
        public enum Mode
        {
            Fixed = 0,
            Orbit = 1,
            Hands = 2
        }

        [SerializeField] private Transform target;
        [SerializeField] private Mode mode = Mode.Fixed;

        private Vector3 _fixedPos;
        private Quaternion _fixedRot;

        public void Bind(Transform lookTarget, Camera cam)
        {
            target = lookTarget;
            _fixedPos = cam.transform.position;
            _fixedRot = cam.transform.rotation;
        }

        public void Cycle()
        {
            mode = (Mode)(((int)mode + 1) % 3);
        }

        public string ModeLabel => LocalizationService.T(mode.ToString());

        private void LateUpdate()
        {
            if (target == null)
                return;

            switch (mode)
            {
                case Mode.Orbit:
                    var t = Time.time * 0.15f;
                    transform.position = target.position + new Vector3(Mathf.Sin(t) * 3.2f, 1.6f, Mathf.Cos(t) * 3.2f);
                    transform.LookAt(target.position + Vector3.up * 0.25f);
                    break;
                case Mode.Hands:
                    transform.position = target.position + new Vector3(0.15f, 1.2f, -2.2f);
                    transform.LookAt(target.position + Vector3.up * 0.2f);
                    break;
                default:
                    transform.SetPositionAndRotation(_fixedPos, _fixedRot);
                    break;
            }
        }
    }
}
