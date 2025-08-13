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
    public int maxJumpCount = 2;   // 최대 점프 횟수
    private int currentJumpCount;  // 남은 점프 횟수
    private float lastJumpTime;
    public float groundCheckDelay = 0.1f; // 점프 직후 0.1초 동안은 바닥 판정 안 함

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
        currentJumpCount = maxJumpCount; // 시작 시 점프 횟수 초기화
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();

        // 착지 판정 딜레이가 지난 뒤에만 체크
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

            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z); // 이전 점프 속도 초기화
            _rigidbody.AddForce(Vector2.up * JumpPower, ForceMode.Impulse);
            currentJumpCount--; // 점프 횟수 차감
            lastJumpTime = Time.time; // 점프한 시점 기록

        }
    }

    private IEnumerator ResetJumpPressedFlag()
    {
        yield return null; // 1프레임 대기
        //jumpPressedThisFrame = false;
    }

    bool IsGrounded()
    {
        // 점프 직후 일정 시간은 땅 판정 안 하도록
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