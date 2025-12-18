using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro; // List 셔플을 위해 필요합니다.

public class CardGameManager : MonoBehaviour
{
    // --- 설정 변수 ---
    [Header("카드 설정")]
    public GameObject cardPrefab; // Card.cs가 붙어 있는 UI 버튼 프리팹
    public Transform boardParent; // 카드가 배치될 부모 오브젝트 (예: Grid Layout Group)
    public List<Sprite> cardFrontSprites; // 사용할 카드 그림 스프라이트 리스트 (짝수 개여야 합니다!)
    public int boardSize = 4; // 보드의 가로/세로 크기 (예: 4x4, 총 16장)

    [Header("게임 로직")]
    public float flipBackDelay = 1.0f; // 짝이 틀렸을 때 다시 뒤집어지는 대기 시간
    
    [Header("UI")]
    public TMP_Text tryText;

    // --- 내부 상태 ---
    private List<Card> allCards = new List<Card>(); // 생성된 모든 카드 오브젝트 리스트
    private List<Card> flippedCards = new List<Card>(); // 현재 뒤집힌 카드 (최대 2개)
    private int totalPairs; // 총 짝의 개수
    private int matchesFound = 0; // 현재까지 맞춘 짝의 개수
    private bool isChecking = false; // 현재 짝 검사 코루틴이 진행 중인지 여부
    private int tryCount;

    // --- 초기화 ---
    void Start()
    {
        // 유효성 검사: 보드 크기는 짝수여야 합니다 (예: 4x4 = 16장)
        if (boardSize % 2 != 0 || boardSize * boardSize > cardFrontSprites.Count * 2)
        {
            Debug.LogError("보드 크기 또는 카드 스프라이트 개수가 유효하지 않습니다.");
            return;
        }
        
        tryText.text = "Try Match Count: " + tryCount;
        totalPairs = (boardSize * boardSize) / 2;
        GenerateBoard();
    }
    
    // --- 유틸리티: 현재 매니저가 바쁜지 확인 ---
    public bool IsBusy()
    {
        return isChecking || flippedCards.Count >= 2;
    }
    
    // --- 유틸리티: 카드 Sprite 가져오기 ---
    public Sprite GetCardSprite(int id)
    {
        // Card.cs에서 앞면 이미지로 사용하기 위해 Sprite를 제공합니다.
        return cardFrontSprites[id];
    }

    // --- 1. 보드 생성 및 셔플 ---
    private void GenerateBoard()
    {
        int totalCards = boardSize * boardSize;
        List<int> cardIDs = new List<int>(); // 짝 ID를 저장할 리스트

        // 1. 짝 ID 리스트 생성 (각 ID를 두 번씩 추가)
        for (int i = 0; i < totalPairs; i++)
        {
            cardIDs.Add(i);
            cardIDs.Add(i);
        }

        // 2. 피셔-예이츠 셔플(Fisher-Yates Shuffle) 알고리즘을 사용한 섞기
        ShuffleList(cardIDs);

        // 3. 카드 오브젝트 생성 및 ID 할당
        for (int i = 0; i < totalCards; i++)
        {
            GameObject cardGO = Instantiate(cardPrefab, boardParent);
            Card card = cardGO.GetComponent<Card>();

            // ID와 초기 이미지(뒷면)를 설정하며 카드 초기화
            card.Initialize(this, cardIDs[i], cardFrontSprites[cardIDs[i]]);
            allCards.Add(card);
        }
    }

    // --- 리스트 셔플 알고리즘 ---
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    // --- 2. 카드 뒤집기 이벤트 처리 ---
    public void CardFlipped(Card card)
    {
        flippedCards.Add(card);

        // 두 장의 카드가 뒤집혔으면 짝 검사 시작
        if (flippedCards.Count == 2)
        {
            StartCoroutine(CheckMatch());
        }
    }

    // --- 3. 짝 검사 로직 (코루틴) ---
    private IEnumerator CheckMatch()
    {
        isChecking = true;
        
        // 짝 검사 전 잠시 대기
        yield return new WaitForSeconds(flipBackDelay * 0.5f); 

        Card card1 = flippedCards[0];
        Card card2 = flippedCards[1];

        if (card1.cardID == card2.cardID)
        {
            // --- 짝이 맞았을 경우 ---
            Debug.Log("짝 맞음!");
            card1.MatchFound();
            card2.MatchFound();
            
            matchesFound++;
            
            // 모든 짝을 찾았는지 확인
            if (matchesFound >= totalPairs)
            {
                Debug.Log("게임 승리! 모든 짝을 찾았습니다.");
                // TODO: 게임 종료/승리 UI 표시
            }
        }
        else
        {
            // --- 짝이 틀렸을 경우 ---
            Debug.Log("짝 틀림. 다시 뒤집습니다.");
            
            // 잠시 대기하여 플레이어가 틀린 짝을 볼 수 있게 합니다.
            yield return new WaitForSeconds(flipBackDelay); 

            card1.Flip(false); // 뒷면으로
            card2.Flip(false); // 뒷면으로
        }

        tryCount++;
        tryText.text = "Try Match Count: " + tryCount;
        // 다음 턴을 위해 뒤집힌 카드 리스트를 비우고 상태를 초기화합니다.
        flippedCards.Clear();
        isChecking = false;
    }
}