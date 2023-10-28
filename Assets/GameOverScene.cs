using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using SotS;
using UniRx;

public class GameOverScene : MonoBehaviour
{
    [SerializeField] Vector2 playerFinalOffset;
    [SerializeField] float flashSpan;
    [SerializeField] float deadAnimationSpan;
    [SerializeField] float cameraMoveSpan;
    [SerializeField] float gameOverTextAppearSpan;
    [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("pressAttackButtonTextAppearSpan")] float buttonAppearSpan;
    [SerializeField] float blackOutSpan;
    [SerializeField] float reviveWaitSpan;
    [SerializeField] UnityEngine.UI.Image gameOverTextImage;
    [SerializeField] Transform playerDeadAnimation;
    [SerializeField] UnityEngine.UI.Button giveUpButton;
    [SerializeField] UnityEngine.UI.Button reviveButton;
    [SerializeField] TMPro.TMP_Text remainingCount;
    [SerializeField] UnityEngine.UI.Image flashImage;
    [SerializeField] string nextSceneName;
    [SerializeField] ChangeSceneToTitle _changeSceneToTitle;
    [SerializeField] WipeEffet noWipe;
    [SerializeField] WipeEffet outWipe;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Animator gameOverAnimatorSarah;

    Vector3 playerScreenCoordinate;
    float orthoCamSize;

    ButtonProcessor giveUpButtonProcessor;
    ButtonProcessor reviveButtonProcessor;

    static UniRx.Subject<Unit> subjectGameOver = new UniRx.Subject<Unit>();
    public static System.IObservable<UniRx.Unit> observableGameOver {get => subjectGameOver;}

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
        giveUpButtonProcessor = new ButtonProcessor(giveUpButton);
        reviveButtonProcessor = new ButtonProcessor(reviveButton);

        subjectGameOver.OnNext(Unit.Default);

        StartCoroutine(StartCo());
    }

    IEnumerator StartCo()
    {
        flashImage.color = Color.white;
        flashImage.DOFade(0, flashSpan);

        yield return new WaitForSeconds(0.8f); //flash待ち

        gameOverAnimatorSarah.SetTrigger("AnimStartTrigger");

        yield return new WaitForSeconds(deadAnimationSpan);

        Camera.main.transform
            .DOMove(
                (Vector3)(Vector2)playerDeadAnimation.transform.position -
                    (Vector3)(Vector2)Camera.main.ViewportToWorldPoint((Vector3)(playerFinalOffset + Vector2.one)/2) +
                    Vector3.forward * Camera.main.transform.position.z,
                cameraMoveSpan)
            .SetEase(Ease.OutExpo);

        yield return new WaitForSeconds(cameraMoveSpan);

        gameOverTextImage.DOFade(1, gameOverTextAppearSpan);
        audioSource.Play();
        //inputToFireEvent.enabled = true;

        yield return new WaitForSeconds(1);

        giveUpButtonProcessor.EnableButton(true);
        if(SotS.ReviveController.IsRevivable)
        {
            reviveButtonProcessor.EnableButton(true);
            reviveButtonProcessor.SetAsSelected();
        }
        else
        {
            reviveButtonProcessor.ActivateObject(false);
            giveUpButtonProcessor.SetAsSelected();
        }

        reviveButtonProcessor.Appear(buttonAppearSpan);
        giveUpButtonProcessor.Appear(buttonAppearSpan);
        remainingCount.DOFade(1, buttonAppearSpan);
    }

    public void LockButtons()
    {
        giveUpButtonProcessor.EnableButton(false);
        reviveButtonProcessor.EnableButton(false);
    }

    public void GiveUp()
    {
        GameLifeCycle.CloseGame(GameLifeCycle.CloseCause.GiveUp);
        StartCoroutine(GiveUpCo());
    }

    IEnumerator GiveUpCo()
    {
        flashImage.color = Color.clear;
        flashImage.DOFade(1, blackOutSpan);
        yield return new WaitForSeconds(blackOutSpan);
        
        _changeSceneToTitle.StoreAndChangeScene();
    }

    public void Revive()
    {
        StartCoroutine(ReviveCo());
    }

    public IEnumerator ReviveCo()
    {
        using(System.IDisposable reviveSuspensor = ReviveController.GetReviveSuspensor())
        {
            if(reviveSuspensor == null){ yield break; }

            yield return new WaitForSeconds(reviveWaitSpan);

            flashImage.color = Color.clear;
            flashImage.DOFade(1, blackOutSpan);
            yield return new WaitForSeconds(blackOutSpan);

            TransitionEffect.InWipeEffect = noWipe;
            TransitionEffect.OutWipeEffect = outWipe;
        }
    }

    class ButtonProcessor
    {
        UnityEngine.UI.Button button;
        UnityEngine.UI.Image image;
        TMPro.TMP_Text text;

        public ButtonProcessor(UnityEngine.UI.Button button)
        {
            this.button = button;
            this.image = button.GetComponent<UnityEngine.UI.Image>();
            this.text = button.GetComponentInChildren<TMPro.TMP_Text>();
        }

        public void Appear(float duration = 1f)
        {
            if(!button.gameObject.activeSelf){return;}

            image.DOFade(1, duration);
            text.DOFade(1, duration);
        }

        public void ActivateObject(bool does)
        {
            button.gameObject.SetActive(does);
        }

        public void EnableButton(bool does)
        {
            button.interactable = does;
        }

        public void SetAsSelected()
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(button.gameObject);
        }
    }
}
