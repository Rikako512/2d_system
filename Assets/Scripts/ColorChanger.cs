using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class ColorChanger : MonoBehaviour
{
    public GameObject pointContainer;
    private GameObject databall;
    public Material coloredMaterial; // 赤またはグレーのマテリアル
    public Material highlightedMaterial;
    private Renderer databallRenderer;
    public TextMeshProUGUI headertext;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ChangeColor);
    }


    void ChangeColor()
    {
        FindDataball();

        if (databall != null)
        {
            databallRenderer = databall.GetComponent<Renderer>();
            if (databallRenderer != null)
            {
                //originalMaterial = databallRenderer.material;
            }
            else
            {
                Debug.LogError("Renderer component not found on the Databall.");
            }

        }
        else
        {
            Debug.LogError("Databall GameObject is not assigned in the Inspector.");
        }
        

        if (databallRenderer != null && coloredMaterial != null)
        {
            string colorname = coloredMaterial.name;
            if (databallRenderer.material.name.StartsWith(colorname) == true)
            {
                databallRenderer.material = highlightedMaterial;
            }
            else
            {
                databallRenderer.material = coloredMaterial;
            }

        }
        Debug.Log("---------- 操作：データ点の色を変更しました ----------");
    }

    void FindDataball()
    {
        // テキストから最初の1~3桁の整数を抽出
        Match match = Regex.Match(headertext.text, @"^\d{1,3}");
        if (match.Success)
        {
            int num = int.Parse(match.Value);
            
            // pointcontainerの子オブジェクトを検索
            foreach (Transform child in pointContainer.transform)
            {
                if (child.name == num.ToString())
                {
                    databall = child.gameObject;
                    return;
                }
            }
            
            Debug.LogWarning($"No databall found with name: {num}");
        }
        else
        {
            Debug.LogWarning("No valid number found at the start of the text.");
        }
    }

    void OnDestroy()
    {
        GetComponent<Button>().onClick.RemoveListener(ChangeColor);
    }
}
