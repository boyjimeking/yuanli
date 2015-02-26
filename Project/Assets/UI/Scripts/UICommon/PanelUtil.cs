using UnityEngine;
using System.Collections;

public class PanelUtil : MonoBehaviour
{
    public static Color greyColor = new Color(-1, 0, 0);
    /// <summary>
    /// 设置组件颜色
    /// </summary>
    /// <param name="widget"></param>
    /// <param name="color"></param>
    /// <param name="isSingle"></param>
    /// <param name="targetObj"></param>
    public static void SetUIRectColor(UIWidget widget, Color color, bool isSingle = true, GameObject targetObj = null, float alpha = -1)
    {
        if (isSingle)
        {
            if (!widget.color.Equals(color))
                widget.color = color;
        }
        else
        {
            UISprite[] widgets = targetObj.GetComponentsInChildren<UISprite>();
            foreach (UISprite tempWidget in widgets)
            {
                if (!tempWidget.color.Equals(color))
                {
                    if (alpha != -1)
                    {
                        color.a = alpha;
                    }
                    tempWidget.color = color;
                }
            }
        }
    }
    /// <summary>
    /// 设置组件锚点
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="targetTrans"></param>
    /// <param name="vecRelative">0代表left或者是bottom--1代表right或者是top</param>
    /// <param name="vecAbsolute">距离</param>
    public static void SetPanelAnchors(UIRect rect, Transform targetTrans, Vector4 vecRelative, Vector4 vecAbsolute)
    {
        rect.leftAnchor.Set(targetTrans, vecRelative.x, vecAbsolute.x);
        rect.rightAnchor.Set(targetTrans, vecRelative.y, vecAbsolute.y);
        rect.bottomAnchor.Set(targetTrans, vecRelative.z, vecAbsolute.z);
        rect.topAnchor.Set(targetTrans, vecRelative.w, vecAbsolute.w);
    }
    /// <summary>
    /// 面板实际的宽度
    /// </summary>
    /// <returns></returns>
    public static float GetPanelWidth()
    {
        return Screen.width / Screen.height * Constants.UI_HEIGHT;
    }
}
