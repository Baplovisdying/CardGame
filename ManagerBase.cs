using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ManagerBase : MonoBehaviour
{
    public virtual void Init() {}
}

public abstract class ManagerBase<T> : ManagerBase where T : ManagerBase<T>
{
    public static T Instance;

    public override void Init()
    {
        if(Instance==null)
        {
            Instance = this as T;
        }
    }
}
