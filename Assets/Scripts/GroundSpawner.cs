using UnityEngine;

public class GroundSpawner : MonoBehaviour
{
    public GameObject[] groundPrefabs;
    public int groundCount = 10;

    public Rigidbody playerRb;
    public PlayerController playerController; // jumpPower 접근용
    public float minXOffset = -2f;
    public float maxXOffset = 2f;

    void Start()
    {
        if (playerRb == null)
            playerRb = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Rigidbody>();

        if (playerController == null)
            playerController = playerRb?.GetComponent<PlayerController>();

        SpawnGrounds();
    }

    void SpawnGrounds()
    {
        if (playerRb == null || playerController == null)
        {
            Debug.LogError("Player Rigidbody 또는 PlayerController를 찾지 못했습니다.");
            return;
        }

        // 중력과 점프력으로 1단 점프 높이 계산
        float gravity = Mathf.Abs(Physics.gravity.y);
        float jumpVelocity = playerController.JumpPower / playerRb.mass;
        float singleJumpHeight = (jumpVelocity * jumpVelocity) / (2f * gravity);

        // 2단 점프 높이 계산
        float maxJumpHeight = singleJumpHeight * 2f;

        Vector3 spawnPos = transform.position;

        for (int i = 0; i < groundCount; i++)
        {
            // 높이는 1단 ~ 2단 점프 가능 범위에서 랜덤
            spawnPos.y += Random.Range(singleJumpHeight * 0.8f, maxJumpHeight * 0.9f);

            // X축 좌우 랜덤
            spawnPos.x = Random.Range(minXOffset, maxXOffset);

            spawnPos.z = 0;

            int prefabIndex = Random.Range(0, groundPrefabs.Length);
            Instantiate(groundPrefabs[prefabIndex], spawnPos, Quaternion.identity);
        }
    }
}
