using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss : MonoBehaviour
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

    void EnemySpotted()
    {
        if (!_Processed)
        {
            var chardialog = Dungeon.GameController.Instance.Dialogs.CharacterDialog;
            var messages = new List<DialogMessage>();
            messages.Add(new DialogMessage { SpeakerName = "Skeleton Master", Message = @"How do you dare to come into my house?", Color = Color.red });
            messages.Add(new DialogMessage { SpeakerName = "You", Message = @"Heh, you call this stinking tomb a house?", Color = Color.green });
            messages.Add(new DialogMessage { SpeakerName = "Skeleton Master", Message = @"Get lost or I will make you into one of my servants human.", Color = Color.red });
            messages.Add(new DialogMessage
            {
                SpeakerName = "You",
                Message = @"Yes I will go. But you have to go first. "
                    + "And in case you have some gold, give it to me immediately. It would only slow you down.",
                Color = Color.green
            });
            messages.Add(new DialogMessage { SpeakerName = "Skeleton Master", Message = @"Gold? No. No, the gold is mine. You will not take it from me!! Do you hear me!?", Color = Color.red });
            messages.Add(new DialogMessage { SpeakerName = "You", Message = @"Ok, then I will take your miserable life first and gold after.", Color = Color.green });
            chardialog.PlayDialogMessages(messages);
            _Processed = true;
        }
    }
}
