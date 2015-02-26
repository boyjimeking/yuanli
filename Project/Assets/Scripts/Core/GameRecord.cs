using com.pureland.proto;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameRecord
{
    [Serializable]
    abstract class BaseOperation
    {
        public int frame;
        public BattleOperationVO.BattleOperationType operationId;

        public abstract void Replay();
    }

    [Serializable]
    class OpPlaceSoldier : BaseOperation
    {
        public int x;
        public int y;
        public int soldierId;

        public override void Replay()
        {
            BattleManager.Instance.PlayerPlaceSoldierOrSkill(soldierId, x, y,false);
        }
    }

    public static BattleReplayVO GetRecordData()
    {
        return _recordData;
    }
    public static int Frame
    {
        get { return _frame; }
    }

    //  录像时数据
    private static BattleReplayVO _recordData = null;

    //  回放时候的操作数据
    private static LinkedList<BaseOperation> _operation = new LinkedList<BaseOperation>();

    //  录像帧
    private static int _frame = 0;

    private static bool _battleEnded = false;

    /// <summary>
    /// 开始录像
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    public static void StartRecord()
    {
#if UNITY_EDITOR
        BattleManager.logClear();
#endif  //  UNITY_EDITOR

        BattleManager.Instance.BattleStartTime = ServerTime.Instance.Now();

        _battleEnded = false;
        _frame = 0;
        _recordData = new BattleReplayVO()
        {
            seed = BattleRandom.Range(0,0xffff),
            battleStartTime = DateTimeUtil.DateTimeToUnixTimestampMS(BattleManager.Instance.BattleStartTime),
        };
        BattleRandom.SetSeed((int)_recordData.seed);
    }

    /// <summary>
    /// 开始回放
    /// </summary>
    /// <param name="vo"></param>
    public static void StartReplay(BattleReplayVO vo)
    {
#if UNITY_EDITOR
        BattleManager.logClear();
#endif  //  UNITY_EDITOR

        BattleManager.Instance.BattleStartTime = DateTimeUtil.UnixTimestampMSToDateTime(vo.battleStartTime);

        _recordData = vo;

        _operation.Clear();
        //  REMARK：把List转换为LinkedList，提高remove的效率。
        foreach (var op in vo.battleInputs)
        {
            _operation.AddLast(new OpPlaceSoldier() 
            { 
                frame = op.frame, 
                operationId = op.battleOperationType, 
                x = op.battleOperationPlaceSoldierVO.x, 
                y = op.battleOperationPlaceSoldierVO.y, 
                soldierId = op.battleOperationPlaceSoldierVO.cid 
            });
        }

        //  设置随机数种子（TODO：类型不一致）
        BattleRandom.SetSeed((int)vo.seed);

        //  从0帧开始
        _frame = 0;

        _battleEnded = false;
    }

    public static void Update(float dt)
    {
        Assert.Should(_operation != null);
        if(_battleEnded)
            return;

        switch (GameWorld.Instance.worldType) 
        {
            case WorldType.Battle:
                {
                    ++_frame;
                }
                break;
            case WorldType.Replay:
                {
                    while (_operation.Count > 0 && _frame >= _operation.First.Value.frame)
                    {
                        _operation.First.Value.Replay();
                        _operation.RemoveFirst();
                    }
                    //  增加帧数并判断录像结束
                    if (++_frame >= _recordData.battleDuration)
                    {
                        BattleManager.Instance.ForceBattleEnd();
                    }
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 记录放兵
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="soldierId"></param>
    public static void RecordPlaceSoldier(int x, int y, int soldierId)
    {
        _recordData.battleInputs.Add(new BattleOperationVO()
        {
            frame = _frame,
            battleOperationType = BattleOperationVO.BattleOperationType.PlaceSoldier,
            battleOperationPlaceSoldierVO = new BattleOperationPlaceSoldierVO() { cid = soldierId, x = x, y = y }
        });
    }

    public static void OnBattleEnd()
    {
        _battleEnded = true;
        _recordData.battleDuration = _frame;
        Debug.Log("Battle End:" + _frame);
#if UNITY_EDITOR
        if (GameWorld.Instance.worldType != WorldType.Replay)
        {
            Commit();
        }
        if (GameWorld.Instance.worldType != WorldType.Replay)
        {
            File.WriteAllLines(Application.streamingAssetsPath + "/battle_logDamageInfo.txt", BattleManager.logDamageList.ToArray());
            File.WriteAllLines(Application.streamingAssetsPath + "/battle_logMoveInfo.txt", BattleManager.logMoveList.ToArray());
            File.WriteAllLines(Application.streamingAssetsPath + "/battle_logFireBullet.txt", BattleManager.logFireBullet.ToArray());
            File.WriteAllLines(Application.streamingAssetsPath + "/battle_logTryLockTargeter.txt", BattleManager.logTryLockTargeter.ToArray());
            File.WriteAllLines(Application.streamingAssetsPath + "/battle_logWaitTime.txt", BattleManager.logWaitTime.ToArray());
        }
        else
        {
            File.WriteAllLines(Application.streamingAssetsPath + "/replay_logDamageInfo.txt", BattleManager.logDamageList.ToArray());
            File.WriteAllLines(Application.streamingAssetsPath + "/replay_logMoveInfo.txt", BattleManager.logMoveList.ToArray());
            File.WriteAllLines(Application.streamingAssetsPath + "/replay_logFireBullet.txt", BattleManager.logFireBullet.ToArray());
            File.WriteAllLines(Application.streamingAssetsPath + "/replay_logTryLockTargeter.txt", BattleManager.logTryLockTargeter.ToArray());
            File.WriteAllLines(Application.streamingAssetsPath + "/replay_logWaitTime.txt", BattleManager.logWaitTime.ToArray());
        }
#endif  //  UNITY_EDITOR
    }

    public static void Commit()
    {
        //  TODO：这里临时保存到文件（应该提交到服务器）
        if(!_battleEnded)
            OnBattleEnd();
        _recordData.attacker = BattleManager.Instance.attackerData;
        _recordData.defender = BattleManager.Instance.defenderData;
        _recordData.Serialize(Application.streamingAssetsPath + "/record.pbd");
    }
}