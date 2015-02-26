
using System;
using System.Reflection;
using com.pureland.proto;

public class EntityModel
{
    private void FillFieldFrom(object model)
    {
        Type type = model.GetType();
        FieldInfo[] fields = type.GetFields();

        Type thisType = GetType();
        foreach (var field in fields)
        {
            string name = field.Name;
            object data = field.GetValue(model);
            thisType.GetField(name).SetValue(this, data);
        }
    }
    //from building data
    public int baseId;                                      //id
    public int upgradeId;                                   //升级到id
    public RaceType raceType;                               //种族类别
    public EntityType entityType;                           //类型
    public string subType;                                  //子类型
    public string nameForResource;                          //名称,资源用
    public string nameForView;                              //名称,显示相关
    public int level;                                       //等级
    public float attackWeight;                              //攻击评估值,计算综合实力使用
    public float defenseWeight;                             //防御评估值,计算综合实力使用
    public RankType buildNeedType;                          //建造需要的类型
    public int buildNeedLevel;                              //建造需要的id,数值等
    public RankType upgradeNeedType;                        //升级需要的类型
    public int upgradeNeedLevel;                            //升级需要id
    public int hp;                                          //hp
    public ResourceType costResourceType;                   //建造或者升级资源类型
    public int costResourceCount;                           //建造或者升级资源数量
    public int buildTime;                                   //建造或者升级时间(秒)
    public float range;                                     //射程(格) ※ 仅塔有效
    public float blindRange;                                //射程盲区(格) ※ 仅塔有效
    public float rate;                                      //攻击间隔(秒),//炸药爆炸延迟
    public float damage;                                    //伤害
    public float damageForView;                             //伤害,显示相关
    public float splashRange;                               //溅射伤害范围
    public int attackAngle;                                 //扇形攻击区域角度
    public int tileSize;                                    //占地(格)
    public EntityType targetType;                           //目标类型 即EntityAiType值
    public EntityType onlyAttackTargetType = EntityType.None; //仅攻击指定目标类型（和targetType不同，targetType是优先攻击的目标类型。） REMARK：这个有Actor、ActorFly、None三个值，当目标为士兵时候有效）
    public ResourceType resourceType;                       //产资源类型
    public float resourcePerSecond;                         //产资源量每秒
    public int resourcePerSecondForView;                    //产资源量,显示相关
    public int maxResourceStorage;                          //最大资源存储量
    public int buildExp;                                    //建造经验
    public int spaceProvide;                                //人口提供
    public int queueSize;                                   //生产队列长度
    public int workerProvide;                               //工人数量提供
    public int refillCostResourceCount;                     //重置陷阱花费资源数量,类型为Gold
    public bool aimTarget;                                  //炮头是否对准目标（REMARK：目标数量numTarget为1时才有效）
    public float bulletSpeed;                               //子弹速度
    public EntityBulletType bulletType;                     //子弹类型
    public int numTarget;                                   //同时锁定攻击的目标个数（扇形、溅射等多目标该值大部分也为1）REMARK：如果该值大于1，那么对准目标动画无效。
    public string bulletName;                               //子弹名称,资源用
    public string fireEffectName;                           //攻击时使用的动画效果
    public string hitEffectName;                            //命中效果,资源用
    public string buffEffectName;                           //buff效果,资源用
    public string buffType;                                 //buff类型
    public float buffDamage;                                //buff伤害
    public int buffActiveTimes;                             //buff作用次数
    public float buffIntervalTime;                          //buff作用间隔
    public int defense;                                     //防御值
    public int defenseForView;                              //防御值显示用
    public string additionDamageSubType;                    //克制类型,对指定类型有加成效果
    public float additionDamageRatio;                       //克制加成 1.x,2.x
    //兵种独有属性
    public ResourceType trainCostResourceType;              //训练花费资源类型
    public int trainCostResourceCount;                      //训练花费资源
    public int trainTime;                                   //训练时间
    public float speed;                                     //移动速度
    //public AttackRangeType attackRangeType; 去掉了                //攻击距离类型 ※ 仅士兵有效 士兵没有配置range字段 根据该字段确定range大小
    public int cure;                                        //医疗兵治疗量
    public int cureForView;                                 //治疗量显示
    public float cureRange;                                 //医疗兵寻找友军范围
    public int spaceUse;                                    //占人口数量

    public string desc;                                     //描述
}
