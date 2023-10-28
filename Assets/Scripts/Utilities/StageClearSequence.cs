using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Async;
using UniRx;

public class StageClearSequence : MonoBehaviour
{
    [SerializeField] float waitSecond0;
    [SerializeField] SotS.UI.ShowDialogScene showStageResult;
    [SerializeField] float waitSecond1;
    [SerializeField] SotS.UI.ShowDialogScene showRecordUpdate;
    [SerializeField] private ChangeSceneToTitle _changeSceneToTitle;

    public void SequenceForget()
    {
        Sequence().Forget();
    }

    public async UniTask<Unit> Sequence()
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(waitSecond0));
        showStageResult.Show();
        
        await UniTask.WaitWhile(()=>showStageResult.IsModalOpen);
        
        await UniTask.Delay(System.TimeSpan.FromSeconds(waitSecond1));
        
        showRecordUpdate.Show();
        await UniTask.WaitWhile(()=>showRecordUpdate.IsModalOpen);

        _changeSceneToTitle.StoreAndChangeScene();
        return Unit.Default;
    }
}
