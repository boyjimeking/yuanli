using System;
using com.pureland.proto;
using UnityEngine;

public enum EntityType  //顺序很重要,判断大类别
{
    None = 0,
    Tower = 1,
    Resource = 2,
    OtherBuilding = 3,
    __NonWallBuildingEnd,

    Wall = 10,      ///<    墙
    __BuildingEnd,

    Deco = 15,      ///<    场景装饰（树木等
    __BlockageEnd,

    Trap = 20,      ///<    陷阱等
    __CostMapGridEnd, //占用地图格子

    Actor = 30,
    ActorFly,
    __ActorEnd,

    Functional,     //功能类型,保护盾,宝箱等
    Skill,          //技能
}

public enum EntityAiType
{
    PriorToWall = EntityType.Wall,          ///<    优先攻击墙
    PriorToTower = EntityType.Tower,        ///<    优先攻击炮塔
    PriorToResource = EntityType.Resource,  ///<    优先攻击资源建筑
    Other = EntityType.None,                ///<    其他：优先攻击最近的
}

/// <summary>
/// 子弹类型
/// </summary>
public enum EntityBulletType
{
    None = 0,
    Point = 1,              //  针对地点（子弹到达地点后再计算目标） ※ 都有弹道、可以溅射
    Target = 2,             //  针对目标（会跟踪） ※ 都有弹道、可以溅射
    Direct = 3,             //  直接伤害 ※ 可以没弹道、没溅射（扇形、激光直线、高速机枪等属于这类）
    Line = 4,               //  直接伤害 ※ 直线型
    LoopEffectLock = 5,     //  循环（持续）效果（对准目标）
    UnLoopEffectLink = 6,   //  非循环效果（连接目标）
    LoopEffectLink = 7,     //  循环效果（连接目标）

    Max
}

public class BulletTypeUtil
{
    /// <summary>
    /// 子弹是否是打直线型
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static bool IsLine(EntityBulletType t)
    {
        return t == EntityBulletType.Line;
    }

