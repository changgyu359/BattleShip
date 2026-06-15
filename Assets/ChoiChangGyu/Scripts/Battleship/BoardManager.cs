using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public enum BoardType
{
    water,
    land,
    hit,
    miss,
    ship
}

public enum ReturnAttackState
{
    nullification,
    miss,
    hit
}

public class BoardManager : MonoBehaviour
{

    

    private const int boardLength = 11;
    private BoardType[,] board = new BoardType[boardLength, boardLength];
    [SerializeField]
    private Tilemap tilemap;
    
    public Tilemap Tilemap
    { get { return tilemap; } }

    [Header("타일")]
    [SerializeField] private TileBase waterTile;
    [SerializeField] private TileBase landTile;
    [SerializeField] private TileBase shipTile;
    [SerializeField] private TileBase missTile;
    [SerializeField] private TileBase hitTile;
    [SerializeField] private TileBase invalidTile; // 💡여기에 인스펙터에서 배치불가용 흰색 타일을 넣어주세요!

    [SerializeField]
    private Ship[] ships;
    public int PlaceCount
    { get; set; }

    private int remainShips;
    public int RemainShips
    { get { return remainShips; } }

    public bool isPlaying
    { get; set; }

    public List<Vector3Int> LastSunkShipCells { get; private set; }

    public bool isReady=false;

    private void Awake()
    {
        
        isPlaying = false;
    }

    private void Start()
    {
        BoundsInt bounds = tilemap.cellBounds;

        for (int x = bounds.xMin; x <= bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y <= bounds.yMax; y++)
            {
                Vector3Int localPlace = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(localPlace);

                int arrayX = x;
                int arrayY = y;

                if (arrayX >= 0 && arrayX < 11 && arrayY >= 0 && arrayY < 11)
                {
                    if (tile == landTile)
                        board[arrayX, arrayY] = BoardType.land;
                    else if (tile == shipTile)
                        board[arrayX, arrayY] = BoardType.ship;
                    else
                        board[arrayX, arrayY] = BoardType.water;
                }
            }
        }

