using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoUIDisplay : MonoBehaviour
{
    private TextMeshProUGUI headerText;
    private TextMeshProUGUI scrollText;
    public GameObject databall;
    public Material selectedMaterial;
    private bool isSelected = false;
    private Material originalMaterial;
    private Renderer ballRenderer;

    void Start()
    {
        if (databall != null)
        {
            ballRenderer = databall.GetComponent<Renderer>();
            if (ballRenderer != null)
            {
                originalMaterial = ballRenderer.material;
            }
            else
            {
                Debug.LogError("Renderer component not found on the target sphere.");
            }
        }
        else
        {
            Debug.LogError("Target Sphere is not assigned in the inspector.");
        }
    }

    public void OnMouseDown()
    {
        isSelected = !isSelected;

        GameObject headerTextobj = GameObject.FindGameObjectWithTag("scrollheader");
        GameObject scrollTextobj = GameObject.FindGameObjectWithTag("scrolltext");
        
        if (headerTextobj != null && scrollTextobj != null)
        {
            headerText = headerTextobj.GetComponent<TextMeshProUGUI>();
            scrollText = scrollTextobj.GetComponent<TextMeshProUGUI>();
        }      

        if (isSelected)
        {
            // Headerのテキストを更新
            UpdateHeaderAndDetails();
            ChangeSphereAppearance(true);
            Debug.Log("---------- 操作：データ点のUIを表示しました ----------"); 
        }
        else
        {
            ChangeSphereAppearance(false);
        }
    }

    void UpdateHeaderAndDetails()
    {
        if (databall != null)
        {
            string dataName = databall.name;
            if (int.TryParse(dataName, out int index) && index >= 0 && index < CSVData.occupationList.Count)
            {
                headerText.text = CSVData.occupationList[index];
                string data = CSVData.GetDataForIndex(index); // CSVDataからデータを取得
                scrollText.text = data; // Scroll Textを更新
            }
            else
            {
                Debug.LogWarning("Invalid data name or index: " + dataName);
            }
        }
        else
        {
            Debug.LogError("Target Databall or Header Text is not assigned in the inspector.");
        }
    }

    void ChangeSphereAppearance(bool selected)
    {
        if (ballRenderer != null)
        {
            Material currentMaterial = ballRenderer.material;

            // RedPointまたはGrayPointの場合は何もしない（そのまま）
            if (currentMaterial.name.StartsWith("Red_point") || currentMaterial.name.StartsWith("Gray_point"))
            {
                return;
            }

            // RedPointまたはGrayPoint以外の場合
            if (selected)
            {
                ballRenderer.material = selectedMaterial;
            }
            else
            {
                ballRenderer.material = originalMaterial;
            }
        }
    }
}
