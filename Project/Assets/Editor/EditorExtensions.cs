using UnityEditor;
using UnityEngine;

public class EditorExtensions
{

    public const string root = "EditorExtensions/";
    [MenuItem(EditorExtensions.root + "Refresh", false)]
    public static void Flush()
    {
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem(EditorExtensions.root + "Generate Actor Animation %T", false,2)]
    public static void CreateActorAnimation()
    {
        GenerateAnimation.GenerateActorAnimation();
    }

    [MenuItem(EditorExtensions.root + "Generate Tower Animation", false, 3)]
    public static void CreateTowerAnimation()
    {
        GenerateAnimation.CreateTowerAnimation();
    }

    [MenuItem(EditorExtensions.root + "Sprite Collection (Modified) %G", false,1)]
    public static void CreateSpriteCollectionModified()
    {
        string path = tk2dEditorUtility.CreateNewPrefab("SpriteCollection");
        if (path.Length != 0)
        {
            GameObject go = new GameObject();
            tk2dSpriteCollection spriteCollection = go.AddComponent<tk2dSpriteCollection>();
            spriteCollection.filterMode = FilterMode.Bilinear;
            spriteCollection.sizeDef = tk2dSpriteCollectionSize.PixelsPerMeter(40);
            spriteCollection.version = tk2dSpriteCollection.CURRENT_VERSION;
            if (tk2dCamera.Editor__Inst != null)
            {
                spriteCollection.sizeDef.CopyFrom(tk2dSpriteCollectionSize.ForTk2dCamera(tk2dCamera.Editor__Inst));
            }
            tk2dEditorUtility.SetGameObjectActive(go, false);

            Object p = PrefabUtility.CreateEmptyPrefab(path);
            PrefabUtility.ReplacePrefab(go, p, ReplacePrefabOptions.ConnectToPrefab);

            GameObject.DestroyImmediate(go);

            // Select object
            Selection.activeObject = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
        }
    }
}
