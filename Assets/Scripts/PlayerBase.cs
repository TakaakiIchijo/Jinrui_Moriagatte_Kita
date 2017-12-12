using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class PlayerBase : MonoBehaviour
{
    public Camera mainCamera;
    NavMeshAgent agent;
    public PlayerAnimation playerAnimation;

    int layerMask;
    bool isMoving = false;

    Vector3 currentDirection;

    // Use this for initialization
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        playerAnimation.OnIdle();

        layerMask = LayerMask.GetMask("Floor");

        gameObject.SetActive(false);
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public void ChangeDirection(Vector3 direction)
    {
        currentDirection = direction;
        // その場所に、Nav Mesh Agentをアタッチしたオブジェクトを移動させる
        agent.SetDestination(currentDirection);
    }

    // Update is called once per frame
    public void TapMove()
    {
        RaycastHit hit;

        isMoving = true;
        // マウスの位置からRayを発射して、
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        // 物体にあたったら、
        if (Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            ChangeDirection(hit.point);

            playerAnimation.OnWalk();
        }
    }

    private void Update()
    {
        // 目的地とプレイヤーとの距離が1以下になったら、
        if (Vector3.Distance(currentDirection, transform.position) < 1.0f && isMoving)
        {
            // "Run"アニメーションから抜け出す
            isMoving = false;
            playerAnimation.OnIdle();
        }
    }
}