    /// <summary>
    /// 是否是直接伤害型子弹
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static bool IsDirectDamage(EntityBulletType t)
    {
        switch (t)
        {
            case EntityBulletType.Direct:
            case EntityBulletType.Line:
            case EntityBulletType.LoopEffectLock:
            case EntityBulletType.UnLoopEffectLink:
            case EntityBulletType.LoopEffectLink:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// 是否需要发射子弹
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static bool IsFireBullet(EntityBulletType t)
    {
        switch (t)
        {
            case EntityBulletType.Point:
            case EntityBulletType.Target:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// 子弹是否是循环效果
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static bool IsLoopEffect(EntityBulletType t)
    {
        switch (t)
        {
            case EntityBulletType.LoopEffectLock:
            case EntityBulletType.LoopEffectLink:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// 子弹是否是非循环效果（REMARK：注意 循环效果+非循环效果也不是全部子弹类型、还有发射型等）
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static bool IsUnLoopEffect(EntityBulletType t)
    {
        return t == EntityBulletType.UnLoopEffectLink;
    }
}

public class EntityTypeUtil
{
    /// <summary>
    /// 任意目标对象
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsAnyTargeter(EntityType type)
    {
        return (IsAnyBuilding(type) || IsAnyActor(type));
    }

    /// <summary>
    /// 是否是建筑型对象(包括墙)
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsAnyBuilding(EntityType type)
    {
        return type < EntityType.__BuildingEnd;
    }

    /// <summary>
    /// 是否是建筑型对象(不包括墙)
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsAnyNonWallBuilding(EntityType type)
    {
        return type < EntityType.__NonWallBuildingEnd;
    }

    /// <summary>
    /// 是否是阻塞型对象
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsAnyBlockage(EntityType type)
    {
        return type < EntityType.__BlockageEnd;
    }

    public static bool IsCostMapGrid(EntityType type)
    {
        return type < EntityType.__CostMapGridEnd;
    }

    public static bool IsAnyActor(EntityType type)
    {
        return type >= EntityType.Actor && type < EntityType.__ActorEnd;
    }

    public static bool IsAnyTower(EntityType type)
    {
        return type == EntityType.Tower;
    }
    /// <summary>
    /// 是否是陷阱
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static bool IsAnyTrap(EntityModel model)
    {
        return model.entityType == EntityType.Trap;
    }

    /// <summary>
    /// 是否是飞天陷阱
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static bool IsFlyTrap(EntityModel model)
    {
        return IsAnyTrap(model) && (model.subType == "FlyMineA" || model.subType == "FlyMineB");
    }

    /// <summary>
    /// 是否是可飞行兵种
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsFlyable(EntityType type)
    {
        return type == EntityType.ActorFly;
    }

    public static bool IsSkill(EntityModel model)
    {
        return model.entityType == EntityType.Skill;
    }

    /// <summary>
    /// 是否是基地
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static bool IsCenterBuilding(EntityModel model)
    {
        return model.subType == "Base";
    }
    /// <summary>
    /// 是否是兵工厂
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static bool IsArmyShop(EntityModel model)
    {
        return model.subType == "Army";
    }

    /// <summary>
    /// 是否是技能工厂
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static bool IsSkillShop(EntityModel model)
    {
        return model.subType == "Laboratory";
    }
    /// <summary>
    /// 是否是军营
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static bool IsBarracks(EntityModel model)
    {
        return model.subType == "Barracks";
    }

    /// <summary>
    /// 是否是资源生成器
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static bool IsGatherResourceBuilding(EntityModel model)
    {
        return model.resourceType != ResourceType.None && model.resourcePerSecond > 0;
    }

    /// <summary>
    /// 是否是资源存储器
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static bool IsStorageResoruceBuilding(EntityModel model)
    {
        return model.resourceType != ResourceType.None && Math.Abs(model.resourcePerSecond) < Mathf.Epsilon;
    }

    /// <summary>
    /// 是否是能生产的建筑
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static bool IsAnyProductBuilding(EntityModel model)
    {
        return IsArmyShop(model) || IsSkillShop(model);
    }
    /// <summary>
    /// 是否是公会建筑
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static bool IsFederal(EntityModel model)
    {
        return model.subType == "Federal";
    }
    /// <summary>
    /// 是否是建筑工人小屋
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static bool IsWorkerHouse(EntityModel model)
    {
        return model.subType == "WorkHouse";
    }
    /// <summary>
    /// 是否是工人
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static bool IsWorker(EntityModel model)
    {
        return model.subType == "Worker";
    }
    /// <summary>
    /// 是否是治疗兵
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static bool IsCurer(EntityModel model)
    {
        return model.cure > 0;
    }
    /// <summary>
    /// 是否是炸弹人
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static bool IsBombMan(EntityModel model)
    {
        //  TODO
        return model.subType == "Esoldier";
    }


    /// <summary>
    /// [特殊技能] 穿墙
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static bool IsTraitOverTheWall(EntityModel model)
    {
        return model.subType == "TDsoldier";
    }

    /// <summary>
    /// [特殊技能] 攻击吸血（吸收一定比例的伤害值回血）
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static bool IsTraitSuckBlood(EntityModel model)
    {
        return model.subType == "TBsoldier";
    }

    /// <summary>
    /// [特殊技能] 召唤师
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static bool IsTraitSummoner(EntityModel model)
    {
        return model.subType == "TCsoldier";
    }

    /// <summary>
    /// [特殊技能] 瞬间移动（不能穿墙）
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static bool IsTraitFlashMove(EntityModel model)
    {
        return model.subType == "TFsoldier";
    }

    /// <summary>
    /// [特殊技能] 引导（即死） ※ 仅针对塔等静态目标
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static bool IsTraitImmediatelyDie(EntityModel model)
    {
        return model.subType == "TGsoldier";
    }

    /// <summary>
    /// [特殊技能] 上帝的祝福（死亡后周围友军大回血）
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static bool IsTraitBlessing(EntityModel model)
    {
        return model.subType == "TEsoldier";
    }
    /// <summary>
    /// Diamond
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static bool IsDiamond(EntityModel model)
    {
        return model.subType == "Diamond";
    }
    /// <summary>
    /// Gold
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static bool IsGold(EntityModel model)
    {
        return model.subType == "Gold";
    }
    /// <summary>
    /// Oil
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static bool IsOil(EntityModel model)
    {
        return model.subType == "Oil";
    }
    /// <summary>
    /// Medal
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static bool IsMedal(EntityModel model)
    {
        return model.subType == "Medal";
    }

    /// <summary>
    /// 是否是研究所,升级技能的地方
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public static bool IsResearch(EntityModel model)
    {
        return model.subType == "Research";
    }
}