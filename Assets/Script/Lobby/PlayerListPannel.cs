using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Core;

namespace Game.Lobby
{
    public class PlayerListPannel : MonoBehaviour
    {
        private GameManager gameManager = null;
        private List<PlayerUnit> playerUnits = new List<PlayerUnit>();

        [SerializeField] private Transform contentTrans = null;
        [SerializeField] private PlayerUnit playerUnitPrefab = null;

        public void Start()
        {
            gameManager = GameManager.Instance;
        }

        public void FixedUpdate()
        {
            // not require high frequency to update
            UpdatePlayerList();
        }

        public void UpdatePlayerList()
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

                unit.SetInfo(playerData.PlayerName, playerData.IsReady);
                playerUnits.Add(unit);
            }
        }
    }
}
