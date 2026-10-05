using ClayForge.App;
using ClayForge.Data;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace ClayForge.EditorTools
{
    public static class ProjectBootstrap
    {
        private const string Root = "Assets/_Project";

        [MenuItem("ClayForge/Bootstrap Project")]
        public static void Run()
        {
            EnsureFolders();
            CreateClayTypes();
            TryAssignUrp();
            CreateScene("Scenes/Boot/Boot", typeof(BootLoader));
            CreateScene("Scenes/MainMenu/MainMenu", typeof(MainMenuDirector));
            CreateScene("Scenes/Workshop/Workshop", typeof(WorkshopDirector));
            CreateScene("Scenes/Gallery/Gallery", typeof(GalleryDirector));
            SetBuildScenes();
            ConfigurePlayer();
            AssetDatabase.SaveAssets();
            Debug.Log("ClayForge prototype bootstrap complete.");
        }

        public static void RunFromCommandLine()
        {
            try
            {
                Run();
                EditorApplication.Exit(0);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
                EditorApplication.Exit(1);
            }
        }

        private static void EnsureFolders()
        {
            string[] folders =
            {
                "Scripts/Core/Physics", "Scripts/Systems", "Scripts/UI", "Scripts/Save", "Scripts/Steam",
                "Scripts/Editor", "Prefabs", "Materials", "Models", "Shaders", "Scenes/Boot", "Scenes/MainMenu",
                "Scenes/Workshop", "Scenes/Gallery", "Data/ClayTypes", "Data/Events"
            };
            foreach (var folder in folders)
            {
                var path = Root + "/" + folder;
                if (!AssetDatabase.IsValidFolder(path))
                {
                    var parts = folder.Split('/');
                    var current = Root;
                    foreach (var part in parts)
                    {
                        var next = current + "/" + part;
                        if (!AssetDatabase.IsValidFolder(next))
                            AssetDatabase.CreateFolder(current, part);
                        current = next;
                    }
                }
            }
        }

        private static void CreateClayTypes()
        {
            CreateClay("ClayType_Red", "red", "Red clay", "Красная глина", 0.78f, 0.48f, 0.08f, 1f,
                new Color(0.55f, 0.22f, 0.12f), new Color(0.62f, 0.32f, 0.2f), new Color(0.72f, 0.38f, 0.24f));
            CreateClay("ClayType_White", "white", "White stoneware", "Белая глина", 0.7f, 0.58f, 0.1f, 0.95f,
                new Color(0.82f, 0.78f, 0.72f), new Color(0.86f, 0.83f, 0.78f), new Color(0.92f, 0.9f, 0.86f));
            CreateClay("ClayType_Black", "black", "Black clay", "Чёрная глина", 0.62f, 0.66f, 0.09f, 1.1f,
                new Color(0.16f, 0.14f, 0.13f), new Color(0.22f, 0.2f, 0.18f), new Color(0.18f, 0.16f, 0.15f));
            CreateClay("ClayType_Porcelain", "porcelain", "Porcelain", "Фарфор", 0.88f, 0.4f, 0.14f, 0.85f,
                new Color(0.9f, 0.9f, 0.88f), new Color(0.93f, 0.93f, 0.91f), new Color(0.96f, 0.96f, 0.95f));
        }

        private static void CreateClay(
            string file,
            string id,
            string en,
            string ru,
            float plasticity,
            float strength,
            float shrink,
            float density,
            Color wet,
            Color leather,
            Color fired)
        {
            var path = $"{Root}/Data/ClayTypes/{file}.asset";
            var asset = AssetDatabase.LoadAssetAtPath<ClayType>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<ClayType>();
                AssetDatabase.CreateAsset(asset, path);
            }

            asset.Configure(id, en, ru, plasticity, strength, shrink, density, wet, leather, fired);
            EditorUtility.SetDirty(asset);
        }

        private static void CreateScene(string relative, System.Type director)
        {
            var path = $"{Root}/{relative}.unity";
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            var holder = new GameObject(director.Name);
            holder.AddComponent(director);
            EditorSceneManager.SaveScene(scene, path);
        }

        private static void SetBuildScenes()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene($"{Root}/Scenes/Boot/Boot.unity", true),
                new EditorBuildSettingsScene($"{Root}/Scenes/MainMenu/MainMenu.unity", true),
                new EditorBuildSettingsScene($"{Root}/Scenes/Workshop/Workshop.unity", true),
                new EditorBuildSettingsScene($"{Root}/Scenes/Gallery/Gallery.unity", true)
            };
        }

        private static void ConfigurePlayer()
        {
            PlayerSettings.productName = "ClayForge";
            PlayerSettings.companyName = "ClayForge";
            PlayerSettings.defaultScreenWidth = 1920;
            PlayerSettings.defaultScreenHeight = 1080;
            var assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset");
            if (assets != null && assets.Length > 0)
            {
                var so = new SerializedObject(assets[0]);
                var input = so.FindProperty("activeInputHandler");
                if (input != null)
                {
                    input.intValue = 2;
                    so.ApplyModifiedPropertiesWithoutUndo();
                }
            }
        }

        private static void TryAssignUrp()
        {
            var urpType = System.Type.GetType("UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset, Unity.RenderPipelines.Universal.Runtime");
            if (urpType == null)
                return;

            var path = $"{Root}/Settings/URP-ClayForge.asset";
            if (!AssetDatabase.IsValidFolder($"{Root}/Settings"))
                AssetDatabase.CreateFolder(Root, "Settings");

            var pipeline = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(path);
            if (pipeline == null)
            {
                var create = urpType.GetMethod("Create", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, null, System.Type.EmptyTypes, null);
                pipeline = create != null
                    ? (RenderPipelineAsset)create.Invoke(null, null)
                    : (RenderPipelineAsset)ScriptableObject.CreateInstance(urpType);
                AssetDatabase.CreateAsset(pipeline, path);
            }

            GraphicsSettings.defaultRenderPipeline = pipeline;
            QualitySettings.renderPipeline = pipeline;
        }
    }
}
