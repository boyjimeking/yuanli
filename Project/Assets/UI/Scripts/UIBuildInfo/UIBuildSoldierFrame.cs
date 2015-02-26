using UnityEngine;
using System.Collections;
using com.pureland.proto;
using System.Collections.Generic;

public class UIBuildSoldierFrame : MonoBehaviour
{
    //兵种
    public GameObject prefabBuildSoldier;
    //Frame标题
    public UILabel txtTip;
    //内容信息区域
    public Transform soldierCon;
    //建筑描述
    public UILabel txtBuildDes;
    //删除容器
    public GameObject deleteCon;
    //删除提示文本
    public UILabel txtDeleteTip;
    //确定删除按钮
    public GameObject btnConfirmDelete;
    //取消按钮
    public GameObject btnCancelDelete;
    //兵种数据记录
    private Dictionary<string, GameObject> dicSoldierData = new Dictionary<string, GameObject>();
    //记录建筑信息
    private TileEntity tileEntity;
    void OnEnable()
    {
        UIEventListener.Get(btnConfirmDelete).onClick += OnClickButton;
        UIEventListener.Get(btnCancelDelete).onClick += OnClickButton;
    }
    void OnDisable()
    {
        UIEventListener.Get(btnConfirmDelete).onClick -= OnClickButton;
        UIEventListener.Get(btnCancelDelete).onClick -= OnClickButton;
    }

    private void OnClickButton(GameObject go)
    {
        if (go.Equals(btnCancelDelete))
        {
            txtBuildDes.gameObject.SetActive(true);
            deleteCon.SetActive(false);
            if (EntityTypeUtil.IsBarracks(tileEntity.model))
            {
                UpdateBarrackSoldierCon();
            }
            else if (EntityTypeUtil.IsFederal(tileEntity.model))
            {

            }
        }
        else if (go.Equals(btnConfirmDelete))
        {
            if (EntityTypeUtil.IsBarracks(tileEntity.model))
            {
                DeleteMyArmies();
            }
            else if (EntityTypeUtil.IsFederal(tileEntity.model))
            {

            }
        }
    }

    private void DeleteMyArmies()
    {
        List<ArmyVO> armies = DataCenter.Instance.Defender.armies;
        List<ArmyVO> deleteArmies = new List<ArmyVO>();
        foreach (ArmyVO armyVO in armies)
        {
            ArmyVO soldierArmyVO = dicSoldierData[ModelUtil.GetEntityModel(armyVO.cid).subType].GetComponent<UIBuildSoldier>().SoldierVO;
            if (armyVO.amount != soldierArmyVO.amount)
            {
                deleteArmies.Add(new ArmyVO() { cid = soldierArmyVO.cid, amount = armyVO.amount - soldierArmyVO.amount });
            }
        }
        if (deleteArmies.Count > 0)
        {

        }
    }
    public void SetSoldierInfo(TileEntity tileEntity)
    {
        this.tileEntity = tileEntity;
        if (EntityTypeUtil.IsBarracks(tileEntity.model))
        {
            //刷兵营的兵种
            txtTip.text = "所有军队:";
            UpdateBarrackSoldierCon();
        }
        else if (EntityTypeUtil.IsFederal(tileEntity.model))
        {
            //联盟兵种

        }
    }
    /// <summary>
    /// 刷新兵种信息
    /// </summary>
    private void UpdateBarrackSoldierCon()
    {
        foreach (KeyValuePair<string, GameObject> keyValuePair in dicSoldierData)
        {
            GameObject.Destroy(keyValuePair.Value);
        }
        dicSoldierData.Clear();
        List<ArmyVO> armies = DataCenter.Instance.Defender.armies;
        for (int i = 0, imax = armies.Count; i < imax; i++)
        {
            GameObject obj = (GameObject)GameObject.Instantiate(prefabBuildSoldier, Vector3.zero, Quaternion.identity);
            obj.transform.localScale = Vector3.zero;
            obj.transform.localPosition = new Vector3(-340 + i * (obj.GetComponent<UISprite>().width + 20), 0);
            obj.SetActive(true);
            obj.GetComponent<UIBuildSoldier>().SoldierVO = new ArmyVO() { cid = armies[i].cid, amount = armies[i].amount };
            obj.GetComponent<UIBuildSoldier>().ClickReduceButton += ReduceSoldier;
            dicSoldierData.Add(ModelUtil.GetEntityModel(armies[i].cid).subType, obj);
        }
    }
    private void ReduceSoldier()
    {
        txtBuildDes.gameObject.SetActive(false);
        deleteCon.SetActive(true);
    }
}
