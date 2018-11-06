using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour {
    [SerializeField,Range(100,3000)] float power;
    [SerializeField] KeyCode temp_Key;
    [SerializeField] GameObject groundSensor;
    [SerializeField] string groundTagName;
    private bool jumpable = false;

    // Use this for initialization
    private void Start()
    {
        var c=groundSensor.AddComponent<GroundSensor>();
        c.setJumpable = this.SetJumpable;
        c.groundTagName = this.groundTagName;
        c.enabled = true;
    }
    // Update is called once per frame
    void Update() {
        if (jumpable&&Input.GetKeyDown(temp_Key))
        {
            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, power));
            jumpable = false;
        }
    }
    private void SetJumpable(bool b) { jumpable = b; }

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
