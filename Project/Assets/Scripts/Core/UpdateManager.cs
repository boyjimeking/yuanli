
using System.Collections.Generic;

public class UpdateManager : Singleton<UpdateManager>
{
    private readonly List<IUpdate> updateList = new List<IUpdate>();
    private readonly List<IUpdate> delayedToAdd = new List<IUpdate>();
    private readonly List<IUpdate> delayedToRemove = new List<IUpdate>();
    public void AddUpdate(IUpdate update)
    {
        delayedToAdd.Add(update);
    }

    public void RemoveUpdate(IUpdate update)
    {
        delayedToRemove.Add(update);
    }

    public void Clear()
    {
        updateList.Clear();
        delayedToAdd.Clear();
        delayedToRemove.Clear();
    }

    public void Update(float dt)
    {
        if (delayedToRemove.Count > 0)
        {
            foreach (var update in delayedToRemove)
            {
                updateList.Remove(update);
            }
            delayedToRemove.Clear();
        }
        if (delayedToAdd.Count > 0)
        {
            updateList.AddRange(delayedToAdd);
            delayedToAdd.Clear();
        }
        
        foreach (var update in updateList)
        {
            update.Update(dt);
        }
    }
}
