using UnityEngine;
using System.Collections;

public class UILoadingPanel : MonoBehaviour
{
    public UISprite progress;
    //一个异步操作
    private AsyncOperation async;
    private float currentProgress = -3;
    void Start()
    {
        StartCoroutine(loadScene());
    }

    IEnumerator loadScene()
    {
        async = Application.LoadLevelAsync("Test");
        async.allowSceneActivation = false;
        yield return async;
    }
    public float LoadProgress
    {
        set
        {
            progress.fillAmount = value;
        }
    }
    void Update()
    {
        if (async == null)
        {
            return;
        }
        int totalProgress = 0;
        if (async.progress >= 0.9f)
        {
            totalProgress = 100;
        }
        else
        {
            totalProgress = (int)(async.progress * 100);
        }
        if (currentProgress < totalProgress)
            currentProgress = currentProgress + 0.5f;
        else
        {
            async.allowSceneActivation = true;
        }
        LoadProgress = currentProgress / totalProgress;
    }
}
