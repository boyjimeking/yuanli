using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModuleHomeLand : IModule
{
    //家园服务器返回数据
    public com.pureland.proto.CampVO campVO;
    public string ModuleName()
    {
        return GameModule.MODULE_HOMELAND;
    }

    public void ClearModule()
    {
    }
}
