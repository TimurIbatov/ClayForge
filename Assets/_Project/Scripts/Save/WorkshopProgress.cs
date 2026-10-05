using System;
using UnityEngine;
using ClayForge.Systems;

namespace ClayForge.Save
{
    [Serializable]
    public sealed class WorkshopProgressData
    {
        public int coins = 120;
        public int softClay = 8;
        public int firewood = 4;
        public int pigment = 2;
        public int glaze = 1;
        public int finishedPieces;
        public string selectedClay = "red";
    }

    public sealed class WorkshopProgress : MonoBehaviour
    {
        [SerializeField] private WorkshopEconomy economy;
        public WorkshopProgressData Data { get; private set; } = new();
        private const string SaveKey = "clayforge.progress.v1";

        private void Start() => Load();

        public void Save()
        {
            Data.coins = economy.Get(ResourceKind.Coins);
            Data.softClay = economy.Get(ResourceKind.SoftClay);
            Data.firewood = economy.Get(ResourceKind.Firewood);
            Data.pigment = economy.Get(ResourceKind.Pigment);
            Data.glaze = economy.Get(ResourceKind.Glaze);
            PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(Data));
            PlayerPrefs.Save();
        }

        public void Load()
        {
            if (!PlayerPrefs.HasKey(SaveKey)) return;
            Data = JsonUtility.FromJson<WorkshopProgressData>(PlayerPrefs.GetString(SaveKey)) ?? new WorkshopProgressData();
            economy.Restore(ResourceKind.Coins, Data.coins);
            economy.Restore(ResourceKind.SoftClay, Data.softClay);
            economy.Restore(ResourceKind.Firewood, Data.firewood);
            economy.Restore(ResourceKind.Pigment, Data.pigment);
            economy.Restore(ResourceKind.Glaze, Data.glaze);
        }

        private void OnApplicationPause(bool pause) { if (pause) Save(); }
        private void OnApplicationQuit() => Save();
    }
}
