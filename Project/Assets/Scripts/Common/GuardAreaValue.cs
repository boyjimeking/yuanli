using System;

/// <summary>
/// 战斗中建筑防御区域（该区域内不可放兵）值的枚举
/// </summary>
[Flags]
public enum GuardAreaValue
{
    NIL = 0x01,                         //  非防御区域

    Zero = 0x00,                        //  防御区域 无方向

    Top = 0x10,                         //  防御区域 上
    Bottom = 0x20,                      //  防御区域 下
    Left = 0x40,                        //  防御区域 左
    Right = 0x80,                       //  防御区域 右

    TopRight = Top | Right,             //  防御区域 两方向
    TopLeft = Top | Left,
    BottomLeft = Bottom | Left,
    BottomRight = Bottom | Right,

    All = Top | Bottom | Left | Right,  //  防御区域 四方向

    Uninitialized = 0xffff,             //  防御区域 ※ 尚未初始化方向
}
