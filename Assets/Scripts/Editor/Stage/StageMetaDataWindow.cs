
using UnityEditor;

public class StageMetaDataWindow : StageMetaData.StageMetaDataWindowBase
{
    [MenuItem("Window/Stage Meta Data Editor Window")]
    private static void Open()
    {
        GetWindow<StageMetaDataWindow>("Stage Meta Data Editor Window");
    }
}
