using ClayForge.App;
using ClayForge.Localization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ClayForge.UI
{
    public sealed class GalleryController : MonoBehaviour
    {
        [SerializeField] private Text body;

        public void Bind(Text bodyText) => body = bodyText;

        private void Update()
        {
            if (body != null)
                body.text = LocalizationService.T("galleryEmpty");
        }

        public void Back() => SceneManager.LoadScene(SceneIds.MainMenu);
    }
}
