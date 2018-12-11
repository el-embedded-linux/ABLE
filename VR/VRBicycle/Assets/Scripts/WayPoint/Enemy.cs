using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    List<Transform> waypoint;
    Vector3 startPos;
    Vector3 distPos;
    float duration;
    int waypointIndex;
    public int HP;

    public void Init(List<Transform> waypoint)
    {
        this.waypoint = waypoint;
        if (waypoint == null)
            return;

        HP = 10;
        duration = 0;
        waypointIndex = 0;
        transform.position = this.waypoint[0].position;

        gameObject.SetActive(true);
        StartCoroutine(MoveWaypoint());
    }

    IEnumerator MoveWaypoint()
    {
        float elapsedTime = 0;
        while (waypoint.Count > waypointIndex)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > duration)
            {
                elapsedTime = 0;
                NextWaypoint();
            }

            transform.position = Linear(elapsedTime, startPos, distPos, duration);
            yield return null;
        }

        gameObject.SetActive(false);
    }

    void NextWaypoint()
    {
        Transform target = waypoint[waypointIndex];
        startPos = target.position;

        if (++waypointIndex >= waypoint.Count)
            return;

        target = waypoint[waypointIndex];
        transform.LookAt(target);

        distPos = target.position - startPos;
        duration = (waypointIndex % 2 == 1) ? 2f : 0.5f;
    }

    public Vector3 Linear(float t, Vector3 b, Vector3 c, float d)
    {
        return c * t / d + b;
    }
}


