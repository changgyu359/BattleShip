using UnityEngine;
using UnityEngine.Tilemaps;

public class InputManager : MonoBehaviour
{

    private static InputManager instance;
    public static InputManager Instance
        { get { return instance; } }

    [Header("타일맵")]
    [SerializeField] private Tilemap player1Tilemap;
    [SerializeField] private Tilemap player2Tilemap;

    [Header("보드 매니저")]
    [SerializeField] private BoardManager player1Board;
    [SerializeField] private BoardManager player2Board;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }


    private void Update()
    {
      
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(player1Board.PlaceCount<player1Board.RemainShips)
            {
                Debug.Log("배를 모두 배치해주세요!");
                return;
            }


            int myId = TCPManager.Instance.IsServer ? 1 : 2;
            TCPManager.Instance.SendReadyDataEvnet(myId);
            BattleshipManger.Instance.PressReadyButton(myId);
        }

        
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            bool isMyTurn = (TCPManager.Instance.IsServer && BattleshipManger.Instance.CurrentState == GameState.player1Turn) ||
                        (!TCPManager.Instance.IsServer && BattleshipManger.Instance.CurrentState == GameState.player2Turn);

            if (isMyTurn)
            {
                Vector3 localPos = player2Tilemap.transform.InverseTransformPoint(mousePos);
                Vector3Int cellPos = player2Tilemap.LocalToCell(localPos);

                if (player2Tilemap.HasTile(cellPos))
                {
                    // 로컬에서 헛스윙 공격 판정을 내리지 않고, 상대방에게 좌표만 보냅니다. (State 0: 공격 요청)
                    TCPManager.Instance.SendBattleShipDataEvent(cellPos.x, cellPos.y, 0);
                }
            }

            //if(BattleshipManger.Instance.CurrentState==GameState.player1Turn)
            //{
            //    if (!TCPManager.Instance.IsServer) return;

            //    Vector3 localPos = player2Tilemap.transform.InverseTransformPoint(mousePos);
            //    Vector3Int cellPos = player2Tilemap.LocalToCell(localPos);

            //    if (player2Tilemap.HasTile(cellPos))
            //    {
            //        ReturnAttackState hitState = player2Board.AttackCell(cellPos);

            //        if(hitState != ReturnAttackState.nullification)
            //        {
            //            TCPManager.Instance.SendBattleShipDataEvent(cellPos.x, cellPos.y);

            //            bool isHit = (hitState == ReturnAttackState.hit);
            //            BattleshipManger.Instance.OnAttackExecuted(isHit);
            //        }

            //        //if (hitState == ReturnAttackState.miss)
            //        //    BattleshipManger.Instance.OnAttackExecuted(false);
            //        //else if (hitState == ReturnAttackState.hit)
            //        //    BattleshipManger.Instance.OnAttackExecuted(true);
            //    }
            //}
            //else if(BattleshipManger.Instance.CurrentState == GameState.player2Turn)
            //{
            //    if(TCPManager.Instance.IsServer) return;    

            //    Vector3 localPos = player2Tilemap.transform.InverseTransformPoint(mousePos);
            //    Vector3Int cellPos = player2Tilemap.LocalToCell(localPos);

            //    if (player2Tilemap.HasTile(cellPos))
            //    {
            //        ReturnAttackState hitState = player2Board.AttackCell(cellPos);

            //        if (hitState != ReturnAttackState.nullification)
            //        {
            //            TCPManager.Instance.SendBattleShipDataEvent(cellPos.x, cellPos.y);

            //            bool isHit = (hitState == ReturnAttackState.hit);
            //            BattleshipManger.Instance.OnAttackExecuted(isHit);
            //        }
            //        //if (hitState == ReturnAttackState.miss)
            //        //    BattleshipManger.Instance.OnAttackExecuted(false);
            //        //else if (hitState == ReturnAttackState.hit)
            //        //    BattleshipManger.Instance.OnAttackExecuted(true);
            //    }
            //}


        }
    }

    public void ExecuteNetworkAttack(BattleShipData data)
    {

        Vector3Int cellPos = new Vector3Int(data.X, data.Y, 0);

        if (data.State == 0)
        {
            // [방어자 시점] 공격을 당함
            ReturnAttackState result = player1Board.AttackCell(cellPos);

            if (result != ReturnAttackState.nullification)
            {
                int responseState = (result == ReturnAttackState.hit) ? 2 : 1;
                int[] sunkX = null;
                int[] sunkY = null;

                // 만약 이번 공격으로 내 배가 침몰했다면 좌표 리스트를 배열로 가공합니다.
                if (player1Board.LastSunkShipCells != null && player1Board.LastSunkShipCells.Count > 0)
                {
                    responseState = 3; // State 3: 명중 및 침몰 상태코드 정의
                    sunkX = new int[player1Board.LastSunkShipCells.Count];
                    sunkY = new int[player1Board.LastSunkShipCells.Count];

                    for (int i = 0; i < player1Board.LastSunkShipCells.Count; i++)
                    {
                        sunkX[i] = player1Board.LastSunkShipCells[i].x;
                        sunkY[i] = player1Board.LastSunkShipCells[i].y;
                    }
                }

                // 결과를 상대방(공격자)에게 전송
                TCPManager.Instance.SendBattleShipDataEvent(data.X, data.Y, responseState, sunkX, sunkY);
                BattleshipManger.Instance.OnAttackExecuted(result == ReturnAttackState.hit);
            }
        }
        else
        {
            // [공격자 시점] 내가 공격한 결과를 돌려받음
            bool isHit = (data.State == 2 || data.State == 3);

            // 시각 보드에 내 격자 업데이트
            player2Board.ForceAttackResult(cellPos, isHit);

            // 만약 상대방 배가 침몰했다고 응답이 오면(State == 3), 같이 넘어온 좌표들을 기반으로 주변을 밝힙니다.
            if (data.State == 3 && data.SunkX != null && data.SunkY != null)
            {
                player2Board.ForceRevealSunkArea(data.SunkX, data.SunkY);

                player2Board.NetworkShipSunk();
            }

            BattleshipManger.Instance.OnAttackExecuted(isHit);
        }
    }

}
