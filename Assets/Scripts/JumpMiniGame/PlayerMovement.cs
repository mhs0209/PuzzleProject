using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("이동 및 점프 속도")]
    public float runSpeed = 5f;
    public float jumpForce = 400f;

    private Rigidbody2D rb;
    private bool isGrounded; // 바닥에 닿았는지 확인하는 플래그
    public Transform groundCheck; // 바닥 검사 위치 오브젝트
    public LayerMask groundLayer; // 바닥 레이어 마스크

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // 초기 수평 이동 시작
        rb.velocity = new Vector2(runSpeed, rb.velocity.y);
    }

    void Update()
    {
        // 바닥 검사 (GroundCheck 오브젝트를 사용하여 바닥 레이어와 겹치는지 확인)
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        // 모바일 입력 (화면 터치) 또는 마우스 왼쪽 클릭 감지
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) && isGrounded)
        {
            Jump();
        }
    }

    // --- 점프 로직 ---
    private void Jump()
    {
        // 현재 수직 속도를 0으로 만든 후 힘을 가하여 안정적인 점프를 보장
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(new Vector2(0f, jumpForce));
    }
    
    // --- 충돌 처리 (게임 오버) ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // "Obstacle" 태그를 가진 오브젝트와 충돌했을 때
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // 게임 오버 처리
            TimingGameManager.Instance.GameOver();
            // 플레이어 움직임 중지
            rb.velocity = Vector2.zero;
            rb.isKinematic = true; 
        }
    }
}