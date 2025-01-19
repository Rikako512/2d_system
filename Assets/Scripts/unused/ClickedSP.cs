using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ClickedSP : MonoBehaviour
{
    private int col_X = -1;
    private int col_Y = -1;

    private GameObject plotter; //大きい方のSP
    private PointRenderer pointRenderer;
    private GameObject pointcontainer;
    private TextMeshProUGUI xAxisText;
    private TextMeshProUGUI yAxisText;

    void Start()
    {
        plotter = GameObject.FindGameObjectWithTag("main_plotter");
        pointcontainer = GameObject.FindGameObjectWithTag("main_pointcontainer");
        
        GameObject xAxisObj = GameObject.FindGameObjectWithTag("main_X_axis");
        GameObject yAxisObj = GameObject.FindGameObjectWithTag("main_Y_axis");

        xAxisText = xAxisObj?.GetComponent<TextMeshProUGUI>();
        yAxisText = yAxisObj?.GetComponent<TextMeshProUGUI>();

        if (xAxisText == null || yAxisText == null)
        {
            Debug.LogError("One or more axis text components not found");
        }

        // PointRendererコンポーネントを取得
        if (plotter != null)
        {
            pointRenderer = plotter.GetComponent<PointRenderer>();
            if (pointRenderer == null)
            {
                Debug.LogError("PointRenderer component not found on Plotter object");
            }
        }
        else
        {
            Debug.LogError("Plotter object not assigned");
        }
    }

    public void OnMouseDown()
    {
        AssignColumnValues();
        UpdateAxisText(); //UIの表記を更新
        UpdatePointRenderer(); // データ点群の分布を更新
        Debug.Log("---------- 操作：Selected by metricのSPをクリックしました ----------");
    }

    private void AssignColumnValues()
    {
        string quadName = transform.name;
        string[] numbers = quadName.Split(',');
        if (numbers.Length == 2)
        {
            col_X = int.Parse(numbers[0].Trim());
            col_Y = int.Parse(numbers[1].Trim());
        }
    }

    private void UpdateAxisText()
    {
        List<string> columnList = new List<string>(CSVData.pointList[1].Keys);

        xAxisText.text = $"X axis: {columnList[col_X]}";
        yAxisText.text = $"Y axis: {columnList[col_Y]}";

        Debug.Log($"Updated column values: X={col_X}, Y={col_Y}");
    }

    private void UpdatePointRenderer()
    {
        if (pointRenderer != null)
        {
            if (pointcontainer != null && pointcontainer.transform.childCount == 0)
            {
                pointRenderer.PlotDataPoints(col_X, col_Y);
            }
            else
            {
                pointRenderer.UpdateDataPoints(col_X, col_Y);
            }
            Debug.Log($"Updated PointRenderer: column1={col_X}, column2={col_Y}");
        }
        else
        {
            Debug.LogError("PointRenderer is not assigned");
        }
    }

}
