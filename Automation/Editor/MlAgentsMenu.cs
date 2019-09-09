using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Diagnostics;
using System;
using MLAgents;

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

    static void PrepScene()
    {
        var academies = GameObject.FindObjectsOfType<Academy>();

        // If only one academy, and only one brain, we know what to do
        // Otherwise, we'd need some kind of config file
        if (academies.Length == 1 && academies[0].broadcastHub.broadcastingBrains.Count == 1)
        {
            var b = academies[0].broadcastHub.broadcastingBrains[0];
            academies[0].broadcastHub.SetControlled(b, true);
        }

        var hiddenAgents = new List<Agent>();
        // Activate all environments, identified by the root-level object that has an agent somewhere in it
        foreach (var agent in Resources.FindObjectsOfTypeAll<Agent>())
        {
            if (agent.hideFlags == HideFlags.None && PrefabUtility.GetPrefabType(agent) != PrefabType.Prefab && PrefabUtility.GetPrefabType(agent) != PrefabType.ModelPrefab)
            {
                hiddenAgents.Add(agent);
                var tx = agent.transform;
                // Climb the hierarchy
                while (tx.parent != null)
                {
                    tx = tx.parent;
                }
                if (tx.gameObject.activeInHierarchy == false) tx.gameObject.SetActive(true);
            }
        }
    }

    static void StartTraining(string root, bool load = false)
    {
        string configFile = EditorPrefs.GetString("MLAgentsConfigFile", "config/trainer_config.yaml");
        string runId = EditorPrefs.GetString("MLAgentsRunId", "");
        if (runId.Trim() == "") runId = SceneManager.GetActiveScene().name;

        var loadCommand = "";
        if (load == true) loadCommand = " --load";

        Process process = new Process();
        process.StartInfo.WorkingDirectory = root;
        process.StartInfo.FileName = "mlagents-learn";
        process.StartInfo.Arguments = configFile + " --train " + loadCommand + " --run-id=" + runId;
        process.StartInfo.UseShellExecute = true;
        process.Start();

        System.Threading.Thread.Sleep(10000);
        EditorApplication.isPlaying = true;
    }

    // Start training
    [MenuItem("ML-Agents/Start Training")]
    static void StartConsole()
    {
        string root;
        if (CheckPreferences(out root))
        {
            PrepScene();

            StartTraining(root, false);
        }
    }

    // Start training
    [MenuItem("ML-Agents/Start Training (with load)")]
    static void StartConsoleLoad()
    {
        string root;
        if (CheckPreferences(out root))
        {
            PrepScene();

            StartTraining(root, true);
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
