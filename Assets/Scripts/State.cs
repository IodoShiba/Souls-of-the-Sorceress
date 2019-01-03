using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class State
/// 状態遷移の各状態を表すクラス
/// StateManagerクラスと併せて使う
/// </summary>
[RequireComponent(typeof(StateManager))]
public abstract class State : MonoBehaviour
{
    StateManager owner;
    [SerializeField] UnityEngine.Events.UnityEvent InitializeCallbacks = new UnityEngine.Events.UnityEvent();
    [SerializeField] UnityEngine.Events.UnityEvent TerminateCallbacks = new UnityEngine.Events.UnityEvent();
    private List<System.Action> initializeActions = new List<System.Action>();
    private List<System.Action> terminateActions = new List<System.Action>();
    [SerializeField]private bool isCurrent = false;

    public bool IsCurrent
    {
        get
        {
            return isCurrent;
        }
    }


    private void Awake()
    {
        initializeActions.Add((System.Action)(() => { this.isCurrent = true; }));
        terminateActions.Add((System.Action)(() => { this.isCurrent = false; }));
    }

    private void Start()
    {
        owner = GetComponent<StateManager>();
    }

    /// <summary>
    /// 初期化用の関数　フィールド等を初期化する目的で使う
    /// 状態遷移がこの状態に突入した時に1回だけ呼ばれる
    /// </summary>
    public abstract void Initialize();

    /// <summary>
    /// 状態を、今の状態に留めるか、別の状態に移すかを決める関数
    /// 遷移先の状態を表すStateクラスインスタンスないしnullを返す
    /// nullを返す場合は現在の状態を継続（もしくは遷移先を決定）し、直後にそのインスタンスのExecute()（遷移があればInitialize()）が呼ばれる
    /// 
    /// 【警告】
    /// この関数がnull以外を返した場合、直後に返り値のStateインスタンスのCheck()も呼ばれる。
    /// そのCheck()の返り値もnull以外ならば、再び返り値のStateインスタンスのCheck()が呼ばれる。
    /// この処理はいずれかのStateインスタンスのCheck()がnullを返す（=遷移先の状態が決定する）まで"同一フレーム内で"続く
    /// つまり、Check()関数に書き込んだ条件に不備があると、Check()関数の呼び出しが循環し（無限ループ）、Unityエディターが動作停止する恐れがある。
    /// この関数の定義ないし変更の際には条件の設定にくれぐれも注意すること。
    /// </summary>
    public abstract State Check();

    /// <summary>
    /// この状態であるときに（通常毎フレーム）実行する内容を表す関数
    /// しかし、この関数は内容が空であるべき関数である
    /// </summary>
    public abstract void Execute();

    /// <summary>
    /// 状態遷移が起こり、この状態を抜けるときに1回だけ呼ばれる関数
    /// 後始末に使う
    /// </summary>
    public abstract void Terminate();

    /// <summary>
    /// StateManagerから呼ぶための関数　使うな
    /// </summary>
    public void _InvokeInitializeCallBack()
    {
        InitializeCallbacks.Invoke();
        foreach(var f in initializeActions)
        {
            f();
        }
    }
    
    /// <summary>
    /// StateManagerから呼ぶための関数　使うな
    /// </summary>
    public void _InvokeTerminateCallBack()
    {
        TerminateCallbacks.Invoke();
        foreach (var f in terminateActions)
        {
            f();
        }
    }

    /// <summary>
    /// Initialize()と同時に呼ぶコールバック関数を登録する
    /// </summary>
    /// <param name="listener">登録するコールバック関数</param>
    public void RegisterInitialize(System.Action listener) {
        initializeActions.Add(listener);
    }
    
    /// <summary>
    /// Terminate()と同時に呼ぶコールバック関数を登録する
    /// </summary>
    /// <param name="listener">登録するコールバック関数</param>
    public void RegisterTerminate(System.Action listener) {
        terminateActions.Add(listener);
    }

    /// <summary>
    /// Initialize()と同時に呼ぶコールバック関数と、Terminate()と同時に呼ぶコールバック関数を同時に登録する
    /// </summary>
    /// <param name="initializeListener">Initialize()と同時に呼ぶコールバック関数</param>
    /// <param name="terminateListener">Terminate()と同時に呼ぶコールバック関数</param>
    public void RegisterTurnAction(System.Action initializeListener,System.Action terminateListener)
    {
        RegisterInitialize(initializeListener);
        RegisterTerminate(terminateListener);
    }

    /// <summary>
    /// 今、別の状態に移ろうとした場合、ちょうどその状態に遷移できるか判定し、
    /// 移れる場合は遷移先自身を、移れない場合はnullを返す関数
    /// このStateのCheck内で
    /// <code>
    ///     return Precheck(GetComponent<TargetState>());
    /// </code>
    /// のようにして使う
    /// 遷移先のStateのCheck()が二重で呼ばれてしまうので注意
    /// </summary>
    /// <param name="destination">遷移先の状態</param>
    /// <returns></returns>
    public State Precheck(State destination)
    {
        return destination.Check() == null ? destination : null;
    }

    
}

[RequireComponent(typeof(StateManager))]
public abstract class HorizontalDirectionDependentState : State
{
    protected int dirSign = 0;
    public void SetDirection(int sign)
    {
        dirSign = sign;
    }
}

