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
    private bool rememberIsHorizon = true;

    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 originalPosition;
    private Vector3 trueOriginPosition;

    public List<Vector3Int> occupiedCells = new List<Vector3Int>();

    [SerializeField]
    private BoardManager boardManager;


    private bool isPlaced=false;


    private void Start()
    {
        health = size;
        
    }

    private void Awake()
    {
        trueOriginPosition=transform.position;
        isHorizon = true;
    }

 

    public void TakeDamage()
    {
        health -= 1;
        if(health <= 0)
        {

            boardManager.OnShipSunk(this);
        }
    }

    private void Update()
    {
        if (isDragging && Input.GetKeyDown(KeyCode.R))
        {
            ChangeDirection();
            UpdateDragPosition();
        }
    }

    private void OnMouseDown()
    {
        if(boardManager.isReady)
            return;

        originalPosition = transform.position;
        boardManager.RemoveFromBoard(this);
        rememberIsHorizon = isHorizon;

        
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if (boardManager.isReady)
            return;

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
        Vector3 localPos = tilemap.transform.InverseTransformPoint(rawTargetPos);
        Vector3Int cellPosition = tilemap.LocalToCell(localPos);
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
        if (boardManager.isReady)
            return;

        isDragging = false;

        // 드래그 중에 이미 완전히 정렬된 좌표(transform.position)를 사용하므로
        // TryPlaceShip 내부 로직이 아주 명확해집니다.
        if (boardManager.TryPlaceShip(transform.position, this))
        {
            // 배치 성공 시 현재 위치 고정
            originalPosition = transform.position;
            if(!isPlaced)
                boardManager.PlaceCount++;
            isPlaced = true;
        }
        else
        {
            // 배치 실패 시 원래 위치로 복귀 후 해당 자리에 다시 세팅
            if (rememberIsHorizon != isHorizon)
                ChangeDirection();
            transform.position = originalPosition;
            boardManager.TryPlaceShip(transform.position, this);
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

    public void ResetShipPosition()
    {
        transform.position = trueOriginPosition;
        if (!isHorizon)
            ChangeDirection();
        isPlaced = false;

        health = size;

        ClearOccupiedCells();

    }

}