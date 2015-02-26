using System;
using com.pureland.proto;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DebugTileMap : MonoBehaviour
{
    private bool paused = false;
    private int replaySpeed = 1;
    private int oldReplaySpeed;
    // Use this for initialization
    void Start()
    {
        GameWorld.Instance.Init();
    }

    void FixedUpdate()
    {
        for (int i = 0; i < replaySpeed; i++)
        {
            GameWorld.Instance.Update(1f / Constants.LOGIC_FPS);
        }
    }

    void OnGUI()
    {
        return;
        if (GameWorld.Instance.Loading)
            return;

        switch (GameWorld.Instance.worldType)
        {
            case WorldType.Home:
                {
                    //if (GUI.Button(new Rect(120, 400, 50, 50), "出兵"))
                    //{
                    //    GameWorld.Instance.ChangeLoading(WorldType.Battle);
                    //    return;
                    //}
                    //  o(╯□╰)o
                    if (GUI.Button(new Rect(120, 500, 50, 50), "回放"))
                    {
                        GameWorld.Instance.ChangeLoading(WorldType.Replay);
                        return;
                    }
                }
                break;
            case WorldType.Battle:
                {
                    if (GUI.Button(new Rect(120, 400, 50, 50), "回营"))
                    {
                        replaySpeed = 1;
                        Time.timeScale = replaySpeed;
                        //GameWorld.Instance.BackToMyCamp();
                        return;
                    }
                }
                break;
            case WorldType.Replay:
                {
                    if (GUI.Button(new Rect(120, 400, 50, 50), "回营"))
                    {
                        replaySpeed = 1;
                        Time.timeScale = replaySpeed;
                        //GameWorld.Instance.BackToMyCamp();
                        return;
                    }
                    if (GUI.Button(new Rect(200, 400, 50, 50), "x" + replaySpeed))
                    {
                        replaySpeed *= 2;
                        if (replaySpeed > 8)
                            replaySpeed = 1;
                        Time.timeScale = replaySpeed;
                    }
                    if (GUI.Button(new Rect(280, 400, 50, 50), paused ? ">" : "||"))
                    {
                        paused = !paused;
                        if (paused)
                        {
                            oldReplaySpeed = replaySpeed;
                            replaySpeed = 0;
                        }
                        else
                        {
                            replaySpeed = oldReplaySpeed;
                        }
                        Time.timeScale = replaySpeed;
                    }
                }
                break;
            case WorldType.Visit:
                break;
            default:
                break;
        }

        if (GameWorld.Instance.worldType == WorldType.Home)
        {
            if (GUI.Button(new Rect(Screen.width - 200, Screen.height - 200, 50, 50), "建造"))
            {
                var model = DataCenter.Instance.FindEntityModelByResourceName("HtowerA11");
                var tileEntity = TileEntity.Create(OwnerType.Defender, model);
                tileEntity.Init();
                ((IsoWorldModeBuilder)GameWorld.Instance.CurrentWorldMode).SetBuildingObject(tileEntity);
            }
            if (GUI.Button(new Rect(Screen.width - 120, Screen.height - 200, 50, 50), "陷阱"))
            {
                var model = DataCenter.Instance.FindEntityModelByResourceName("HtowerA11");
                model.entityType = EntityType.Trap;
                model.range = 1.5f;
                var tileEntity = TileEntity.Create(OwnerType.Defender, model);
                tileEntity.Init();
                ((IsoWorldModeBuilder)GameWorld.Instance.CurrentWorldMode).SetBuildingObject(tileEntity);
            }
            if (GUI.Button(new Rect(Screen.width - 200, Screen.height - 100, 50, 50), "墙"))
            {
                var wallModel = DataCenter.Instance.FindEntityModelByResourceName("HwallB01");
                var tileEntity = TileEntity.Create(OwnerType.Defender, wallModel);
                tileEntity.Init();
                (GameWorld.Instance.CurrentWorldMode as IsoWorldModeBuilder).SetBuildingObject(tileEntity, true);
            }
            if (GUI.Button(new Rect(Screen.width - 120, Screen.height - 100, 50, 50), "军营"))
            {
                var camp = DataCenter.Instance.FindEntityModelByResourceName("HhouseK01");
                var tileEntity = TileEntity.Create(OwnerType.Defender, camp);
                tileEntity.Init();
                (GameWorld.Instance.CurrentWorldMode as IsoWorldModeBuilder).SetBuildingObject(tileEntity, true);
            }
            if (GUI.Button(new Rect(Screen.width - 200, Screen.height - 500, 100, 100), "保存村庄"))
            {
                var campVO = DataCenter.Instance.Defender;
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                ProtoBuf.Serializer.Serialize<ProtoBuf.IExtensible>(stream, campVO);
                System.Byte[] bytes = stream.ToArray();
                using (FileStream fs = new FileStream(Application.streamingAssetsPath + "/last_save_village.pbd", FileMode.Create))
                {
                    using (BinaryWriter writer = new BinaryWriter(fs))
                    {
                        writer.Write(bytes);
                    }
                }
                Debug.Log("Saved");
            }
            if (GUI.Button(new Rect(Screen.width - 200, Screen.height - 400, 100, 100), "加载村庄"))
            {
                byte[] bytes = File.ReadAllBytes(Application.streamingAssetsPath + "/last_save_village.pbd");
                var stream = new MemoryStream(bytes);
                var campVO = ProtoBuf.Serializer.Deserialize<CampVO>(stream);
                GameWorld.Instance.Create(WorldType.Home, campVO);
            }
            if (GUI.Button(new Rect(Screen.width - 100, Screen.height - 400, 100, 100), "加载村庄-战斗"))
            {
                byte[] bytes = File.ReadAllBytes(Application.streamingAssetsPath + "/last_save_village.pbd");
                var stream = new MemoryStream(bytes);
                var campVO = ProtoBuf.Serializer.Deserialize<CampVO>(stream);
                GameWorld.Instance.Create(WorldType.Battle, campVO, campVO);
            }
        }
        else if (GameWorld.Instance.worldType == WorldType.Battle)
        {
            if (GUI.Button(new Rect(Screen.width - 200, Screen.height - 240, 120, 50), "技能"))
            {
                //  TODO:
                GameSkillManager.Instance.AddSkill(null, 40, 40);
            }
            if (GUI.Button(new Rect(Screen.width - 200, Screen.height - 300, 120, 50), "提交录像"))
            {
                GameRecord.Commit();
            }
            if (GUI.Button(new Rect(20, Screen.height - 200, 100, 50), "进攻-甲壳虫"))
            {
                var actor = DataCenter.Instance.FindEntityModelByResourceName("HsoldierD01");
                //actor.subType = "Esoldier"; // 炸弹人属性
                //actor.hitEffectName = "Effect1";
                //actor.splashRange = 3.0f;
                //actor.additionDamageRatio = 100;
                (GameWorld.Instance.CurrentWorldMode as IsoWorldModeAttack).SetSpawnId(actor.baseId);
            }
            if (GUI.Button(new Rect(20, Screen.height - 270, 100, 50), "进攻-医疗兵"))
            {
                var actor = new EntityModel();
                actor.raceType = RaceType.Predator;
                actor.entityType = EntityType.ActorFly;
                actor.nameForResource = "PsoldierB01";
                actor.hp = 100000;
                actor.speed = 2.5f;
                actor.damage = 10;
                actor.rate = 3.0f;
                actor.bulletType = EntityBulletType.Direct;     //  直接伤害

                actor.cure = 10;    //  医疗兵相关属性
                actor.cureRange = 80;
                actor.range = 10;
                actor.splashRange = 10;

                (GameWorld.Instance.CurrentWorldMode as IsoWorldModeAttack).SetSpawnId(actor.baseId);
            }
        }
    }

}
