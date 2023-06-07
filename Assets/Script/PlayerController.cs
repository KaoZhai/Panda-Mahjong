using Fusion;
using Game.Play;

namespace Game.Core
{
    public class PlayerController : NetworkBehaviour
    {
        private GameManager gameManager;

        [Networked] public int PlayerOrder { get; set; }
        [Networked] public string PlayerID { get; set; }
        [Networked] public string PlayerName { get; set; }
        [Networked] public string LastTileID { get; set; }

        public override void Spawned()
        {
            gameManager = GameManager.Instance;

            // tableManager = gameManager.gameObject.Find("Canvas").GetComponent<TableManager>();

            if (Object.HasInputAuthority)
            {
                PlayerID = gameManager.PlayerID;
                PlayerName = gameManager.PlayerName;
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {

        }

        #region - rpc

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
        public void SetPlayerOrder_RPC(int playerOrder)
        {
            PlayerOrder = playerOrder;
        }

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
        public void SetLastTile_RPC(string lastTileID)
        {
            LastTileID = lastTileID;
        }

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
        public void DealTiles_RPC(string tileId)
        {
            gameManager.TableManager.AssignTileToPlayer(PlayerID, tileId);
        }

        #endregion

        #region - callback

        public void OnOrderChange()
        {
            gameManager.TableManager.SetPlayerOrder(PlayerID, PlayerOrder);
        }

        public void OnLastTileChange()
        {
            // tableManager.BeforeNextPlayer();
        }

        #endregion
    }
}
