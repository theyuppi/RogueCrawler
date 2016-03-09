using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.0f;
    int currentTarget = 0;
    private Vector3 velocity = Vector3.zero;
    public EnemyHandler eHandler;

    void Update()
    {
        Vector3 goalPos = target.position;
        goalPos.z = transform.position.z;
        transform.position = Vector3.SmoothDamp(transform.position, goalPos, ref velocity, smoothTime);
        if (Input.GetKeyUp(KeyCode.Space))
        {
            SetTarget(currentTarget);
            currentTarget++;
            if (currentTarget >= eHandler.enemyList.Count)
            {
                currentTarget = 0;
            }
        }
    }

    public void SetTarget(int following)
    {
        target = eHandler.enemyList[following].gameObject.transform;
    }
}
