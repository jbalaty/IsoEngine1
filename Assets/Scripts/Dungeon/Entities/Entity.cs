using UnityEngine;
using System.Collections;
using IsoEngine1;

namespace Dungeon
{
    public class Entity : MonoBehaviour, IGameLogicEntity
    {
        public static EntityAction NoAction = new EntityAction("NoAction");

        public EntitiesManager EntitiesManager;
        [SerializeField]
        public EntityAction CurrentAction = Entity.NoAction;
        [SerializeField]
        public EntityAction NextAction = Entity.NoAction;

        protected void Start()
        {
            EntitiesManager = GameObject.Find("Entities").GetComponent<EntitiesManager>();
            EntitiesManager.RegisterEntity(this);
        }

        void OnDestroy()
        {
            EntitiesManager.DeregisterEntity(this);
        }

        public EntityAction PlanNextAction()
        {
            var result = DoPlanNextAction(CurrentAction); ;
            NextAction = result;
            return result;
        }
        public EntityAction ProcessNextAction()
        {
            CurrentAction = NextAction;
            NextAction = NoAction;
            DoProcessNextAction(CurrentAction);
            return CurrentAction;
        }

        protected virtual EntityAction DoPlanNextAction(EntityAction currentAction)
        {
            return NoAction;
        }

        protected virtual void DoProcessNextAction(EntityAction action)
        {
        }
        public Vector2Int GetTilePosition()
        {
            return new Vector2Int(transform.position, EVectorComponents.XZ);
        }

        public void Log(string message, Object context = null)
        {
            Debug.Log(this.name + ": " + message, context);
        }
    }
}