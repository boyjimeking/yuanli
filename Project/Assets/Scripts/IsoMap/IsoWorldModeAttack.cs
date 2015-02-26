using System;
using System.Collections.Generic;
using TouchScript;
using TouchScript.Gestures;
using TouchScript.Gestures.Simple;
using UnityEngine;
using System.Collections;

public class IsoWorldModeAttack : IsoWorldMode {

	enum SpawnState
	{
		None,
		Spawn
	}
	
	private int spawnId;

	private SpawnState state;
    private SpawnState State
    {
        get { return state; }
        set
        {
            state = value;
            time = Time.time;
        }
    }
    private float time;

    public void SetSpawnId(int id)
	{
        spawnId = id;
	}
    #region Input

    protected override void OnPan(Vector2 screenPosition, Vector3 deltaPosition)
    {
        if (State != SpawnState.Spawn)
        {
            base.OnPan(screenPosition,deltaPosition);
        }
    }

    protected override void OnTap(Vector2 screenPosition)
    {
        SpawnAtScreenPosition(screenPosition);
    }

    protected override void OnLongPress(Vector2 screenPosition)
    {
        int x, y;
        if (IsoHelper.ScreenPositionToEdge(screenPosition, out x, out y))
        {
            State = SpawnState.Spawn;
        }
    }

    protected override void OnRelease()
    {
        if (TouchManager.Instance.NumberOfTouches == 0)
        {
            StopSpawn();
        }
    }

    protected override bool OnZoomStart()
    {
        if(state == SpawnState.Spawn)
        {
        	return false;
        }
        return true;
    }
    #endregion input

    protected override void OnEnter()
    {
        base.OnEnter();
    }

    protected override void OnExit()
    {
        StopSpawn();
        base.OnExit();
    }

    private void StopSpawn()
	{
		State = SpawnState.None;
	}

    void SpawnAllFingers()
    {
        foreach (var activeTouch in TouchManager.Instance.ActiveTouches)
        {
            SpawnAtScreenPosition(activeTouch.Position);
        }
    }

    void SpawnAtScreenPosition(Vector2 pos)
    {
        int x, y;
        IsoHelper.ScreenPositionToEdge(pos, out x, out y);
        x = Mathf.Clamp(x, 0, Constants.EDGE_WIDTH-1);
        y = Mathf.Clamp(y, 0, Constants.EDGE_HEIGHT-1);
        BattleManager.Instance.PlayerPlaceSoldierOrSkill(spawnId, x, y);
    }

    public override void Update(float dt)
    {
        if (state == SpawnState.Spawn)
        {
            if (Time.time > time + Constants.SPAWN_INTERVAL_TIME)
            {
                time = Time.time;
                SpawnAllFingers();  //  TODO：回放下兵时机不对（待处理
            }
        }
        else
        {
            base.Update(dt);
        }
    }
}
