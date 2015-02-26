using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Rubystyle;

public class UserAnimationCallbackInfos
{
    public Action<string> func;
    public string animationName;
}

public class EntityViewComponent : MonoBehaviour
{
    protected tk2dSpriteAnimator animator;
    [HideInInspector]
    public tk2dUIProgressBar hpBar;
    [HideInInspector] 
    public Transform body;
    [HideInInspector] 
    public Transform shadow;

    //private tk2dSpriteAttachPoint attachPoint;
    //private tk2dBaseSprite sprite = null;
    public event System.Action<List<Vector3>> OnFireBullet = null;     //  or direct damage ...
    private tk2dSpriteAttachPoint effAttachPoint;

    private Dictionary<tk2dSpriteAnimationClip, UserAnimationCallbackInfos> _animationFinishCallbacks;

    virtual public void Init(bool faceToMainCamera=true)
    {
        _animationFinishCallbacks = null;

        if (faceToMainCamera)
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<tk2dSprite>() != null)
                {
                    IsoHelper.FaceToWorldCamera(child);
                    if (child.name.IndexOf("Shadow") != -1)
                    {
                        shadow = child;
                        IsoHelper.MoveAlongCamera(child, Constants.SHADOW_Z_ORDER);
                    }
                }
            }
        }
        body = transform.FindChild("body");
        if (body == null)//多状态的
        {
            SwitchBody(1);
        }else
        {
            animator = body.GetComponentInChildren<tk2dSpriteAnimator>();
            //attachPoint = body.GetComponentInChildren<tk2dSpriteAttachPoint>();
            this.effAttachPoint = body.GetComponentInChildren<tk2dSpriteAttachPoint>();
            //RegisterSpriteChangeEvent();
        }
        if (animator != null)
        {
            animator.AnimationCompleted = ontk2dSpriteAnimatorCallback_Completed;
            animator.AnimationEventTriggered = OnAnimationEventTriggered;

            animator.enabled = false;   //  REMARK：禁用掉然后手动更新
        }
    }

    private void OnAnimationEventTriggered(tk2dSpriteAnimator caller, tk2dSpriteAnimationClip currentClip, int currentFrame)
    {
        var sprite = caller.Sprite;
        var def = sprite.CurrentSprite;
        //  REMARK：这里应该判断下是否是攻击动画，目前假设所有有AttackPoint的都为攻击动画。？
        if (def.attachPoints.Length > 0)
        {
            //  处理子弹发射事件
            if (OnFireBullet != null)
            {
                List<Vector3> listFirePosition = new List<Vector3>();
                foreach (var ap in def.attachPoints)
                {
                    //  子弹发射位置
                    listFirePosition.Add(transform.TransformPoint(Vector3.Scale(ap.position, sprite.scale)));
                }
#if UNITY_EDITOR
                BattleManager.logFireBullet.Add(GameRecord.Frame + ":" + string.Join(",", listFirePosition.RubyMap(v => v.ToString()).ToArray()));
#endif  //  UNITY_EDITOR
                //  回调
                OnFireBullet(listFirePosition);
            }
        }
    }
    //private void TryInitEffAttachPoint()
    //{
    //    this.effAttachPoint = body.GetComponentInChildren<tk2dSpriteAttachPoint>();
    //    if (aps != null)
    //    {
    //        foreach (var ap in aps.attachPoints)
    //        {
    //            //  REMARK：效果挂掉名字（EffPoint） 考虑移动到 Constant 。
    //            if (!string.IsNullOrEmpty(ap.gameObject.name) && ap.gameObject.name.Contains("EffPoint"))
    //            {
    //                if (this.effAttachPoint == null)
    //                    this.effAttachPoint = new List<Vector3>();
    //                //Vector3 p = ap.position;
    //                //effAttachPoint.Add(new Vector2(p.x, p.z));
    //                this.effAttachPoint.Add(ap.position);
    //            }
    //        }
    //    }
    //}

    //private void RegisterSpriteChangeEvent()
    //{
    //    if (this.sprite != null)
    //    {
    //        sprite.SpriteChanged -= sprite_SpriteChanged;
    //    }
    //    this.sprite = body.GetComponentInChildren<tk2dBaseSprite>();
    //    if (this.sprite != null)
    //    {
    //        this.sprite.SpriteChanged += sprite_SpriteChanged;
    //        //sprite_SpriteChanged(this.sprite);   //  初始化的时候处理一下???
    //    }
    //}

    //private void sprite_SpriteChanged(tk2dBaseSprite obj)
    //{
    //    //  是否是攻击动画判断
    //    if (animator == null)
    //        return;
    //    var clip = animator.CurrentClip;
    //    if (clip == null || string.IsNullOrEmpty(clip.name))
    //        return;
    //    if (!clip.name.Contains("Attack"))  //  TODO：这里暂时直接根据动画名的Attack判断
    //        return;


    //}

    /// <summary>
    /// 切换显示状态
    /// </summary>
    /// <param name="bodyIndex"></param>
    public void SwitchBody(int bodyIndex)
    {
        var toBody = transform.FindChild("body" + bodyIndex);
        if (toBody != null)
        {
            body = toBody;
            foreach (Transform child in transform)
            {
                if (child == body)
                {
                    child.gameObject.SetActive(true);
                }
                else if(child.name.StartsWith("body"))
                {
                    child.gameObject.SetActive(false);
                }
            }
            animator = body.GetComponentInChildren<tk2dSpriteAnimator>();
            //attachPoint = body.GetComponentInChildren<tk2dSpriteAttachPoint>();
            this.effAttachPoint = body.GetComponentInChildren<tk2dSpriteAttachPoint>();
            //RegisterSpriteChangeEvent();

            if (animator != null)
                animator.enabled = false;   //  REMARK：禁用掉然后手动更新
        }
    }
    public void SetHpBarPercent(float percent)
    {
        if (hpBar == null)
        {
            hpBar =
                ((GameObject) ResourceManager.Instance.LoadAndCreate("Misc/HPBar")).GetComponent<tk2dUIProgressBar>();
            AddSubView(hpBar.gameObject, new Vector3(0, Constants.HPBAR_VERTICAL_HEIGHT, 0));
            IsoHelper.FaceToWorldCamera(hpBar.transform);
        }
        hpBar.Value = percent;
    }

    public void AddSubView(GameObject view, Vector3 localPosition)
    {
        view.transform.parent = transform;
        view.transform.localPosition = localPosition;
    }

    protected void SetAnimatorFpsFactor(float fpsFactor)
    {
        if (animator != null)
        {
            if (fpsFactor > 0)
            {
                animator.ClipFps *= fpsFactor;
            }
            else
            {
                animator.ClipFps = 0;
            }
        }
    }

    virtual public void PlayAnimation(string animationName, EntityAnimationDirection animationDirection, float fps = 0.0f, Action<string> callback = null)
    {
        tk2dSpriteAnimationClip clip;
        if (animationDirection == null)
        {
            clip = animator.GetClipByName(animationName);
        }
        else
        {
            if (animationDirection.flipX)
            {
                animator.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                animator.transform.localScale = new Vector3(1, 1, 1);
            }
            //animator.Sprite.FlipX = animationDirection.flipX;
            clip = animator.GetClipByName(animationName + "_" + animationDirection.direction);
        }
        Assert.Should(clip != null,animationName + ",animationDirection:" + animationDirection);
        if (callback != null)
        {
            if (_animationFinishCallbacks == null)
            {
                _animationFinishCallbacks = new Dictionary<tk2dSpriteAnimationClip, UserAnimationCallbackInfos>();
            }

            UserAnimationCallbackInfos infos = new UserAnimationCallbackInfos() { func = callback, animationName = animationName };
            _animationFinishCallbacks.Add(clip, infos);
        }
        animator.Play(clip);
        SetAnimatorFpsFactor(fps);
    }

    private void ontk2dSpriteAnimatorCallback_Completed(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
    {
        if (_animationFinishCallbacks != null)
        {
            if (_animationFinishCallbacks.ContainsKey(clip))
            {
                UserAnimationCallbackInfos userCallbackInfos = _animationFinishCallbacks[clip];
                _animationFinishCallbacks.Remove(clip);
                userCallbackInfos.func(userCallbackInfos.animationName);
            }
        }
    }

    public void SetSprite(string spriteName)
    {
        body.GetComponent<tk2dSprite>().SetSprite(spriteName);
    }

    //virtual public List<Transform> GetFirePositions()
    //{
    //    if (attachPoint != null && attachPoint.attachPoints.Count > 0)
    //    {
    //        return attachPoint.attachPoints;
    //    }
    //    return null;
    //}
    public List<Transform> GetEffAttachPoint()
    {
        if (this.effAttachPoint != null && this.effAttachPoint.attachPoints.Count > 0)
        {
            return this.effAttachPoint.attachPoints;
        }
        return null;
        //if (this.effAttachPoint != null)
        //{
        //    //  REMARK：效果挂掉名字（EffPoint） 考虑移动到 Constant 。
        //    var retv = this.effAttachPoint.attachPoints.RubySelect(ap => !string.IsNullOrEmpty(ap.gameObject.name) && ap.gameObject.name.Contains("EffPoint"));
        //    if (retv.Count > 0)
        //        return retv;
        //}
        //return null;
    }

    public virtual void UpdateView(float dt)
    {
        if (animator != null)
            animator.UpdateAnimation(dt);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
#endif

}
