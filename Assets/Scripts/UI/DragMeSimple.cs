using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using IsoEngine1;

[RequireComponent(typeof(Image))]
public class DragMeSimple : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private GameObject m_DraggingIcon;
    private RectTransform m_DraggingPlane;


    public bool SetSizeSameAsInputImage = true;
    public event Action<List<RaycastResult>> DragResult;

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        var canvas = Utils.FindInParents<Canvas>(gameObject);
        if (canvas == null)
            return;
        // We have clicked something that can be dragged.
        // What we want to do is create an icon for this.
        m_DraggingIcon = new GameObject("dragIcon");
        m_DraggingIcon.transform.SetParent(canvas.transform, true);
        m_DraggingIcon.transform.SetAsLastSibling();

        var image = m_DraggingIcon.AddComponent<Image>();
        // The icon will be under the cursor.
        // We want it to be ignored by the event system.
        //m_DraggingIcon.AddComponent<IgnoreRaycast>();
        image.sprite = GetComponent<Image>().sprite;
        if (SetSizeSameAsInputImage && this.GetComponent<Image>() != null)
        {
            var origImage = this.GetComponent<Image>();
            image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 60f);
            image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 60f);
        }
        else
        {
            image.SetNativeSize();
        }

        /*if (dragOnSurfaces)
            m_DraggingPlane = transform as RectTransform;
        else
            m_DraggingPlane = canvas.transform as RectTransform;
        */
        SetDraggedPosition(eventData);
    }
    public virtual void OnDrag(PointerEventData data)
    {
        if (m_DraggingIcon != null)
            SetDraggedPosition(data);
    }
    private void SetDraggedPosition(PointerEventData data)
    {
        var rt = m_DraggingIcon.GetComponent<RectTransform>();
        rt.position = data.position;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (m_DraggingIcon != null)
            Destroy(m_DraggingIcon);
        var rcResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, rcResults);
        if (DragResult != null) DragResult(rcResults);
    }
}