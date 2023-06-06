using Fusion;

namespace Game.Core
{
    public class PlayerNetworkData : NetworkBehaviour
    {
        private GameManager gameManager;

        [Networked] public string PlayerID { get; set; }
        [Networked] public string PlayerName { get; private set; }
        [Networked(OnChanged = nameof(OnIsReadyChanged))] public NetworkBool IsReady { get; set; }

        public override void Spawned()
        {
            gameManager = GameManager.Instance;

            transform.SetParent(GameManager.Instance.transform);

            gameManager.PlayerList.Add(Object.InputAuthority, this);
            gameManager.UpdatePlayerList();

            if (Object.HasInputAuthority)
            {
                PlayerID = gameManager.PlayerID;
                PlayerName = gameManager.PlayerName;
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            gameManager.PlayerList.Remove(Object.InputAuthority);
            gameManager.UpdatePlayerList();
        }

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
        public void SetReady_RPC(bool isReady)
        {
            IsReady = isReady;
        }

        private static void OnIsReadyChanged(Changed<PlayerNetworkData> changed)
        {
            GameManager.Instance.UpdatePlayerList();
        }
    }
}
