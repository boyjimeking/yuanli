using UnityEngine;
using System.Collections;

public class UIGameDevelopErrorWnd : UIBaseWnd
{
    public GameObject btnCopy;
    public UITextList textList;
    protected override void Awake()
    {
        base.Awake();
        this.hasClose = true;
        this.layer = UIMananger.UILayer.UI_TIPS_LAYER;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        UIEventListener.Get(btnCopy).onClick += OnClickButton;
    }

    private void OnClickButton(GameObject go)
    {
        TextEditor textEditor = new TextEditor();
        textEditor.content = new GUIContent(textList.textLabel.text);
        textEditor.OnFocus();
        textEditor.Copy();
    }
    public void SetErrorMessage(string errorMsg)
    {
        textList.Add(errorMsg);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        UIEventListener.Get(btnCopy).onClick -= OnClickButton;
        textList.Clear();
    }
}
