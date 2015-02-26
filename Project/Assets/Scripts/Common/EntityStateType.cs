public enum EntityStateType
{
    Idle,       //  发呆中
    Thinking,   //  分步计算中（缓慢思考状态）
    Moving,     //  移动中：仅士兵有效
    Rotating,   //  旋转中：仅炮塔有效
    Attacking,  //  攻击中
    Dead,       //  死亡
}