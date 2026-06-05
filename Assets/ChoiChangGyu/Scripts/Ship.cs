using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ship : MonoBehaviour
{
    [SerializeField] private int size;
    public int Size => size;
    private int health;

    [SerializeField] private Tilemap tilemap;

    private bool isHorizon = true;
    public bool IsHorizon => isHorizon;

    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 originalPosition;

    public List<Vector3Int> occupiedCells = new List<Vector3Int>();


    private void Start()
    {
        health = size;
    }

    public void TakeDamage()
    {
        health -= 1;
        if(health <= 0)
        {
            
            BoardManager.Instance.OnShipSunk(this);
        }
    }

    private void Update()
    {
        if (isDragging && Input.GetKeyDown(KeyCode.R))
        {
            ChangeDirection();
            // 회전했을 때도 즉시 스냅 위치를 업데이트해 줍니다.
            UpdateDragPosition();
        }
    }

    private void OnMouseDown()
    {
        originalPosition = transform.position;
        BoardManager.Instance.RemoveFromBoard(this);

        // 피봇이 Center이므로 마우스 포인터와의 오프셋을 구합니다.
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            UpdateDragPosition();
        }
    }

    // 드래그 중 실시간 격자 스냅 로직
    private void UpdateDragPosition()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 rawTargetPos = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;

        // 현재 마우스 위치가 속한 타일의 정수 좌표를 구함
        Vector3Int cellPosition = tilemap.WorldToCell(rawTargetPos);
        // 그 타일의 월드 정중앙 좌표를 가져옴
        Vector3 cellCenter = tilemap.GetCellCenterWorld(cellPosition);

        // 💡 짝수/홀수 및 가로/세로에 따른 피봇 위치 실시간 보정
        if (size % 2 == 0)
        {
            // 짝수선 정렬: 가로일 때는 X축으로 0.5f, 세로일 때는 Y축으로 0.5f 밀어주어야 
            // 에디터 상에서 격자 선에 이쁘게 맞아떨어집니다.
            Vector3 adjustment = isHorizon ? new Vector3(0.5f, 0, 0) : new Vector3(0, 0.5f, 0);
            transform.position = cellCenter + adjustment;
        }
        else
        {
            // 홀수는 타일 정중앙이 피봇과 일치하므로 그대로 안착
            transform.position = cellCenter;
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;

        // 드래그 중에 이미 완전히 정렬된 좌표(transform.position)를 사용하므로
        // TryPlaceShip 내부 로직이 아주 명확해집니다.
        if (BoardManager.Instance.TryPlaceShip(transform.position, this))
        {
            // 배치 성공 시 현재 위치 고정
            originalPosition = transform.position;
        }
        else
        {
            // 배치 실패 시 원래 위치로 복귀 후 해당 자리에 다시 세팅
            transform.position = originalPosition;
            BoardManager.Instance.TryPlaceShip(transform.position, this);
        }
    }

    public void ChangeDirection()
    {
        isHorizon = !isHorizon;
        gameObject.transform.Rotate(0, 0, 90f);
    }

    public void ClearOccupiedCells()
    {
        occupiedCells.Clear();
    }

    
}