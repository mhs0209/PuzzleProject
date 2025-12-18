using TMPro;
using UnityEngine;

public class TimingGameManager : MonoBehaviour
{
    // 싱글톤 패턴 (어디서든 쉽게 접근 가능)
    public static TimingGameManager Instance;

    [Header("UI 연결")]
    public TMP_Text scoreText;
    public ObstacleSpawner spawner; // 스포너 스크립트 연결
    
    private int score = 0;
    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- 점수 증가 ---
    public void IncreaseScore()
    {
        if (isGameOver) return;
        score++;
        scoreText.text = "점수: " + score.ToString();
    }
    
    // --- 게임 오버 처리 ---
    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        
        Debug.Log("게임 오버! 최종 점수: " + score);
        
        // 장애물 생성을 중단합니다.
        spawner.StopSpawning(); 
        
        // TODO: 게임 오버 UI 활성화, 재시작 버튼 표시 등
    }
}