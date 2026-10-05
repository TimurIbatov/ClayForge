using ClayForge.Localization;
using ClayForge.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClayForge.App
{
    public sealed class MainMenuDirector : MonoBehaviour
    {
        private void Start()
        {
            var cam = Camera.main != null ? Camera.main : new GameObject("Main Camera").AddComponent<Camera>();
            cam.tag = "MainCamera";
            cam.backgroundColor = new Color(0.22f, 0.16f, 0.12f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            if (cam.GetComponent<AudioListener>() == null)
                cam.gameObject.AddComponent<AudioListener>();
            if (FindAnyObjectByType<EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
            }

            var canvasGo = new GameObject("Menu");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasGo.AddComponent<GraphicRaycaster>();

            var title = CreateLabel(canvasGo.transform, 64, new Vector2(0, 220));
            var subtitle = CreateLabel(canvasGo.transform, 28, new Vector2(0, 150));
            var play = CreateMenuButton(canvasGo.transform, new Vector2(0, 40));
            var gallery = CreateMenuButton(canvasGo.transform, new Vector2(0, -30));
            var quit = CreateMenuButton(canvasGo.transform, new Vector2(0, -100));
            var lang = CreateMenuButton(canvasGo.transform, new Vector2(0, -180), 160);

            var menu = canvasGo.AddComponent<MainMenuController>();
            menu.Bind(title, subtitle, play.GetComponentInChildren<Text>(), gallery.GetComponentInChildren<Text>(),
                quit.GetComponentInChildren<Text>(), lang.GetComponentInChildren<Text>());
            play.onClick.AddListener(menu.OpenWorkshop);
            gallery.onClick.AddListener(menu.OpenGallery);
            quit.onClick.AddListener(menu.Quit);
            lang.onClick.AddListener(menu.ToggleLanguage);
        }

        private static Text CreateLabel(Transform parent, int size, Vector2 pos)
        {
            var go = new GameObject("Label");
            go.transform.SetParent(parent, false);
            var text = go.AddComponent<Text>();
            text.font = UiFont.Default();
            text.fontSize = size;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color(0.95f, 0.88f, 0.76f);
            var rt = text.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(900, 90);
            rt.anchoredPosition = pos;
            return text;
        }

        private static Button CreateMenuButton(Transform parent, Vector2 pos, float width = 280)
        {
            var go = new GameObject("Button");
            go.transform.SetParent(parent, false);
            var image = go.AddComponent<Image>();
            image.color = new Color(0.32f, 0.18f, 0.12f, 0.92f);
            var button = go.AddComponent<Button>();
            button.targetGraphic = image;
            var rt = image.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(width, 56);
            rt.anchoredPosition = pos;
            var labelGo = new GameObject("Text");
            labelGo.transform.SetParent(go.transform, false);
            var text = labelGo.AddComponent<Text>();
            text.font = UiFont.Default();
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = 24;
            text.color = new Color(0.96f, 0.9f, 0.8f);
            var trt = text.rectTransform;
            trt.anchorMin = Vector2.zero;
            trt.anchorMax = Vector2.one;
            trt.offsetMin = Vector2.zero;
            trt.offsetMax = Vector2.zero;
            return button;
        }
    }
}
