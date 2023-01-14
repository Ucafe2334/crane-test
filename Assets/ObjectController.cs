using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectController : MonoBehaviour
{
    [Header("Grab Area")]
    [SerializeField] private Collider grabArea;
    [SerializeField] private Transform objectArea;
    [SerializeField] private Rigidbody objectRigidBody;
    [SerializeField] private ClawnController clawn;

    [Header("Point")]
    [SerializeField] private int poin = 0;
    [SerializeField] private TextMeshProUGUI text;

    void OnTriggerEnter(Collider other){
        //Add poin into UI and destroying Object
        if(other.gameObject.tag == "goal"){
            int a = int.Parse(text.text);
            a += poin;
            text.text = a.ToString();
            Destroy(this.gameObject,1);
        }
    }
    void OnTriggerStay(Collider other){
        //making the object follow the crane when the crane closed. but still can fall
        if(other.gameObject.tag == "grab"){
            if(clawn.stateM == State.Close){
                transform.parent = grabArea.transform;
                objectRigidBody.constraints =  RigidbodyConstraints.FreezePositionZ |  RigidbodyConstraints.FreezePositionX;
            }else if(clawn.stateM == State.Open){
                transform.parent = null;
                objectRigidBody.constraints =  RigidbodyConstraints.None; 
            }
        }
    }

    void OnTriggerExit(Collider other){
        //making the object stop follow the crane when fall
        if(other.gameObject.tag == "grab"){
            transform.parent = objectArea.transform;
            objectRigidBody.constraints =  RigidbodyConstraints.None;
        }
    }
}
