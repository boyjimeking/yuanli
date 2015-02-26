using UnityEngine;
using System.Collections;

public class GuardAreaView : MonoBehaviour 
{
    public void Init(GuardAreaValue[,] guardMap)
    {
        transform.position = IsoHelper.MoveAlongCamera(Vector3.zero, Constants.GUARD_VIEW_Z_ORDER);
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        var tile01 = (GameObject)ResourceManager.Instance.Load("Misc/Tile01");
        var tile02 = (GameObject)ResourceManager.Instance.Load("Misc/Tile02");
        var tile03 = (GameObject)ResourceManager.Instance.Load("Misc/Tile03");

        for (int i = 0; i < Constants.EDGE_WIDTH; i++)
        {
            for (int j = 0; j < Constants.EDGE_HEIGHT; j++)
            {
                GameObject prefab = null;
                var value = guardMap[i, j];
                var angle = 0;
                if (value == GuardAreaValue.BottomRight)
                {
                    prefab = tile02;
                    angle = 0;
                }else if (value == GuardAreaValue.BottomLeft)
                {
                    prefab = tile02;
                    angle = 90;
                }else if (value == GuardAreaValue.TopLeft)
                {
                    prefab = tile02;
                    angle = 180;
                }else if (value == GuardAreaValue.TopRight)
                {
                    prefab = tile02;
                    angle = -90;
                }else if (value == GuardAreaValue.Right)
                {
                    prefab = tile01;
                    angle = 0;
                }else if (value == GuardAreaValue.Bottom)
                {
                    prefab = tile01;
                    angle = 90;
                }else if (value == GuardAreaValue.Left)
                {
                    prefab = tile01;
                    angle = 180;
                }else if (value == GuardAreaValue.Top)
                {
                    prefab = tile01;
                    angle = -90;
                }else if (value == GuardAreaValue.Zero)
                {
                    if (i + 1 < Constants.EDGE_WIDTH && j - 1 > 0 && guardMap[i + 1, j - 1] == GuardAreaValue.NIL)
                    {
                        prefab = tile03;
                        angle = 0;
                    }else if (i - 1 > 0 && j - 1 > 0 && guardMap[i - 1, j - 1] == GuardAreaValue.NIL)
                    {
                        prefab = tile03;
                        angle = 90;
                    }else if (i - 1 > 0 && j + 1 < Constants.EDGE_HEIGHT && guardMap[i - 1, j + 1] == GuardAreaValue.NIL)
                    {
                        prefab = tile03;
                        angle = 180;
                    }else if (i + 1 < Constants.EDGE_WIDTH && j + 1 < Constants.EDGE_HEIGHT &&
                              guardMap[i + 1, j + 1] == GuardAreaValue.NIL)
                    {
                        prefab = tile03;
                        angle = -90;
                    }
                }
                if (prefab != null)
                {
                    var v1 = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                    v1.transform.parent = this.transform;
                    v1.transform.localRotation = Quaternion.Euler(90, angle, 0);
                    v1.transform.localPosition = new Vector3(i - 0.5f,0,j - 0.5f);
                    if (GameWorld.Instance.worldType == WorldType.Battle)
                    {
                        v1.GetComponent<tk2dSprite>().color = new Color(0.8f,0.2f,0.2f);
                    }
                }
            }
        }
    }

    public void Show(bool delayHide)
    {
        gameObject.SetActive(true);
        if (delayHide)
        {
            StopAllCoroutines();
            StartCoroutine(DelayHide());
        }
    }

    private IEnumerator DelayHide()
    {
        yield return new WaitForSeconds(Constants.DELAY_HIDE_GUARD_AREA_VIEW);
        Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