        PlaceCount = 0;
        remainShips = ships.Length;
        // 시작할 때 미리 배치 불가 구역을 한번 계산해 줍니다.
        UpdateInvalidTiles();

        
    }

    private bool CanPlaceShip(int _x, int _y, Ship _ship)
    {
        int width = _ship.IsHorizon ? _ship.Size : 1;
        int height = _ship.IsHorizon ? 1 : _ship.Size;

        // 1. 배가 놓일 자리가 보드 범위를 벗어나거나 땅(land)인지 체크
        for (int i = 0; i < _ship.Size; i++)
        {
            int checkX = _ship.IsHorizon ? _x + i : _x;
            int checkY = _ship.IsHorizon ? _y : _y + i;

            if (checkX < 0 || checkX >= boardLength || checkY < 0 || checkY >= boardLength) return false;
            if (board[checkX, checkY] == BoardType.land) return false;
        }

        // 2. 주변 1칸에 다른 배가 스치고 있는지 체크 (배틀쉽 표준 룰 반영)
        int startX = Mathf.Max(0, _x - 1);
        int startY = Mathf.Max(0, _y - 1);
        int endX = Mathf.Min(boardLength - 1, _x + width);
        int endY = Mathf.Min(boardLength - 1, _y + height);

        for (int i = startX; i <= endX; i++)
        {
            for (int j = startY; j <= endY; j++)
            {
                if (board[i, j] == BoardType.ship)
                    return false;
            }
        }
        return true;
    }

    public void PlaceShip(int _x, int _y, Ship _ship)
    {
        if (CanPlaceShip(_x, _y, _ship))
        {
            for (int i = 0; i < _ship.Size; i++)
            {
                int placeX = _ship.IsHorizon ? _x + i : _x;
                int placeY = _ship.IsHorizon ? _y : _y + i;

                _ship.occupiedCells.Add(new Vector3Int(placeX, placeY, 0));

                board[placeX, placeY] = BoardType.ship;
            }
        }
    }

    public bool TryPlaceShip(Vector3 shipPosition, Ship ship)
    {
        Vector3 correctedPosition = shipPosition;
        float distanceToHead = (ship.Size - 1) * 0.5f;

        if (ship.IsHorizon)
            correctedPosition.x -= distanceToHead;
        else
            correctedPosition.y -= distanceToHead;

        Vector3 localPos = tilemap.transform.InverseTransformPoint(correctedPosition);
        Vector3Int cellPos = tilemap.LocalToCell(localPos);

        if (CanPlaceShip(cellPos.x, cellPos.y, ship))
        {
            ship.ClearOccupiedCells();
            PlaceShip(cellPos.x, cellPos.y, ship);

            // 시각적 타일 배치
            for (int i = 0; i < ship.Size; i++)
            {
                int pX = ship.IsHorizon ? cellPos.x + i : cellPos.x;
                int pY = ship.IsHorizon ? cellPos.y : cellPos.y + i;
                tilemap.SetTile(new Vector3Int(pX, pY, 0), shipTile);
            }

            // 💡 배가 성공적으로 놓였으니 배치 불가 구역을 갱신합니다.
            UpdateInvalidTiles();
            return true;
        }

        return false;
    }

    public void RemoveFromBoard(Ship _ship)
    {
        foreach (Vector3Int cell in _ship.occupiedCells)
        {
            if (cell.x >= 0 && cell.x < boardLength && cell.y >= 0 && cell.y < boardLength)
            {
                board[cell.x, cell.y] = BoardType.water;
                tilemap.SetTile(cell, waterTile);
            }
        }
        _ship.ClearOccupiedCells();

        // 💡 배를 들어 올렸으니 묶여있던 금지 구역을 해제하기 위해 다시 계산합니다.
        UpdateInvalidTiles();
    }

    // 💡 실시간으로 배치 불가 영역을 찾아서 하얗게 그려주는 핵심 함수
    private void UpdateInvalidTiles()
    {
        // 게임이 이미 시작되었다면 연산할 필요가 없습니다.
        if (isPlaying) return;

        for (int x = 0; x < boardLength; x++)
        {
            for (int y = 0; y < boardLength; y++)
            {
                // 이미 배가 깔려있거나 땅인 곳은 건드리지 않습니다.
                if (board[x, y] == BoardType.ship || board[x, y] == BoardType.land)
                    continue;

                bool isSurroundedByShip = false;

                // 해당 칸 주변 8방향을 검사해서 배가 한 칸이라도 인접해 있는지 확인합니다.
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        int checkX = x + i;
                        int checkY = y + j;

                        if (checkX >= 0 && checkX < boardLength && checkY >= 0 && checkY < boardLength)
                        {
                            if (board[checkX, checkY] == BoardType.ship)
                            {
                                isSurroundedByShip = true;
                                break;
                            }
                        }
                    }
                    if (isSurroundedByShip) break;
                }

                Vector3Int currentCell = new Vector3Int(x, y, 0);

                // 주변에 배가 있다면 배치 불가 타일(흰색)을 깔아줍니다.
                if (isSurroundedByShip)
                {
                    tilemap.SetTile(currentCell, invalidTile);
                }
                else
                {
                    // 주변에 배가 없다면 깨끗한 일반 물 타일로 되돌립니다.
                    tilemap.SetTile(currentCell, waterTile);
                }
            }
        }
    }

    public void GameStart(bool isEnemyBoard)
    {
        isPlaying = true;

        // 게임 시작 시, 배치 불가 구역 타일들을 포함한 모든 배 자리를 물 타일로 초기화합니다.
        for (int x = 0; x < boardLength; x++)
        {
            for (int y = 0; y < boardLength; y++)
            {
                if (tilemap.GetTile(new Vector3Int(x, y, 0)) == invalidTile)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), waterTile);
                }
            }
        }

        if( isEnemyBoard )
        {
            for(int x = 0;x < boardLength; x++)
            {
                for(int y = 0;y < boardLength; y++)
                {
                    if (board[x,y]==BoardType.ship)
                    {
                        tilemap.SetTile(new Vector3Int(x,y, 0), waterTile);
                    }
                }
            }
            
        }

        foreach (Ship ship in ships)
        {
            ship.gameObject.SetActive(false);
        }

    }

    private Ship GetShipAt(Vector3Int cellPos)
    {
        foreach (Ship ship in ships)
        {
            if (ship.occupiedCells.Contains(cellPos))
                return ship;
        }
        return null;
    }

    public ReturnAttackState AttackCell(Vector3Int cellPos)
    {
        LastSunkShipCells = null;

        if (cellPos.x < 0 || cellPos.x >= boardLength || cellPos.y < 0 || cellPos.y >= boardLength) 
            return ReturnAttackState.nullification;
        if (board[cellPos.x, cellPos.y] == BoardType.hit || board[cellPos.x, cellPos.y] == BoardType.miss) 
            return ReturnAttackState.nullification;

        if (board[cellPos.x, cellPos.y] == BoardType.water)
        {
            board[cellPos.x, cellPos.y] = BoardType.miss;
            tilemap.SetTile(cellPos, missTile);

            return ReturnAttackState.miss;
        }
        else if (board[cellPos.x, cellPos.y] == BoardType.ship)
        {
            board[cellPos.x, cellPos.y] = BoardType.hit;
            tilemap.SetTile(cellPos, hitTile);

            Ship targetShip = GetShipAt(cellPos);
            if (targetShip != null)
            {
                targetShip.TakeDamage();
            }
            return ReturnAttackState.hit;
        }
        return ReturnAttackState.nullification;
    }

    public void OnShipSunk(Ship sunkShip)
    {
        LastSunkShipCells = new List<Vector3Int>(sunkShip.occupiedCells);

        foreach (Vector3Int cell in sunkShip.occupiedCells)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Vector3Int targetPos = new Vector3Int(cell.x + i, cell.y + j, 0);

                    if (targetPos.x >= 0 && targetPos.y >= 0 && targetPos.x < boardLength && targetPos.y < boardLength)
                    {
                        if (board[targetPos.x, targetPos.y] == BoardType.water)
                        {
                            board[targetPos.x, targetPos.y] = BoardType.miss;
                            tilemap.SetTile(targetPos, missTile);
                        }
                    }
                }
            }
        }
        remainShips--;
        BattleshipManger.Instance.CheckGameOver(this);
    }

    public void ForceAttackResult(Vector3Int cellPos, bool isHit)
    {
        if (cellPos.x < 0 || cellPos.x >= boardLength || cellPos.y < 0 || cellPos.y >= boardLength) return;

        board[cellPos.x, cellPos.y] = isHit ? BoardType.hit : BoardType.miss;
        tilemap.SetTile(cellPos, isHit ? hitTile : missTile);
    }

    public void ForceRevealSunkArea(int[] sunkX, int[] sunkY)
    {
        if (sunkX == null || sunkY == null || sunkX.Length != sunkY.Length) return;

        for (int k = 0; k < sunkX.Length; k++)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Vector3Int targetPos = new Vector3Int(sunkX[k] + i, sunkY[k] + j, 0);

                    if (targetPos.x >= 0 && targetPos.y >= 0 && targetPos.x < boardLength && targetPos.y < boardLength)
                    {
                        // 내가 보는 상대 보드 격자가 아직 아무것도 안 맞은 기본 상태(water)라면 miss로 강제 변경
                        if (board[targetPos.x, targetPos.y] == BoardType.water)
                        {
                            board[targetPos.x, targetPos.y] = BoardType.miss;
                            tilemap.SetTile(targetPos, missTile);
                        }
                    }
                }
            }
        }
    }

    public void NetworkShipSunk()
    {
        remainShips--;
        BattleshipManger.Instance.CheckGameOver(this);
    }

    public void SetRemainShips(int _count)
    {
        remainShips = _count;
    }

    public void ResetAllShipPosition()
    {
        foreach (Ship ship in ships)
        {
            ship.gameObject.SetActive(true);
            ship.ResetShipPosition();
        }

        PlaceCount = 0;
        remainShips = ships.Length;
    }

    public void ResetBoardState()
    {
        isPlaying = false;
        LastSunkShipCells = null;

        for(int x=0; x<boardLength; x++)
        {
            for(int y=0; y<boardLength; y++)
            {
                if (board[x,y]!=BoardType.land)
                {
                    board[x,y] = BoardType.water;
                    tilemap.SetTile(new Vector3Int(x,y,0),waterTile);
                }
            }
        }
    }

}