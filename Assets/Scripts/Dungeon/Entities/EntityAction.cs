using UnityEngine;
using System.Collections;

namespace Dungeon
{
    [System.Serializable]
    public class EntityAction
    {
        public string Name;
        public EntityAction(string name)
        {
            this.Name = name;
        }
    }

    [System.Serializable]
    public class EntityAction<T1> : EntityAction
    {
        public T1 Param0 { get; set; }

        public EntityAction(string name, T1 param0)
            : base(name)
        {
            Param0 = param0;
        }
    }

    [System.Serializable]
    public class EntityAction<T1, T2> : EntityAction
    {
        public T1 Param0 { get; set; }
        public T2 Param1 { get; set; }

        public EntityAction(string name, T1 param0, T2 param1)
            : base(name)
        {
            Param0 = param0;
            Param1 = param1;
        }
    }
}