using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float JumpPower;
    private Vector2 curMovementInput;
    public LayerMask groundLayerMask;

    [Header("Look")]
    public Transform cameraContainer;
    public float minLook;
    public float maxLook;
    private float camCurXRot;
    public float lookSensitivity;
    public Vector2 mousedelta;
    public bool canLook = true;

    [Header("Jump Settings")]
    public int maxJumpCount = 2;   // �ִ� ���� Ƚ��
    private int currentJumpCount;  // ���� ���� Ƚ��
    private float lastJumpTime;
    public float groundCheckDelay = 0.1f; // ���� ���� 0.1�� ������ �ٴ� ���� �� ��

    public Action inventory;
    private Rigidbody _rigidbody;

    //private bool jumpPressedThisFrame = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        currentJumpCount = maxJumpCount; // ���� �� ���� Ƚ�� �ʱ�ȭ
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();

        // ���� ���� �����̰� ���� �ڿ��� üũ
        if (IsGrounded() && (Time.time - lastJumpTime > groundCheckDelay))
        {
            currentJumpCount = maxJumpCount;

        }
    }
    private void LateUpdate()
    {
        CameraLook();
    }
    void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= moveSpeed;
        dir.y = _rigidbody.velocity.y;

        _rigidbody.velocity = dir;
    }

    void CameraLook()
    {
        camCurXRot += mousedelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minLook, maxLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mousedelta.x * lookSensitivity, 0);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }

    public void Onlook(InputAction.CallbackContext context)
    {
        mousedelta = context.ReadValue<Vector2>();
    }

        public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && currentJumpCount > 0)
        {
            //jumpPressedThisFrame = true;
            StartCoroutine(ResetJumpPressedFlag());

            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z); // ���� ���� �ӵ� �ʱ�ȭ
            _rigidbody.AddForce(Vector2.up * JumpPower, ForceMode.Impulse);
            currentJumpCount--; // ���� Ƚ�� ����
            lastJumpTime = Time.time; // ������ ���� ���

        }
    }

    private IEnumerator ResetJumpPressedFlag()
    {
        yield return null; // 1������ ���
        //jumpPressedThisFrame = false;
    }

    bool IsGrounded()
    {
        // ���� ���� ���� �ð��� �� ���� �� �ϵ���
        if (Time.time - lastJumpTime < groundCheckDelay)
            return false;

        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) +(transform.up * 0.01f), Vector3.down)
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 1.05f, groundLayerMask))
            {
                return true;
            }
        }
        return false;
    }
}