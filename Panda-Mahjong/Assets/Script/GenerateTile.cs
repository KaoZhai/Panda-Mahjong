using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class GenerateTile : MonoBehaviour
{
    public GameObject mahjongPrefab; // 麻將Prefab
    public Vector3 startPosition; // 生成起始位置
    public float spacing = 2.0f; // 麻將間距
    public Transform parent;

    void Start()
    {
        GenerateMahjong();
    }

    void GenerateMahjong()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 1; j <= 9; j++)
            {
                GameObject mahjong = Instantiate(mahjongPrefab, startPosition + new Vector3(j * spacing, 0, i * spacing), Quaternion.identity, parent);
                mahjong.name = string.Format("Mahjong_{0}_{1}", i + 1, j);
            }
        }
    }
}
