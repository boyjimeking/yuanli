using System;
using com.pureland.proto;
using Org.BouncyCastle.Ocsp;
using UnityEngine;
using System.Collections;
using BestHTTP;
using XSharper.Core;

public class NetManager : Singleton<NetManager>
{
    /**
     * 服务器数据响应
     * */
    private void OnServerResponse(HTTPRequest request, HTTPResponse response)
    {
        if (null == response)
        {
            GameTipsManager.Instance.ShowGameTips("网络连接失败");
            return;
        }
        if (response.StatusCode != 200)
        {
            GameTipsManager.Instance.ShowGameTips("网络异常:" + response.StatusCode);
            return;
        }
        if (request != null && (request.Tag as NetCommand).IsCancelled)
        {
            Debug.Log("HttpRequest Cannelled...");
            return;
        }
        System.IO.MemoryStream stream = new System.IO.MemoryStream(response.Data);
        var resp = ProtoBuf.Serializer.Deserialize<BaseResp>(stream);
        Debug.Log((NetCommand)request.Tag + " 接收到服务器数据:" + Dump.ToDump(resp, new DumpSettings() { MaxDepth = 10 }));
        if (resp.errorType > 0)
        {
            Debug.Log("HttpRequest Error:\n" + resp.errorType);
        }
        if (resp.errorType == BaseResp.ErrorType.IsInFight)
        {
            LoginManager.Instance.RequestEnterGameReCycle();
            NGUIDebug.Log("战斗轮询登陆");
            return;
        }
        else if (resp.errorType == BaseResp.ErrorType.IsOffLine)
        {
            LoginManager.Instance.RequestEnterGame();
            NGUIDebug.Log("下线重新登陆");
            return;
        }
        if (resp.errorMessage != null && resp.errorMessage != "")
        {
            GameTipsManager.Instance.ShowServerErrorMsg(resp.errorMessage);
            return;
        }
        ((NetCommand)request.Tag).OnResponse(resp);
    }

    public void Send(NetCommand command)
    {
        var baseReq = command.CreateRequest();

        if (Constants.ISCLIENT) return;

        Debug.Log(command + " 发送的数据数据:" + Dump.ToDump(baseReq, new DumpSettings() { MaxDepth = 10 }));
        System.IO.MemoryStream stream = new System.IO.MemoryStream();
        ProtoBuf.Serializer.Serialize<ProtoBuf.IExtensible>(stream, baseReq);
        System.Byte[] bs = stream.ToArray();
        HTTPRequest request = new HTTPRequest(new System.Uri(Constants.API_URL), HTTPMethods.Post, OnServerResponse);
        request.Tag = command;
        request.RawData = bs;
        //  绑定
        command.CurrRequest = request;
        //  发送数据
        request.Send();
    }
}
