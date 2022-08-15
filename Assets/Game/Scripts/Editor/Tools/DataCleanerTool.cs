using Game.Managers.StorageManager;

using I2.Loc.SimpleJSON;

using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

namespace Game.Editor
{
    public class DataCleanerTool : EditorWindow
    {
        private static DataCleanerTool window;

        [MenuItem("Tools/Data Cleaner", priority = 0)]
        public static void ManageData()
        {
            //Window
            window = GetWindow<DataCleanerTool>(title: "Data Cleaner", focus: true, utility: true);
            window.maxSize = new Vector2(250, 120);
            window.minSize = new Vector2(250, 120);
            window.ShowUtility();
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();

            var files = Directory.GetFiles(Application.persistentDataPath).Where((x) => !x.EndsWith(".log") && !x.EndsWith("log.txt")).ToArray();
            var directories = Directory.GetDirectories(Application.persistentDataPath);

            GUI.enabled = files.Length != 0 || directories.Length != 0;

            if (GUILayout.Button("Очистить AppData"))
            {
                foreach (var directory in directories)
                {
                    new DirectoryInfo(directory).Delete(true);
                }

                foreach (string filePath in files)
                {
                    File.Delete(filePath);
                }

                EditorGUI.FocusTextInControl(null);
            }

            //List<string> caches = new List<string>();
            //Caching.GetAllCachePaths(caches);

            //GUI.enabled = caches.Count != 0;

            //if (GUILayout.Button("Clean Cache"))
            //{
            //    Caching.ClearCache();

            //    EditorGUI.FocusTextInControl(null);
            //}

            GUI.enabled = true;

            if (GUILayout.Button("Очистить PlayerPrefs"))
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();

                EditorGUI.FocusTextInControl(null);
            }

            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            GUI.enabled = PlayerPrefs.HasKey("save_data");

            if (GUILayout.Button("PlayerPrefs Save"))
			{
                string json = PlayerPrefs.GetString("save_data");

                //var obj = JsonConvert.DeserializeObject(json);
                //json = JsonConvert.SerializeObject(obj, Formatting.Indented);

                var jsonWindow = GetWindow<JsonText>(title: "Json");
                jsonWindow.minSize = new Vector2(400, 700);
                jsonWindow.text = json;
            }
        }

        public string Beautify(string json)
        {
            const int indentWidth = 4;
            const string pattern = "(?>([{\\[][}\\]],?)|([{\\[])|([}\\]],?)|([^{}:]+:)([^{}\\[\\],]*(?>([{\\[])|,)?)|([^{}\\[\\],]+,?))";

            var match = Regex.Match(json, pattern);
            var beautified = new StringBuilder();
            var indent = 0;
            while (match.Success)
            {
                if (match.Groups[3].Length > 0)
                    indent--;

                beautified.AppendLine(
                    new string(' ', indent * indentWidth) +
                    (match.Groups[4].Length > 0
                        ? match.Groups[4].Value + " " + match.Groups[5].Value
                        : (match.Groups[7].Length > 0 ? match.Groups[7].Value : match.Value))
                );

                if (match.Groups[2].Length > 0 || match.Groups[6].Length > 0)
                    indent++;

                match = match.NextMatch();
            }

            return beautified.ToString();
        }
    }

    public class JsonText : EditorWindow
	{
        public string text;
        public Vector2 scroll = new Vector2(0,0);

		private void OnGUI()
		{
            scroll = EditorGUILayout.BeginScrollView(scroll, true, true);
            EditorGUILayout.TextArea(text);
            EditorGUILayout.EndScrollView();
        }
	}
}