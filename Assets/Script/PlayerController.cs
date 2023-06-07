using System;
using Fusion;
using Game.Play;

namespace Game.Core
{
    public class PlayerController : NetworkBehavior
    {
        private GameManager gameManager = null;
        private TableManager tableManager = null;

        [Network] public Guid PlayerID { get; set; }
        [Network] public int PlayerOrder { get; set; }
        [Network] public string PlayerName { get; set; }
        [Network] public string LastTileID { get; set; }

        public override void Spawned()
        {
            gameManager = GameManager.Instance;

            tableManager = GameObject.Find("Canvas").GetComponent<TableManager>();

            if (Object.HasInputAuthority)
            {
                PlayerID = playerNetworkData.PlayerID;
                PlayerName = PlayerNetworkData.PlayerName;
            }
        }

        public override void Despawned()
        {

        }

        #region - rpc

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
        public void SetPlayerOrder_RPC(int playerOrder)
        {
            PlayerOrder = playerOrder;
        }

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
        public void SetLastTile_PRC(string lastTileID)
        {
            LastTileID = lastTileID;
        }

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
        public void DealTiles_RPC(string tileId)
        {
            // TODO: find tile object by id and assign to player
        }

        #endregion

        #region - callback

        public static void OnLastTileChange()
        {
            tableManager.BeforeNextPlayer();
        }

        #endregion
    }
}
