using System;
using com.pureland.proto;
using Org.BouncyCastle.Ocsp;
using ProtoBuf;
using BestHTTP;

public abstract class NetCommand
{
    private static CampVO lastData; //TODO remove

    //  当前的请求以及停止标记
    private HTTPRequest _request = null;
    private bool _stopped = false;
    public HTTPRequest CurrRequest
    {
        set { _request = value; }
    }
    public bool IsCancelled
    {
        get { return _stopped; }
    }

    //  完成回调
    public Action<BaseResp> CompleteCallback = null;
    public Action TimeoutCallback = null;
    
    /// <summary>
    /// create req object
    /// </summary>
    /// <returns></returns>
    public BaseReq CreateRequest()
    {
        var request = new BaseReq();
        request.authToken = DataCenter.Instance.authToken;
        request.timestamp = ServerTime.Instance.GetTimestamp();
        request.sequenceId = DataCenter.Instance.CreateNextRequestSequenceId();
        if (lastData == null || lastData.player.userId == 0)
            request.before = DataCenter.Instance.originDefenderData;//TODO remove when release
        else
        {
            request.before = lastData;
        }
        request.reqWrapper = Execute();
        if (GameWorld.Instance.worldType == WorldType.Battle)
        {
            request.after = ProtoBuf.Serializer.DeepClone(DataCenter.Instance.Attacker);//TODO remove when release
        }
        else
        {
            request.after = ProtoBuf.Serializer.DeepClone(DataCenter.Instance.Defender);//TODO remove when release
        }
        lastData = request.after;
        return request;
    }

    /// <summary>
    /// 停止尚未相应的请求
    /// </summary>
    public void Cancel()
    {
        if (!_stopped && _request != null)
        {
            _stopped = true;
            try
            {
                _request.Abort();
            }
            catch (Exception)
            {
            }
            _request = null;
        }
    }

    public void ExecuteAndSend()
    {
        NetManager.Instance.Send(this);
    }
    /// <summary>
    /// set req data here
    /// </summary>
    public abstract ReqWrapper Execute();

    /// <summary>
    /// handle resp object
    /// </summary>
    public virtual void OnResponse(BaseResp resp)
    {
        //default omit resp
        if (CompleteCallback != null)
        {
            CompleteCallback(resp);
            CompleteCallback = null;
        }
    }
}