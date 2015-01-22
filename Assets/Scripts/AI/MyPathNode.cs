using UnityEngine;
using System.Collections;
using System;

public class MyPathNode : SettlersEngine.IPathNode<System.Object>
{
    public Int32 X;
    public Int32 Y;
    public Boolean IsWalkable;
    public Boolean IsFlyable;

    public bool IsMovement(System.Object unused)
    {
        return IsWalkable;
    }
}
