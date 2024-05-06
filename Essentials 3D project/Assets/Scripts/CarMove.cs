using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMove : MonoBehaviour
{
    [SerializeField] private float speed;
    private float initSpeed = 0.005f;
    private Vector3 carPositionChange;
    private Transform carObjectTransform;

    // Start is called before the first frame update
    void Start()
    {
        carObjectTransform = this.gameObject.GetComponent<Transform>();
        speed = initSpeed;
        carPositionChange = new Vector3(-speed, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        carObjectTransform.position += carPositionChange;
    }
}
