using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SecretDragonWingItem : MonoBehaviour
{
    [SerializeField] UnityEvent onReceived;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            var ascSarah = collision.GetComponent<ActorSarah.ActorStateConnectorSarah>();
            ascSarah.AddProgressLevel();
            ascSarah.AddProgressLevel();

            onReceived.Invoke();

            Destroy(gameObject);
        }
    }
}
