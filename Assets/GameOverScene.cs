using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameOverScene : MonoBehaviour
{
    [SerializeField] float flashSpan;
    [SerializeField] float deadAnimationSpan;
    [SerializeField] float cameraMoveSpan;
    [SerializeField] float gameOverTextAppearSpan;
    [SerializeField] float pressAttackButtonTextAppearSpan;
    [SerializeField] float blackOutSpan;
    [SerializeField] UnityEngine.UI.Image gameOverTextImage;
    [SerializeField] Transform playerDeadAnimation;
    [SerializeField] UnityEngine.UI.Image pressAttackButtonImage;
    [SerializeField] UnityEngine.UI.Image flashImage;
    [SerializeField] string nextSceneName;
    [SerializeField] WipeEffet noWipe;
    [SerializeField] WipeEffet outWipe;
    [SerializeField] InputToFireEvent inputToFireEvent;
    [SerializeField] AudioSource audioSource;

    Vector3 playerScreenCoordinate;
    float orthoCamSize;


    public void SetInitializeData(float orthoCamSize, Vector3 playerScreenCoordinate, int playerDir)
    {
        this.playerScreenCoordinate = playerScreenCoordinate;
        this.orthoCamSize = orthoCamSize;
        Camera.main.orthographicSize = orthoCamSize;
        playerDeadAnimation.position = Camera.main.ScreenToWorldPoint(playerScreenCoordinate);
        playerDeadAnimation.localScale = new Vector3(-1*playerDir, 1, 1)*2;
    }

    private void Start()
    {
        StartCoroutine(StartCo());
    }

    IEnumerator StartCo()
    {
        flashImage.color = Color.white;
        flashImage.DOFade(0, flashSpan);

        yield return new WaitForSeconds(deadAnimationSpan);

        Camera.main.transform
            .DOMove(
                (Vector3)(Vector2)playerDeadAnimation.transform.position + Vector3.forward * Camera.main.transform.position.z,
                cameraMoveSpan)
            .SetEase(Ease.OutExpo);

        yield return new WaitForSeconds(cameraMoveSpan);

        gameOverTextImage.DOFade(1, gameOverTextAppearSpan);
        audioSource.Play();
        inputToFireEvent.enabled = true;

        yield return new WaitForSeconds(1);

        pressAttackButtonImage.DOFade(1, pressAttackButtonTextAppearSpan);

    }

    public void GoNext()
    {
        StartCoroutine(GoNextCo());
    }

    IEnumerator GoNextCo()
    {
        flashImage.color = Color.clear;
        flashImage.DOFade(1, blackOutSpan);
        yield return new WaitForSeconds(blackOutSpan);

        TransitionEffect.InWipeEffet = noWipe;
        TransitionEffect.OutWipeEffet = outWipe;
        SceneTransitionManager.TransScene(nextSceneName, null);
        
    }
}
