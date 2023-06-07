using Fusion;

namespace Game.Core
{
    public class PlayerNetworkData : NetworkBehaviour
    {
        private GameManager gameManager = null;

        [Networked] public int UserScore { get; set; } = 0;
        [Networked] public string PlayerId { get; set; }
        [Networked] public string PlayerName { get; set; }

        [Networked(OnChanged = nameof(OnIsReadyChanged))] public NetworkBool IsReady { get; set; }

        public override void Spawned()
        {
            gameManager = GameManager.Instance;

            transform.SetParent(GameManager.Instance.transform);

            gameManager.PlayerList.Add(Object.InputAuthority, this);
            gameManager.UpdatePlayerList();

            if (Object.HasInputAuthority)
            {
                SetPlayerId_RPC(gameManager.PlayerId);
                UpdateScore_RPC(gameManager.UserScore);
                SetPlayerName_RPC(gameManager.PlayerName);
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            gameManager.PlayerList.Remove(Object.InputAuthority);
            gameManager.UpdatePlayerList();
        }

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
        public void SetPlayerId_RPC(string playerId)
        {
            PlayerId = playerId;
        }

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
        public void SetPlayerName_RPC(string playerName)
        {
            PlayerName = playerName;
        }

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
        public void UpdateScore_RPC(int score)
        {
            UserScore += score;
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
