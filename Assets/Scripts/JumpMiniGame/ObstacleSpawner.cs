using System.Collections;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("장애물 설정")]
    public GameObject obstaclePrefab; // 장애물 프리팹
    public float minSpawnTime = 1.5f;
    public float maxSpawnTime = 3f;
    public float spawnXPosition = 10f; // 화면 오른쪽 끝 생성 위치

    private bool isSpawning = false;

    void Start()
    {
        isSpawning = true;
        StartCoroutine(SpawnRoutine());
    }

    // --- 장애물 생성 코루틴 ---
    IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {
            // 랜덤한 시간 간격 대기
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            // 장애물 생성
            Vector3 spawnPos = new Vector3(spawnXPosition, 
                transform.position.y, // 스포너의 Y 위치 사용
                0);
            
            GameObject newObstacle = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
            
            // 생성 후 일정 시간 뒤에 제거되는 컴포넌트를 장애물에 추가해야 합니다.
            // 예시: Destroy(gameObject, 10f)
        }
    }
    
    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
    }
}