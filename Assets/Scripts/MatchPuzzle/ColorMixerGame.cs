using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorMixerGame : MonoBehaviour
{
    [Header("색상 버튼 연결")]
    // 이 리스트 순서가 Red(0), Blue(1), Yellow(2) 순서라고 가정합니다.
    public List<Button> colorButtons; 

    [Header("UI 연결")]
    public Image goalColorDisplay; // 목표 색상을 보여주는 UI
    public Image currentColorDisplay; // 현재 섞인 색상을 보여주는 UI
    public TMP_Text timerText;
    public TMP_Text scoreText;
    
    [Header("게임 설정")]
    public float roundTime = 10f; // 라운드 제한 시간

    // R, G, B 순서로 현재 섞인 색상 가중치 (0~1 사이)
    private Color currentColorMix = Color.black; 
    private Color goalColor; 
    private float timeRemaining;
    private bool isGameActive = false;
    private int score;

    // --- 시작 ---
    void Start()
    {
        // 버튼 이벤트 연결 (각 버튼의 인덱스를 전달)
        for (int i = 0; i < colorButtons.Count; i++)
        {
            int index = i;
            colorButtons[i].onClick.AddListener(() => MixColor(index));
        }
        scoreText.text = "Score: " + score;
        StartNewRound();
    }

    void Update()
    {
        if (!isGameActive) return;

        timeRemaining -= Time.deltaTime;
        timerText.text = "Time: " + Mathf.Max(0, timeRemaining).ToString("F1");

        if (timeRemaining <= 0)
        {
            FailRound("시간 초과!");
        }
    }

    // --- 새로운 라운드 시작 ---
    private void StartNewRound()
    {
        isGameActive = true;
        timeRemaining = roundTime;
        
        // 1. 목표 색상 랜덤
        goalColor.r = Mathf.Min(1f, goalColor.r = Random.Range(0, 3) * 0.5f);
        goalColor.g = Mathf.Min(1f, goalColor.g = Random.Range(0, 3) * 0.5f);
        goalColor.b = Mathf.Min(1f, goalColor.b = Random.Range(0, 3) * 0.5f);
        goalColor.a = 1;
        
        goalColorDisplay.color = goalColor;

        // 2. 현재 섞인 색상 초기화 (검정/어두운 회색)
        currentColorMix = Color.black; 
        currentColorDisplay.color = currentColorMix;
    }

    // --- 색상 섞기 로직 ---
    public void MixColor(int buttonIndex)
    {
        if (!isGameActive) return;

        // 버튼 클릭 시 해당 색상의 채널을 증가시킵니다.
        // 이 예시에서는 섞이는 횟수가 아니라, 섞는 동작 자체를 1회로 간주합니다.
        
        if (buttonIndex == 0) // Red
        {
            currentColorMix.r = Mathf.Min(1f, currentColorMix.r + 0.5f);
        }
        else if (buttonIndex == 1) // Green
        {
            currentColorMix.g = Mathf.Min(1f, currentColorMix.g + 0.5f);
        }
        else if (buttonIndex == 2) // Blue
        {
            currentColorMix.b = Mathf.Min(1f, currentColorMix.b + 0.5f);
        }
        else if (buttonIndex == 3) // Reset
        {
            currentColorMix.r = 0;
            currentColorMix.g = 0;
            currentColorMix.b = 0;
        }
        
        currentColorDisplay.color = currentColorMix;
        
        // 섞을 때마다 정답 검사
        CheckForWin();
    }

    // --- 승리 조건 검사 ---
    private void CheckForWin()
    {
        // 목표 색상과 현재 색상이 일치하는지 확인 (Color.Equals 대신 근사치 비교)
        // 약 95% 이상 유사하면 정답으로 간주
        if (ColorSimilarity(currentColorMix, goalColor) > 0.95f)
        {
            WinRound();
        }
        // TODO: 만약 목표 색을 지나치게 섞어버리면 오답 처리 로직 추가 가능
    }

    // --- 두 색상 간의 유사도 계산 (벡터 거리 사용) ---
    private float ColorSimilarity(Color c1, Color c2)
    {
        // 두 색상의 RGB 벡터 간의 거리(유클리드 거리)를 계산하여 유사도 측정
        Vector3 v1 = new Vector3(c1.r, c1.g, c1.b);
        Vector3 v2 = new Vector3(c2.r, c2.g, c2.b);
        float distance = Vector3.Distance(v1, v2);
        
        // 거리가 0이면 완벽히 일치 (유사도 1.0), 거리가 멀수록 유사도 0에 가까워짐
        // Max distance is sqrt(3) ~ 1.732 (black to white)
        return 1f - (distance / 1.732f); 
    }

    // --- 라운드 성공 ---
    private void WinRound()
    {
        isGameActive = false;
        score++;
        scoreText.text = "Score: " + score;
        Debug.Log("성공! 다음 라운드로 이동");
        StartNewRound();
    }

    // --- 라운드 실패 ---
    private void FailRound(string reason)
    {
        isGameActive = false;
        Debug.Log("실패: " + reason);
        // TODO: 게임 오버 처리
    }
}