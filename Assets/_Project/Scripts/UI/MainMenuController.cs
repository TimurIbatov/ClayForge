using ClayForge.App;
using ClayForge.Localization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ClayForge.UI
{
    public sealed class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Text title;
        [SerializeField] private Text subtitle;
        [SerializeField] private Text playLabel;
        [SerializeField] private Text galleryLabel;
        [SerializeField] private Text quitLabel;
        [SerializeField] private Text langLabel;

        public void Bind(Text titleText, Text subtitleText, Text play, Text gallery, Text quit, Text lang)
        {
            title = titleText;
            subtitle = subtitleText;
            playLabel = play;
            galleryLabel = gallery;
            quitLabel = quit;
            langLabel = lang;
            Refresh();
        }

        private void Update() => Refresh();

        public void OpenWorkshop() => SceneManager.LoadScene(SceneIds.Workshop);

        public void OpenGallery() => SceneManager.LoadScene(SceneIds.Gallery);

        public void Quit() => Application.Quit();

        public void ToggleLanguage()
        {
            var loc = FindAnyObjectByType<LocalizationService>();
            loc?.Toggle();
            Refresh();
        }

        private void Refresh()
        {
            if (title != null) title.text = LocalizationService.T("title");
            if (subtitle != null) subtitle.text = LocalizationService.T("subtitle");
            if (playLabel != null) playLabel.text = LocalizationService.T("play");
            if (galleryLabel != null) galleryLabel.text = LocalizationService.T("gallery");
            if (quitLabel != null) quitLabel.text = LocalizationService.T("quit");
            if (langLabel != null) langLabel.text = LocalizationService.T("lang");
        }
    }
}
