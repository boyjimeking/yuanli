using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Collections;
using Object = UnityEngine.Object;

public class GenerateAnimation
{
    public static void GenerateActorAnimation()
    {
        CreateAnimation(CreateActorAnimation);
    }

    public static void CreateTowerAnimation()
    {
        CreateAnimation(CreateTowerAnimation);
    }

    private static void CreateAnimation(Func<tk2dSpriteCollectionData,List<tk2dSpriteAnimationClip>> handler)
    {
        //获取在Project视图中选择的所有游戏对象
        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

        float i = 0;
        //遍历所有的游戏对象
        foreach (Object obj in SelectedAsset)
        {
            EditorUtility.DisplayProgressBar("Generating", "Generating Animations", i++ / SelectedAsset.Length);
            var spriteCollection = ((GameObject)obj).GetComponent<tk2dSpriteCollection>();
            if (spriteCollection != null)
            {
                var path = AssetDatabase.GetAssetPath(spriteCollection);
                path = path.Replace(".prefab", "") + "_Animation.prefab";
                Debug.Log(path);

                GameObject go = new GameObject();
                tk2dSpriteAnimation spriteAnimation = go.AddComponent<tk2dSpriteAnimation>();

                spriteAnimation.clips = handler(spriteCollection.spriteCollection).ToArray();
                Object p = PrefabUtility.CreateEmptyPrefab(path);
                PrefabUtility.ReplacePrefab(go, p, ReplacePrefabOptions.ConnectToPrefab);
                GameObject.DestroyImmediate(go);

                tk2dEditorUtility.GetOrCreateIndex().AddSpriteAnimation(AssetDatabase.LoadAssetAtPath(path, typeof(tk2dSpriteAnimation)) as tk2dSpriteAnimation);
                tk2dEditorUtility.CommitIndex();
            }
        }
        EditorUtility.ClearProgressBar();
        //刷新编辑器
        AssetDatabase.Refresh();
    }

    private static List<tk2dSpriteAnimationClip> CreateActorAnimation(tk2dSpriteCollectionData spriteCollectionData)
    {
        List<tk2dSpriteAnimationClip> clips = new List<tk2dSpriteAnimationClip>();
        foreach (var name in GetAllNames(spriteCollectionData))
        {
            clips.AddRange(CreateAnimationClip(spriteCollectionData, name, "Attack", new[] { "TopRight", "Right", "BottomRight" },tk2dSpriteAnimationClip.WrapMode.Once));
            clips.AddRange(CreateAnimationClip(spriteCollectionData, name, "Run", new[] { "TopRight", "Right", "BottomRight" },tk2dSpriteAnimationClip.WrapMode.Loop));
            clips.AddRange(CreateAnimationClip(spriteCollectionData, name, "Stand", new[] { "" },tk2dSpriteAnimationClip.WrapMode.Loop));
            clips.AddRange(CreateAnimationClip(spriteCollectionData, name, "Victory", new[] { "" },tk2dSpriteAnimationClip.WrapMode.Once));
        }
        return clips;
    }

    private static List<tk2dSpriteAnimationClip> CreateTowerAnimation(tk2dSpriteCollectionData spriteCollectionData)
    {
        List<tk2dSpriteAnimationClip> clips = new List<tk2dSpriteAnimationClip>();
        foreach (var name in GetAllNames(spriteCollectionData))
        {
            clips.AddRange(CreateAnimationClip(spriteCollectionData, name, "Attack", new[] { "Top","TopRight", "Right", "BottomRight","Bottom" },tk2dSpriteAnimationClip.WrapMode.Once));
            clips.AddRange(CreateAnimationClip(spriteCollectionData, name, "Stand", new[] { "" },tk2dSpriteAnimationClip.WrapMode.Loop));
        }
        return clips;
    }
    private static List<string> GetAllNames(tk2dSpriteCollectionData spriteCollectionData)
    {
        List<string> names = new List<string>();
        foreach (var spriteDefine in spriteCollectionData.spriteDefinitions)
        {
            if (spriteDefine.name != "")//REMARK 删除贴图后,spriteDefine可能还在,名字变为空
            {
                var name = spriteDefine.name;
                if(spriteDefine.name.IndexOf("_") > 0)
                    name = spriteDefine.name.Substring(0, spriteDefine.name.IndexOf("_"));
                if (!names.Contains(name))
                {
                    names.Add(name);
                }
            }
        }
        return names;
    }
    private static tk2dSpriteAnimationClip[] CreateAnimationClip(tk2dSpriteCollectionData spriteCollectionData,string name,string action,string[] directionNames,tk2dSpriteAnimationClip.WrapMode wrapMode)
    {
        List<tk2dSpriteDefinition> frames = new List<tk2dSpriteDefinition>();
        foreach (var spriteDefine in spriteCollectionData.spriteDefinitions)
        {
            if (spriteDefine.name.Contains(name + "_" + action.ToLower()))
            {
                frames.Add(spriteDefine);
            }
        }
        frames = frames.OrderBy(o => o.name).ToList();
        var frameCount = frames.Count / directionNames.Length;
        tk2dSpriteAnimationClip[] clips = new tk2dSpriteAnimationClip[directionNames.Length];
        for (int i = 0; i < directionNames.Length; i++)
        {
            var clip = new tk2dSpriteAnimationClip();
            clips[i] = clip;
            clip.fps = 10;
            if (directionNames[i] == "")
            {
                clip.name = name + "_" + action;
            }
            else
            {
                clip.name = name + "_" + action + "_" + directionNames[i];
            }
            clip.frames = new tk2dSpriteAnimationFrame[frameCount];
            clip.wrapMode = wrapMode;
            for (int j = 0; j < clip.frames.Length; ++j)
            {
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
                frame.spriteCollection = spriteCollectionData;
                frame.spriteId = frame.spriteCollection.GetSpriteIdByName(frames[i * frameCount + j].name);

                clip.frames[j] = frame;
            }
        }
        return clips;
    }
}
