using UnityEngine;
using UnityEngine.Tilemaps;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;

    private void Update()
    {
        // 1. 게임 시작(스페이스바) 입력 처리
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BoardManager.Instance.GameStart();
        }

        // 2. 플레이어 공격(좌클릭) 입력 처리
        if (Input.GetMouseButtonDown(0))
        {
            // 플레이 중이 아닐 때는 입력 무시 (예: 배 배치 단계 등)
            if (!BoardManager.Instance.isPlaying) return;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            Vector3Int cellPos = tilemap.WorldToCell(mousePos);

            // 마우스 클릭 위치가 실제 타일맵 격자 내부에 있을 때만 보드 매니저 호출
            if (tilemap.HasTile(cellPos))
            {
                BoardManager.Instance.AttackCell(cellPos);
            }
        }
    }

}
