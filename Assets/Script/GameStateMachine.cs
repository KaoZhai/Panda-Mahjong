using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.PlayingRoom {
    public enum GameState 
    {
        Default,
        BuPai,
        Draw,
        Hu,
        ZiMo,
        Chi,
        Pong,
        MingGang,
        AnGang,
        JiaGang,
        PlayedTile,
        NextPlayerRound,
        OppositePlayerRound,
        LastPlayerRound,
    }
    public class StateMachine : MonoBehaviour
    {
        private GameState currentState; // 當前狀態

        public StateMachine()
        {
            currentState = GameState.Default;
        }
        public void Update()
        {
            switch (currentState)
            {
                case GameState.Default:
                    break;
                case GameState.BuPai:
                    break;
                case GameState.Draw:
                    break;
                case GameState.Hu:
                    break;
                case GameState.ZiMo:
                    break;
                case GameState.Chi:
                    break;
                case GameState.Pong:
                    break;
                case GameState.MingGang:
                    break;
                case GameState.AnGang:
                    break;
                case GameState.JiaGang:
                    break;
                case GameState.PlayedTile:
                    break;
                case GameState.NextPlayerRound:
                    break;
                case GameState.OppositePlayerRound:
                    break;
                case GameState.LastPlayerRound:
                    break;
            }
        }

        public void ChangeState(GameState newState)
        {
            currentState = newState;
        }

    }
}