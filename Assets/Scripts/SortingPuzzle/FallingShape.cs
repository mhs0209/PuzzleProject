using UnityEngine;

public class FallingShape : MonoBehaviour
{
    [Header("속성")]
    public string shapeType; // 이 도형의 타입 (예: "Square", "Triangle")

    [Header("떨어지는 속도")]
    public float fallSpeed = 2f;
    
    private bool isBeingDragged = false;
    private Vector3 offset;
    private SortingGameManager manager;

    void Start()
    {
        manager = FindObjectOfType<SortingGameManager>();
    }

    void Update()
    {
        // 드래그 중이 아니면 계속 아래로 떨어집니다.
        if (!isBeingDragged)
        {
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
        }
        
        // 화면 아래쪽(Y=-6 등)으로 너무 내려가면 실패 처리
        if (transform.position.y < -6f && !isBeingDragged)
        {
            manager.ShapeMissed(this);
        }
    }

    // --- 드래그 시작 (터치) ---
    void OnMouseDown()
    {
        if (manager.IsGameOver()) return;
        isBeingDragged = true;
        
        // 도형의 위치와 마우스 위치 간의 오프셋 계산
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    // --- 드래그 중 ---
    void OnMouseDrag()
    {
        if (!isBeingDragged) return;
        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePos + offset;
    }

    // --- 드래그 종료 ---
    void OnMouseUp()
    {
        if (!isBeingDragged) return;
        isBeingDragged = false;
        
        // 드래그가 끝났을 때 분류 검사를 위해 매니저에게 알림
        manager.CheckShapePlacement(this);
    }

    // --- 분류함 충돌 검사 (옵션: OnTriggerStay2D 사용 가능) ---
    // 여기서는 매니저의 CheckShapePlacement에서 최종 위치를 검사하는 방식 사용
}