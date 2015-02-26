using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIBuildOperationWnd : UIBaseWnd
{
    //按钮数组
    private Dictionary<string, GameObject> dicButtons = new Dictionary<string, GameObject>();
    //数据信息
    private TileEntity tileEntity;
    //该建筑按钮名字
    private List<string> listButtonName;
    //PlayTween
    private UIPlayTween uiPlayTween;
    public UILabel txtName;
    protected override void Awake()
    {
        base.Awake();
        this.hasClose = false;
        this.layer = UIMananger.UILayer.UI_FIXED_LAYER;
        this.CloseOrHideType = UICloseOrHideType.CLOSE_WORLD_TYPE_BATTLE;
    }
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    private void InitButtons()
    {
        if (dicButtons.Count <= 0)
        {
            foreach (string btnName in listButtonName)
            {
                dicButtons.Add(btnName, this.transform.Find(btnName).gameObject);
            }
        }
        //重新设定坐标
        int width = (dicButtons.Count - 1) * 70 + (dicButtons.Count - 1) * 20;
        int index = 0;
        TweenPosition tweenPositon;
        TweenAlpha tweenAlpha;
        foreach (KeyValuePair<string, GameObject> keyValuePair in dicButtons)
        {
            keyValuePair.Value.transform.localPosition = new Vector3(-width * 0.5f + index * (70 + 20), -110, 0);
            keyValuePair.Value.SetActive(true);
            keyValuePair.Value.GetComponent<UIBuildOperationButton>().ClickButton += OnClickButton;
            keyValuePair.Value.GetComponent<UIBuildOperationButton>().Entity = this.tileEntity;
            tweenPositon = keyValuePair.Value.AddMissingComponent<TweenPosition>();
            tweenPositon.delay = index * 0.02f;
            tweenPositon.from = new Vector3(-width * 0.5f + index * (70 + 20), -110, 0);
            tweenPositon.to = new Vector3(-width * 0.5f + index * (70 + 20), -10, 0);
            tweenPositon.tweenFactor = 0;
            tweenPositon.duration = 0.2f;
            tweenPositon.enabled = false;
            tweenAlpha = keyValuePair.Value.AddMissingComponent<TweenAlpha>();
            tweenAlpha.delay = index * 0.02f;
            tweenAlpha.tweenFactor = 0;
            tweenAlpha.from = 0;
            tweenAlpha.to = 1;
            tweenAlpha.duration = 0.2f;
            tweenAlpha.enabled = false;
            index++;
        }
        uiPlayTween.playDirection = AnimationOrTween.Direction.Forward;
        uiPlayTween.Play(true);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        uiPlayTween = this.transform.GetComponent<UIPlayTween>();
    }

    private void OnClickButton(bool isClick)
    {
        this.tileEntity = null;
        HideButtons();
    }
    protected override void OnDisable()
    {
        foreach (KeyValuePair<string, GameObject> keyValuePair in dicButtons)
        {
            keyValuePair.Value.SetActive(false);
        }
        dicButtons.Clear();
        txtName.text = "";
    }
    public TileEntity CurTileEntity
    {
        set
        {
            this.tileEntity = value;
            if (dicButtons.Count > 0)
            {
                HideButtons();
                return;
            }
            ShowButtons();
        }
    }
    private void HideButtons()
    {
        EventDelegate eventDelegate = new EventDelegate(this, "OnTweenOver");
        eventDelegate.oneShot = true;
        uiPlayTween.onFinished.Add(eventDelegate);
        uiPlayTween.playDirection = AnimationOrTween.Direction.Reverse;
        uiPlayTween.Play(true);
    }
    private void OnTweenOver()
    {
        foreach (KeyValuePair<string, GameObject> keyValuePair in dicButtons)
        {
            keyValuePair.Value.SetActive(false);
        }
        dicButtons.Clear();
        if (null != this.tileEntity)
        {
            ShowButtons();
        }
        else
        {
            UIMananger.Instance.CloseWin(this.gameObject.name);
        }
    }
    private void ShowButtons()
    {
        if (null != this.tileEntity)
        {
            txtName.text = tileEntity.model.nameForView + " (等级" + tileEntity.model.level + ")";
            listButtonName = OperationButtonUtil.GetButtonList(this.tileEntity);
            InitButtons();
        }
    }

}
