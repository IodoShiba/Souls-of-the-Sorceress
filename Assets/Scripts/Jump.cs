using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : Ability {
    [SerializeField,Range(100,3000)] float power;
    [SerializeField] KeyCode temp_Key;
    [SerializeField] GameObject groundSensor;
    [SerializeField] string groundTagName;
    private bool jumpable = false;
    public override bool Momential
    {
        get
        {
            return true;
        }
    }
    // Use this for initialization
    private void Start()
    {
        var c=groundSensor.AddComponent<GroundSensor>();
        c.setJumpable = this.SetJumpable;
        c.groundTagName = this.groundTagName;
        c.enabled = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (jumpable && Input.GetKeyDown(temp_Key))
        {
            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, power));
            jumpable = false;
        }
    }
    private void SetJumpable(bool b) { jumpable = b; }
    public override void Act()
    {
        if (jumpable && Input.GetKeyDown(temp_Key))
        {
            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, power));
            jumpable = false;
        }
    }

    private class GroundSensor : MonoBehaviour
    {
        public System.Action<bool> setJumpable;
        public string groundTagName;

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.tag == groundTagName) { 
                setJumpable(true);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.tag == groundTagName)
            {
                setJumpable(false);
            }
        }
    }
}
/*public class Jump : Ability
{
    [SerializeField, Range(100, 3000)] float power;
    public override bool Momential
    {
        get
        {
            return true;
        }
    }
    // Use this for initialization
    private void Start()
    {
    }
    public override void Act()
    {
        gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, power));
    }
    
}*/
