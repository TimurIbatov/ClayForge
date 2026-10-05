using ClayForge.Core;
using ClayForge.Save;
using ClayForge.Steam;
using UnityEngine;

namespace ClayForge.Systems
{
    public sealed class OrderSystem : MonoBehaviour
    {
        [SerializeField] private ClayBody clay;

        public void Bind(ClayBody body) => clay = body;

        public void SubmitCurrentPiece()
        {
            if (clay == null)
                return;

            ClaySaveService.SaveCurrent(clay);
            SteamService.Instance?.UnlockAchievement("FIRST_PIECE");
        }
    }
}
