using ClayForge.App;
using ClayForge.Core;
using ClayForge.Localization;
using ClayForge.Systems;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ClayForge.UI
{
    public sealed class WorkshopHud : MonoBehaviour
    {
        [SerializeField] private PotteryWheel wheel;
        [SerializeField] private CenteringSystem centering;
        [SerializeField] private ClayBody clay;
        [SerializeField] private DryingSystem drying;
        [SerializeField] private FiringSystem firing;
        [SerializeField] private Text wheelLabel;
        [SerializeField] private Text centeringLabel;
        [SerializeField] private Text moistureLabel;
        [SerializeField] private Text hintsLabel;
        [SerializeField] private Text cameraLabel;
        [SerializeField] private Image centeringFill;
        [SerializeField] private WorkshopCamera workshopCamera;

        public void Bind(
            PotteryWheel potteryWheel,
            CenteringSystem centeringSystem,
            ClayBody clayBody,
            DryingSystem dryingSystem,
            FiringSystem firingSystem,
            WorkshopCamera cam,
            Text wheelText,
            Text centeringText,
            Text moistureText,
            Text hints,
            Text camLabel,
            Image fill)
        {
            wheel = potteryWheel;
            centering = centeringSystem;
            clay = clayBody;
            drying = dryingSystem;
            firing = firingSystem;
            workshopCamera = cam;
            wheelLabel = wheelText;
            centeringLabel = centeringText;
            moistureLabel = moistureText;
            hintsLabel = hints;
            cameraLabel = camLabel;
            centeringFill = fill;
        }

        private void Update()
        {
            if (wheel != null && wheelLabel != null)
                wheelLabel.text = $"{LocalizationService.T("wheel")}  {wheel.CurrentSpeed:0} rpm";

            if (centering != null && centeringLabel != null)
                centeringLabel.text = $"{LocalizationService.T("centering")}  {centering.Score * 100f:0}%";

            if (centeringFill != null && centering != null)
            {
                centeringFill.fillAmount = centering.Score;
                centeringFill.color = Color.Lerp(new Color(0.75f, 0.25f, 0.18f), new Color(0.45f, 0.72f, 0.38f), centering.Score);
            }

            if (clay != null && moistureLabel != null)
                moistureLabel.text = $"{LocalizationService.T("moisture")}  {clay.Moisture * 100f:0}%";

            if (hintsLabel != null)
                hintsLabel.text = LocalizationService.T("hints");

            if (cameraLabel != null && workshopCamera != null)
                cameraLabel.text = workshopCamera.ModeLabel;
        }

        public void BackToMenu() => SceneManager.LoadScene(SceneIds.MainMenu);
    }
}
