using ClayForge.Localization;
using ClayForge.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClayForge.App
{
    public sealed class GalleryDirector : MonoBehaviour
    {
        private void Start()
        {
            var cam = Camera.main != null ? Camera.main : new GameObject("Main Camera").AddComponent<Camera>();
            cam.tag = "MainCamera";
            cam.backgroundColor = new Color(0.18f, 0.15f, 0.13f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            if (FindAnyObjectByType<EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
            }

            var canvasGo = new GameObject("Gallery");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();

            var bodyGo = new GameObject("Body");
            bodyGo.transform.SetParent(canvasGo.transform, false);
            var body = bodyGo.AddComponent<Text>();
            body.font = UiFont.Default();
            body.alignment = TextAnchor.MiddleCenter;
            body.fontSize = 28;
            body.color = new Color(0.9f, 0.85f, 0.75f);
            var rt = body.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(40, 80);
            rt.offsetMax = new Vector2(-40, -80);

            var btnGo = new GameObject("Back");
            btnGo.transform.SetParent(canvasGo.transform, false);
            var img = btnGo.AddComponent<Image>();
            img.color = new Color(0.3f, 0.18f, 0.12f);
            var button = btnGo.AddComponent<Button>();
            var brt = img.rectTransform;
            brt.anchorMin = brt.anchorMax = new Vector2(0.5f, 0f);
            brt.pivot = new Vector2(0.5f, 0f);
            brt.anchoredPosition = new Vector2(0, 36);
            brt.sizeDelta = new Vector2(200, 48);
            var tGo = new GameObject("Text");
            tGo.transform.SetParent(btnGo.transform, false);
            var t = tGo.AddComponent<Text>();
            t.font = UiFont.Default();
            t.alignment = TextAnchor.MiddleCenter;
            t.fontSize = 22;
            t.color = Color.white;
            t.text = LocalizationService.T("back");
            var trt = t.rectTransform;
            trt.anchorMin = Vector2.zero;
            trt.anchorMax = Vector2.one;
            trt.offsetMin = Vector2.zero;
            trt.offsetMax = Vector2.zero;

            var gallery = canvasGo.AddComponent<GalleryController>();
            gallery.Bind(body);
            button.onClick.AddListener(gallery.Back);
        }
    }
}
