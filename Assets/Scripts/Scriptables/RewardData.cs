using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "ScriptableObject/RewardData")]
public class RewardData : ScriptableObject
{
    public enum RewardType
    {
        addAwakeGaugeOnDead,
    }

    [System.Serializable]
    public class RewardDatum
    {
        [SerializeField] RewardType type;
        [SerializeField] int intAmount;
        [SerializeField] float floatAmount;

        public RewardType Type { get => type; }
        public int IntAmount { get => intAmount; }
        public float FloatAmount { get => floatAmount; }
    }

    [SerializeField] List<RewardDatum> rewards;

    public List<RewardDatum> Rewards { get => rewards; }


    //#if UNITY_EDITOR
    //    [CustomPropertyDrawer(typeof(RewardDatum))]
    //    class RewardDatumDrawer : PropertyDrawer
    //    {
    //        const int alineh = 16;

    //        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => alineh * 2;

    //        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //        {
    //            SerializedProperty rewardTypeP = property.FindPropertyRelative("type");
    //            SerializedProperty intAmountP = property.FindPropertyRelative("intAmount");
    //            SerializedProperty floatAmountP = property.FindPropertyRelative("floatAmount");

    //            typeof(RewardType).GetFields(System.Reflection.BindingFlags.Public)
    //            EditorGUI.PropertyField(position, intAmountP);
    //            position.position += Vector2.down * alineh;
    //            EditorGUI.PropertyField(position, floatAmountP);
    //            position.position += Vector2.down * alineh;
    //        }
    //    }
    //#endif
}
