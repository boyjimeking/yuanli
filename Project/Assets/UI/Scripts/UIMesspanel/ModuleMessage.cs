using UnityEngine;
using System.Collections;

public class ModuleMessage : IModule
{
    public string ModuleName()
    {
        return GameModule.MODULE_MESSAGE;
    }

    public void ClearModule()
    {
        throw new System.NotImplementedException();
    }
}
