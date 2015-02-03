using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace Dungeon
{
    public class InventoryPanel : MonoBehaviour
    {
        Inventory SourceInventory;
        Inventory TargetInventory;
        public GameObject ContentList;
        public GameObject ContentListItemPrefab;
        public GameObject ItemDetail;
        public Slider ItemDetailSlider;
        public Text ItemDetailText;
        public InventoryItem ItemDetailInventoryItem;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Clear()
        {
            foreach (Transform child in ContentList.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        public void Refresh()
        {
            Clear();
            var counter = 0;
            var atLestOne = false;
            foreach (var ii in SourceInventory.GetItems())
            {
                var newitem = Instantiate(ContentListItemPrefab) as GameObject;
                newitem.transform.SetParent(ContentList.transform);
                var panelItem = newitem.GetComponent<InventoryPanelItem>();
                panelItem.InventoryPanel = this;
                panelItem.SetupItem(ii);
                if (counter++ == 0)
                {
                    ShowDetail(ii);
                }
                atLestOne = true;
            }
            if (!atLestOne)
            {
                // hide detail
                foreach (Transform t in ItemDetail.transform)
                {
                    t.gameObject.SetActive(false);
                }
            }
        }

        public void SetupInventoryComponent(Inventory sourceInventory, Inventory targetInventory)
        {
            this.SourceInventory = sourceInventory;
            this.TargetInventory = targetInventory;
            Refresh();
        }

        public void TakeAll()
        {
            foreach (var ii in SourceInventory.GetItems())
            {
                var realAmountRemoved = SourceInventory.AddItem(ii.Item, -ii.Amount);
                TargetInventory.AddItem(ii.Item, -realAmountRemoved);
            }
            SourceInventory.CompactItems();
            Clear();
            this.gameObject.SetActive(false);
        }

        public void Take(InventoryItem ii, float amount = -1)
        {
            var realAmountRemoved = SourceInventory.AddItem(ii.Item, amount >= 0 ? Mathf.Max(-amount, -ii.Amount) : -ii.Amount);
            TargetInventory.AddItem(ii.Item, -realAmountRemoved);
            SourceInventory.CompactItems();
            Refresh();
        }

        public void ShowDetail(InventoryItem ii)
        {
            ItemDetailInventoryItem = ii;
            ItemDetail.SetActive(true);
            foreach (Transform t in ItemDetail.transform)
            {
                if (t.name == "Image")
                {
                    var img = t.GetComponent<Image>();
                    img.sprite = ii.Item.Icon;
                }
                else if (t.name == "Name")
                {
                    var txt = t.GetComponent<Text>();
                    txt.text = ii.Item.Name;
                }
                else if (t.name == "Description")
                {
                    var txt = t.GetComponent<Text>();
                    txt.text = "" + ii.Item.Description;
                }
                else if (t.name == "Slider")
                {
                    ItemDetailSlider = t.GetComponent<Slider>();
                    ItemDetailSlider.value = ItemDetailSlider.maxValue = ii.Amount;
                }
                else if (t.name == "Amount")
                {
                    ItemDetailText = t.GetComponent<Text>();
                    ItemDetailText.text = ItemDetailSlider.value + " / " + ii.Amount;
                }
            }
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
            foreach (Transform t in this.transform)
            {
                if (t.name == "Title")
                {
                    t.GetComponent<Text>().text = objname;
                }
            }
        }
    }
}