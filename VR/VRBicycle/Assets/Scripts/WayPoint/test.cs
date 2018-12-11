using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {

    private int currentNode = 0;
    private UnityEngine.AI.NavMeshAgent agent;
    public List<Transform> waypoint = new List<Transform>(); //위치정보를 가지고 있는 리스트 선언.

    void Start() {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.autoBraking = false;
        GotoNext();
    }

    void FixedUpdate() {
        if (!agent.pathPending && agent.remainingDistance < 2f)
            GotoNext(); //목적지까지의 거리가 2이하거나 도착했으면 함수실행
        if (currentNode == waypoint.Count)
            currentNode = 0;
        agent.speed = reeed.bySpeed;
        //마지막 노드 (웨이포인트)로 도착하였을 때는 초기화 시켜준다 _ 이 부분을 읽음으로써
        //웨이포인트를 연결해주는 편이 좋습니다. 되돌아오게 하고 싶다면  currentNode -= currentNode 정도로 해주는게 좋겠습니다.
    }

    void GotoNext() {
        agent.destination = waypoint[currentNode].position;
        currentNode = (currentNode + 1);

//네브메쉬를 이용하여 목적지를 설정해준 리스트 웨이포인트 위치로 이동
    }

//기즈모 부분은 웨이포인트 설정
    private void OnDrawGizmos() {
        for (int i = 0; i < waypoint.Count; i++) {
            Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
            Gizmos.DrawSphere(waypoint[i].transform.position, 2);
            Gizmos.DrawWireSphere(waypoint[i].transform.position, 20f);

            if (i < waypoint.Count - 1) {
                if (waypoint[i] && waypoint[i + 1]) {
                    Gizmos.color = Color.red;
                    if (i < waypoint.Count - 1)
                        Gizmos.DrawLine(waypoint[i].position, waypoint[i + 1].position);
                    if (i < waypoint.Count - 2) {
                        Gizmos.DrawLine(waypoint[waypoint.Count - 1].position, waypoint[0].position);
                    }
                }
            }
        }
    }
}