using ClayForge.Core.Physics;
using UnityEngine;

namespace ClayForge.Input
{
    /// <summary>
    /// Prototype input: mouse orbits both hands around the clay, buttons apply pressure.
    /// </summary>
    public sealed class GameplayInput : MonoBehaviour
    {
        public float OrbitAngle { get; private set; }
        public float Height01 { get; private set; } = 0.45f;
        public Vector2 Nudge { get; private set; }
        public bool LeftPress { get; private set; }
        public bool RightPress { get; private set; }
        public bool Strong { get; private set; }
        public bool Fine { get; private set; }
        public bool ToggleWheel { get; private set; }
        public bool AddWater { get; private set; }
        public bool CutMode { get; private set; }
        public float Scroll { get; private set; }

        public ClayToolMode ToolMode => CutMode ? ClayToolMode.Cut : ClayToolMode.Press;

        private void Update()
        {
            ToggleWheel = UnityEngine.Input.GetKeyDown(KeyCode.Space);
            AddWater = UnityEngine.Input.GetKeyDown(KeyCode.F);
            CutMode = UnityEngine.Input.GetKey(KeyCode.C);
            Strong = UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift);
            Fine = UnityEngine.Input.GetKey(KeyCode.LeftControl) || UnityEngine.Input.GetKey(KeyCode.RightControl);
            LeftPress = UnityEngine.Input.GetMouseButton(0);
            RightPress = UnityEngine.Input.GetMouseButton(1);
            Scroll = UnityEngine.Input.mouseScrollDelta.y;
            Nudge = new Vector2(
                UnityEngine.Input.GetAxisRaw("Horizontal"),
                UnityEngine.Input.GetAxisRaw("Vertical"));

            if (UnityEngine.Input.GetKey(KeyCode.Q))
                Height01 -= Time.deltaTime * 0.55f;
            if (UnityEngine.Input.GetKey(KeyCode.E))
                Height01 += Time.deltaTime * 0.55f;

            var mouse = UnityEngine.Input.mousePosition;
            OrbitAngle = (mouse.x / Mathf.Max(1f, Screen.width) - 0.5f) * 220f;
            Height01 = Mathf.Clamp01(Mathf.Lerp(Height01, mouse.y / Mathf.Max(1f, Screen.height), 8f * Time.deltaTime));
        }
    }
}
