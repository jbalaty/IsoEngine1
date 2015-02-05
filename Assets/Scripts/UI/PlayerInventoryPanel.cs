using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


namespace Dungeon
{
    public interface IInventoryPanel
    {
        void Setup(Inventory inv1, Inventory inv2);
        void SetName(string name);
    }

    public class PlayerInventoryPanel : MonoBehaviour, IInventoryPanel
    {
        Inventory SourceInventory;
        Inventory TargetInventory;

        public PlayerInventoryPanelItems ItemsPanel;
        public Transform ContentListItemPrefab;

        public GameObject ItemDetail;
        Slider ItemDetailSlider;
        Text ItemDetailText;
        Button DropButton;
        InventoryItem ItemDetailInventoryItem;

        // Use this for initialization
        void Start()
        {
            ItemsPanel.ItemActionButton1ClickedEvent += ItemsPanel_ItemActionButton1ClickedEvent;
            ItemsPanel.ItemClickedEvent += ItemsPanel_ItemClickedEvent;
            //ToggleDetail(false);
        }

        void ItemsPanel_ItemClickedEvent(InventoryItem obj)
        {
            ShowDetail(obj);
        }

        void ItemsPanel_ItemActionButton1ClickedEvent(InventoryItem obj)
        {
            ShowDetail(obj);
        }

        public void Refresh()
        {
            ItemsPanel.Refresh(SourceInventory.GetItems());
            if (SourceInventory.ItemsCount == 0) ToggleDetail(false);
            else
            {
                ShowDetail(SourceInventory.GetItems()[0]);
            }
        }

        public void ToggleDetail(bool? show)
        {
            // hide detail
            foreach (Transform t in ItemDetail.transform)
            {
                t.gameObject.SetActive(show.Value);
            }
        }

        public void Setup(Inventory sourceInventory, Inventory targetInventory)
        {
            this.SourceInventory = sourceInventory;
            this.TargetInventory = targetInventory;
            ItemsPanel.Setup(sourceInventory.GetItems(), ContentListItemPrefab);
            Refresh();
        }

        //public void TakeAll()
        //{
        //    foreach (var ii in SourceInventory.GetItems())
        //    {
        //        var amount = -ii.Amount;
        //        var realAmountRemoved = SourceInventory.AddItem(ii.Item, -ii.Amount);
        //        TargetInventory.AddItem(ii.Item, -(amount - realAmountRemoved.Amount));
        //    }
        //    SourceInventory.CompactItems();
        //    ItemsPanel.Clear();
        //    this.gameObject.SetActive(false);
        //}

        public void Drop(InventoryItem ii, float amount)
        {
            if (ii != null && amount > 0f)
            {
                RememberDetailSelection(() =>
                {
                    SourceInventory.DropItem(ii.Item, amount);
                    Refresh();
                });
            }
        }

        public void ShowDetail(InventoryItem ii)
        {
            ItemDetailInventoryItem = ii;
            var img = ItemDetail.transform.FindChild("Image").GetComponent<Image>();
            img.sprite = ii.Item.Icon;
            var drag = img.GetComponent<DragMeSimple>();
            drag.DragResult -= drag_DragResult;
            drag.DragResult += drag_DragResult;
            var name = ItemDetail.transform.FindChild("Name").GetComponent<Text>();
            name.text = ii.Item.Name;
            var desc = ItemDetail.transform.FindChild("Description").GetComponent<Text>();
            desc.text = "" + ii.Item.Description;
            DropButton = ItemDetail.transform.FindChild("DropButton").GetComponent<Button>();
            ItemDetailSlider = ItemDetail.transform.FindChild("Slider").GetComponent<Slider>();
            ItemDetailSlider.value = ItemDetailSlider.maxValue = ii.Amount;
            ItemDetailText = ItemDetail.transform.FindChild("Amount").GetComponent<Text>();
            ItemDetailText.text = ItemDetailSlider.value + " / " + ii.Amount;
            // action buttons
            var actionButtons = ItemDetail.transform.FindChild("ActionButtons").GetComponent<HorizontalLayoutGroup>();
            foreach (Transform t in actionButtons.transform) t.gameObject.SetActive(false);
            var useButton = actionButtons.transform.Find("UseButton").GetComponent<Button>();
            var equipButton = actionButtons.transform.Find("EquipButton").GetComponent<Button>();
            if (ii.Item.Type == Items.EItemType.Potion)
            {
                useButton.gameObject.SetActive(true);
            }
            else if (ii.Item.Type == Items.EItemType.Weapon)
            {
                equipButton.gameObject.SetActive(true);
            } 
            ToggleDetail(true);
        }

        void drag_DragResult(List<UnityEngine.EventSystems.RaycastResult> obj)
        {
            foreach (var rcr in obj)
            {
                if (rcr.gameObject.name == "Quickbar")
                {
                    Debug.Log("Dragged intem in Quickbar");
                }
            }
        }

        public void OnSliderChange()
        {
            var enabled = false;
            if (ItemDetailText != null)
            {
                ItemDetailText.text = ItemDetailSlider.value + " / " + ItemDetailInventoryItem.Amount;
                enabled = ItemDetailSlider.value > 0f;
            }
            DropButton.interactable = enabled;
        }

        public void OnDetailDrop()
        {
            Drop(ItemDetailInventoryItem, ItemDetailSlider.value);
        }

        public void OnUseItem()
        {
            RememberDetailSelection(() =>
            {
                SourceInventory.UseItem(ItemDetailInventoryItem.Item);
                SourceInventory.CompactItems();
                Refresh();
            });
        }

        public void OnEquipItem()
        {
            RememberDetailSelection(() =>
            {
                SourceInventory.EquipItem(ItemDetailInventoryItem.Item);
                Refresh();
            });
        }

        public void RememberDetailSelection(System.Action innerProcedure)
        {
            // rember old selection
            var tmpItem = ItemDetailInventoryItem;
            // execute code
            innerProcedure();
            // try to select the item again
            var ii = SourceInventory.FindItem(tmpItem.Item);
            if (ii != null)
            {
                ItemDetailInventoryItem = ii;
                ShowDetail(ii);
            }
        }

        public void SetName(string objname)
        {
            this.transform.Find("Title").GetComponent<Text>().text = objname;
        }

    }
}