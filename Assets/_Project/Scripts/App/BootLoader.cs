using ClayForge.App;
using ClayForge.Localization;
using ClayForge.Steam;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClayForge.App
{
    public sealed class BootLoader : MonoBehaviour
    {
        private void Awake()
        {
            if (FindAnyObjectByType<LocalizationService>() == null)
            {
                var loc = new GameObject("Localization");
                loc.AddComponent<LocalizationService>();
            }

            if (FindAnyObjectByType<SteamService>() == null)
            {
                var steam = new GameObject("SteamService");
                steam.AddComponent<SteamService>();
            }
        }

        private void Start() => SceneManager.LoadScene(SceneIds.MainMenu);
    }
}
