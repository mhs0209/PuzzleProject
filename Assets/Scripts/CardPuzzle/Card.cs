using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    // --- 설정 변수 ---
    [Header("시각적 요소")]
    public Image cardImage; // 카드의 이미지를 표시할 Image 컴포넌트
    public Sprite cardBackSprite; // 카드의 뒷면 이미지 스프라이트
    
    // --- 내부 상태 ---
    [HideInInspector] public int cardID; // 이 카드가 가진 그림의 ID (짝을 맞추기 위한 고유 번호)
    private CardGameManager manager; // 게임 매니저 참조
    private bool isFlipped = false; // 현재 앞면인지 뒷면인지 여부
    private bool isMatched = false; // 이미 짝이 맞춰져 제거된 카드인지 여부

    // --- 초기화 ---
    public void Initialize(CardGameManager gameManager, int id, Sprite frontSprite)
    {
        manager = gameManager;
        cardID = id;
        
        // 카드 오브젝트에 있는 Button 컴포넌트의 OnClick 이벤트에 연결
        GetComponent<Button>().onClick.AddListener(OnCardClicked);
        
        // 카드의 뒷면 이미지와 앞면 이미지(숨김 상태)를 저장하고 뒷면을 보여줍니다.
        cardImage.sprite = cardBackSprite; 
        
        // NOTE: 앞면 이미지는 게임 매니저에서 내부적으로 관리하여 보여줄 때만 사용합니다.
    }

    // --- 플레이어 입력 ---
    private void OnCardClicked()
    {
        // 짝이 맞춰진 카드이거나, 이미 뒤집혀 있거나, 현재 게임 매니저가 바쁜 상태(예: 짝 검사 중)라면 무시합니다.
        if (isMatched || isFlipped || manager.IsBusy())
        {
            return;
        }

        // 카드를 뒤집고, 매니저에게 클릭을 알립니다.
        Flip(true);
        manager.CardFlipped(this);
    }

    // --- 카드 뒤집기 ---
    public void Flip(bool flipToFront)
    {
        if (isMatched) return;

        if (flipToFront)
        {
            // 앞면으로 뒤집기: 매니저가 제공한 해당 카드 ID의 실제 그림을 표시합니다.
            cardImage.sprite = manager.GetCardSprite(cardID);
            isFlipped = true;
        }
        else
        {
            // 뒷면으로 뒤집기
            cardImage.sprite = cardBackSprite;
            isFlipped = false;
        }
    }

    // --- 짝 맞춤 처리 ---
    public void MatchFound()
    {
        isMatched = true;
        // 짝이 맞았을 때 시각적 피드백 (예: 투명하게 만들거나 비활성화)
        // 현재는 버튼의 상호작용만 비활성화합니다.
        GetComponent<Button>().interactable = false;
        
        // TODO: 애니메이션이나 파티클 효과 추가
    }
}