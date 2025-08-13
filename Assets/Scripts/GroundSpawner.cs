using UnityEngine;

public class GroundSpawner : MonoBehaviour
{
    public GameObject[] groundPrefabs;
    public int groundCount = 10;

    public Rigidbody playerRb;
    public PlayerController playerController; // jumpPower ���ٿ�
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
            Debug.LogError("Player Rigidbody �Ǵ� PlayerController�� ã�� ���߽��ϴ�.");
            return;
        }

        // �߷°� ���������� 1�� ���� ���� ���
        float gravity = Mathf.Abs(Physics.gravity.y);
        float jumpVelocity = playerController.JumpPower / playerRb.mass;
        float singleJumpHeight = (jumpVelocity * jumpVelocity) / (2f * gravity);

        // 2�� ���� ���� ���
        float maxJumpHeight = singleJumpHeight * 2f;

        Vector3 spawnPos = transform.position;

        for (int i = 0; i < groundCount; i++)
        {
            // ���̴� 1�� ~ 2�� ���� ���� �������� ����
            spawnPos.y += Random.Range(singleJumpHeight * 0.8f, maxJumpHeight * 0.9f);

            // X�� �¿� ����
            spawnPos.x = Random.Range(minXOffset, maxXOffset);

            spawnPos.z = 0;

            int prefabIndex = Random.Range(0, groundPrefabs.Length);
            Instantiate(groundPrefabs[prefabIndex], spawnPos, Quaternion.identity);
        }
    }
}
