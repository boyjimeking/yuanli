using System.Runtime.InteropServices;
using Org.BouncyCastle.Asn1.X509;
using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// component 'TowerView'
/// ADD COMPONENT DESCRIPTION HERE
/// </summary>
[AddComponentMenu("Scripts/TowerView")]
public class TowerView : EntityViewComponent
{
    private tk2dSpriteAnimationClip reversedStand;
    private Action<float> m_updateRotate = null;

    private EntityAnimationDirection lastDirection = new EntityAnimationDirection()
    {
        direction = EntityDirection.Top,
        flipX = false
    };

    public override void PlayAnimation(string animationName, EntityAnimationDirection animationDirection, float fps = 0.0f, Action<string> callback = null)
    {
        StopAllCoroutines();
        if (animationDirection != null && animationName.Contains(AnimationNames.Stand))
        {
            FaceToDirection(animationName, animationDirection, fps, callback);
        }
        else
        {
            lastDirection = animationDirection;
            base.PlayAnimation(animationName, animationDirection, fps, callback);
        }
    }

    public override void UpdateView(float dt)
    {
        base.UpdateView(dt);
        if (m_updateRotate != null)
        {
            m_updateRotate(dt);
        }
    }

    private void FaceToDirection(string animationName, EntityAnimationDirection direction, float fps, Action<string> callback)
    {
        if (direction.Equals(lastDirection))
        {
            if (callback != null) callback(animationName);
            return;
        }
        var currentFrame = DirectionToFrame(lastDirection);
        var targetFrame = DirectionToFrame(direction);
        animator.Sprite.FlipX = false;
        var deltaFrame = targetFrame - currentFrame;
        if (deltaFrame < 0) deltaFrame += 16;
        if (deltaFrame > 8)
        {
            if (reversedStand == null)
            {
                var stand = animator.GetClipByName(animationName);
                reversedStand = new tk2dSpriteAnimationClip();
                reversedStand.CopyFrom(stand);
                System.Array.Reverse(reversedStand.frames);
            }
            currentFrame = 15 - currentFrame;
            targetFrame = 15 - targetFrame;
            animator.PlayFromFrame(reversedStand, currentFrame);
        }
        else
        {
            animator.PlayFromFrame(animationName, currentFrame);
        }

        SetAnimatorFpsFactor(fps);

        Assert.Should(animator.CurrentClip.frames.Length >= 16, "invalid frame count ... " + this.gameObject.name);

        //  设置 updater
        m_updateRotate = (dt) => 
        {
            if (animator.CurrentFrame != targetFrame)
                return;
            animator.Stop();
            lastDirection = direction;
            if (callback != null)
            {
                callback(animationName);
            }
            m_updateRotate = null;
        };
    }

    //private IEnumerator FaceToDirection(string animationName, EntityAnimationDirection direction, float fps, Action<string> callback)
    //{
    //    if (direction.Equals(lastDirection))
    //    {
    //        if (callback != null) callback(animationName);
    //        yield break;
    //    }
    //    var currentFrame = DirectionToFrame(lastDirection);
    //    var targetFrame = DirectionToFrame(direction);
    //    animator.Sprite.FlipX = false;
    //    var deltaFrame = targetFrame - currentFrame;
    //    if (deltaFrame < 0) deltaFrame += 16;
    //    if (deltaFrame > 8)
    //    {
    //        if (reversedStand == null)
    //        {
    //            var stand = animator.GetClipByName(animationName);
    //            reversedStand = new tk2dSpriteAnimationClip();
    //            reversedStand.CopyFrom(stand);
    //            System.Array.Reverse(reversedStand.frames);
    //        }
    //        currentFrame = 15 - currentFrame;
    //        targetFrame = 15 - targetFrame;
    //        animator.PlayFromFrame(reversedStand, currentFrame);
    //    }
    //    else
    //    {
    //        animator.PlayFromFrame(animationName, currentFrame);
    //    }

    //    SetAnimatorFpsFactor(fps);

    //    Assert.Should(animator.CurrentClip.frames.Length >= 16, "invalid frame count ... " + this.gameObject.name);

    //    while (animator.CurrentFrame != targetFrame)
    //    {
    //        yield return 0;
    //    }
    //    animator.Stop();
    //    lastDirection = direction;
    //    if (callback != null)
    //    {
    //        callback(animationName);
    //    }
    //}

    private int DirectionToFrame(EntityAnimationDirection direction)
    {
        var frame = 0;
        switch (direction.direction)
        {
            case EntityDirection.Top:
            frame = 0;
            break;
            case EntityDirection.TopRight:
            frame = 2;
            break;
            case EntityDirection.Right:
            frame = 4;
            break;
            case EntityDirection.BottomRight:
            frame = 6;
            break;
            case EntityDirection.Bottom:
            frame = 8;
            break;
        }
        if (direction.flipX)
            frame = 16 - frame;
        return frame;
    }
}
