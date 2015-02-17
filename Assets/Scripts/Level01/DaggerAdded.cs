using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Dungeon;
using Dungeon.Items;

public class DaggerAdded : MonoBehaviour
{
    bool _Processed = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void ItemAddedToInventory(InventoryItem ii)
    {
        Process(ii.Item);
    }
    void ItemPickedUp(Item item)
    {
        Process(item);
    }

    void Process(Item item)
    {
        if (!_Processed && item.Name == "Dagger")
        {
            var chardialog = Dungeon.GameController.Instance.Dialogs.CharacterDialog;
            var messages = new List<DialogMessage>();
            messages.Add(new DialogMessage
            {
                SpeakerName = "You",
                Color = Color.green,
                Message = @"Argh, dagger. I can imagine far better weapon againts these skeletons."
            });
            messages.Add(new DialogMessage
            {
                SpeakerName = "You",
                Color = Color.green,
                Message = @"I hope it is sharp enough."
            });
            messages.Add(new DialogMessage
            {
                SpeakerName = "Subconscious mind",
                Color = Color.white,
                Message = @"It would serve me better if I put it from my bag into my hand."
            });
            chardialog.PlayDialogMessages(messages);
            _Processed = true;
        }
    }
}
