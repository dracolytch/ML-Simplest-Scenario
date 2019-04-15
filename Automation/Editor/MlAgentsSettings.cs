using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MlAgentsSettings : MonoBehaviour {

    // Have we loaded the prefs yet
    private static bool prefsLoaded = false;

    // The Preferences
    public static string MlAgentsRoot = "";
    public static string ConfigFile = "config/trainer_config.yaml";
    public static string RunId = "";

    // Add preferences section named "My Preferences" to the Preferences window
    [PreferenceItem("ML-Agents")]
    public static void PreferencesGUI()
    {
        // Load the preferences
        if (!prefsLoaded)
        {
            MlAgentsRoot = EditorPrefs.GetString("MLAgentsRoot", MlAgentsRoot);
            ConfigFile = EditorPrefs.GetString("MLAgentsConfigFile", ConfigFile);
            RunId = EditorPrefs.GetString("MLAgentsRunId", RunId);
            prefsLoaded = true;
        }

        // Preferences GUI
        MlAgentsRoot = EditorGUILayout.TextField("ML-Agents Root", MlAgentsRoot);
        ConfigFile = EditorGUILayout.TextField("Config File", ConfigFile);
        RunId = EditorGUILayout.TextField("Run ID (blank for Scene name)", RunId);

        // Save the preferences
        if (GUI.changed)
        {
            EditorPrefs.SetString("MLAgentsRoot", MlAgentsRoot);
            EditorPrefs.SetString("MLAgentsConfigFile", ConfigFile);
            EditorPrefs.SetString("MLAgentsRunId", RunId);
        }
    }
}
