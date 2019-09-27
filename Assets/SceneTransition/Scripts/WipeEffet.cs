using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "ScriptableObject/WipeEffect")]
public class WipeEffet : ScriptableObject
{
    [SerializeField] Material shaderMaterial;
    [SerializeField] Texture texture;
    [SerializeField] AnimationCurve borderSIn;
    [SerializeField] AnimationCurve borderEIn;
    [SerializeField] AnimationCurve borderSOut;
    [SerializeField] AnimationCurve borderEOut;
    [SerializeField] float inTime;
    [SerializeField] float intervalTime;
    [SerializeField] float outTime;

    public Material ShaderMaterial { get => shaderMaterial; }
    public Texture Texture { get => texture; }
    public AnimationCurve BorderSIn { get => borderSIn; }
    public AnimationCurve BorderEIn { get => borderEIn; }
    public AnimationCurve BorderSOut { get => borderSOut; }
    public AnimationCurve BorderEOut { get => borderEOut; }
    public float InTime { get => inTime; }
    public float IntervalTime { get => intervalTime; }
    public float OutTime { get => outTime; }
}
