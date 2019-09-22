using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorePlayerAndChangeScene : MonoBehaviour
{
    [SerializeField,UnityEngine.Serialization.FormerlySerializedAs("destinationSceneNameOnTriggerEnter")] string defaultDestinationSceneName;
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

            StoreAndChangeSene(player, defaultDestinationSceneName);
        }
    }

    public void StoreAndChangeSene(string destinationSceneName)
    {
        StoreAndChangeSene(ActorManager.PlayerActor.GetComponent<Player>(), destinationSceneName);
    }

    public void StoreAndChangeSeneTimed(float time)
    {
        StartCoroutine(WaitAndChange(time, defaultDestinationSceneName));
    }

    public void StoreAndChangeSeneTimed(string destinationSceneName, float time)
    {
        StartCoroutine(WaitAndChange(time, destinationSceneName));
    }

    IEnumerator WaitAndChange(float t,string dest)
    {
        yield return new WaitForSeconds(t);
        StoreAndChangeSene(ActorManager.PlayerActor.GetComponent<Player>(), defaultDestinationSceneName);
    }


    public void StoreAndChangeSene(Player player,string destinationSceneName)
    {
        saveData.StorePlayerData(player);

        TransitionEffect.WipeEffet = wipeEffet;
        SceneTransitionManager.TransScene(destinationSceneName, null);
    }
}
