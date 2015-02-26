using UnityEngine;
using System.Collections;
using com.pureland.proto;

public class FightSearchCommand : NetCommand
{
    private FightSearchReq.SearchType searchType;
    public FightSearchCommand(FightSearchReq.SearchType searchType)
    {
        this.searchType = searchType;
    }
    public override com.pureland.proto.ReqWrapper Execute()
    {
        FightSearchReq fightSearch = new FightSearchReq();
        fightSearch.searchType = this.searchType;
        return new ReqWrapper() { requestType = ReqWrapper.RequestType.FightSearch, fightSearchReq = fightSearch };
    }
    //public override void OnResponse(BaseResp resp)
    //{
    //    if (resp.errorType > 0)
    //    {
    //        GameTipsManager.Instance.ShowGameTips("搜索失败");
    //        return;
    //    }
    //    DataCenter.Instance.SetHomelandData(resp.respWrapper.campResp.campVO);
    //}
}
