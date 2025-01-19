using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuadInteractionHandler : MonoBehaviour
{
    private Color highlightColor = new Color(0.973f, 0.953f, 0.549f); // クリックした時の色
    private  Color hoveredColor = new Color(0.31f, 0.80f, 0.89f); // ホバー時の色
    private  Color hoveredColor02 = new Color(0.31f, 0.89f, 0.31f); // ホバー時の色その２
    private  Color unhighlightColor = new Color(1.0f, 1.0f, 1.0f); // 元の色
    private Color selectedColor = new Color(0.878f, 0.412f, 0.549f); // selectionの色

    private int[] clickedNum = new int[2];

    private List<Renderer> affectedRenderers = new List<Renderer>();
    private bool isHovered = false;
    private static List<Renderer> selectedRenderers = new List<Renderer>();

    // 静的変数で選択されたQuadの情報を保持
    private static QuadInteractionHandler selectedQuad = null;

    private static int col_X = -1;
    private static int col_Y = -1;

    public GameObject plotter; //大きい方のSP
    private PointRenderer pointRenderer; //大きい方のSP
    public TextMeshProUGUI xAxisText;
    public TextMeshProUGUI yAxisText;

    public Transform SPM;

    public GameObject pointcontainer;


    void Start()
    {
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
        // 前の選択をリセット
        ResetAllHighlights();

        selectedQuad = this;
        Debug.Log($"---------- 操作：Quad: {gameObject.name} を選択しました ----------");

        AssignColumnValues(gameObject.name);        
        UpdateQuadState(gameObject.name, true);
        UpdateAxisText();
    }
        
    public void OnMouseEnter()
    {
        isHovered = true;
        //Debug.Log($"Quad {gameObject.name} hover entered");
        UpdateQuadState(gameObject.name, false);
    }

    public void OnMouseExit()
    {
        isHovered = false;
        //Debug.Log($"Quad {gameObject.name} hover exited");
        UpdateQuadState(gameObject.name, false);
    }

    private void UpdateQuadState(string quadName, bool isSelecting)
    {
        ParseQuadName(quadName);
        HighlightRelatedQuads(isSelecting, isHovered);
    }

    private void AssignColumnValues(string quadName)
    {
        string[] parts = quadName.Split(',');
        if (parts.Length != 2 || !int.TryParse(parts[0], out int value1) || !int.TryParse(parts[1], out int value2))
        {
            Debug.LogError($"Invalid quad name format: {quadName}");
            return;
        }

        col_X = value2;
        col_Y = value1;

        Debug.Log($"Updated column values: X={col_X}, Y={col_Y}");

        UpdatePointRenderer();
    }

    private void UpdateAxisText()
    {
        List<string> columnList = new List<string>(CSVData.pointList[1].Keys);

        xAxisText.text = $"{columnList[col_X]}";
        yAxisText.text = $"{columnList[col_Y]}";
    }

    // Scatterplotの軸の変数を変更
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
    
    private void ParseQuadName(string name)
    {
        string[] parts = name.Split(',');
        if (parts.Length == 2 && int.TryParse(parts[0], out clickedNum[0]) && int.TryParse(parts[1], out clickedNum[1]))
        {
            //Debug.Log($"Parsed numbers: {clickedNum[0]}, {clickedNum[1]}");
        }
        else
        {
            Debug.LogError($"Failed to parse quad name: {name}");
        }
    }
    
    private void HighlightRelatedQuads(bool isSelecting, bool isHovering)
    {
        if (isSelecting)
        {
            ResetAllHighlights();
        }
        else
        {
            ResetHighlights();
        }

        // col_x, col_yに基づいてQuadをハイライト
        
        if (col_X != -1)
        {
            HighlightQuads(1, 0, isSelecting, isHovering, col_X);
        }
        if (col_Y != -1)
        {
            HighlightQuads(0, 0, isSelecting, isHovering, col_Y);
        }

        if(isHovering)
        {
            HighlightQuads(-1, -1, isSelecting, isHovering, null);
        }
        
    }

    private void HighlightQuads(int partsIndex, int clickedNumIndex, bool isSelecting, bool isHovering, int? specificValue = null)
    {
        foreach (Transform child in SPM)
        {
            string childName = child.name;
            string[] parts = childName.Split(',');
            bool shouldHighlight = false;

            if (specificValue.HasValue) // colとして選ばれた箇所
            {
                shouldHighlight = parts.Length == 2 && int.TryParse(parts[partsIndex], out int num) && num == specificValue.Value;
            }
            else if (partsIndex == -1 && clickedNumIndex == -1)
            {
                shouldHighlight = parts.Length == 2 && 
                    (int.TryParse(parts[0], out int num1) && num1 == clickedNum[0] || 
                    int.TryParse(parts[1], out int num2) && num2 == clickedNum[1]);
            }

            if (shouldHighlight)
            {
                SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    Color colorToApply;
                    
                    if (child.CompareTag("selected"))
                    {
                        colorToApply = selectedColor; // "selected"タグが付いているQuadは常にselectedColorを使用
                    }
                    else if (isSelecting || selectedRenderers.Contains(spriteRenderer))
                    {
                        colorToApply = highlightColor; // クリック時
                        
                        if (!selectedRenderers.Contains(spriteRenderer))
                        {
                            selectedRenderers.Add(spriteRenderer);
                        }
                    }
                    else if (isHovering && !child.CompareTag("selected"))
                    {
                        if (selectedQuad != null && selectedQuad.isHovered)
                        {
                            colorToApply = hoveredColor02;
                        }
                        else
                        {
                            colorToApply = hoveredColor; // ホバー時
                        }
                    }
                    else
                    {
                        colorToApply = unhighlightColor;
                    }

                    spriteRenderer.color = colorToApply;
                    //Debug.Log($"Applied {materialToApply.name} to {child.name}");
                    if (!affectedRenderers.Contains(spriteRenderer))
                        affectedRenderers.Add(spriteRenderer);
                }
            }
        }
    }

    private void ResetAllHighlights()
    {
        foreach (SpriteRenderer spriteRenderer in selectedRenderers)
        {
            if (!spriteRenderer.gameObject.CompareTag("selected"))
            {
                spriteRenderer.color = unhighlightColor;
            }
        }
        selectedRenderers.Clear();

        foreach (SpriteRenderer spriteRenderer in affectedRenderers)
        {
            if (!spriteRenderer.gameObject.CompareTag("selected"))
            {
                spriteRenderer.color = unhighlightColor;
            }
        }
        affectedRenderers.Clear();
    }

    private void ResetHighlights()
    {
        foreach (SpriteRenderer spriteRenderer in affectedRenderers)
        {
            if (!selectedRenderers.Contains(spriteRenderer) && !spriteRenderer.gameObject.CompareTag("selected"))
            {
                spriteRenderer.color = unhighlightColor;
            }
        }
        affectedRenderers.RemoveAll(r => !selectedRenderers.Contains(r) && !r.gameObject.CompareTag("selected"));
    }


    void OnDestroy()
    {
        // 選択されていたQuadが破棄される場合、選択状態をリセット
        if (selectedQuad == this)
        {
            selectedQuad = null;
        }
    }
}
