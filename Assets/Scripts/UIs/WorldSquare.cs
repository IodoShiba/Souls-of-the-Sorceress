using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class SelectableUI : MonoBehaviour
{
    [SerializeField] protected bool isHilightable;
    [SerializeField] protected bool isSelectable;

    public void Select()
    {
        if (isSelectable)
        {
            OnSelected();
        }
    }

    protected abstract void OnSelected();
    public abstract SelectableUI GetNeighbor(Vector2Int input);

    public SelectableUI TryGetThis() => isHilightable ? this : null;
}

public class WorldSquare : SelectableUI
{
    [System.Serializable] class Neighbor
    {
        public SelectableUI target;
        public bool autoDirection;
        public Vector2Int direction;
    }

    [SerializeField] Neighbor[] neighbors;
    [SerializeField] UnityEngine.Events.UnityEvent onSelected;

    public override SelectableUI GetNeighbor(Vector2Int direction)
    {
        for (int i = 0; i < neighbors.Length; ++i)
        {
            if (neighbors[i].direction == direction)
            {
                return neighbors[i].target.TryGetThis();
            }
        }
        return null;
    }

    protected override void OnSelected()
    {
        onSelected.Invoke();
    }

    public void GoToScene(string sceneName)
    {
        //TransitionEffect.InWipeEffet = ;
        //TransitionEffect.OutWipeEffet = ;
        SceneTransitionManager.TransScene(sceneName, null);
    }

    private void Awake()
    {
        for (int i = neighbors.Length - 1; i >= 0; --i)
        {
            if (neighbors[i].autoDirection)
            {
                Vector2 diff = neighbors[i].target.transform.position - transform.position;
                if(System.Math.Abs(diff.x) > System.Math.Abs(diff.y))
                {
                    neighbors[i].direction = Vector2Int.right * System.Math.Sign(diff.x);
                }
                else
                {
                    neighbors[i].direction = Vector2Int.up * System.Math.Sign(diff.y);
                }
            }
        }
    }

    //private void Awake()
    //{
    //    float[] angleBorders = new float[] { 45, 135, 225, 315 };
    //    int borderCount = angleBorders.Length;
    //    Neighbor[] _neighbors = new Neighbor[neighbors.Length];

    //    int undecide = _neighbors.Length;

    //    for (int i = 0; i < neighbors.Length; ++i)
    //    {
    //        _neighbors[i] = neighbors[i];
    //    }

    //    for (int i = _neighbors.Length - 1; i >= 0; --i)
    //    {
    //        if (!neighbors[i].autoDirection)
    //        {
    //            Neighbor t = _neighbors[i];
    //            neighbors[i] = _neighbors[undecide - 1];
    //            _neighbors[undecide - 1] = t;
    //            --undecide;
    //        }
    //    }

    //    for (int i = 0; i < neighbors.Length; ++i)
    //    {
    //        if(neighbors[i].autoDirection)
    //        {
    //            for(int j = 0; j < angleBorders[j] - 1; ++j)
    //            {

    //            }
    //        }
    //    }

    //    void Fold(int _i,int _j)
    //    {
    //        angleBorders[_i] = angleBorders[_i] + Mathf.DeltaAngle(angleBorders[_i], angleBorders[_j]) / 2;
    //        for (int k = _j; k < borderCount - 1; ++k)
    //        {
    //            float t = angleBorders[k];
    //            angleBorders[k] = angleBorders[k + 1];
    //            angleBorders[k + 1] = t;
    //        }
    //        --borderCount;
    //    }
    //}

}
