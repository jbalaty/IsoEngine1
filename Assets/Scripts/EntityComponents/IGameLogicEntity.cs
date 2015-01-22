﻿using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace Dungeon
{

    public interface IGameLogicEntity : IEventSystemHandler
    {
        EntityAction PlanNextAction();
        EntityAction ProcessNextAction();
    }
}
