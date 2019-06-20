using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace ActorFunction
{
    [System.Serializable]
    public class ActorFunction<FieldsType, MethodType> where FieldsType : ActorFunctionFields where MethodType : ActorFunctionMethod<FieldsType>
    {
        [SerializeField] MethodType method;
        [SerializeField] FieldsType fields;

        public MethodType Method { get => method; }
        public FieldsType Fields { get => fields; protected set => fields = value; }

        public void CallableUpdate() { method.CallableUpdate(fields); }
    }

    public class ActorFunctionFields { }

    public abstract class ActorFunctionMethod<FieldSetType> : MonoBehaviour where FieldSetType : ActorFunctionFields
    {
        public abstract void CallableUpdate(in FieldSetType fields);
    }
}


//実装方法
//1つのActorFunctionをフィールド部分、メソッド部分それぞれ別のクラスに分けて実装する
//メソッド部がフィールド部のフィールドを自由に参照できるようにするため
//メソッド部はフィールド部の内部クラスとなるが
//Unityは内部クラスをコンポーネントとして認識しないため
//外部にメソッド部を継承したクラスを作る
//フィールド部とメソッド部を統合するクラスとしてActorFunction<フィールド部型名, メソッド部型名>を継承したクラスを定義する
//・フィールド部（[Serializable]付）を定義、その中にメソッド部を定義
//・クラスの外側にメソッド部の子クラスを定義
//・ActorFunction<フィールド部型名, メソッド部型名>（[Serializable]付）を定義する

//[System.Serializable]
//public class Func1Fields : ActorFunctionFields
//{
//    //フィールド...
//
//    public class Method : ActorFunctionMethod<Func1Fields>
//    {
//        public override void Use(Func1Fields fields)
//        {
//            Debug.Log("Func1:" + fields.fl);
//        }
//    }
//}
//
//public class Func1Method : Func1Fields.Method { }
//
//[System.Serializable]
//public class Func1 : ActorFunction<Func1Fields, Func1Method> { }