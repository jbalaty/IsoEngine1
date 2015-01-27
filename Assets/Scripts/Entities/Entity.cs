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
        public Movement Movement;
        public MapProxy MapProxy;
        public TextMeshSpawner TextMeshSpawner;
        public ETileLayer MapLayer = ETileLayer.Object0;
        [SerializeField]
        public bool IsWalkable = true;
        public Vector2Int StartPosition;



        protected void Awake()
        {
            Movement = this.GetComponent<Movement>();
            MapProxy = this.GetComponent<MapProxy>();
            TextMeshSpawner = this.GetComponent<TextMeshSpawner>();
            if (MapProxy == null)
            {
                MapProxy = this.gameObject.AddComponent<MapProxy>();
            }
        }
        protected void Start()
        {
            EntitiesManager = GameObject.Find("Entities").GetComponent<EntitiesManager>();
            EntitiesManager.RegisterEntity(this);
            StartPosition = GetTilePosition();
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
        public Vector2Int GetTilePosition(bool getMovementNextPosition = false)
        {
            if (getMovementNextPosition && Movement != null)
            {
                return this.Movement.NextPosition;
            }
            else
            {
                return new Vector2Int(this.transform.position, EVectorComponents.XZ);
            }
        }

        public void Log(string message, Object context = null)
        {
            Debug.Log(this.name + ": " + message, context);
        }
    }
}