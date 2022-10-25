using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 同時巻き込みで覚醒ゲージ上昇量を増加させる
/// </summary>
public class ComboCalculator : MonoBehaviour
{
    public class Viewer : MonoBehaviour
    {
        [SerializeField] private ComboCalculator target;

        [SerializeField] Vector2 relativeShift;
        [SerializeField] ComboToast toastPrefab;

        [SerializeField] ComboToast[] comboToasts;
        int currentToastIndex = 0;

        protected ComboCalculator Target
        {
            get => target == null ? (target = ActorManager.PlayerActor.GetComponent<ComboCalculator>()) : target;
            set => target = value;
        }

        private void Start()
        {
            //for(int i=0;i< comboToasts.Length; ++i)
            //{
            //    comboToasts[i] = Instantiate(toastPrefab, Vector3.zero, Quaternion.identity);
            //    comboToasts[i].transform.parent = (transform);
            //    comboToasts[i].gameObject.SetActive(false);
            //}
            Target.onComboIncrementedInt.AddListener(OnComboIncremented);
            Target.onReset.AddListener(OnReset);
            transform.localScale = Vector3.one;
        }

        void OnComboIncremented(int count)
        {
            comboToasts[currentToastIndex]
                .OnComboIncrement(
                    ActorManager.PlayerActor.transform.position + new Vector3(relativeShift.x, relativeShift.y, 0),
                    count);
        }

        void OnReset()
        {
            comboToasts[currentToastIndex].EndCombo();

            for (int i = 0; i < comboToasts.Length; ++i)
            {
                currentToastIndex = (currentToastIndex + 1) % comboToasts.Length;
                if (!comboToasts[currentToastIndex].gameObject.activeSelf)
                {
                    break;
                }
            }
        }
    }

    [System.Serializable] class UnityEvent_int : UnityEngine.Events.UnityEvent<int> { }

    [SerializeField] AnimationCurve awakeIncomeCurve;
    [SerializeField] ActionAwake awake;
    [SerializeField] UnityEngine.Events.UnityEvent onComboIncremented;
    [SerializeField] UnityEvent_int onComboIncrementedInt;
    [SerializeField] UnityEngine.Events.UnityEvent onReset;
    [Tooltip("覚醒アイテムの沸き数の大まかな値")]
    [SerializeField] int oneItemSpawnCount;
    [SerializeField,UnityEngine.Serialization.FormerlySerializedAs("amountsOfOneItem")] float[] awakeAddAmounts;
    [SerializeField] float maxVelocityMagnitude;
    [SerializeField,DisabledField] int comboCount = 0;
    float addedAmount;

    const int MAX_COMBO = 16;
    Mortal[] attackedMortals = new Mortal[MAX_COMBO];
    int attackedMortalsCount;

    bool IsInCombo { get => comboCount > 0; }
    private UnityEvent_int OnComboIncrementedInt { get => onComboIncrementedInt; }
    public UnityEvent OnReset { get => onReset; }

    void Awake()
    {
        ResetCombo();
    }

    // private void OnGUI()
    // {
    //     GUI.Label(new Rect(0, 0, Screen.width, Screen.height), comboCount.ToString());
    // }

    public void ResetCombo()
    {
        comboCount = 0;
        addedAmount = 0;

        attackedMortalsCount = 0;

        onReset.Invoke();
    }

    public void IncrementCombo()
    {
        ++comboCount;

        onComboIncremented.Invoke();
        onComboIncrementedInt.Invoke(comboCount);
    }

    public void IncrementCombo(Mortal subject)
    {
        if(!(subject is Enemy)) { return; }
        if (!IsSubjectAttackedFirst(subject)) { return; }
        IncrementCombo();
        SummonAwakeAddItems(subject);
    }

    bool IsSubjectAttackedFirst(Mortal subject)
    {
        for (int i = 0; i < attackedMortalsCount; ++i)
        {
            if (subject == attackedMortals[i]) { return false; }
        }
        if (attackedMortalsCount < attackedMortals.Length)
        {
            attackedMortals[attackedMortalsCount] = subject;
            ++attackedMortalsCount;
        }
        
        return true;
    }

    float AwakeAddAmount(int combos)
    {
        if(awakeIncomeCurve.length == 0) { return 0; }
        if (combos < 1) { combos = 1; }

        float maxTime = awakeIncomeCurve.keys[awakeIncomeCurve.length - 1].time;
        return awakeIncomeCurve.Evaluate(Mathf.Min(combos, maxTime));
    }

    //public void AddAwakeGauge(int comboCount)
    //{
    //    float add = AwakeAddAmount(comboCount);
    //    float income = add - addedAmount;
    //    addedAmount = add;
    //    Debug.Log("Awake add:" + add);
    //    awake.AddAwakeGauge(income);
    //}

    public void SummonAwakeAddItems(Mortal subject)
    {
        float add = AwakeAddAmount(comboCount);
        float income = add - addedAmount;
        addedAmount = add;

        for (int i = awakeAddAmounts.Length - 1; i >= 0; --i)
        {
            float capacityOfAnItem = i > 0 ? income - oneItemSpawnCount * awakeAddAmounts[i - 1] : income;

            for(capacityOfAnItem -= awakeAddAmounts[i]; capacityOfAnItem > 0; capacityOfAnItem -= awakeAddAmounts[i])
            {
                float magnitude = Random.Range(0, maxVelocityMagnitude);
                float phase = Random.Range(0, 2 * Mathf.PI);
                System.Numerics.Complex velocityC = System.Numerics.Complex.FromPolarCoordinates(magnitude, phase);
                Vector2 velocity = new Vector2((float)velocityC.Real, (float)velocityC.Imaginary);

                AwakeChargeItemPool.Borrow(subject.transform.position, velocity, awakeAddAmounts[i]);
                income -= awakeAddAmounts[i];
            }
        }
    }
}
