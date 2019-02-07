using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeInputCurriculumViz : MonoBehaviour
{

    public int Rows = 4;
    public int Cols = 5;
    public int Layers = 1;
    public GameObject VizPrefab;
    public Material VisMaterial;
    public float ColorOffset = 1;
    public float ColorScale = 1;
    public float ColSpacing = 2f;
    public float RowSpacing = 2.5f;
    public float LayerSpacing = 1f;
    public ThreeInputCurriculumAgent agent;

    // Use this for initialization
    public void Visualize()
    {
        // Clear existing children
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        // Redraw
        var count = 1;
        Vector3 Cursor = new Vector3(ColSpacing * Cols * -0.5f, RowSpacing * Rows * -0.5f, LayerSpacing * Layers * -0.5f);

        for (var z = 0; z < Layers; z++)
        {
            for (var y = 0; y < Rows; y++)
            {
                for (var x = 0; x < Cols; x++)
                {
                    var marker = Instantiate<GameObject>(VizPrefab, Cursor, Quaternion.identity, transform);
                    marker.name = "VizMarker" + count;
                    count++;

                    var ren = marker.GetComponent<Renderer>();
                    var value = marker.GetComponent<VizOutput>();
                    if (ren != null)
                    {
                        var mat = Instantiate<Material>(VisMaterial);
                        var val = agent.CalculateReward(Cursor, agent.Target.transform.position);
                        value.value = val;
                        if (val > 0) mat.color = Color.Lerp(Color.black, Color.white, ColorOffset + (val * ColorScale));
                        else mat.color = Color.Lerp(Color.black, Color.red, ColorOffset + (val * ColorScale * -1f));
                        ren.material = mat;
                    }

                    Cursor.x += ColSpacing;
                }

                Cursor.x = ColSpacing * Cols * -0.5f; // Reset X position
                Cursor.y += RowSpacing;
            }
            Cursor.y = RowSpacing * Rows * -0.5f; // Reset Y position
            Cursor.z += LayerSpacing;
        }

    }
}
