using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//state
public enum State{ Open,Close,Grab,Pull,Bring};

public class ClawnController : MonoBehaviour
{
    [SerializeField] Rigidbody clawnRigidBody;
    [SerializeField] GameObject grabArea;
    [Header("Movement")]
    [SerializeField] private float speed;
    private Vector2 moveInput;

    [Header("Clawn")]
    [SerializeField] Transform clawnTipController;
    [SerializeField] float turnSpeed = 1f;
    [SerializeField] float openAngle = -45f;
    [SerializeField] float closeAngle = 0f;
    float now = 0f;
    
    [Header("StateMachine")]
    public State stateM;
    bool lockCrane = false;
    

    [Header("Moving Area")]
    [SerializeField] Vector2 height;
    [SerializeField] Transform max,min,goal;
    Vector3 go;
    
      // Start is called before the first frame update
    void Start()
    {
        clawnRigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        MachineState();
        MachineController();
        
    }

    //Controling what crane must do for every state
    private void MachineController(){
        //make player can't controll the crane, when certain state
        if(!lockCrane){
            MovePlayer();
        }

        switch(stateM){
        case State.Open:
            OpeningClawn();
            break;
        case State.Close:
            ClosingClawn();
            break;
        case State.Pull:
            MoveCrane();
            break;
        case State.Bring:
            MoveCrane();
            break;
        case State.Grab:
            MoveCrane();
            break;
        default:
            break;
        }        
    }

    //Controling State for the crane machine
    private void MachineState(){
        //when crane want to grab object and changing state to close state
        if(transform.position.y <= height.y+0.1f && stateM == State.Grab){
            stateM = State.Close;
            lockCrane = true;
        }
        //when already pull the object and changing state to bring state
        if(transform.position.y >= height.x-0.01f && stateM == State.Pull){
           stateM = State.Bring;
        }
        /*when player want to grab object, locking the controller and define
        where the crane go (to grab the object)*/
        if(Input.GetKey("space") && stateM == State.Open){
            stateM = State.Grab;
            lockCrane = true;
            go = new Vector3(transform.position.x,height.y,transform.position.z);
        }
        //when crane in pull state and define where the crane must go (to pulling the object)
        if(stateM == State.Pull){
            go = new Vector3(transform.position.x,height.x,transform.position.z);
        }
        //when crane in bring state and define where the crane must go after pulling the object
        if(stateM == State.Bring){
            go = new Vector3(goal.position.x,transform.position.y,goal.position.z);
        } 
        //when crane arrive to destination, unlock the the controller and change to open state
        if(AlmostEqual(transform.position,go,0.1f) && stateM == State.Bring){
           stateM = State.Open;
           lockCrane = false;
        }
    }
    //Move the crane to certain area
    private void MovePlayer(){
        //For Horizontal movement
        if(transform.position.x >= max.position.x){
            transform.position = new Vector3(
                transform.position.x-0.1f,
                transform.position.y,
                transform.position.z);     
        }else if(transform.position.x <= min.position.x){
            transform.position = new Vector3(
                    transform.position.x+0.1f,
                    transform.position.y,
                    transform.position.z);
        } else{
             moveInput.x = Input.GetAxisRaw("Horizontal");
        }
        //For Vertical Movement
        if(transform.position.z >= max.position.z){
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y,
                transform.position.z-0.1f);
           
        }else if(transform.position.z <= min.position.z){
            transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y,
                    transform.position.z+0.1f); 
        } else{
            moveInput.y = Input.GetAxisRaw("Vertical");
        }
        //Move the crane
        moveInput.Normalize();
        clawnRigidBody.velocity = new Vector3(moveInput.x * speed, clawnRigidBody.velocity.y, moveInput.y * speed);
    }
    //Making Crane Open
    private void OpeningClawn(){      
        if (now >= openAngle){
            now -= turnSpeed;
            clawnTipController.rotation = Quaternion.Euler(now, 0f, 0f);
        }else if(stateM == State.Open){
            grabArea.SetActive(false);
            lockCrane = false;
        }        
    } 
    //Making Crane Closed
    private void ClosingClawn(){
        grabArea.SetActive(true);
        if (now <= closeAngle)
        {
            now += turnSpeed;
            clawnTipController.rotation = Quaternion.Euler(now, 0f, 0f);
        } else if(stateM == State.Close){
          stateM = State.Pull;  
        }
    } 
    //Moving Crane to certain positions
    private void MoveCrane(){
        transform.position = Vector3.Lerp(transform.position,go, speed * Time.deltaTime);   
    }
    //Comparing 2 Vector, are those two vector is equal or not
    private static bool AlmostEqual(Vector3 v1, Vector3 v2, float tolerance){
        if (Mathf.Abs(v1.x - v2.x) > tolerance) return false;
        if (Mathf.Abs(v1.y - v2.y) > tolerance) return false;
        if (Mathf.Abs(v1.z - v2.z) > tolerance) return false;
        return true;
    }
}
