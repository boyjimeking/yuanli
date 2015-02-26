using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : Singleton<PlayerManager>
{
    public int tempId = 23601;//TODO?
    public List<int> GetSortedArmys(List<int> armysOriginal)
    {
        List<int> armys = new List<int>(armysOriginal);
        armys.Sort(0, armysOriginal.Count, new SortClass());
        return armysOriginal;
    }
    private class SortClass : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            EntityModel modelX = DataCenter.Instance.FindEntityModelById(x);
            EntityModel modelY = DataCenter.Instance.FindEntityModelById(y);
            //都开放了
            if (modelX.buildNeedLevel <= PlayerManager.Instance.tempId && modelY.buildNeedLevel <= PlayerManager.Instance.tempId)
            {
                if (modelX.level < modelY.level)
                    return 1;
                else if (modelX.level > modelY.level)
                    return -1;
                else
                    return 0;
            }
            else if (modelX.buildNeedLevel < PlayerManager.Instance.tempId && modelY.buildNeedLevel > PlayerManager.Instance.tempId)
            {
                return -1;
            }
            else if (modelX.buildNeedLevel > PlayerManager.Instance.tempId && modelY.buildNeedLevel < PlayerManager.Instance.tempId)
            {
                return 1;
            }
            return 0;
        }
    }
}
