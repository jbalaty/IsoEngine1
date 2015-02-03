using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Dungeon
{
    public class InventoryPanelItem : MonoBehaviour
    {
        InventoryItem InventoryItem;
        public InventoryPanel InventoryPanel;
        // Use this for initialization
        void Start()
        {
        }

        public void SetupItem(InventoryItem inventoryitem)
        {
            InventoryItem = inventoryitem;
            foreach (Transform t in this.transform)
            {
                if (t.name == "Image")
                {
                    var img = t.GetComponent<Image>();
                    img.sprite = inventoryitem.Item.Icon;
                }
                else if (t.name == "Name")
                {
                    var txt = t.GetComponent<Text>();
                    txt.text = inventoryitem.Item.Name;
                }
                else if (t.name == "Amount")
                {
                    var txt = t.GetComponent<Text>();
                    txt.text = "" + inventoryitem.Amount;
                }
            }
        }

        public void Take(float amount){
            InventoryPanel.Take(this.InventoryItem);
            GameObject.Destroy(this.gameObject);
        }

        public void ShowDetail()
        {
            InventoryPanel.ShowDetail(this.InventoryItem);
        }
    }
}