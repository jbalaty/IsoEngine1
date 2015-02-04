using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Dungeon
{
    public class PlayerInventoryPanelItem : MonoBehaviour
    {
        [HideInInspector]
        public InventoryItem InventoryItem;
        public event System.Action<InventoryItem> ActionButton1ClickedEvent;
        public event System.Action<InventoryItem> ItemClickedEvent;

        protected Button ActionButton1;
        protected Image Image;
        protected Text Name;
        protected Text Amount;

        // Use this for initialization
        void Awake()
        {
            //Image = Utils.FindByNameInChildren(this.transform, "Image", true).GetComponent<Image>();
            //Name = Utils.FindByNameInChildren(this.transform, "Name", true).GetComponent<Text>();
            //Amount = Utils.FindByNameInChildren(this.transform, "Amount", true).GetComponent<Text>();
            ActionButton1 = this.transform.Find("ActionButton1").GetComponent<Button>();
            Image = this.transform.Find("Image").GetComponent<Image>();
            Name = this.transform.Find("Name").GetComponent<Text>();
            Amount = this.transform.Find("Image/Amount").GetComponent<Text>();
        }

        public void SetupItem(InventoryItem inventoryitem)
        {
            InventoryItem = inventoryitem;
            Image.sprite = inventoryitem.Item.Icon;
            Name.text = inventoryitem.Item.Name;
            Amount.text = "" + inventoryitem.Amount;
        }

        public void Take()
        {
            if (ActionButton1ClickedEvent != null) ActionButton1ClickedEvent(InventoryItem);
        }

        public void ShowDetail()
        {
            if (ItemClickedEvent != null) ItemClickedEvent(InventoryItem);
        }
    }
}