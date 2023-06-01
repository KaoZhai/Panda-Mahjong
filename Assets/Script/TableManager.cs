using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.PlayingRoom {
    
    public enum MovementType {
        None,
        Win,
        PlayTile,
        MingGang,
        AnGang,
        JiaGang,
        Pong,
        Chi
    }

    public class TableManager : MonoBehaviour
    {
        public static TableManager Instance
        {
            get;
            private set;
        }
        [SerializeField] private List<Player> players = new List<Player>();
        [SerializeField] private TileWall tileWall;

        private string activePlayerId = 0;

        private GameObject lastTile = null;
        
        [SerializeField] private GameObject winningBtn, chiBtn, pongBtn, gangBtn;
        

        public TileWall TileWall
        {
            get { return tileWall;}
        }
        public GameObject LastTile
        {
            set { lastTile = value; }
            get { return lastTile; }
        }
        public int ActivePlayerId
        {
            get { return activePlayerId; }
        }

        private MovementType moveStatus = MovementType.None;

        public bool MoveStatus {
            get { return moveStatus; }
            set { moveStatus = value; }
        }

        void Start() 
        {

            tileWall.GetReady(this);
            // todo: player will set player id, now is 0-3
            for(int i = 0; i < 4; i++)
            {
                players[i].PlayerId = i.ToString();
                players[i].TableManager = this;
            }

            // DecideDealer();

            DealTiles();
            OpenDoor();
            AfterDraw();
        }
        
        IEnumerator AfterDraw()
        {
            yield return StartCoroutine(CheckBuhua());
            yield return StartCoroutine(CheckSelfDrawWin());
            yield return StartCoroutine(CheckSelfDrawGang());
            StartCoroutine(PlayRound());
        }
        IEnumerator CheckSelfDrawGang()
        {
            if(players[activePlayerId].IsCanSelfDrawGang())
            {
                SetButton(gangBtn, true);
            }
            yield return null;
        }
        IEnumerator CheckSelfDrawWin()
        {
            if(players[activePlayerId].IsCanSelfDrawWin())
            {
                SetButton(winningBtn, true);
            }
            yield return null;
        }

        IEnumerator CheckBuhua()
        {
            while (!players[activePlayerId].IsDoneBuHua())
            {
                yield return StartCoroutine(BuHua(players[activePlayerId]));
            }
            yield return null;
        }

        public IEnumerator BuHua(Player player)
        {
            int replaceTileNum = player.ShowFlower();
            for (int i = 0; i < replaceTileNum; ++i)
            {
                tileWall.BuPai(player);
            }
        }

        public void Draw()
        {
            tileWall.DealTile(players[activePlayerId]);
        }

        IEnumerator PlayRound()
        {
            moveStatus = MovementType.None;
            // int time_limit = 10;
            // todo time limit not add now
            while(moveStatus == MovementType.None)
            {
                yield return new WaitForSeconds(1f);
            }

            if (moveStatus == MovementType.Win)
            {
                // todo 算台
            }
            else if (moveStatus == MovementType.AnGang)
            {
                yield return StartCoroutine(BuHua(players[activePlayerId]));
                StartCoroutine(AfterDraw());
            }
            else if (moveStatus == MovementType.JiaGang)
            {
                yield return StartCoroutine(BuHua(players[activePlayerId]));
                StartCoroutine(AfterDraw());
            }
            else if(moveStatus == MovementType.PlayTile)
            {
                StartCoroutine(AfterPlay());
            }
            else
            {
                Debug.Log("error status in play round");
                yield return null;
            }
            

        }
        IEnumerable CheckWin()
        {
            if(players[activePlayerId].IsCanWin())
            {
                SetButton(winningBtn, true);
            }
            int time_limit = 2;
            while(moveStatus == MovementType.None && time_limit)
            {
                yield return new WaitForSeconds(1f);
                --time_limit;
            }

            if (moveStatus == MovementType.Win)
            {
                // todo 算台
            }

            // todo 流局
            if (tileWall.RemainTiles == 0)
            {
                
            }
        }


        IEnumerable AfterPlay()
        {
            yield return StartCoroutine(CheckWin());
            yield return StartCoroutine(CheckGang());
            yield return StartCoroutine(CheckPong());
            yield return StartCoroutine(CheckChi());
            int time_limit = 2;
            while(moveStatus == MovementType.None && time_limit)
            {
                yield return new WaitForSeconds(1f);
                --time_limit;
            }


            if (moveStatus == MovementType.MingGang)
            {
                // todo 算台
            }
            else if (moveStatus == MovementType.Pong)
            {
                yield return StartCoroutine(BuHua(players[activePlayerId]));
                yield return StartCoroutine(AfterDraw());
            }
            else if (moveStatus == MovementType.Chi)
            {
                yield return StartCoroutine(BuHua(players[activePlayerId]));
                yield return StartCoroutine(AfterDraw());
            }
            else
            {
                Debug.Log("error status in play round");
                yield return null;
            }
            

        }
        IEnumerator OpenDoor()
        {
            tileWall.DealTile(players[activePlayerId]);
            int cnt = 0;
            while(cnt < 4)
            {
                foreach(Player player in players)
                {
                    if (player.IsDoneBuHua())
                    {
                        cnt += 1;
                    }
                    else
                    {
                        yield return StartCoroutine(BuHua(player));
                    }
                }
            }
            foreach(Player player in players)
            {
                player.SortHandTiles();
            }

        }

        void DealTiles()
        {
            for(int round = 0; round < 4; ++round)
            {
                for(int playerId = 0; playerId < 4; ++playerId)
                {
                    for(int i = 0; i < 4; ++i)
                    {
                        tileWall.DealTile(players[playerId]);
                    }
                }
            }
        }

        void DecideDealer()
        {

            for(int i = 0; i < players.Count; ++i)
            {
                int j = Random.Range(0, players.Count);
                Player tmp = players[i];
                players[i] = players[j];
                players[j] = tmp;
            }
            activePlayerId = 0;
        }

        

        
        public void NextPlayer() 
        {
            activePlayerId = (activePlayerId + 1) % 4;
        }

        public void TurnToPlayer(string playerId)
        {
            activePlayerId = IndexOF(players, playerId);
        }

        void SetButton(GameObject btn, bool isInteractable)
        {
            btn.GetComponent<Button>().interactable = isInteractable;
        }


    }
}