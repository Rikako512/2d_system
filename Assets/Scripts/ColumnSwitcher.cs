using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using System.Linq;

public class ColumnSwitcher : MonoBehaviour
{
    public TextMeshPro textMeshPro;
    public TextMeshPro secondTextMeshPro;
    public Transform SPM; // SPMオブジェクトへの参照

    private static List<ColumnSwitcher> selectedObjects = new List<ColumnSwitcher>();
    //private static ColumnSwitcher firstSelected = null;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private float moveSpeed = 10f;

    private List<Transform> linkedQuads = new List<Transform>();

    
    private void Start()
    {
        targetPosition = transform.position;
        textMeshPro = GetComponentInChildren<TextMeshPro>();

        FindLinkedQuads();
    }

    private void OnMouseDown()
    {/*
        if (firstSelected == null)
        {
            firstSelected = this;
            ChangeTextColor(Color.yellow);
        }
        else if (firstSelected != this)
        {
            ChangeTextColor(Color.yellow);
            StartCoroutine(SwapPositions(firstSelected, this));
            firstSelected = null;
            Debug.Log("---------- 操作：次元を手動で入れ替えました ----------");
        }
        else
        {
            ChangeTextColor(Color.white);
            firstSelected = null;
        }*/

        if (!selectedObjects.Contains(this))
        {
            selectedObjects.Add(this);
            ChangeTextColor(Color.yellow);

            if (selectedObjects.Count == 2)
            {
                StartCoroutine(SwapPositions(selectedObjects[0], selectedObjects[1]));
                Debug.Log("---------- 操作：次元を手動で入れ替えました ----------");
            }
        }
    }

    private void ChangeTextColor(Color color)
    {
        if (textMeshPro != null)
        {
            textMeshPro.color = color;
        }
        if (secondTextMeshPro != null)
        {
            secondTextMeshPro.color = color;
        }
    }

    private void FindLinkedQuads()
    {
        string parentName = transform.parent.name;
        string columnName = transform.name;

        if (SPM != null)
        {
            if (parentName == "X")
            {
                linkedQuads.AddRange(FindAllQuadsInChild(columnName, 1));
            }
            else if (parentName == "Y")
            {
                linkedQuads.AddRange(FindAllQuadsInChild(columnName, 0));
            }
        }
    }	

    private bool IsValidCName(string name)
    {
        string[] parts = name.Split(',');
        if (parts.Length != 2) return false;
        
        return parts.All(part => 
            part.Trim().Length == 1 && char.IsDigit(part.Trim()[0]));
    }

    private List<Transform> FindAllQuadsInChild(string columnName, int indexToCheck)
    {
        List<Transform> quads = new List<Transform>();
        if (SPM != null)
        {
            quads.AddRange(SPM.GetComponentsInChildren<Transform>()
                .Where(t => {
                    string[] parts = t.name.Split(',');
                    return parts.Length == 2 && parts[indexToCheck].Trim() == columnName;
                }));
        }
        return quads;
    }

    private void Update()
	{
        Vector3 previousPosition;
        //Vector3 newPosition;
        Vector3 movement;

		    if (isMoving)
		    {
		        previousPosition = transform.position;
		        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
		        movement = transform.position - previousPosition;
		
		        foreach (var linkedObject in linkedQuads)
		        {
		            if (linkedObject != null)
		            {
		                linkedObject.position += movement;
		            }
		        }
	             	
		        if (transform.position == targetPosition)
		        {
		            isMoving = false;
		        }
		    }
	}
/*
    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (!selectedObjects.Contains(this))
        {
            selectedObjects.Add(this);
            
            if (textMeshPro != null)
            {
                textMeshPro.color = Color.yellow;
            }

            if (selectedObjects.Count == 2)
            {
                StartCoroutine(SwapPositions(selectedObjects[0], selectedObjects[1]));
                
            }
        }
    }
    */

    private IEnumerator SwapPositions(ColumnSwitcher obj1, ColumnSwitcher obj2)
    {
        Vector3 tempPosition = obj1.transform.position;
        
        obj1.targetPosition = obj2.transform.position;
        obj2.targetPosition = tempPosition;
        
        obj1.isMoving = true;
        obj2.isMoving = true;

        yield return new WaitForSeconds(1f);

        obj1.ChangeTextColor(Color.white);
        obj2.ChangeTextColor(Color.white);

        selectedObjects.Clear();
    }
}
