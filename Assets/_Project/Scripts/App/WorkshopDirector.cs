using ClayForge.Core;
using ClayForge.Data;
using ClayForge.Input;
using ClayForge.Localization;
using ClayForge.Systems;
using ClayForge.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClayForge.App
{
    /// <summary>
    /// Builds a playable workshop at runtime so the prototype works even before baked lighting/prefabs exist.
    /// </summary>
    public sealed class WorkshopDirector : MonoBehaviour
    {
        [SerializeField] private ClayType[] clayTypes;

        private PotteryWheel _wheel;
        private ClayBody _clay;
        private WorkshopCamera _cameraRig;
        private int _clayIndex;

        private void Start() => Build();

        public void NextClay()
        {
            if (clayTypes == null || clayTypes.Length == 0)
                return;
            _clayIndex = (_clayIndex + 1) % clayTypes.Length;
            ApplyClayType();
        }

        private void ApplyClayType()
        {
            if (_clay == null || clayTypes == null || clayTypes.Length == 0)
                return;
            _clay.Configure(clayTypes[_clayIndex], _clay.GetComponent<ClayPhysics>());
            _clay.ResetLump();
        }

        private void Build()
        {
            if (clayTypes == null || clayTypes.Length == 0)
                clayTypes = CreateDefaultClays();

            EnsureEventSystem();
            var lightGo = new GameObject("Sun");
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1f, 0.91f, 0.78f);
            light.intensity = 1.15f;
            lightGo.transform.rotation = Quaternion.Euler(42f, -35f, 0f);
            RenderSettings.ambientLight = new Color(0.42f, 0.36f, 0.3f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.55f, 0.48f, 0.4f);
            RenderSettings.fogDensity = 0.012f;

            var table = GameObject.CreatePrimitive(PrimitiveType.Cube);
            table.name = "Table";
            table.transform.position = new Vector3(0f, -0.38f, 0f);
            table.transform.localScale = new Vector3(3.4f, 0.28f, 2.2f);
            SetColor(table, new Color(0.38f, 0.24f, 0.14f));

            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Floor";
            floor.transform.position = new Vector3(0f, -0.72f, 0f);
            floor.transform.localScale = new Vector3(12f, 0.2f, 12f);
            SetColor(floor, new Color(0.28f, 0.22f, 0.18f));

            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "WindowWall";
            wall.transform.position = new Vector3(0f, 1.4f, 3.2f);
            wall.transform.localScale = new Vector3(10f, 4f, 0.2f);
            SetColor(wall, new Color(0.72f, 0.62f, 0.5f));

            var wheelRoot = new GameObject("PotteryWheel");
            var surface = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            surface.name = "WheelSurface";
            surface.transform.SetParent(wheelRoot.transform, false);
            surface.transform.localScale = new Vector3(1.15f, 0.07f, 1.15f);
            SetColor(surface, new Color(0.18f, 0.18f, 0.19f));

            var hub = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            hub.name = "WheelHub";
            hub.transform.SetParent(wheelRoot.transform, false);
            hub.transform.localPosition = new Vector3(0f, -0.16f, 0f);
            hub.transform.localScale = new Vector3(0.18f, 0.16f, 0.18f);
            SetColor(hub, new Color(0.12f, 0.12f, 0.12f));

            var clayAnchor = new GameObject("ClayAnchor").transform;
            clayAnchor.SetParent(surface.transform, false);
            clayAnchor.localPosition = new Vector3(0f, 1.05f, 0f);

            _wheel = wheelRoot.AddComponent<PotteryWheel>();
            _wheel.Bind(surface.transform, clayAnchor);
            _wheel.SetSpeed(90f);

            var clayGo = new GameObject("ClayBody");
            clayGo.transform.SetParent(clayAnchor, false);
            clayGo.AddComponent<MeshFilter>();
            var renderer = clayGo.AddComponent<MeshRenderer>();
            renderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
            clayGo.AddComponent<MeshCollider>();
            var physics = clayGo.AddComponent<ClayPhysics>();
            _clay = clayGo.AddComponent<ClayBody>();
            _clay.Configure(clayTypes[0], physics);

            var inputGo = new GameObject("GameplayInput");
            var input = inputGo.AddComponent<GameplayInput>();

            var left = CreateHand("LeftHand", new Color(0.85f, 0.72f, 0.58f));
            var right = CreateHand("RightHand", new Color(0.8f, 0.66f, 0.52f));
            left.Bind(HandSide.Left, _clay);
            right.Bind(HandSide.Right, _clay);

            var rig = new GameObject("HandRig").AddComponent<HandRig>();
            rig.Bind(clayGo.transform, left, right, input);

            var centering = gameObject.AddComponent<CenteringSystem>();
            centering.Bind(_clay, _wheel, left, right);

            var drying = gameObject.AddComponent<DryingSystem>();
            drying.Bind(_clay);
            var firing = gameObject.AddComponent<FiringSystem>();
            firing.Bind(_clay);
            var glazing = gameObject.AddComponent<GlazingSystem>();
            glazing.Bind(_clay);
            var orders = gameObject.AddComponent<OrderSystem>();
            orders.Bind(_clay);

            var camGo = GameObject.Find("Main Camera") ?? new GameObject("Main Camera");
            var cam = camGo.GetComponent<Camera>() ?? camGo.AddComponent<Camera>();
            if (camGo.GetComponent<AudioListener>() == null)
                camGo.AddComponent<AudioListener>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.55f, 0.47f, 0.38f);
            camGo.transform.position = new Vector3(0f, 1.15f, -1.85f);
            camGo.transform.LookAt(new Vector3(0f, 0.28f, 0f));
            _cameraRig = camGo.AddComponent<WorkshopCamera>();
            _cameraRig.Bind(clayGo.transform, cam);

            BuildHud(_wheel, centering, _clay, drying, firing, glazing, orders, _cameraRig);
            gameObject.AddComponent<WorkshopLoop>().Bind(input, _wheel, _clay, drying, _cameraRig, this);
        }

        private void BuildHud(
            PotteryWheel wheel,
            CenteringSystem centering,
            ClayBody clay,
            DryingSystem drying,
            FiringSystem firing,
            GlazingSystem glazing,
            OrderSystem orders,
            WorkshopCamera cam)
        {
            var canvasGo = new GameObject("WorkshopHUD");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasGo.AddComponent<GraphicRaycaster>();

            var wheelText = CreateText(canvasGo.transform, "WheelLabel", new Vector2(40, -30), 28, TextAnchor.UpperLeft);
            var centerText = CreateText(canvasGo.transform, "CenterLabel", new Vector2(40, -70), 24, TextAnchor.UpperLeft);
            var moisture = CreateText(canvasGo.transform, "Moisture", new Vector2(40, -108), 22, TextAnchor.UpperLeft);
            var hints = CreateText(canvasGo.transform, "Hints", new Vector2(40, 24), 18, TextAnchor.LowerLeft);
            hints.rectTransform.anchorMin = new Vector2(0f, 0f);
            hints.rectTransform.anchorMax = new Vector2(1f, 0f);
            hints.rectTransform.pivot = new Vector2(0f, 0f);
            hints.rectTransform.sizeDelta = new Vector2(-80, 70);
            var camLabel = CreateText(canvasGo.transform, "Cam", new Vector2(-40, -30), 22, TextAnchor.UpperRight);
            camLabel.rectTransform.anchorMin = new Vector2(1f, 1f);
            camLabel.rectTransform.anchorMax = new Vector2(1f, 1f);
            camLabel.rectTransform.pivot = new Vector2(1f, 1f);

            var barBg = CreateImage(canvasGo.transform, "CenterBarBg", new Vector2(40, -150), new Vector2(280, 16), new Color(0f, 0f, 0f, 0.35f));
            barBg.rectTransform.pivot = new Vector2(0f, 1f);
            var fill = CreateImage(barBg.transform, "CenterFill", Vector2.zero, Vector2.zero, Color.green);
            fill.rectTransform.anchorMin = Vector2.zero;
            fill.rectTransform.anchorMax = Vector2.one;
            fill.rectTransform.offsetMin = Vector2.zero;
            fill.rectTransform.offsetMax = Vector2.zero;
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;

            CreateButton(canvasGo.transform, LocalizationService.T("dry"), new Vector2(-40, 90), () => drying.Begin());
            CreateButton(canvasGo.transform, LocalizationService.T("fire"), new Vector2(-40, 150), () => firing.Begin());
            CreateButton(canvasGo.transform, LocalizationService.T("glaze"), new Vector2(-40, 210), () => glazing.ApplyGlaze("celadon"));
            CreateButton(canvasGo.transform, LocalizationService.T("reset"), new Vector2(-40, 270), () => clay.ResetLump());
            CreateButton(canvasGo.transform, LocalizationService.T("back"), new Vector2(-40, 330), () => UnityEngine.SceneManagement.SceneManager.LoadScene(SceneIds.MainMenu));

            var hud = canvasGo.AddComponent<WorkshopHud>();
            hud.Bind(wheel, centering, clay, drying, firing, cam, wheelText, centerText, moisture, hints, camLabel, fill);
        }

        private static HandController CreateHand(string name, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = name;
            go.transform.localScale = Vector3.one * 0.11f;
            SetColor(go, color);
            Object.Destroy(go.GetComponent<Collider>());
            return go.AddComponent<HandController>();
        }

        private static void SetColor(GameObject go, Color color)
        {
            var renderer = go.GetComponent<Renderer>();
            if (renderer == null)
                return;
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            renderer.material = new Material(shader) { color = color };
        }

        private static ClayType[] CreateDefaultClays()
        {
            return new[]
            {
                ClayType.CreateRuntime("red", "Red clay", "Красная глина", 0.78f, 0.48f, 0.08f, 1f,
                    new Color(0.55f, 0.22f, 0.12f), new Color(0.62f, 0.32f, 0.2f), new Color(0.72f, 0.38f, 0.24f)),
                ClayType.CreateRuntime("white", "White stoneware", "Белая глина", 0.7f, 0.58f, 0.1f, 0.95f,
                    new Color(0.82f, 0.78f, 0.72f), new Color(0.86f, 0.83f, 0.78f), new Color(0.92f, 0.9f, 0.86f)),
                ClayType.CreateRuntime("black", "Black clay", "Чёрная глина", 0.62f, 0.66f, 0.09f, 1.1f,
                    new Color(0.16f, 0.14f, 0.13f), new Color(0.22f, 0.2f, 0.18f), new Color(0.18f, 0.16f, 0.15f)),
                ClayType.CreateRuntime("porcelain", "Porcelain", "Фарфор", 0.88f, 0.4f, 0.14f, 0.85f,
                    new Color(0.9f, 0.9f, 0.88f), new Color(0.93f, 0.93f, 0.91f), new Color(0.96f, 0.96f, 0.95f))
            };
        }

        private static void EnsureEventSystem()
        {
            if (FindAnyObjectByType<EventSystem>() != null)
                return;
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        private static Text CreateText(Transform parent, string name, Vector2 anchored, int size, TextAnchor align)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var text = go.AddComponent<Text>();
            text.font = UiFont.Default();
            text.fontSize = size;
            text.alignment = align;
            text.color = new Color(0.95f, 0.9f, 0.82f);
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            var rt = text.rectTransform;
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = anchored;
            rt.sizeDelta = new Vector2(720, 48);
            if (align == TextAnchor.UpperRight)
            {
                rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(1f, 1f);
                rt.sizeDelta = new Vector2(420, 48);
            }

            return text;
        }

        private static Image CreateImage(Transform parent, string name, Vector2 anchored, Vector2 size, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var image = go.AddComponent<Image>();
            image.color = color;
            var rt = image.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = anchored;
            rt.sizeDelta = size;
            return image;
        }

        private static void CreateButton(Transform parent, string label, Vector2 fromTopRight, UnityEngine.Events.UnityAction action)
        {
            var go = new GameObject(label);
            go.transform.SetParent(parent, false);
            var image = go.AddComponent<Image>();
            image.color = new Color(0.18f, 0.13f, 0.1f, 0.82f);
            var button = go.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(action);
            var rt = image.rectTransform;
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(1f, 0f);
            rt.anchoredPosition = fromTopRight;
            rt.sizeDelta = new Vector2(200, 46);

            var textGo = new GameObject("Label");
            textGo.transform.SetParent(go.transform, false);
            var text = textGo.AddComponent<Text>();
            text.font = UiFont.Default();
            text.text = label;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color(0.95f, 0.9f, 0.82f);
            text.fontSize = 20;
            var trt = text.rectTransform;
            trt.anchorMin = Vector2.zero;
            trt.anchorMax = Vector2.one;
            trt.offsetMin = Vector2.zero;
            trt.offsetMax = Vector2.zero;
        }
    }

    public static class UiFont
    {
        public static Font Default()
        {
            return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
                   ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
    }
}
