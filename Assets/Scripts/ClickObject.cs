using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;

public class ClickObject : MonoBehaviour
{
    //public bool renderLargeGraphPrefab = true;
    public GameObject plotter; 
    public bool firstclick;


    public GameObject panel_label_x;
    public GameObject panel_label_y;

    private static readonly Color highlightColor = new Color(0.973f, 0.953f, 0.549f); // ハイライトの色
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private static GameObject lastHighlighted;

    private int click_X;
    private int click_Y;

    private int before_X;
    private int before_Y;

    private GameObject container; // Large Graph の PointContainer

    private float xMin;
    private float yMin;

    private float xMax;
    private float yMax;

    private string xColumnName;
    private string yColumnName;

    private int rowCount; // ここで取得するとエラー出る

    private List<GameObject> points = new List<GameObject>(); // データ点のオブジェクトを格納するリスト
    /*
    private List<Vector3> before_point_positions = new List<Vector3>();　// 使ってないっぽい
    private List<Vector3> after_point_positions = new List<Vector3>(); // これ毎回解放すべき？
    //private Vector3[] before_point_positions = new Vector3[rowCount];
    //private Vector3[] after_point_positions = new Vector3[rowCount];
   
    
    private GameObject before_point;
    private Vector3 after_point_position;

    private Vector3 targetPosition;
    //private List<Vector3> targetPosition = new List<Vector3>();
    private float movementSpeed = 5.0f; // 調整可能な移動速度
    private bool isMoving = false;
    */
    private PointRenderer plotterScript; // データ点をプロットするスクリプト
    private int firstNumber;
    private int secondNumber;


    // Scatterplot Matrix の Quard がクリックされたら実行
    public void OnMouseDown()
    {
        Debug.Log("========== ClickObject 開始 ==========");
        rowCount = CSVData.pointList.Count;
        string objectName = gameObject.name;
        Match match = Regex.Match(objectName, @"(\d), (\d)");
        //Debug.Log("Square clicked!, " + objectName);
        List<string> columnList = new List<string>(CSVData.pointList[1].Keys);


        if (match.Success)
        {
            firstNumber = int.Parse(match.Groups[1].Value);
            secondNumber = int.Parse(match.Groups[2].Value);

            Debug.Log("Clicked: " + firstNumber + ", " + secondNumber);
        }
        else
        {
            Debug.LogWarning("オブジェクト名が期待された形式ではありません: " + objectName);
        }

        click_Y = firstNumber;
        click_X = secondNumber;

        xColumnName = columnList[click_X];
        yColumnName = columnList[click_Y];

        AssignLabels();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // 前回のハイライトをリセット
        ResetLastHighlighted();

        // 現在のオブジェクトをハイライト
        HighlightObject();

        if (firstclick == true)
        {
            // 初回のクリックで大きいグラフに点をプロット
            PlacePrefabLargeGraph();
            firstclick = false;
        }
        else
        {
            //2回目以降は点をアニメーションで移動させる
            //PointAnimation();
        }

        /*
 
            // Call PlacePoint methods defined below
            GameObject largeGraph = GameObject.Find("Large Graph");
            //GameObject container = GameObject.Find("GraphContainer");

            // まだLarge Graphがない場合 = 初めてマトリクス上のオブジェクトをクリックした場合、Large Graph を生成する
            if(largeGraph == null)
            {
            }else{ // マトリックス上のオブジェクトをクリックしたのが２回目以降のとき

                // ＊＊　直前にLargeGraphで表示されていた散布図のデータ点をcontainerに保存　＊＊
                container = largeGraph.transform.Find("GraphFrame/PointContainer").gameObject;

                // 既にハイライトされたオブジェクトがある場合、ハイライトをリセット
                if(lastHighlighted != null)
                {
                   // 直前にハイライトされたオブジェクトの3つの次元に対応する値を取得する
                   if(matrixScatterplot.transform.tag == "Scatterplot")
                   {
                      GameObject before_plot = lastHighlighted.transform.parent.gameObject; 

                      // 直前に選択していた散布図の軸の次元2つをbefore_X,Yに保存
                      PointRenderer before_plotter = before_plot.transform.Find("GraphFrame/Plotter").gameObject.GetComponent<PointRenderer>();
                      before_X = before_plotter.column1;
                      before_Y = before_plotter.column2;
                  
                  
                   }

                   // ハイライトされたオブジェクトからクリックしたオブジェクトへの経路を見つける
                   //FindPath(before_X, before_Y, before_Z, click_X, click_Y, click_Z);

                   // データ点の遷移アニメーションを実行
                   //PointAnimation();
                }
            }
       
        */


        Debug.Log("========== ClickObject 終了 ==========");
    }

    
    private void HighlightObject()
    {
        // Spriteの色を黄色に変更
        if (spriteRenderer != null)
        {
            spriteRenderer.color = highlightColor;
            gameObject.tag = "highlighted";
            lastHighlighted = gameObject;
        }

        // クリックしたオブジェクトの色をハイライトカラーに変更
        //this.GetComponent<MeshRenderer>().materials[0].color = highlightColor;

        // クリックしたオブジェクトのタグを Highlighted に変更
        //this.tag = "Highlighted";
        //Debug.Log(this.tag);
    }
    private void ResetLastHighlighted()
    {
        if (lastHighlighted != null && lastHighlighted != gameObject)
        {
            SpriteRenderer lastRenderer = lastHighlighted.GetComponent<SpriteRenderer>();
            if (lastRenderer != null)
            {
                ClickObject lastClickObject = lastHighlighted.GetComponent<ClickObject>();
                if (lastClickObject != null)
                {
                    lastRenderer.color = lastClickObject.originalColor;
                }
            }
            lastHighlighted.tag = "Untagged";
        }
    }

