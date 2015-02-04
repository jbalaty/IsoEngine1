using UnityEngine;
using System.Collections.Generic;


namespace Dungeon
{
    public class PlayerInventoryPanelItems : MonoBehaviour
    {

        protected Transform ContentList;
        public Transform ContentListItemPrefab;
        public event System.Action<InventoryItem> ItemActionButton1ClickedEvent;
        public event System.Action<InventoryItem> ItemClickedEvent;
        int Counter;
        // Use this for initialization
        void Awake()
        {
            ContentList = this.transform.Find("ItemsContent/Content");
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Setup(IEnumerable<InventoryItem> items, Transform itemPrefab)
        {
            ContentList = this.transform.Find("ItemsContent/Content");
            ContentListItemPrefab = itemPrefab;
            Refresh(items);
        }

        public void Refresh(IEnumerable<InventoryItem> items)
        {
            Clear();
            foreach (var item in items)
            {
                AddItem(item);
            }
        }

        void AddItem(InventoryItem item)
        {
            Debug.Log("Adding inv item");
            var newitem = Instantiate(ContentListItemPrefab) as Transform;
            newitem.name += "_" + Counter++;
            newitem.SetParent(ContentList.transform);
            var itemScript = newitem.GetComponent<PlayerInventoryPanelItem>();
            itemScript.ActionButton1ClickedEvent += ItemActionButton1Clicked;
            itemScript.ItemClickedEvent += ItemClicked;
            itemScript.SetupItem(item);
        }

        void ItemClicked(InventoryItem obj)
        {
            if (ItemClickedEvent != null) ItemClickedEvent(obj);
        }

        void ItemActionButton1Clicked(InventoryItem obj)
        {
            if (ItemActionButton1ClickedEvent != null) ItemActionButton1ClickedEvent(obj);

        }

        public void Clear()
        {
            foreach (Transform child in ContentList.transform)
            {
                child.gameObject.SetActive(false);
                GameObject.Destroy(child.gameObject);
            }
        }

        public InventoryItem GetItem(int idx)
        {
            InventoryItem result = null;
            var t = ContentList.GetChild(idx);
            if (t != null)
            {
                result = t.GetComponent<PlayerInventoryPanelItem>().InventoryItem;
            }
            return result;
        }

        public int Count
        {
            get
            {
                return ContentList.transform.childCount;
            }
        }


    }
}
