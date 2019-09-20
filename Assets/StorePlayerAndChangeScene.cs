using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorePlayerAndChangeScene : MonoBehaviour
{
    [SerializeField] string destinationSceneName;
    [SerializeField] SaveData saveData;
    [SerializeField] WipeEffet wipeEffet;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == TagName.player)
        {
            Player player = collision.GetComponent<Player>();
            if (player == null)
            {
                throw new System.NullReferenceException($"Detected GameObject '{collision.name}' does not have 'Player' Component.");
            }

            saveData.StorePlayerData(player);
            //UnityEngine.SceneManagement.SceneManager.LoadScene(destinationSceneName);

            TransitionEffect.WipeEffet = wipeEffet;
            SceneTransitionManager.TransScene(destinationSceneName, null);
            
        }
    }
}
