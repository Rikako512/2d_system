using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Python.Runtime;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using TMPro;

public class ScagnosticsAnalyzer : MonoBehaviour
{
    private Color highlightColor = new Color(0.878f, 0.412f, 0.549f); // ハイライトの色
    private  Color unhighlightColor = new Color(1.0f, 1.0f, 1.0f); // 元の色
    private TMP_Dropdown dropdown;
    public TextMeshProUGUI outputText;
    public GameObject SPM;

    void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();

        if (dropdown != null)
        {
            // ドロップダウンの値が選択されたときのイベントリスナーを追加
            dropdown.onValueChanged.AddListener(SelectionSPs);
        }
        else
        {
            Debug.LogError("Dropdownコンポーネントが見つかりません");
        }

    }

    void SelectionSPs(int value)
    {
        if (outputText != null)
        {
            outputText.text = value.ToString();
        }

        string metric = gameObject.name;
        Debug.Log(metric + "で選択した個数: " + value);

        if (CalculateMetrics.IsInitialized)
        {
            List<int[]> selected_list = CalculateMetrics.top10variables[metric].Take(value).ToList();
            HighlightAllSPs(selected_list);
        }
        else
        {
            Debug.LogWarning("Metrics not initialized yet.");
        }
        Debug.Log("---------- 操作：MetricでSelectionを行いました ----------");
    }

    private void HighlightAllSPs(List<int[]> list)
    {
        ResetSelections();

        if (list != null && list.Count > 0)
        {

            foreach (int[] combination in list)
            {
                if (combination.Length >= 2)
                {
                    int col_x = combination[0];
                    int col_y = combination[1];

                    HighlightQuadbyMetric(col_x, col_y);
                    Debug.Log($"selected by metric: {col_y}, {col_x}");
                }
                else
                {
                    Debug.LogWarning("Combination does not have enough elements.");
                }
            }
        }
        else
        {
            Debug.LogError("Analysis failed or returned null.");
        }
    }

    private void HighlightQuadbyMetric(int col_x, int col_y)
    {
        string quadName = $"{col_y}, {col_x}";
        Transform quadTransform = SPM.transform.Find(quadName);
        
        if (quadTransform != null)
        {
            GameObject quadObject = quadTransform.gameObject;
            SpriteRenderer spriteRenderer = quadObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = highlightColor;
                quadObject.tag = "selected";
            }
            else
            {
                Debug.LogWarning($"SpriteRenderer not found on {quadName}");
            }
        }
        else
        {
            Debug.LogWarning($"Quad named {quadName} not found in SPM");
        }        
    }

    private void ResetSelections()
    {
        foreach (Transform child in SPM.transform)
        {
            // タグが"selected"のQuadを探す
            if (child.CompareTag("selected"))
            {
                SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = unhighlightColor;
                    child.tag = "Untagged"; // 元のタグに戻す
                }
            }
        }
    }
}
