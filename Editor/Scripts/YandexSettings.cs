#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class YandexSettings : EditorWindow
{
    public const string CssPath = "TemplateData/style.css";
    public const string lanscape = "width: 100%; height: 100%;";
    public const string Portrait = "aspect-ratio: 0.5625;";
    public const string ReplaceTag = "!DisplayMode!";

    public YandexSettingsWraper wraper;

    [MenuItem("Yandex/Settings")]
    static void Init()
    {
        YandexSettings window = (YandexSettings)EditorWindow.GetWindow(typeof(YandexSettings));
        if(EditorPrefs.HasKey(nameof(YandexSettings)))
        {
            window.wraper = JsonUtility.FromJson<YandexSettingsWraper>(
                    EditorPrefs.GetString(nameof(YandexSettings)));
        }
        else
        {
            window.wraper = new YandexSettingsWraper();
        }
        window.Show();
    }
    private void OnDisable()
    {
        EditorPrefs.SetString(
            nameof(YandexSettings), 
            JsonUtility.ToJson(wraper)
            );
    }
    private void OnGUI()
    {
        if(GUILayout.Button("Import Tamplate"))
        {
            AssetDatabase.ImportPackage("Packages/com.imgames.yandexsdk/Package Resources/Tamplate.unitypackage", true);
        }
        if(wraper==null)
        {
            Debug.Log("aaa");   
        }
        wraper.mode = (DisplayMode)EditorGUILayout.EnumPopup("DisplayMode", wraper.mode);
    }
    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if(target == BuildTarget.WebGL)
        {
            YandexSettingsWraper YA =  JsonUtility.FromJson<YandexSettingsWraper>(
                EditorPrefs.GetString(nameof(YandexSettings)));

            var pathToCss = Path.Combine(pathToBuiltProject, CssPath);
            string str = string.Empty;

            using (StreamReader reader = File.OpenText(pathToCss))
            {
                str = reader.ReadToEnd();
            }

            str = str.Replace(ReplaceTag, YA.mode == DisplayMode.Portrait? Portrait : lanscape);

            using (StreamWriter file = new StreamWriter(pathToCss))
            {
                file.Write(str);
            }
        }
    }
}
[Serializable]
public class YandexSettingsWraper
{
    public DisplayMode mode = DisplayMode.Landscape;
}
[Serializable]
public enum DisplayMode
{
    Portrait,
    Landscape
}
#endif
