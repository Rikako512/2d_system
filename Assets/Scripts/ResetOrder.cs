using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetOrder : MonoBehaviour
{
    public GameObject SPM;
    private Dictionary<Transform, Vector2> initialPositions = new Dictionary<Transform, Vector2>();

    void Start()
    {
        if (SPM != null)
        {
            // SPMの全ての子孫オブジェクトの初期位置を保存
            SaveInitialPositions(SPM.transform);
        }
        else
        {
            Debug.LogError("SPM or SPM_select is not assigned in the Inspector.");
        }

        GetComponent<Button>().onClick.AddListener(ResetPositions);
    }

    void SaveInitialPositions(Transform parent)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>())
        {
            initialPositions[child] = child.localPosition;
        }
    }

    public void ResetPositions()
    {
        // 全ての保存されたオブジェクトを初期位置に戻す
        foreach (var kvp in initialPositions)
        {
            kvp.Key.localPosition = kvp.Value;
        }
  
        Debug.Log("---------- 操作：ResetOrderボタンを押しました ----------");
    }

    public Vector3 GetInitialPosition(Transform transform)
    {
        if (initialPositions.TryGetValue(transform, out Vector2 position))
        {
            return position;
        }
        return transform.localPosition;
    }

}
