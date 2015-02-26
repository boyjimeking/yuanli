using UnityEngine;
using System.Collections;

public class ModuleBuildOpt : IModule
{
    public string ModuleName()
    {
        return GameModule.MODULE_BUILDOPT;
    }

    public void ClearModule()
    {
        throw new System.NotImplementedException();
    }
}
