using UnityEngine;
using System.Collections;

public class UIProfessionDes : MonoBehaviour
{
    void OnEnable()
    {
        UIEventListener.Get(this.gameObject).onClick += OnClickPanel;
    }

    private void OnClickPanel(GameObject go)
    {
        LoginManager.Instance.ShowProfessionDes(false);
    }
    void OnDisable()
    {
        UIEventListener.Get(this.gameObject).onClick -= OnClickPanel;
    }
}
