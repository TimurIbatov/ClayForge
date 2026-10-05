using System;
using System.IO;
using ClayForge.Core;
using ClayForge.Core.Physics;
using ClayForge.Steam;
using UnityEngine;

namespace ClayForge.Save
{
    public static class ClaySaveService
    {
        private static string DirectoryPath => Path.Combine(Application.persistentDataPath, "Pieces");

        public static void SaveCurrent(ClayBody clay)
        {
            if (clay?.Solver == null)
                return;

            Directory.CreateDirectory(DirectoryPath);
            var snapshot = clay.Solver.Capture();
            snapshot.ClayTypeId = clay.ClayType != null ? clay.ClayType.Id : "red";
            var json = JsonUtility.ToJson(snapshot, true);
            var name = "piece_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".json";
            var path = Path.Combine(DirectoryPath, name);
            File.WriteAllText(path, json);
            SteamService.Instance?.SaveToCloud(name, System.Text.Encoding.UTF8.GetBytes(json));
        }

        public static ClayShapeSnapshot LoadLatest()
        {
            if (!Directory.Exists(DirectoryPath))
                return default;

            var files = Directory.GetFiles(DirectoryPath, "piece_*.json");
            if (files.Length == 0)
                return default;

            Array.Sort(files);
            return JsonUtility.FromJson<ClayShapeSnapshot>(File.ReadAllText(files[^1]));
        }
    }
}
