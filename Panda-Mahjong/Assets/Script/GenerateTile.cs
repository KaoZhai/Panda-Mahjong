using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        int cardIndex = 1;
        for (int i = 0; i < 42; i++)
        {
            GameObject mahjong = Instantiate(mahjongPrefab, startPosition + new Vector3(i * spacing, 0, 0), Quaternion.identity, parent);
            GameObject face = mahjong.transform.Find("Face").gameObject;
            Image faceImage = face.GetComponent<Image>();
            
            Sprite img = Resources.Load<Sprite>("Image/Mahjong/" + cardIndex.ToString());
            if (img) 
            {
                faceImage.sprite = img;
                Debug.Log("成功設定圖像" + "Image/Mahjong/" + cardIndex.ToString());
            }
            else
            {
                Debug.Log("無法設定圖像" + "Image/Mahjong/" + cardIndex.ToString());
            }
            mahjong.name = string.Format("Mahjong_{0}", i + 1);
            ++cardIndex;
        }
    }
}
