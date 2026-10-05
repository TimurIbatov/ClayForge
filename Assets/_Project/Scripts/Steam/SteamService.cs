using UnityEngine;

namespace ClayForge.Steam
{
    public interface ISteamPlatform
    {
        void UnlockAchievement(string id);
        void SaveToCloud(string key, byte[] data);
        byte[] LoadFromCloud(string key);
    }

    public sealed class NullSteamPlatform : ISteamPlatform
    {
        public void UnlockAchievement(string id) =>
            Debug.Log($"[Steam stub] achievement {id}");

        public void SaveToCloud(string key, byte[] data) =>
            Debug.Log($"[Steam stub] cloud save {key} ({data?.Length ?? 0} bytes)");

        public byte[] LoadFromCloud(string key) => null;
    }

    /// <summary>
    /// Drop in Steamworks.NET and compile with STEAMWORKS_NET to activate.
    /// </summary>
    public sealed class SteamworksNetPlatform : ISteamPlatform
    {
        public void UnlockAchievement(string id)
        {
#if STEAMWORKS_NET
            // SteamUserStats.SetAchievement(id); SteamUserStats.StoreStats();
#endif
            Debug.Log($"[Steamworks.NET seam] UnlockAchievement({id})");
        }

        public void SaveToCloud(string key, byte[] data)
        {
#if STEAMWORKS_NET
            // SteamRemoteStorage.FileWrite(key, data, data.Length);
#endif
            Debug.Log($"[Steamworks.NET seam] SaveToCloud({key})");
        }

        public byte[] LoadFromCloud(string key)
        {
#if STEAMWORKS_NET
            // return SteamRemoteStorage.FileRead...
#endif
            return null;
        }
    }

    public sealed class SteamService : MonoBehaviour, ISteamPlatform
    {
        public static SteamService Instance { get; private set; }

        [SerializeField] private bool useSteamworksWhenAvailable;

        private ISteamPlatform _platform;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            _platform = useSteamworksWhenAvailable ? new SteamworksNetPlatform() : new NullSteamPlatform();
        }

        public void UnlockAchievement(string id) => _platform?.UnlockAchievement(id);

        public void SaveToCloud(string key, byte[] data) => _platform?.SaveToCloud(key, data);

        public byte[] LoadFromCloud(string key) => _platform?.LoadFromCloud(key);
    }
}
