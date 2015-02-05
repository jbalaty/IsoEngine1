using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


namespace Dungeon
{
    public class InventoryPanel : MonoBehaviour, IInventoryPanel
    {
        Inventory SourceInventory;
        Inventory TargetInventory;

        public PlayerInventoryPanelItems ItemsPanel;
        public Transform ContentListItemPrefab;
        public bool CloseIfEmpty = true;

        public GameObject ItemDetail;
        Slider ItemDetailSlider;
        Text ItemDetailText;
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
            Take(obj);
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

        public void TakeAll()
        {
            foreach (var ii in SourceInventory.GetItems())
            {
                var amount = -ii.Amount;
                var realAmountRemoved = SourceInventory.AddItem(ii.Item, -ii.Amount);
                TargetInventory.AddItem(ii.Item, -(amount - realAmountRemoved.Amount));
            }
            SourceInventory.CompactItems();
            ItemsPanel.Clear();
            this.gameObject.SetActive(false);
            Refresh();
            if (CloseIfEmpty && SourceInventory.ItemsCount == 0) ToggleDialog(false);
        }

        public void Take(InventoryItem ii, float amount = -1)
        {
            amount = amount >= 0 ? Mathf.Max(-amount, -ii.Amount) : -ii.Amount;
            var realAmountRemoved = SourceInventory.AddItem(ii.Item, amount);
            TargetInventory.AddItem(ii.Item, -(amount - realAmountRemoved.Amount));
            SourceInventory.CompactItems();
            Refresh();
            if (CloseIfEmpty && SourceInventory.ItemsCount == 0) ToggleDialog(false);
        }

        public void ShowDetail(InventoryItem ii)
        {
            ItemDetailInventoryItem = ii;
            var img = ItemDetail.transform.FindChild("Image").GetComponent<Image>();
            img.sprite = ii.Item.Icon;
            var name = ItemDetail.transform.FindChild("Name").GetComponent<Text>();
            name.text = ii.Item.Name;
            var desc = ItemDetail.transform.FindChild("Description").GetComponent<Text>();
            desc.text = "" + ii.Item.Description;
            ItemDetailSlider = ItemDetail.transform.FindChild("Slider").GetComponent<Slider>();
            ItemDetailSlider.value = ItemDetailSlider.maxValue = ii.Amount;
            ItemDetailText = ItemDetail.transform.FindChild("Amount").GetComponent<Text>();
            ItemDetailText.text = ItemDetailSlider.value + " / " + ii.Amount;
            ToggleDetail(true);
        }

        public void OnSliderChange()
        {
            if (ItemDetailText != null)
                ItemDetailText.text = ItemDetailSlider.value + " / " + ItemDetailInventoryItem.Amount;
        }

        public void OnDetailTake()
        {
            Take(ItemDetailInventoryItem, ItemDetailSlider.value);
        }

        public void SetName(string objname)
        {
            this.transform.Find("Title").GetComponent<Text>().text = objname;
        }
        public void ToggleDialog(bool show)
        {
            this.gameObject.SetActive(show);
        }
    }
}