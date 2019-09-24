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
        public float margin;
        public bool forceSpriteFullyContainCamera;
    }

    [SerializeField] BoxCollider2D area;
    //[SerializeField] GameObject monitoringGameObject;
    [SerializeField] List<Layer> layers;

    private void FixedUpdate()
    {
        //Debug.Log(area.size);
        if (area == null) { throw new System.ArgumentNullException($"field '{nameof(area)}' cannot be null."); }
        if (area.bounds.size.x < float.Epsilon || area.bounds.size.y < float.Epsilon) { throw new System.ArgumentException($"bound of field '{nameof(area)}' is too small or too thin."); }

        Camera mainCam = Camera.main;
        Vector3 cameraSizeHalf = new Vector3(mainCam.aspect * mainCam.orthographicSize, mainCam.orthographicSize,0);

        for (int i = 0; i < layers.Count; ++i)
        {
            Vector2 monitObjPos = mainCam.transform.position;
            Vector2 posProp = (monitObjPos - (Vector2)area.bounds.center);
            Vector3 movableSizeArea = area.bounds.size - 2 * cameraSizeHalf;
            posProp.x /= movableSizeArea.x / 2;
            posProp.y /= movableSizeArea.y / 2;
            Vector3 movableSizeSprite = layers[i].spriteRenderer.bounds.size - 2 * cameraSizeHalf - layers[i].margin * Vector3.one;

            if (layers[i].forceSpriteFullyContainCamera)
            {
                posProp.x = Mathf.Clamp(posProp.x, -1, 1);
                posProp.y = Mathf.Clamp(posProp.y, -1, 1);
            }

            layers[i].spriteRenderer.transform.position = (Vector3)monitObjPos + new Vector3(
                -movableSizeSprite.x / 2 * posProp.x,
                -movableSizeSprite.y / 2 * posProp.y,
                layers[i].spriteRenderer.transform.position.z
                );
        }
    }
}
