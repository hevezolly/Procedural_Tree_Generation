using UnityEngine;
using UnityEditor;
using System.IO;
 
public static class XmlSublimeOpener
{

    const string TextEditor = "D:\\tools\\Sublime Text\\sublime_text.exe";

    [MenuItem("Assets/Open in Text Editor...", priority = 12)]
    static void OpenInTextEditor()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), AssetDatabase.GetAssetPath(Selection.activeObject));
        System.Diagnostics.Process.Start(TextEditor,$"\"{filePath}\"");
    }

    [MenuItem("Assets/Open in Text Editor...", true)]
    static bool ValidateOpenInTextEditor()
    {
        return AssetDatabase.GetAssetPath(Selection.activeObject).EndsWith(".xml", System.StringComparison.OrdinalIgnoreCase);
    }

    [MenuItem("Assets/Create/LSystem/lsystem-xml", priority = 0)]
    public static void CreateLsystemXmlFile()
    {
        string projectWindowPath = AssetDatabase.GetAssetPath(Selection.activeObject);

        string filename = "new l-system.xml"; //Default name that will be marked to rename

        string assetPath = AssetDatabase.GenerateUniqueAssetPath(projectWindowPath + "/" + filename);

        var fullPath = Path.Combine(Application.dataPath.Replace("/Assets", "/"));

        var file = File.CreateText(fullPath + assetPath);
        file.WriteLine("<?xml version=\"1.0\"?>");
        file.WriteLine("<lsml xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"");
        file.WriteLine("	xsi:schemaLocation=\"LSystem D:/Components/LSML/LSML.xsd\"");
        file.WriteLine("	xmlns=\"LSystem\">");
        file.WriteLine();
        file.WriteLine();
        file.WriteLine("    <declaration>");
        file.WriteLine();
        file.WriteLine("    <parameters></parameters>");
        file.WriteLine();
        file.WriteLine("    <modules></modules>");
        file.WriteLine();
        file.WriteLine("    <axiome></axiome>");
        file.WriteLine();
        file.WriteLine("    </declaration>");
        file.WriteLine();
        file.WriteLine();
        file.WriteLine();
        file.WriteLine("</lsml>");
        file.Close();

        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/Create/LSystem/tree-xml", priority = 0)]
    public static void CreateTreeXmlFile()
    {
        var projectWindowPath = AssetDatabase.GetAssetPath(Selection.activeObject);

        var templateFilePath = Path.Combine(Application.dataPath, "Editor/treeTemplate.xml");

        var template = File.ReadAllText(templateFilePath);

        var filename = "new tree l-system.xml"; //Default name that will be marked to rename

        var assetPath = AssetDatabase.GenerateUniqueAssetPath(projectWindowPath + "/" + filename);

        var fullPath = Path.Combine(Application.dataPath.Replace("/Assets", "/"), assetPath);

        var file = File.CreateText(fullPath);
        file.Write(template);
        file.Close();

        AssetDatabase.Refresh();
    }

}
