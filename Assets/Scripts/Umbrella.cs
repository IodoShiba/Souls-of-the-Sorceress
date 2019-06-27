using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Umbrella : MonoBehaviour {
    Vector3 defpos;
    System.Action updateImple = null;
    [SerializeField] Player player;
    [SerializeField] Color _colorOrdinary;
    [SerializeField] Color _colorAwaken;
    [SerializeField] Color _colorBlueAwaken;
    SpriteRenderer spriteRenderer = null;
    string coroutineName;

    // Use this for initialization
    void Start () {
        defpos = transform.localPosition ;
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.parent.GetComponent<PlayerStates.Awakening.Ordinary>().RegisterInitialize(() => spriteRenderer.color = _colorOrdinary);
        transform.parent.GetComponent<PlayerStates.Awakening.Awaken>().RegisterInitialize(() => spriteRenderer.color = _colorAwaken);
        transform.parent.GetComponent<PlayerStates.Awakening.BlueAwaken>().RegisterInitialize(() => spriteRenderer.color = _colorBlueAwaken);

    }
	
	// Update is called once per frame
	void Update ()
    {
        Color color = spriteRenderer.color;
        if (player.DoesUmbrellaWork())
        {
            color.a = 1;
        }
        else
        {
            color.a = 0.2f;
        }
        spriteRenderer.color = color;
    }

    public void PlayerGliding()
    {
        transform.localPosition = new Vector3(0, 0.5f, defpos.z);
    }

    public void PlayerRisingAttack()
    {
        transform.localPosition = new Vector3(0, 1f, defpos.z);
    }

    public void PlayerDropAttack()
    {
        transform.localPosition = new Vector3(0, -1f, defpos.z);
    }

    public void PlayerGuard()
    {
        transform.localPosition = new Vector3(1f, 0, defpos.z);
    }

    public void Default()
    {
        if (!string.IsNullOrEmpty(coroutineName))
        {
            StopCoroutine(coroutineName);
            coroutineName = null;
        }
        transform.localPosition = defpos;
    }

    public void StartMotion(string name)
    {
        StopCoroutine(coroutineName);
        StartCoroutine(name);
        coroutineName = name;
    }

    IEnumerator PlayerVerticalSlash()
    {
        float t = 0;
        const float r = 1;
        const float _motionLength = 0.2f;
        double theta = 0;
        Vector3 rv = new Vector3(0, 0, transform.localPosition.z);
        while (true)
        {
            theta = System.Math.PI * ((-2.0 / 4) * System.Math.Min(t / _motionLength,1) + 1.0 / 4);
            rv.x = (float)System.Math.Cos(theta);
            rv.y = (float)System.Math.Sin(theta);
            transform.localPosition = rv;
            t += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator PlayerReturnSlash()
    {
        float t = 0;
        const float r = 1;
        const float _motionLength = 0.2f;
        double theta = 0;
        Vector3 rv = new Vector3(0, 0, transform.localPosition.z);
        while (true)
        {
            theta = System.Math.PI * (1.0 * System.Math.Min(t / _motionLength, 1) - 1.0 / 4);
            rv.x = (float)System.Math.Cos(theta);
            rv.y = (float)System.Math.Sin(theta);
            transform.localPosition = rv;
            t += Time.deltaTime;
            yield return null;
        }
    }
    
    IEnumerator PlayerSmashSlash()
    {
        float t = 0;
        const float r = 1;
        const float _motionLength = 0.2f;
        double theta = 0;
        Vector3 rv = new Vector3(0, 0, transform.localPosition.z);
        while (true)
        {
            theta = System.Math.PI * ((-7.0 / 6) * System.Math.Min(t / _motionLength, 1) + 1.0);
            rv.x = (float)System.Math.Cos(theta);
            rv.y = (float)System.Math.Sin(theta);
            transform.localPosition = rv;
            t += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator PlayerAerialSlash()
    {
        float t = 0;
        const float r = 1;
        const float _motionLength = 0.2f;
        double theta = 0;
        Vector3 rv = new Vector3(0, 0, transform.localPosition.z);
        while (true)
        {
            theta = System.Math.PI * ((-6.0 / 4) * System.Math.Min(t / _motionLength, 1) + 3.0/4);
            rv.x = (float)System.Math.Cos(theta);
            rv.y = (float)System.Math.Sin(theta);
            transform.localPosition = rv;
            t += Time.deltaTime;
            yield return null;
        }
    }

    public void StartCoroutineForEvent(string name)
    {
        StartCoroutine(name);
    }
}
