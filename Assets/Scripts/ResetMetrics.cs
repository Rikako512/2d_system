using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class ResetMetrics : MonoBehaviour
{
    public GameObject SPM;
    private  Color unhighlightColor = new Color(1.0f, 1.0f, 1.0f); // 元の色
    public TextMeshProUGUI outputText01;
    public TextMeshProUGUI outputText02;
    public TextMeshProUGUI outputText03;
    public TextMeshProUGUI outputText04;


    void Start()
    {
        GetComponent<Button>().onClick.AddListener(DestroySelectedSPs);
    }

    void DestroySelectedSPs()	
    {
        outputText01.text = "0";
        outputText02.text = "0";
        outputText03.text = "0";
        outputText04.text = "0";

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
    
        Debug.Log("---------- 操作：ResetMetricボタンを押しました ----------");
    }

}
