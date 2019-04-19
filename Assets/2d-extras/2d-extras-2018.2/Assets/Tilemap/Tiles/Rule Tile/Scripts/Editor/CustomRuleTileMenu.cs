namespace UnityEditor
{
    static class CustomRuleTileMenu
    {
        [MenuItem("Assets/Create/Custom Rule Tile Script", false, 89)]
        static void CreateCustomRuleTile()
        {
            CreateScriptAsset("Assets/2d-extras/2d-extras-2018.2/Assets/Tilemap/Tiles/Rule Tile/ScriptTemplates/NewCustomRuleTile.cs.txt", "NewCustomRuleTile.cs");
        }

        static void CreateScriptAsset(string templatePath, string destName)
        {
            typeof(ProjectWindowUtil)
                .GetMethod("CreateScriptAsset", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                .Invoke(null, new object[] { templatePath, destName });
        }
    }
}
