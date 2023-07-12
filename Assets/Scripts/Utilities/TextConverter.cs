using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TextConverter : MonoBehaviour
{
    [System.Serializable]
    public class UnityEventTextConversion : UnityEvent<string> { }

    [System.Serializable]
    struct TextPair
    {
        public string textSource;
        public string textDestination;
    }

    [SerializeField] TextPair[] textPairs;
    [SerializeField] UnityEventTextConversion eventTextConverted;

    Dictionary<string, string> textPairsDictionary = new Dictionary<string, string>(16);


    private void Awake()
    {
        foreach(var pair in textPairs)
        {
            textPairsDictionary[pair.textSource] = pair.textDestination;
        }
    }

    public void SendTextEvent(string sourceText)
    {
        string dest;
        string textConverted = textPairsDictionary.TryGetValue(sourceText, out dest) ? dest : sourceText;

        eventTextConverted.Invoke(textConverted);
    }
}
