using System.Collections.Generic;
using UnityEngine;
using Game.Core;

namespace Game.Lobby
{
    public class PlayerListPanel : MonoBehaviour
    {
        private GameManager gameManager;
        private List<PlayerUnit> playerUnits = new();

        [SerializeField] private Transform contentTrans;
        [SerializeField] private PlayerUnit playerUnitPrefab;

        public void Start()
        {
            gameManager = GameManager.Instance;
        }

        public void FixedUpdate()
        {
            // not require high frequency to update
            UpdatePlayerList();
        }

        private void UpdatePlayerList()
        {
            foreach (var unit in playerUnits)
            {
                Destroy(unit.gameObject);
            }

            playerUnits.Clear();

            foreach (var player in gameManager.PlayerList)
            {
                var unit = Instantiate(playerUnitPrefab, contentTrans);
                var playerData = player.Value;

                unit.SetInfo(playerData.PlayerName, playerData.IsReady, playerData.UserScore);
                playerUnits.Add(unit);
            }
        }
        
    }
}
