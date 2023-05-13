using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TableManager : MonoBehaviour
{
    public GameObject mahjongPrefab; // 麻將Prefab
    public Vector3 startPosition; // 生成起始位置
    public float spacing = 2.0f; // 麻將間距
    public Transform parent;
    List<GameObject> tiles = new List<GameObject>();
    void Start() 
    {
        // GenerateAllMahjong();
        GenerateAllTile();

        // Shuffle();
    }

    void SetTileFace(GameObject mahjong)
    {
        int cardFace_index = mahjong.GetComponent<Tile>().cardFace_index;
        GameObject face = mahjong.transform.Find("Face").gameObject;
        Image faceImage = face.GetComponent<Image>();
        Sprite img = Resources.Load<Sprite>("Image/Mahjong/" + cardFace_index.ToString());
        if (img) 
        {
            faceImage.sprite = img;
            // Debug.Log("成功設定圖像" + "Image/Mahjong/" + cardFace_index.ToString());
        }
        else
        {
            Debug.Log("無法設定圖像" + "Image/Mahjong/" + cardFace_index.ToString());
        }
        
    }

    void GenerateTileId(Tile tile, int cnt)
    {
        tile.id = tile.tile_type.ToString() + "_" + 
                    tile.tile_number.ToString() + "_" +
                    cnt.ToString();
    }

    void GenerateAllTile()
    {
        int cardFace_index = 1;
        // Season       1-4
        for (int i = 1; i <= 4; ++i)
        {

            GameObject tile_obj = Instantiate(mahjongPrefab, startPosition + new Vector3(i * spacing, 0, 0), Quaternion.identity, parent);
            Tile tile =  tile_obj.GetComponent<Tile>();
            if(tile==null)
            {
                Debug.LogError("沒有 tile_script");
            }
            tile.tile_type = TileType.Season;
            tile.tile_number = i;
            tile.cardFace_index = cardFace_index;
            GenerateTileId(tile, 1);
            Debug.Log(tile.id);
            tiles.Add(tile_obj);
            tile_obj.name = tile.id;
            SetTileFace(tile_obj);
            ++cardFace_index;
            
        }
        // Flower       5-8
        for (int i = 1; i <= 4; ++i)
        {

            GameObject tile_obj = Instantiate(mahjongPrefab, startPosition + new Vector3(i * spacing, 0, 0), Quaternion.identity, parent);
            Tile tile =  tile_obj.GetComponent<Tile>();
            if(tile==null)
            {
                Debug.LogError("沒有 tile_script");
            }
            tile.tile_type = TileType.Flower;
            tile.tile_number = i;
            tile.cardFace_index = cardFace_index;
            GenerateTileId(tile, 1);
            Debug.Log(tile.id);
            tiles.Add(tile_obj);
            tile_obj.name = tile.id;
            SetTileFace(tile_obj);
            ++cardFace_index;
            
        }
        // Wind         9-12 
        for (int i = 1; i <= 4; ++i)
        {
            for (int j = 1; j <= 4; ++j)
            {
                GameObject tile_obj = Instantiate(mahjongPrefab, startPosition + new Vector3(i * spacing, 0, 0), Quaternion.identity, parent);
                Tile tile =  tile_obj.GetComponent<Tile>();
                if(tile==null)
                {
                    Debug.LogError("沒有 tile_script");
                }
                tile.tile_type = TileType.Wind;
                tile.tile_number = i;
                tile.cardFace_index = cardFace_index;
                GenerateTileId(tile, j);
                Debug.Log(tile.id);
                tiles.Add(tile_obj);
                tile_obj.name = tile.id;
                SetTileFace(tile_obj);
            }
            ++cardFace_index; 
        }
        // Dragon       13-15
        for (int i = 1; i <= 3; ++i)
        {
            for (int j = 1; j <= 4; ++j)
            {
                GameObject tile_obj = Instantiate(mahjongPrefab, startPosition + new Vector3(i * spacing, 0, 0), Quaternion.identity, parent);
                Tile tile =  tile_obj.GetComponent<Tile>();
                if(tile==null)
                {
                    Debug.LogError("沒有 tile_script");
                }
                tile.tile_type = TileType.Dragon;
                tile.tile_number = i;
                tile.cardFace_index = cardFace_index;
                GenerateTileId(tile, j);
                Debug.Log(tile.id);
                tiles.Add(tile_obj);
                tile_obj.name = tile.id;
                SetTileFace(tile_obj);
            }
            ++cardFace_index; 
        }
        // Character    16-24
        for (int i = 1; i <= 9; ++i)
        {
            for (int j = 1; j <= 4; ++j)
            {
                GameObject tile_obj = Instantiate(mahjongPrefab, startPosition + new Vector3(i * spacing, 0, 0), Quaternion.identity, parent);
                Tile tile =  tile_obj.GetComponent<Tile>();
                if(tile==null)
                {
                    Debug.LogError("沒有 tile_script");
                }
                tile.tile_type = TileType.Character;
                tile.tile_number = i;
                tile.cardFace_index = cardFace_index;
                GenerateTileId(tile, j);
                Debug.Log(tile.id);
                tiles.Add(tile_obj);
                tile_obj.name = tile.id;
                SetTileFace(tile_obj);
            }
            ++cardFace_index; 
        }
        // Bamboo       25-33
        for (int i = 1; i <= 9; ++i)
        {
            for (int j = 1; j <= 4; ++j)
            {
                GameObject tile_obj = Instantiate(mahjongPrefab, startPosition + new Vector3(i * spacing, 0, 0), Quaternion.identity, parent);
                Tile tile =  tile_obj.GetComponent<Tile>();
                if(tile==null)
                {
                    Debug.LogError("沒有 tile_script");
                }
                tile.tile_type = TileType.Bamboo;
                tile.tile_number = i;
                tile.cardFace_index = cardFace_index;
                GenerateTileId(tile, j);
                Debug.Log(tile.id);
                tiles.Add(tile_obj);
                tile_obj.name = tile.id;
                SetTileFace(tile_obj);
            }
            ++cardFace_index; 
        }
        // Dot          34-42
        for (int i = 1; i <= 9; ++i)
        {
            for (int j = 1; j <= 4; ++j)
            {
                GameObject tile_obj = Instantiate(mahjongPrefab, startPosition + new Vector3(i * spacing, 0, 0), Quaternion.identity, parent);
                Tile tile =  tile_obj.GetComponent<Tile>();
                if(tile==null)
                {
                    Debug.LogError("沒有 tile_script");
                }
                tile.tile_type = TileType.Dot;
                tile.tile_number = i;
                tile.cardFace_index = cardFace_index;
                GenerateTileId(tile, j);
                Debug.Log(tile.id);
                tiles.Add(tile_obj);
                tile_obj.name = tile.id;
                SetTileFace(tile_obj);
            }
            ++cardFace_index; 
        }
    }
}