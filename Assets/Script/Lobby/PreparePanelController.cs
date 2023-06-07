using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Core;

namespace Game.Lobby
{
    public enum EnumPanel
    {
        Start,
        StartSetting,
        Lobby,
        RoomList,
        RoomCreating,
        Waiting,
        WaitingSetting
    }

    public class PreparePanelController
    {
        private Dictionary<EnumPanel, GameObject> panelDict = new Dictionary<EnumPanel, GameObject>();

        public void AddPanel(EnumPanel enumPanel, GameObject panel)
        {
            Debug.Log($"add panel: {enumPanel}");
            panelDict.TryAdd(enumPanel, panel);
        }

        public void ClearPanel()
        {
            panelDict.Clear();
        }

        public void OpenPanel(EnumPanel enumPanel)
        {
            foreach (var content in panelDict)
                content.Value.SetActive(false);

            switch (enumPanel)
            {
                case EnumPanel.Start:
                    try
                    {
                        panelDict[EnumPanel.Start].SetActive(true);
                    }
                    catch (ArgumentException error)
                    {
                        Debug.Log(error);
                    }
                    break;
                case EnumPanel.StartSetting:
                    try
                    {
                        // panelDict[EnumPanel.Start].SetActive(true);
                        panelDict[EnumPanel.StartSetting].SetActive(true);
                    }
                    catch (ArgumentException error)
                    {
                        Debug.Log(error);
                    }
                    break;
                case EnumPanel.Lobby:
                    try
                    {
                        panelDict[EnumPanel.Lobby].SetActive(true);
                    }
                    catch (ArgumentException error)
                    {
                        Debug.Log(error);
                    }
                    break;
                case EnumPanel.RoomList:
                    try
                    {
                        panelDict[EnumPanel.Lobby].SetActive(true);
                        panelDict[EnumPanel.RoomList].SetActive(true);
                    }
                    catch (ArgumentException error)
                    {
                        Debug.Log(error);
                    }
                    break;
                case EnumPanel.RoomCreating:
                    try
                    {
                        panelDict[EnumPanel.Lobby].SetActive(true);
                        panelDict[EnumPanel.RoomCreating].SetActive(true);
                    }
                    catch (ArgumentException error)
                    {
                        Debug.Log(error);
                    }
                    break;
                case EnumPanel.Waiting:
                    try
                    {
                        panelDict[EnumPanel.Waiting].SetActive(true);
                        // if current user is host, it will show game setting panel
                        if (GameManager.Instance.Runner.IsServer)
                            panelDict[EnumPanel.WaitingSetting].SetActive(true);
                    }
                    catch (ArgumentException error)
                    {
                        Debug.Log(error);
                    }
                    break;
            }
        }
    }
}
