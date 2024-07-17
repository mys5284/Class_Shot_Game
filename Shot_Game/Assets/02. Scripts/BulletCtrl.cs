using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public float damage = 20f; //총알 공격력
    public float speed = 2000f; //총알 이동 속도

    Transform tr;
    Rigidbody rb;
    TrailRenderer trail;

    // Start is called before the first frame update
    void Awake()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();


       // GetComponent<Rigidbody>().AddForce(transform.forward * speed);  
    }

    private void OnEnable()
    {
        rb.AddForce(transform.forward * speed);
    }

    private void OnDisable()
    {
        trail.Clear();
        tr.rotation = Quaternion.identity;
        rb.Sleep();
    }

}
