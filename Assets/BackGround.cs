using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackGround : MonoBehaviour
{
    [System.Serializable]
    class Layer
    {
        public SpriteRenderer spriteRenderer;
        public SpriteRenderer spriteRendererLoopX;
        public SpriteRenderer spriteRendererLoopY;
        public SpriteRenderer spriteRendererLoopXY;
        [Range(0,1)] public float sizeProportion;
        public float margin;
    }

    [SerializeField] BoxCollider2D area;
    [SerializeField] GameObject monitoringGameObject;
    [SerializeField] List<Layer> layers;

    private void FixedUpdate()
    {
        Debug.Log(area.size);
        if (area == null) { throw new System.ArgumentNullException($"field '{nameof(area)}' cannot be null."); }
        if (area.bounds.size.x < float.Epsilon || area.bounds.size.y < float.Epsilon) { throw new System.ArgumentException($"bound of field '{nameof(area)}' is too small or too thin."); }

        for (int i = 0; i < layers.Count; ++i)
        {
            Vector2 monitObjPos = monitoringGameObject.transform.position;
            Vector2 posProp = (monitObjPos - (Vector2)area.bounds.center);
            posProp.x /= area.bounds.size.x / 2;
            posProp.y /= area.bounds.size.y / 2;

            layers[i].spriteRenderer.transform.position = (Vector3)monitObjPos + new Vector3(
                -layers[i].spriteRenderer.bounds.size.x / 2 * layers[i].sizeProportion * posProp.x,
                -layers[i].spriteRenderer.bounds.size.y / 2 * layers[i].sizeProportion * posProp.y,
                layers[i].spriteRenderer.transform.position.z
                );
        }

    }
}
