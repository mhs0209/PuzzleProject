using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SortingGameManager : MonoBehaviour
{
    [Header("UI 및 스폰 설정")]
    public TMP_Text scoreText;
    public Transform spawnPoint;
    public GameObject[] shapePrefabs; // 도형 프리팹 배열 (인덱스 0: Square, 1: Triangle 등)

    [Header("분류함 설정")]
    // 이 리스트에는 분류함 오브젝트들(Collider2D가 붙은)을 연결해주세요.
    public List<TargetBin> targetBins; 

    [Header("게임 설정")]
    public float minSpawnInterval = 1f;
    public float maxSpawnInterval = 2f;

    private int score = 0;
    private bool isGameOver = false;

    void Start()
    {
        scoreText.text = "Score: 0";
        StartCoroutine(SpawnShapes());
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    // --- 도형 생성 코루틴 ---
    IEnumerator SpawnShapes()
    {
        while (!isGameOver)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));

            // 랜덤하게 도형 선택
            int randomIndex = Random.Range(0, shapePrefabs.Length);
            GameObject selectedPrefab = shapePrefabs[randomIndex];

            Instantiate(selectedPrefab, spawnPoint.position, Quaternion.identity);
        }
    }

    // --- 도형 배치 검사 (OnMouseUp 시 호출) ---
    public void CheckShapePlacement(FallingShape shape)
    {
        // 1. 도형이 현재 어떤 분류함의 Collider2D와 겹치는지 확인
        bool placedCorrectly = false;
        
        foreach (TargetBin bin in targetBins)
        {
            // 도형의 Collider와 분류함의 Collider가 겹치는지 확인 (Bounding Box 검사)
            if (bin.GetComponent<Collider2D>().OverlapPoint(shape.transform.position))
            {
                // 겹친 분류함의 타입과 도형의 타입이 일치하는지 확인
                if (bin.binType == shape.shapeType)
                {
                    // 정답!
                    score++;
                    scoreText.text = "Score: " + score;
                    placedCorrectly = true;
                    Debug.Log("분류 성공: " + shape.shapeType);
                    break;
                }
                else
                {
                    // 오답! 잘못된 분류함에 넣음
                    GameOver();
                    Destroy(shape.gameObject);
                    return;
                }
            }
        }
        
        // 2. 처리 완료된 도형은 제거
        Destroy(shape.gameObject);

        // 3. 분류함에 넣지 못하고 드래그를 놓았을 경우 (오답 처리 가능)
        if (!placedCorrectly)
        {
            // TODO: 분류함 영역 밖에서 놓았을 때의 추가적인 패널티 로직
        }
    }
    
    // --- 도형이 바닥에 닿아 놓쳤을 때 ---
    public void ShapeMissed(FallingShape shape)
    {
        if (isGameOver) return;
        Debug.Log("도형을 놓쳤습니다!");
        GameOver();
        Destroy(shape.gameObject);
    }

    // --- 게임 오버 ---
    private void GameOver()
    {
        isGameOver = true;
        StopAllCoroutines();
        Debug.Log("게임 오버! 최종 점수: " + score);
        // TODO: 모든 게임 오브젝트 정리 및 UI 표시
    }
}