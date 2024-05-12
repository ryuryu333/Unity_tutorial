using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;



public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Header("インスペクターで指定")]
    [SerializeField] private GameObject obstaclesParentObject;
    [SerializeField] private GameObject obstacleObjectPrefab;
    [SerializeField] private GameObject fallingObject;
    [SerializeField] private GameObject uiTextObject;


    [Header("デバック用")]
    //落下物関連
    [SerializeField] private bool objectHasFallen = false;
    //障害物関連
    //障害物は横一列に等間隔に並び、一定速度で横に動く
    private List<float> obstacleYPositionList = new() { 2.5f, 5f };
    private float obstacleXPositionInterval = 3f;
    private List<float> obstacleSpeedList = new() { -2f, 4f };
    private List<Obstacle> obstacleList = new();
    private class Obstacle
    {
        public GameObject Object;
        public float Speed;
    }
    //ゲーム進行フラグ
    private bool gameEnd = false;
    //ゴール関連
    [SerializeField] private bool touchGoal = false;
    public bool TouchGoal
    {
        set 
        { 
            touchGoal = value;
            if(gameEnd == false && touchGoal) GameClear();
        }
    }
    //ゲームオーバー関連
    [SerializeField] private bool touchGround = false;
    public bool TouchGround
    {
        set
        {
            touchGround = value;
            if (gameEnd == false && touchGround) GameOver();
        }
    }
    //UI関連
    private TextMeshProUGUI uiTextMeshPro;
    private string gameClearText = "Game Clear";
    private string gameOverText = "GameOver";
    //カメラ関連
    private (float rightEdge, float leftEdge) CameraEdge;


    // Start is called before the first frame update
    void Start()
    {
        //インスペクターで指定する情報がnullの場合はエラーを出す
        if (obstacleObjectPrefab == null) Debug.LogError("obstacleObjectPrefab is not set in the GameManager");
        if (obstaclesParentObject == null) Debug.LogError("obstaclesParentObject is not set in the GameManager");
        if (fallingObject == null) Debug.LogError("fallingObject is not set in the GameManager");
        if (uiTextObject == null) Debug.LogError("UiTextObject is not set in the GameManager");
        //cameraに映っている右端・左端の座標を取得
        CameraEdge.rightEdge = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
        CameraEdge.leftEdge = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        //障害物をobstacleObjectPrefabを使って生成
        //y座標はobstacleYPositionList
        //obstacleYPositionListの個数だけ生成
        //x座標はleftEdgeから始まり、obstaclePositionIntervalの間隔で生成
        //prefabの親はObstaclesParentObject
        //生成した障害物はobstacleListに追加
        for (int i = 0; i < obstacleYPositionList.Count; i++)
        {
            for (float x = CameraEdge.leftEdge; x <= CameraEdge.rightEdge; x += obstacleXPositionInterval)
            {
                GameObject obstacleObject = Instantiate(obstacleObjectPrefab, new Vector3(x, obstacleYPositionList[i], 0), Quaternion.identity);
                obstacleObject.transform.parent = obstaclesParentObject.transform;
                Obstacle obstacle = new()
                {
                    Object = obstacleObject,
                    Speed = obstacleSpeedList[i]
                };
                obstacleList.Add(obstacle);
            }
        }
        //uiTextObjectからTextコンポーネントを取得
        uiTextMeshPro = uiTextObject.GetComponent<TextMeshProUGUI>();
        if (uiTextMeshPro == null) Debug.LogError("TextMeshPro is not attached to the uiTextObject");
        //Tmpという名前のオブジェクトを探索して、子を含めて削除する
        GameObject tmp = GameObject.Find("Tmp");
        if (tmp != null)
        {
            foreach (Transform child in tmp.transform)
            {
                Destroy(child.gameObject);
            }
            Destroy(tmp);
        }
    }


    // Update is called once per frame
    void Update()
    {
        //Enterキーが押されたらfallingObjectのy座標の固定を解除
        if (objectHasFallen == false && Input.GetKeyDown(KeyCode.Return))
        {
            fallingObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            objectHasFallen = true;
        }
        //Rキーが押されたらゲーム現在のシーンをリロード
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
        //障害物をSpeedの速度で移動
        //右端、もしくは左端に到達したら逆の端に戻す
        foreach (Obstacle obstacle in obstacleList)
        {
            if (obstacle.Object.transform.position.x >= CameraEdge.rightEdge)
            {
                obstacle.Object.transform.position = new Vector3(CameraEdge.leftEdge, obstacle.Object.transform.position.y, 0);
            }
            else if (obstacle.Object.transform.position.x <= CameraEdge.leftEdge)
            {
                obstacle.Object.transform.position = new Vector3(CameraEdge.rightEdge, obstacle.Object.transform.position.y, 0);
            }
            obstacle.Object.transform.position += new Vector3(obstacle.Speed * Time.deltaTime, 0, 0);
        }
    }

    //ゲームクリア処理
    private void GameClear()
    {
        uiTextMeshPro.text = gameClearText;
        gameEnd = true;
    }

    //ゲームオーバー処理
    private void GameOver()
    {
        uiTextMeshPro.text = gameOverText;
        gameEnd = true;
    }
}


