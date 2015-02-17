using UnityEngine;
using System.Collections.Generic;

public class MainChest : MonoBehaviour {

    bool _Processed = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public void ChestOpened()
    {
        if (!_Processed)
        {
            var chardialog = Dungeon.GameController.Instance.Dialogs.CharacterDialog;
            var messages = new List<DialogMessage>();
            messages.Add(new DialogMessage
            {
                SpeakerName = "You",
                Color = Color.green,
                Message = "The old rotten skeleton has quite a tresure behind his back. Who would anticipate that?"
            });
            messages.Add(new DialogMessage
            {
                SpeakerName = "You",
                Color = Color.green,
                Message = "But for what purpose?"
            });
            messages.Add(new DialogMessage
            {
                SpeakerName = "You",
                Color = Color.green,
                Message = "And there is another important question. How would I sneak with this treasure behind the village guards?"
            });
            messages.Add(new DialogMessage
            {
                SpeakerName = "You",
                Color = Color.green,
                Message = "..... after 5 minutes of thinking ....."
            });
            messages.Add(new DialogMessage
            {
                SpeakerName = "You",
                Color = Color.green,
                Message = "Aaahh. Treasure, old skeleton bones, rags and dirt? I think one terrifying skeleton should scare the guards enough."
            });
            messages.Add(new DialogMessage
            {
                SpeakerName = "",
                Color = Color.red,
                Message = "-------------------------------------------------"
            });
            messages.Add(new DialogMessage
            {
                SpeakerName = "",
                Color = Color.red,
                Message = "That is all folks. Thanks for playing :-)"
            });
            chardialog.PlayDialogMessages(messages);
            _Processed = true;
        }
    }
}
