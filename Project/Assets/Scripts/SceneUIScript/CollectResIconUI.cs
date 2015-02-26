using UnityEngine;
using System.Collections;
using System;

public class CollectResIconUI : BaseSceneUI
{
    public GameObject btnRes;
    public event Action OnClickEvent;
    public void OnClick(GameObject go)
    {
        if (OnClickEvent != null)
        {
            OnClickEvent();
        }
    }
    public string SpriteName
    {
        set
        {
            btnRes.GetComponent<UISprite>().spriteName = value;
            btnRes.GetComponent<UIButton>().normalSprite = value;
        }
    }
    void OnEnable()
    {
        UIEventListener.Get(btnRes).onClick += OnClick;
    }
    void OnDisable()
    {
        UIEventListener.Get(btnRes).onClick -= OnClick;
    }
}
