using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorePlayerAndChangeScene : MonoBehaviour
{
    [SerializeField,UnityEngine.Serialization.FormerlySerializedAs("destinationSceneName")] string destinationSceneNameOnTriggerEnter;
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

            StoreAndChangeSene(player, destinationSceneNameOnTriggerEnter);
        }
    }

    public void StoreAndChangeSene(string destinationSceneName)
    {
        StoreAndChangeSene(ActorManager.PlayerActor.GetComponent<Player>(), destinationSceneName);
    }

    public void StoreAndChangeSene(Player player,string destinationSceneName)
    {
        saveData.StorePlayerData(player);

        TransitionEffect.WipeEffet = wipeEffet;
        SceneTransitionManager.TransScene(destinationSceneNameOnTriggerEnter, null);
    }
}
