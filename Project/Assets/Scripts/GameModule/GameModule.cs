using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameModule
{
    //家园
    public const string MODULE_HOMELAND = "module_homeland";
    //商店
    public const string MODULE_SHOP = "module_shop";
    //训练兵
    public const string MODULE_PRODUCTSOLDIER = "module_productsoldier";
    //建筑操作
    public const string MODULE_BUILDOPT = "module_buildopt";
    //攻防记录信息
    public const string MODULE_MESSAGE = "module_message";
    //存储模块
    private static Dictionary<string, IModule> hashModule = new Dictionary<string, IModule>();
    /**
     * 添加一个模块
     * @param module 模块实例
     * */
    public static void AddModule(IModule module)
    {
        if (!hashModule.ContainsKey(module.ModuleName()))
        {
            hashModule.Add(module.ModuleName(), module);
        }
    }
    /**
     * 得到模块
     * @param modulelName 模块名
     * */
    public static IModule GetModule(string mouduleName)
    {
        return hashModule[mouduleName];
    }
}
