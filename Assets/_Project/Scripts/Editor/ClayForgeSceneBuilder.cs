#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ClayForgeSceneBuilder
{
    [MenuItem("ClayForge/Build Workshop Scene")]
    public static void BuildWorkshopScene()
    {
        string dir = "Assets/_Project/Scenes";
        if (!AssetDatabase.IsValidFolder("Assets/_Project"))
            AssetDatabase.CreateFolder("Assets", "_Project");
        if (!AssetDatabase.IsValidFolder(dir))
            AssetDatabase.CreateFolder("Assets/_Project", "Scenes");

        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        var table = GameObject.CreatePrimitive(PrimitiveType.Cube);
        table.name = "Table";
        table.transform.position = new Vector3(0f, -0.05f, 0f);
        table.transform.localScale = new Vector3(2.4f, 0.1f, 2.4f);
        SetColor(table, new Color(0.25f, 0.22f, 0.18f));

        var wheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        wheel.name = "Wheel";
        wheel.transform.position = new Vector3(0f, 0.04f, 0f);
        wheel.transform.localScale = new Vector3(1.8f, 0.04f, 1.8f);
        SetColor(wheel, new Color(0.35f, 0.32f, 0.28f));

        var clay = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        clay.name = "Clay";
        clay.transform.SetParent(wheel.transform, true);
        clay.transform.position = new Vector3(0f, 0.45f, 0f);
        clay.transform.localScale = new Vector3(0.55f, 0.4f, 0.55f);
        SetColor(clay, new Color(0.55f, 0.38f, 0.28f));

        var hands = new GameObject("Hands");
        var workshop = new GameObject("Workshop");

        AddIfExists(wheel, "PotteryWheel");
        AddIfExists(clay, "ClayBody");
        AddIfExists(clay, "ClayPhysics");
        AddIfExists(clay, "CenteringSystem");
        AddIfExists(hands, "HandController");
        AddIfExists(hands, "HandRig");
        AddIfExists(workshop, "WorkshopController");
        AddIfExists(workshop, "WorkshopContext");
        AddIfExists(workshop, "WorkshopBootstrap");
        AddIfExists(workshop, "Workshop");

        TryAddAny(workshop, "Workshop");

        WireAll(new[] { table, wheel, clay, hands, workshop });

        var cam = Camera.main;
        if (cam != null)
        {
            cam.transform.position = new Vector3(1.7f, 1.5f, -2.3f);
            cam.transform.LookAt(new Vector3(0f, 0.35f, 0f));
        }

        string path = dir + "/Workshop.unity";
        EditorSceneManager.SaveScene(scene, path);
        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(path, true)
        };

        Selection.activeGameObject = wheel;
        SceneView.FrameLastActiveSceneView();
        Debug.Log("[ClayForge] Сцена собрана: " + path);
    }

    static void SetColor(GameObject go, Color c)
    {
        var r = go.GetComponent<Renderer>();
        if (r == null) return;
        var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        var mat = new Material(shader) { color = c };
        r.sharedMaterial = mat;
    }

    static Component AddIfExists(GameObject go, string typeName)
    {
        var t = FindType(typeName);
        if (t == null || !typeof(Component).IsAssignableFrom(t)) return null;
        if (go.GetComponent(t) != null) return go.GetComponent(t);
        return go.AddComponent(t);
    }

    static void TryAddAny(GameObject go, string startsWith)
    {
        foreach (var t in AppDomain.CurrentDomain.GetAssemblies()
                     .SelectMany(a =>
                     {
                         try { return a.GetTypes(); }
                         catch { return Array.Empty<Type>(); }
                     })
                     .Where(t => typeof(MonoBehaviour).IsAssignableFrom(t)
                                 && t.Name.StartsWith(startsWith, StringComparison.OrdinalIgnoreCase)
                                 && !t.IsAbstract))
        {
            if (go.GetComponent(t) == null)
                go.AddComponent(t);
        }
    }

    static Type FindType(string name)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch { return Array.Empty<Type>(); }
            })
            .FirstOrDefault(t => t.Name == name);
    }

    static void WireAll(GameObject[] objects)
    {
        var comps = objects.SelectMany(o => o.GetComponents<MonoBehaviour>()).Where(c => c != null).ToArray();
        foreach (var comp in comps)
        {
            var so = new SerializedObject(comp);
            var it = so.GetIterator();
            bool enter = true;
            while (it.NextVisible(enter))
            {
                enter = false;
                if (it.propertyType != SerializedPropertyType.ObjectReference) continue;
                if (it.name == "m_Script") continue;
                if (it.objectReferenceValue != null) continue;

                var field = comp.GetType().GetField(it.name,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field == null) continue;
                var ft = field.FieldType;

                UnityEngine.Object found = null;
                foreach (var go in objects)
                {
                    if (ft == typeof(GameObject) && NameHintMatch(it.name, go.name))
                    { found = go; break; }
                    if (ft == typeof(Transform) && NameHintMatch(it.name, go.name))
                    { found = go.transform; break; }
                    if (typeof(Component).IsAssignableFrom(ft))
                    {
                        var c = go.GetComponent(ft);
                        if (c != null) { found = c; break; }
                    }
                }
                if (found != null)
                    it.objectReferenceValue = found;
            }
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }

    static bool NameHintMatch(string field, string goName)
    {
        field = field.ToLowerInvariant();
        goName = goName.ToLowerInvariant();
        return field.Contains(goName) || goName.Contains(field.Replace("m_", ""));
    }
}
#endif