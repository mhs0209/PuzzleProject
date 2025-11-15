using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro; // List를 다루기 위해 필요합니다.

public class MemoryPuzzleGame : MonoBehaviour
{
    // --- 설정 변수 ---
    [Header("UI 버튼 연결")]
    // Unity 에디터에서 4개의 버튼(Image 또는 Button 컴포넌트)을 연결해주세요.
    public List<Button> buttons;
    public Button restartButton;
    
    [Header("UI Text 연결")]
    public TMP_Text countText;
    //public TMP_Text resultText;
    
    [Header("색상 설정")]
    public Color normalColor = Color.gray; // 평상시 버튼 색상
    public Color highlightColor = Color.white; // 순서를 보여줄 때의 색상

    [Header("시간 설정")]
    public float flashDuration = 0.5f; // 버튼이 빛나는 시간
    public float waitBetweenFlashes = 0.2f; // 버튼 간 깜빡임 간격

    // --- 내부 상태 변수 ---
    private List<int> sequence = new List<int>(); // 컴퓨터가 보여줄 순서 (버튼 인덱스 저장)
    private int playerSequenceIndex = 0; // 플레이어가 현재 맞춰야 하는 순서의 인덱스
    private bool isPlayerTurn = false; // 현재 플레이어가 입력할 차례인지 여부
    private bool isFlashing = false; // 현재 순서를 보여주는 중인지 여부
    private int maxCount = 0;

    // --- 게임 시작 ---
    void Start()
    {
        InitializeButtons();
        StartNewRound();
    }

    // --- 1. 버튼 초기화 및 이벤트 연결 ---
    private void InitializeButtons()
    {
        // 버튼이 4개인지 확인하고, 각 버튼에 클릭 이벤트를 연결합니다.
        if (buttons.Count != 4)
        {
            Debug.LogError("버튼은 반드시 4개여야 합니다. 현재: " + buttons.Count + "개");
            return;
        }

        for (int i = 0; i < buttons.Count; i++)
        {
            // 클로저(Closure) 문제를 피하기 위해 임시 변수를 사용합니다.
            int buttonIndex = i; 
            
            // UI Button의 OnClick 이벤트에 PlayerInput 함수를 연결하고, 
            // 현재 버튼의 인덱스(0, 1, 2, 3)를 넘겨줍니다.
            buttons[i].onClick.AddListener(() => PlayerInput(buttonIndex));
            
            // 초기 색상 설정
            buttons[i].GetComponent<Image>().color = normalColor;
        }
    }

    // --- 2. 라운드 시작 및 순서 생성 ---
    public void StartNewRound()
    {
        if (isFlashing || isPlayerTurn) return; // 이미 진행 중이면 무시

        if (countText != null && restartButton != null)
        {
            restartButton.gameObject.SetActive(false);
            countText.text = $"Max Count : {maxCount}\nCount : {sequence.Count}";
        }

        // 1. 새로운 랜덤 순서를 리스트에 추가합니다.
        int newIndex = Random.Range(0, buttons.Count); // 0, 1, 2, 3 중 랜덤 선택
        sequence.Add(newIndex);
        
        // 2. 플레이어의 인덱스를 초기화합니다.
        playerSequenceIndex = 0;
        isPlayerTurn = false; // 컴퓨터 턴 시작

        // 3. 순서를 보여주는 코루틴 시작
        StartCoroutine(ShowSequence());
    }

    // --- 3. 순서 시각화 (코루틴) ---
    IEnumerator ShowSequence()
    {
        isFlashing = true;

        yield return new WaitForSeconds(1.0f); // 순서를 보여주기 전 잠시 대기

        // 저장된 순서를 차례대로 반복하며 버튼을 깜빡입니다.
        foreach (int index in sequence)
        {
            // 1. 버튼 활성화 (하이라이트 색상으로 변경)
            Image buttonImage = buttons[index].GetComponent<Image>();
            buttonImage.color = highlightColor;

            // 2. 지정된 시간만큼 대기 (버튼이 빛나는 시간)
            yield return new WaitForSeconds(flashDuration);

            // 3. 버튼 비활성화 (원래 색상으로 복구)
            buttonImage.color = normalColor;

            // 4. 다음 깜빡임까지 짧은 간격 대기
            yield return new WaitForSeconds(waitBetweenFlashes);
        }

        isFlashing = false;
        isPlayerTurn = true; // 순서 보여주기가 끝나면 플레이어 턴으로 전환

        Debug.Log("순서가 끝났습니다. 이제 플레이어 차례입니다. 현재 길이: " + sequence.Count);
    }

    // --- 4. 플레이어 입력 처리 ---
    public void PlayerInput(int buttonIndex)
    {
        if (!isPlayerTurn || isFlashing) return; // 플레이어 턴이 아니거나 깜빡이는 중이면 입력 무시

        // 1. 플레이어가 누른 버튼 인덱스와 현재 맞춰야 할 순서의 버튼 인덱스를 비교합니다.
        if (buttonIndex == sequence[playerSequenceIndex])
        {
            // 정답 처리 (버튼이 맞았을 경우)
            
            // 2. 다음 순서로 넘어갑니다.
            playerSequenceIndex++;

            // 3. 현재 라운드의 모든 순서를 다 맞췄는지 확인합니다.
            if (playerSequenceIndex >= sequence.Count)
            {
                // 라운드 성공!
                Debug.Log("라운드 성공! 다음 레벨로 이동합니다.");
                
                if(sequence.Count > maxCount) maxCount = sequence.Count;
                
                isPlayerTurn = false; // 입력 차단
                // 다음 라운드 시작
                StartNewRound();
            }
        }
        else
        {
            // 오답 처리 (버튼이 틀렸을 경우)
            Debug.Log("게임 오버! 최종 길이: " + (sequence.Count-1));
            
            // 게임 오버 처리 로직 (예: UI 표시, 리셋 함수 호출 등)
            GameOver();
        }
    }

    // --- 5. 게임 오버 처리 ---
    private void GameOver()
    {
        // 순서와 인덱스 초기화
        sequence.Clear();
        playerSequenceIndex = 0;
        isPlayerTurn = false;
        restartButton.gameObject.SetActive(true);
        
        // 게임 오버 UI 등을 표시할 수 있습니다.
        // 현재는 콘솔 로그로 대체합니다.
        Debug.Log("새로운 게임을 시작하려면 StartNewRound()를 호출하세요.");
    }
}