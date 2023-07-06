using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollViewAutoScroller : MonoBehaviour
{
    [SerializeField] float viewportHeightScale = 1.0f;

    ScrollRect scrollRect;
    EventSystem eventSystem;

    GameObject selected;
    GameObject Selected
    { 
        get => selected;
        set 
        {
            if(ReferenceEquals(selected, value)) { return; }
            selected = value;
            isSelectedInContent = IsSelectedInContent(selected.transform, content.transform);
        }
    }
    bool isSelectedInContent;
    RectTransform content;
    RectTransform viewport;

    static Vector3[] cornersBuffer = new Vector3[4];

    // Start is called before the first frame update
    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        eventSystem = EventSystem.current;
        content = scrollRect.content;
        viewport = scrollRect.viewport;
    }

    // Update is called once per frame
    void Update()
    {
        Selected = eventSystem.currentSelectedGameObject;

        if(!isSelectedInContent) { return; }

        var selectedRectTransform = Selected.GetComponent<RectTransform>();

        var currentCamera = Camera.current;
        if (currentCamera == null) { return; }
        viewport.GetWorldCorners(cornersBuffer);
        var vpBottom = currentCamera.WorldToViewportPoint(cornersBuffer[0]).y;
        var vpTop = currentCamera.WorldToViewportPoint(cornersBuffer[1]).y;
        var vpCenter = (vpBottom + vpTop)/2;
        vpBottom = Mathf.Lerp(vpCenter, vpBottom, viewportHeightScale);
        vpTop = Mathf.Lerp(vpCenter, vpTop, viewportHeightScale);

        content.GetWorldCorners(cornersBuffer);
        var contentBottom = currentCamera.WorldToViewportPoint(cornersBuffer[0]).y;
        var contentTop = currentCamera.WorldToViewportPoint(cornersBuffer[1]).y;
        selectedRectTransform.GetWorldCorners(cornersBuffer);
        var selctedBottom = currentCamera.WorldToViewportPoint(cornersBuffer[0]).y;
        var selectedTop = currentCamera.WorldToViewportPoint(cornersBuffer[1]).y;

        var movableRange = (contentTop-contentBottom)-(vpTop-vpBottom);
        if (movableRange < 0.5f)
        {
            movableRange = 0.5f;
        }

        if (selectedTop > vpTop)
        {
            var diff = selectedTop - vpTop;
            scrollRect.verticalScrollbar.value = scrollRect.verticalScrollbar.value + diff/movableRange;
        }
        else if (selctedBottom < vpBottom)
        {
            var diff = vpBottom - selctedBottom;
            scrollRect.verticalScrollbar.value = scrollRect.verticalScrollbar.value - diff/movableRange;
        }
        else
        {
            // do nothing
        }

    }

    bool IsSelectedInContent(Transform _selected, Transform content, int maxDepth = 8)
    {
        if (maxDepth == 0) {return false;}

        // Debug.Log(_selected.gameObject.name);
        if(_selected == null)
        {
            return false;
        }

        if (object.ReferenceEquals(_selected, content))
        {
            return true;
        }

        return IsSelectedInContent(_selected.parent, content, maxDepth - 1);
    }

}
