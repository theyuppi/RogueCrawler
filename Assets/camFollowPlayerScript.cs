using UnityEngine;
using System.Collections;

public class camFollowPlayerScript : MonoBehaviour {

     public Transform target;
     public float smoothTime = 0.0f;
 
     private Vector3 velocity = Vector3.zero;
 
     void Update () {
         Vector3 goalPos = target.position;
         goalPos.z = transform.position.z;
         transform.position = Vector3.SmoothDamp (transform.position, goalPos, ref velocity, smoothTime);
     }
 }
