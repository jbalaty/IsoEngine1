using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using IsoEngine1;

public class DialogMessage
{
    public string SpeakerName;
    public string Message;
    public float DurationSeconds = -1f;
    public Color Color;

}


public class CharacterDialog : MonoBehaviour
{
    public class Options
    {
        public bool PauseGameplay = true;
    }

    public Transform MessagePrefab;
    public Transform HistoryContent;
    public ScrollRect Scrollrect;
    public Button ContinueButton;
    IEnumerator<DialogMessage> currentEnumerator;
    Options CurrentOptions;
    int _ScrollToEnd = 0;
    float _PreviousScrollSize = 0;
    // Use this for initialization
    void Start()
    {
        //HistoryContent = this.transform.FindChild("Panel/HistoryPanel/Content");
        //Scrollbar = this.transform.FindChild("Panel/Scrollbar").GetComponent<Scrollbar>();
        //ContinueButton = this.transform.FindChild("Panel/ContinueButton").GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        ScrollToEndWeird();
        _PreviousScrollSize = Scrollrect.verticalScrollbar.size;
    }

    public void PlayDialogMessages(IEnumerable<DialogMessage> messages, Options opts = null)
    {
        CurrentOptions = opts ?? new Options();
        if (CurrentOptions.PauseGameplay)
        {
            Time.timeScale = 0f;
        }
        Clear();
        this.gameObject.SetActive(true);
        currentEnumerator = messages.GetEnumerator();
        StartCoroutine(Utils.WaitEndOfFrame(() =>
        {
            if (currentEnumerator.MoveNext())
            {
                ShowNextMessage();
            }
        }));
    }

    public void AddMessage(DialogMessage message)
    {
        var prefab = Instantiate(this.MessagePrefab) as Transform;
        prefab.SetParent(HistoryContent);
        prefab.localScale = Vector3.one;
        prefab.localPosition = Vector3.zero;
        var t = prefab.GetComponent<Text>();
        t.text = "<b><color=" + message.Color.ToHex(true) + ">" + message.SpeakerName + ":</color></b> " + message.Message;
        _ScrollToEnd = 1;
    }

    public void ShowNextMessage()
    {
        if (currentEnumerator != null)
        {
            AddMessage(currentEnumerator.Current);
            AdvanceCurrentEnumerator();
        }
        else
        {
            if (CurrentOptions.PauseGameplay)
            {
                Time.timeScale = 1f;
            }
            this.gameObject.SetActive(false);
        }
    }

    void AdvanceCurrentEnumerator()
    {
        if (currentEnumerator.MoveNext())
        {
            ContinueButton.GetComponentInChildren<Text>().text = "Continue";
        }
        else
        {
            ContinueButton.GetComponentInChildren<Text>().text = "End dialog";
            currentEnumerator = null;
        }
    }

    public void Clear()
    {
        foreach (Transform child in HistoryContent)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void ScrollToEnd()
    {
        Canvas.ForceUpdateCanvases();
        Scrollrect.verticalScrollbar.value = 0f;
        Canvas.ForceUpdateCanvases();

    }

    /// <summary>
    /// Scroll to end does not work immediately, but several frames after message is added. We have to watch change in ScrollBar size and 
    /// scroll to end in that moment, but then handle the scrolling back to system for mouse wheel to work
    /// </summary>
    public void ScrollToEndWeird()
    {
        if (_ScrollToEnd == 1 && Scrollrect.verticalScrollbar.size < _PreviousScrollSize)
        {
            Debug.Log("ScrollToEnd Weird");
            Canvas.ForceUpdateCanvases();
            Scrollrect.verticalScrollbar.value = 0f;
            Canvas.ForceUpdateCanvases();
            _ScrollToEnd = 2;
        }
    }
}
