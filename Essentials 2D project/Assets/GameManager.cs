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

    [Header("�C���X�y�N�^�[�Ŏw��")]
    [SerializeField] private GameObject obstaclesParentObject;
    [SerializeField] private GameObject obstacleObjectPrefab;
    [SerializeField] private GameObject fallingObject;
    [SerializeField] private GameObject uiTextObject;


    [Header("�f�o�b�N�p")]
    //�������֘A
    [SerializeField] private bool objectHasFallen = false;
    //��Q���֘A
    //��Q���͉����ɓ��Ԋu�ɕ��сA��葬�x�ŉ��ɓ���
    private List<float> obstacleYPositionList = new() { 2.5f, 5f };
    private float obstacleXPositionInterval = 3f;
    private List<float> obstacleSpeedList = new() { -2f, 4f };
    private List<Obstacle> obstacleList = new();
    private class Obstacle
    {
        public GameObject Object;
        public float Speed;
    }
    //�Q�[���i�s�t���O
    private bool gameEnd = false;
    //�S�[���֘A
    [SerializeField] private bool touchGoal = false;
    public bool TouchGoal
    {
        set 
        { 
            touchGoal = value;
            if(gameEnd == false && touchGoal) GameClear();
        }
    }
    //�Q�[���I�[�o�[�֘A
    [SerializeField] private bool touchGround = false;
    public bool TouchGround
    {
        set
        {
            touchGround = value;
            if (gameEnd == false && touchGround) GameOver();
        }
    }
    //UI�֘A
    private TextMeshProUGUI uiTextMeshPro;
    private string gameClearText = "Game Clear";
    private string gameOverText = "GameOver";
    //�J�����֘A
    private (float rightEdge, float leftEdge) CameraEdge;


    // Start is called before the first frame update
    void Start()
    {
        //�C���X�y�N�^�[�Ŏw�肷����null�̏ꍇ�̓G���[���o��
        if (obstacleObjectPrefab == null) Debug.LogError("obstacleObjectPrefab is not set in the GameManager");
        if (obstaclesParentObject == null) Debug.LogError("obstaclesParentObject is not set in the GameManager");
        if (fallingObject == null) Debug.LogError("fallingObject is not set in the GameManager");
        if (uiTextObject == null) Debug.LogError("UiTextObject is not set in the GameManager");
        //camera�ɉf���Ă���E�[�E���[�̍��W���擾
        CameraEdge.rightEdge = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
        CameraEdge.leftEdge = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        //��Q����obstacleObjectPrefab���g���Đ���
        //y���W��obstacleYPositionList
        //obstacleYPositionList�̌���������
        //x���W��leftEdge����n�܂�AobstaclePositionInterval�̊Ԋu�Ő���
        //prefab�̐e��ObstaclesParentObject
        //����������Q����obstacleList�ɒǉ�
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
        //uiTextObject����Text�R���|�[�l���g���擾
        uiTextMeshPro = uiTextObject.GetComponent<TextMeshProUGUI>();
        if (uiTextMeshPro == null) Debug.LogError("TextMeshPro is not attached to the uiTextObject");
        //Tmp�Ƃ������O�̃I�u�W�F�N�g��T�����āA�q���܂߂č폜����
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
        //Enter�L�[�������ꂽ��fallingObject��y���W�̌Œ������
        if (objectHasFallen == false && Input.GetKeyDown(KeyCode.Return))
        {
            fallingObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            objectHasFallen = true;
        }
        //R�L�[�������ꂽ��Q�[�����݂̃V�[���������[�h
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
        //��Q����Speed�̑��x�ňړ�
        //�E�[�A�������͍��[�ɓ��B������t�̒[�ɖ߂�
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

    //�Q�[���N���A����
    private void GameClear()
    {
        uiTextMeshPro.text = gameClearText;
        gameEnd = true;
    }

    //�Q�[���I�[�o�[����
    private void GameOver()
    {
        uiTextMeshPro.text = gameOverText;
        gameEnd = true;
    }
}


