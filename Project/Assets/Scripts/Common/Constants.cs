public class Constants
{
    public const int UI_WIDTH = 1024;
    public const int UI_HEIGHT = 768;
    public const int LOGIC_FPS = 20;//逻辑帧率
    public const bool ISCLIENT = false;//是否本地客户端
    public const float MAX_CAMERA_ZOOM = 1.3f;  //相机最大放大
    public const float ADJUST_CAMERA_ZOOM = 0.9f;//相机缩放超过此值后自动缩小到此值
    public const float ADJUST_CAMERA_SPEED = 5f;
    public const float MIN_CAMERA_ZOOM = 0.18f; //相机最小放大.

    public const int WIDTH = 88;                ///<    地图格子数 80可建造区域,周围8格不能建造,可下兵,可产生场景装饰
    public const int HEIGHT = 88;

    public const int SAFE_AREA_WIDTH = 4;       //      在四周这个区域内不能建造

    public const int EDGE_WIDTH = WIDTH + 1;    ///<    边数  ※ 士兵等寻路按照边行走（所以边比格子数多1）
    public const int EDGE_HEIGHT = HEIGHT + 1;

    public const string API_URL = "http://192.168.1.197:8080/apis/reqWrapper";
//    public const string API_URL = "http://192.168.1.211:8080/pureland-core/apis/reqWrapper";
    public const string EMPTY = "None";         ///<    数据表里的空
                                                ///
    public const string BUFF_TYPE_SPPEDUP = "speed";        //  buff 类别（速度提升
    public const string BUFF_TYPE_ATTACKUP = "fire";        //  buff 类别（攻击力提升
    public const string BUFF_TYPE_MABI = "palsy";           //  buff 类别（麻痹状态

    public const float STEALABLE_RATIO_STORAGE = 0.25f;  ///<    可以偷的比例,仓库
    public const float MAX_STEALABLE_STORAGE = 200000f;  ///<    仓库可以被偷的最大量,生成器没有限制
    public const float STEALABLE_RATIO_RESOURCE = 0.5f;  ///<    可以偷的比例,生成器

    public const int SPLASH_FULL_DAMAGE_MAX = 10;           ///<    溅射 全伤害最大目标数
    public const float SPLASH_PARTIAL_DAMAGE_RATIO = 0.2f;  ///<    溅射 部分伤害的伤害比例

    public const int COLLECTABLE_ICON_VERTICAL_HEIGHT = 6;  //可收获图标的高度

    public const int HPBAR_VERTICAL_HEIGHT = 3;             //血条的高度

    public const float SPAWN_INTERVAL_TIME = 0.2f;          //进攻时,长按后产生士兵的时间间隔

    public const float FLY_HEIGHT = 2.5f;                     //飞行单位飞的高度

    public const float SUCK_BLOOD_RATIO = 0.2f;             //  特殊技能-吸血 比例

    public const float FEDERAL_DISPATCH_TROOPS_SPACE = 0.5f;    //  公会建筑出兵间隔时间（单位：秒）
    public const float DELAY_HIDE_GUARD_AREA_VIEW = 3f;

    public const int DENOTED_ARMY_ID = 0;               //进攻时,援军的图片代表的id

    public const float FLOOR_Z_ORDER = 50f;             //建筑下面的地砖离相机的位置
    public const float RANGE_Z_ORDER = 40f;             //射程离相机的位置,值越大离相机越远
    public const float BLIND_RANGE_Z_ORDER = 30f;       //盲点射程离相机的位置
    public const float GUARD_VIEW_Z_ORDER = 20f;        //防御范围
    public const float SHADOW_Z_ORDER = 3f;             //阴影离相机的位置
    public const float EFFECT_Z_ORDER = -5f;            //特效离相机的位置
}
