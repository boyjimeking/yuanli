using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// component 'ActorView'
/// ADD COMPONENT DESCRIPTION HERE
/// </summary>
[AddComponentMenu("Scripts/ActorView")]
public class ActorView : EntityViewComponent
{

    private EntityAnimationDirection lastDirection = null;

    public override void PlayAnimation(string animationName, EntityAnimationDirection animationDirection, float fps = 0.0f, Action<string> callback = null)
    {
        if (animationDirection != null)
        {
            if (animationDirection.direction == EntityDirection.Top)//使用右上替代上
            {
                animationDirection.direction = EntityDirection.TopRight;
                if (lastDirection != null)//减少左右频繁转向的问题
                    animationDirection.flipX = lastDirection.flipX;
            }
            else if (animationDirection.direction == EntityDirection.Bottom)//使用左下替代下
            {
                animationDirection.direction = EntityDirection.BottomRight;
                if (lastDirection != null)//减少左右频繁转向的问题
                    animationDirection.flipX = lastDirection.flipX;
            }
        }
        lastDirection = animationDirection;
        base.PlayAnimation(animationName, animationDirection, fps, callback);
    }
}
