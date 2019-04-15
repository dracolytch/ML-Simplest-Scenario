using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Diagnostics;
using System;

public class MlAgentsMenu : MonoBehaviour {

    // Check to see if the root is defined
    static bool CheckPreferences(out string root)
    {
        var temp = EditorPrefs.GetString("MLAgentsRoot", "");
        if (temp.Trim() == "" || System.IO.Directory.Exists(temp) == false)
        {
            EditorUtility.DisplayDialog("Error", "The ML Agents root path has not been set, or it points to a folder that doesn't exist. Plese set this in Edit->Preferences->ML-Agents", "Ok");
            root = "";
            return false;
        }
        else
        {
            root = temp;
            return true;
        }
    }

    // Start training
    [MenuItem("ML-Agents/Start Training")]
    static void StartConsole()
    {
        string root;
        if (CheckPreferences(out root))
        {
            string configFile = EditorPrefs.GetString("MLAgentsConfigFile", "config/trainer_config.yaml");
            string runId = EditorPrefs.GetString("MLAgentsRunId", "");
            if (runId.Trim() == "") runId = SceneManager.GetActiveScene().name;

            Process process = new Process();
            process.StartInfo.WorkingDirectory = root;
            process.StartInfo.FileName = "mlagents-learn";
            process.StartInfo.Arguments = configFile + " --train --run-id=" + runId;
            process.StartInfo.UseShellExecute = true;
            process.Start();
        }
    }

    // Fire up TensorBoard
    [MenuItem("ML-Agents/Start TensorBoard")]
    static void StartTensorboard()
    {
        string root;
        if (CheckPreferences(out root))
        {
            Process process = new Process();
            process.StartInfo.WorkingDirectory = root;
            process.StartInfo.FileName = "tensorboard.exe";
            process.StartInfo.Arguments = "--logdir=summaries";
            process.StartInfo.UseShellExecute = true;
            process.Start();
        }
    }

    // Open a browser to show TensorBoard
    [MenuItem("ML-Agents/Open TensorBoard Browser")]
    static void OpenTensorboard()
    {
        string root;
        if (CheckPreferences(out root))
        {
            Process process = new Process();
            process.StartInfo.FileName = "explorer";
            process.StartInfo.Arguments = "http://" + Environment.MachineName + ":6006";
            process.StartInfo.UseShellExecute = true;
            process.Start();
        }
    }

    // Open the Models folder
    [MenuItem("ML-Agents/Open Models Folder")]
    static void OpenModels()
    {
        string root;
        if (CheckPreferences(out root))
        {
            Process process = new Process();
            process.StartInfo.WorkingDirectory = root;
            process.StartInfo.FileName = "explorer";
            process.StartInfo.Arguments = System.IO.Path.Combine(root, "Models");
            process.Start();
        }
    }
}
