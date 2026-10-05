using UnityEngine;

namespace ClayForge.Input
{
    public sealed class WorkshopControls : MonoBehaviour
    {
        [SerializeField] private Core.PotteryWheel wheel;
        [SerializeField] private Core.WorkshopCamera workshopCamera;
        [SerializeField] private Core.ClayBody clay;

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space)) wheel?.Toggle();
            if (UnityEngine.Input.GetKey(KeyCode.LeftShift) && UnityEngine.Input.GetKeyDown(KeyCode.S)) Save();
            if (UnityEngine.Input.GetKeyDown(KeyCode.F)) clay?.AddWater(0.12f);
            if (UnityEngine.Input.GetKeyDown(KeyCode.R)) clay?.ResetLump();
            if (UnityEngine.Input.GetKeyDown(KeyCode.V)) workshopCamera?.Cycle();
        }

        private static void Save() => FindFirstObjectByType<Save.WorkshopProgress>()?.Save();
    }
}