    private void PlacePrefabLargeGraph()
    {
        Debug.Log("---------- PlacePrefabLargeGraph 開始 ----------");
        plotterScript = plotter.GetComponent<PointRenderer>();
        plotterScript.column1 = click_X;
        plotterScript.column2 = click_Y;
        //plotterScript.renderPoints = true;
        plotterScript.RenderPoints();

        /*
        plotter.SetActive(false); // データ点が2重に作成されてしまうのを防ぐため、無効にしておく
        */
        /*
        // データ点は削除
        container = graph.transform.Find("GraphFrame/PointContainer").gameObject;
        foreach (Transform child in container.transform)
        {
            Destroy(child.gameObject);
        }

        */

        //Debug.Log("クリック検出");
        Debug.Log("---------- PlacePrefabLargeGraph 終了 ----------");
    }

    private void AssignLabels()
    {
        panel_label_x.GetComponent<TextMeshProUGUI>().text = xColumnName;
        panel_label_y.GetComponent<TextMeshProUGUI>().text = yColumnName;
    }

    private void FindPath(int x_1, int y_1, int z_1, int x_2, int y_2, int z_2)
    {
        if (x_1 == x_2 && y_1 == y_2 && z_1 == z_2)
        {
            return;
        }
        // 前の線があったら消す
        GameObject previousPath = GameObject.Find("path_on_Matrix");
        if (previousPath != null)
        { // linrenderer.GameObject != null でいい？
            Destroy(previousPath);
        }

        LineRenderer line = gameObject.AddComponent<LineRenderer>();

        int num = Mathf.Abs(x_1 - x_2) + Mathf.Abs(y_1 - y_2) + Mathf.Abs(z_1 - z_2) + 1; // 頂点の数
        Vector3[] positions = new Vector3[num];
        int count = 0; // 最終的に num と同じになる

        // 各頂点の座標を取得する
        int a, b, c;
        if (x_2 >= x_1)
        {
            for (a = x_1; a <= x_2; a++)
            {
                string name = $"{a}, {y_1}, {z_1}";
                getPosition(name);
            }
        }
        else
        {
            for (a = x_1; a >= x_2; a--)
            {
                string name = $"{a}, {y_1}, {z_1}";
                getPosition(name);
            }
        }

        if (y_2 >= y_1)
        {
            for (b = y_1 + 1; b <= y_2; b++)
            {
                string name = $"{x_2}, {b}, {z_1}"; // a ではなく x_2 でないとだめ。a は1大きくなってる。
                getPosition(name);
            }
        }
        else
        {
            for (b = y_1 - 1; b >= y_2; b--)
            {
                string name = $"{x_2}, {b}, {z_1}";
                getPosition(name);
            }
        }

        if (z_2 >= z_1)
        {
            for (c = z_1 + 1; c <= z_2; c++)
            {
                string name = $"{x_2}, {y_2}, {c}";
                getPosition(name);
            }
        }
        else
        {
            for (c = z_1 - 1; c >= z_2; c--)
            {
                string name = $"{x_2}, {y_2}, {c}";
                getPosition(name);
            }
        }

        void getPosition(string g_name)
        {
            Vector3 position = GameObject.Find(g_name).transform.position;
            position.y = position.y + 2.5f;
            positions[count] = position;
            count++;
        }

        // 線に名前を付ける
        line.name = "path_on_Matrix"; // Cubeのなまえがかわってる？

        // 点の数を指定する
        line.positionCount = positions.Length;

        // 線の色を指定する
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.yellow;
        line.endColor = Color.yellow;

        // 線の太さを指定する
        line.startWidth = 0.5f;
        line.endWidth = 0.5f;

        // 線を引く
        line.SetPositions(positions);
    }

