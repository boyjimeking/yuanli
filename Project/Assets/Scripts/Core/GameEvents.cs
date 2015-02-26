
public class GameEvents
{
    //资源改变
    public const string RESOURCE_CHANGE = "RESOURCE_CHANGE";    //  data:   ResourceVO
    //人口空间改变
    public const string SPACE_CHANGE = "SPACE_CHANGE";
    //工人数量变化
    public const string WORKER_CHANGE = "WORKER_CHANGE";
    //经验变化
    public const string EXP_CHANGE = "EXP_CHANGE";
    //角色升级
    public const string LEVEL_UP = "LEVEL_UP";
    //偷取资源
    public const string STOLEN_RESOURCE = "STOLEN_RESOURCE";    //  data: ResourceVO
    //战斗下兵或者使用技能 REAMRK 没有士兵了 也会抛事件
    public const string BATTLE_SPAWN = "BATTLE_SPAWN";          //  data: baseId
    //战斗进度变化
    public const string BATTLE_PROGRESS_CHANGE = "BATTLE_PROGRESS_CHANGE";
    //战斗结束
    public const string BATTLE_END = "BATTLE_END";
    //开始拖拽建筑
    public const string BEGIN_DRAG_BUILDING = "BEGIN_DRAG";
    //结束拖拽
    public const string END_DRAG_BUILDING = "END_DRAG";
    //建筑建造或者升级完成
    public const string BUILDING_COMPLETE = "BUILDING_COMPLETE";// data: BuildingVO
    //加载完成 历史战斗记录(进攻+防守)
    public const string BATTLE_HISTORY_LOADED = "BATTLE_HISTORY_LOADED";
    //点击全屏遮罩
    public const string CLICK_SCREEN_MASK = "CLICK_SCREEN_MASK";
    //技能升级
    public const string SKILL_UP = "SKILL_UP";                  // data: baseId 当前等级
    //兵种升级
    public const string SOLDIER_UP = "SOLDIER_UP";              // data: baseId 当前等级
    //兵的数量改变
    public const string SOLDIER_COUNT_CHANGE = "SOLDIER_COUNT_CHANGE";
    //陷阱重置了
    public const string TRAP_REFILL = "TRAP_REFILL";            // data: BuildingVO null for all
}