    private void PointAnimation()
    {
        Debug.Log("---------- PointAnimation 開始 ----------");
        // クリックしたオブジェクトの3つの変数の名前を取得
        List<string> columnList = new List<string>(CSVData.pointList[1].Keys);
        xColumnName = columnList[click_X];
        yColumnName = columnList[click_Y];
        float plotScale = 10;

        // 各変数の最小値と最大値を取得
        xMax = Convert.ToSingle(CSVData.min_maxList[xColumnName][1]);
        yMax = Convert.ToSingle(CSVData.min_maxList[yColumnName][1]);

        xMin = Convert.ToSingle(CSVData.min_maxList[xColumnName][0]);
        yMin = Convert.ToSingle(CSVData.min_maxList[yColumnName][0]);

        // 全てのデータ点の直前の座標と新たな座標を取得
        for (var i = 0; i < rowCount; i++) // rowCount はデータ点の数
        {
            // 直前に選択されていた散布図内の各データ点を１つずつ取得して points に格納
            //before_point = container.transform.Find(i.ToString()).gameObject;
            points.Add(container.transform.Find(i.ToString()).gameObject);
            // Vector3 before_point_position = before_point.transform.position;


            if (i == 0)
            { // ちゃんと移動してるか確認
              //Debug.Log("before_point_position: " + before_point.transform.position);
                Debug.Log("before_point_position: " + points[i].transform.position);
            }

            // 移動後の座標を算出
            float x = (Convert.ToSingle(CSVData.pointList[i][xColumnName]) - xMin) / (xMax - xMin);
            x = x + 10.0f - (x * plotScale * 2.0f); // なにこの数字
            float y = (Convert.ToSingle(CSVData.pointList[i][yColumnName]) - yMin) / (yMax - yMin);
            y = y + 3.0f + (y * plotScale * 2.0f);

            // Create vector 3 for positioning particlePoints
            //after_point_position = new Vector3 (x, y, z);
            //after_point_positions.Add(new Vector3 (x, y, z));

            /*
            if(_iEnumerator == null){
            StartCoroutine(MovePoint());
            }
            */
            /*
            _iEnumerator = MovePoint();
            */



            /*
            //instantiate as gameobject variable so that it can be manipulated within loop
            GameObject dataPoint = Instantiate (PointPrefab, Vector3.zero, Quaternion.identity);


            // Make child of PointHolder object, to keep particlePoints within container in hiearchy
            dataPoint.transform.parent = PointHolder.transform;

            // Position point at relative to parent
            dataPoint.transform.localPosition = position;

            dataPoint.transform.localScale = new Vector3(pointScale, pointScale, pointScale);

            // Converts index to string to name the point the index number
            string dataPointName = i.ToString();

            // Assigns name to the prefab
            dataPoint.transform.name = dataPointName;

            if (renderPrefabsWithColor == true)
            {
              // Sets color according to x/y/z value
              dataPoint.GetComponent<Renderer>().material.color = new Color(x, y, z, 1.0f);

              // Activate emission color keyword so we can modify emission color
              dataPoint.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");

              dataPoint.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(x, y, z, 1.0f));

            }
            */
        }

        /*
        // 全てのデータ点を移動させるアニメーションを実行
        if (!isMoving)
        {   
            StartCoroutine(MoveToTargetPosition());
        }

        for (var i = 0; i < rowCount; i++)
        {
          //before_point.transform.position = after_point_position;
          points[i].transform.position = after_point_positions[i];

          if(i == 0){
          //Debug.Log("after_point_position: " + before_point.transform.position);
          Debug.Log("after_point_position: " + points[i].transform.position);
          }
        }
        */

        Debug.Log("---------- PointAnimation 終了 ----------");

    }

    /*
    private IEnumerator MoveToTargetPosition()
    {
        isMoving = true;

        //targetPosition = after_point_position;
        //Vector3 startPosition = before_point.transform.position;
        List<Vector3> startPosition = new List<Vector3>();
        List<float> journeyLength = new List<float>();
        for (var i = 0; i < rowCount; i++)
        {
            startPosition.Add(points[i].transform.position);
            journeyLength.Add(Vector3.Distance(startPosition[i], after_point_positions[i]));
        }
        //float journeyLength = Vector3.Distance(startPosition, targetPosition);
        float max_journeyLength = journeyLength.Max();
        int max_index = journeyLength.IndexOf(journeyLength.Max());
        float startTime = Time.time;

        //while (before_point.transform.position != targetPosition)
        while (points[max_index].transform.position != after_point_positions[max_index])
        { 
            float distanceCovered = (Time.time - startTime) * movementSpeed;
            Debug.Log("moving!!!!!!!!!!!!: "+ distanceCovered);
            //float fractionOfJourney = distanceCovered / journeyLength;
            float fractionOfJourney = distanceCovered / max_journeyLength;
            //before_point.transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);
            for (var i = 0; i < rowCount; i++)
            {
                if(points[i].transform.position != after_point_positions[i])
                {
                    points[i].transform.position = Vector3.Lerp(startPosition[i], after_point_positions[i], fractionOfJourney);
                }
            }
            yield return null;
        }

        isMoving = false;
    }

}
    */
}