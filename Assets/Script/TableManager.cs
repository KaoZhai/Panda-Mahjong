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
    public Transform transform_tileWall;
    public Transform transform_localPlayer;
    public List<Transform> transform_hand = new List<Transform>();
    public Transform underHand;
    public Transform underTilePoll;
    List<int> players = new List<int>();
    List<GameObject> tile_wall = new List<GameObject>();
    List<List<GameObject>> players_tiles = new List<List<GameObject>>();
    int localPlayerId = 1;
    void Start() 
    {
        // GenerateAllMahjong();
        GenerateAllTile();
        Shuffle();
        PickSeatsAndDecideDealer();
        DealTiles();
    }


    void DealTiles()
    {
        for(int i = 0; i < 4; ++i)
        {
            players_tiles.Add(new List<GameObject>());
        }
        for(int k = 0; k < 4; ++k)
        {
            for(int i = 0; i < 4; ++i)
            {
                for(int j = 0; j < 4; ++j)
                {
                    players_tiles[i].Add(tile_wall[0]);
                    tile_wall.RemoveAt(0);
                }
            }
        }
        for(int i = 0; i < 4; ++i)
        {
            Debug.Log("玩家" + i.ToString());
            for(int j = 0; j < players_tiles[i].Count; ++j)
            {
                Debug.Log(players_tiles[i][j]);
                players_tiles[i][j].GetComponent<Game.Tile.Tile>().SetPlayerId(i);
                if(i == localPlayerId)
                {
                    players_tiles[i][j].transform.position = startPosition + new Vector3(j * spacing, 0, 0);
                    players_tiles[i][j].transform.parent = transform_localPlayer;
                    players_tiles[i][j].SetActive(true);
                }
            }
        }
    }

    void PickSeatsAndDecideDealer()
    {
        for(int i = 0; i < 4; ++i)
        {
            players.Add(i);
        }
        for(int i = 0; i < players.Count; ++i)
        {
            int j = Random.Range(0, players.Count);
            int tmp = players[i];
            players[i] = players[j];
            players[j] = tmp;
        }
    }

    void SetTileFace(GameObject mahjong)
    {
        int cardFaceIndex = mahjong.GetComponent<Game.Tile.Tile>().cardFaceIndex;
        GameObject face = mahjong.transform.Find("Face").gameObject;
        Image faceImage = face.GetComponent<Image>();
        Sprite img = Resources.Load<Sprite>("Image/Mahjong/" + cardFaceIndex.ToString());
        if (img) 
        {
            faceImage.sprite = img;
            // Debug.Log("成功設定圖像" + "Image/Mahjong/" + cardFaceIndex.ToString());
        }
        else
        {
            Debug.Log("無法設定圖像" + "Image/Mahjong/" + cardFaceIndex.ToString());
        }
        
    }

    void GenerateTileId(Game.Tile.Tile tile, int cnt)
    {
        tile.id = tile.tileType.ToString() + "_" + 
                    tile.tileNumber.ToString() + "_" +
                    cnt.ToString();
    }

    void GenerateAllTile()
    {
        int cardFaceIndex = 1;
        // Season       1-4
        for (int i = 1; i <= 4; ++i)
        {

            GameObject tile_obj = Instantiate(mahjongPrefab, startPosition + new Vector3(i * spacing, 0, 0), Quaternion.identity, transform_tileWall);
            Game.Tile.Tile tile =  tile_obj.GetComponent<Game.Tile.Tile>();
            if(tile==null)
            {
                Debug.LogError("沒有 tile_script");
            }
            tile.tileType = Game.Tile.TileType.Season;
            tile.tileNumber = i;
            tile.cardFaceIndex = cardFaceIndex;
            tile.SetTableManager(this);
            tile.SetPlayerId(-1);
            tile.SetTilePool(underTilePoll);
            tile.SetHand(underHand);
            GenerateTileId(tile, 1);
            Debug.Log(tile.id);
            tile_wall.Add(tile_obj);
            tile_obj.name = tile.id;
            SetTileFace(tile_obj);
            ++cardFaceIndex;
            
        }
        // Flower       5-8
        for (int i = 1; i <= 4; ++i)
        {

            GameObject tile_obj = Instantiate(mahjongPrefab, startPosition + new Vector3(i * spacing, 0, 0), Quaternion.identity, transform_tileWall);
            Game.Tile.Tile tile =  tile_obj.GetComponent<Game.Tile.Tile>();
            if(tile==null)
            {
                Debug.LogError("沒有 tile_script");
            }
            tile.tileType = Game.Tile.TileType.Flower;
            tile.tileNumber = i;
            tile.cardFaceIndex = cardFaceIndex;
            tile.SetTableManager(this);
            tile.SetPlayerId(-1);
            tile.SetTilePool(underTilePoll);
            tile.SetHand(underHand);
            GenerateTileId(tile, 1);
            Debug.Log(tile.id);
            tile_wall.Add(tile_obj);
            tile_obj.name = tile.id;
            SetTileFace(tile_obj);
            ++cardFaceIndex;
            
        }
        // Wind         9-12 
        for (int i = 1; i <= 4; ++i)
        {
            for (int j = 1; j <= 4; ++j)
            {
                GameObject tile_obj = Instantiate(mahjongPrefab, startPosition + new Vector3(i * spacing, 0, 0), Quaternion.identity, transform_tileWall);
                Game.Tile.Tile tile =  tile_obj.GetComponent<Game.Tile.Tile>();
                if(tile==null)
                {
                    Debug.LogError("沒有 tile_script");
                }
                tile.tileType = Game.Tile.TileType.Wind;
                tile.tileNumber = i;
                tile.cardFaceIndex = cardFaceIndex;
                tile.SetTableManager(this);
                tile.SetPlayerId(-1);
                tile.SetTilePool(underTilePoll);
                tile.SetHand(underHand);
                GenerateTileId(tile, j);
                Debug.Log(tile.id);
                tile_wall.Add(tile_obj);
                tile_obj.name = tile.id;
                SetTileFace(tile_obj);
            }
            ++cardFaceIndex; 
        }
        // Dragon       13-15
        for (int i = 1; i <= 3; ++i)
        {
            for (int j = 1; j <= 4; ++j)
            {
                GameObject tile_obj = Instantiate(mahjongPrefab, startPosition + new Vector3(i * spacing, 0, 0), Quaternion.identity, transform_tileWall);
                Game.Tile.Tile tile =  tile_obj.GetComponent<Game.Tile.Tile>();
                if(tile==null)
                {
                    Debug.LogError("沒有 tile_script");
                }
                tile.tileType = Game.Tile.TileType.Dragon;
                tile.tileNumber = i;
                tile.cardFaceIndex = cardFaceIndex;
                tile.SetTableManager(this);
                tile.SetPlayerId(-1);
                tile.SetTilePool(underTilePoll);
                tile.SetHand(underHand);
                GenerateTileId(tile, j);
                Debug.Log(tile.id);
                tile_wall.Add(tile_obj);
                tile_obj.name = tile.id;
                SetTileFace(tile_obj);
            }
            ++cardFaceIndex; 
        }
        // Character    16-24
        for (int i = 1; i <= 9; ++i)
        {
            for (int j = 1; j <= 4; ++j)
            {
                GameObject tile_obj = Instantiate(mahjongPrefab, startPosition + new Vector3(i * spacing, 0, 0), Quaternion.identity, transform_tileWall);
                Game.Tile.Tile tile =  tile_obj.GetComponent<Game.Tile.Tile>();
                if(tile==null)
                {
                    Debug.LogError("沒有 tile_script");
                }
                tile.tileType = Game.Tile.TileType.Character;
                tile.tileNumber = i;
                tile.cardFaceIndex = cardFaceIndex;
                tile.SetTableManager(this);
                tile.SetPlayerId(-1);
                tile.SetTilePool(underTilePoll);
                tile.SetHand(underHand);
                GenerateTileId(tile, j);
                Debug.Log(tile.id);
                tile_wall.Add(tile_obj);
                tile_obj.name = tile.id;
                SetTileFace(tile_obj);
            }
            ++cardFaceIndex; 
        }
        // Bamboo       25-33
        for (int i = 1; i <= 9; ++i)
        {
            for (int j = 1; j <= 4; ++j)
            {
                GameObject tile_obj = Instantiate(mahjongPrefab, startPosition + new Vector3(i * spacing, 0, 0), Quaternion.identity, transform_tileWall);
                Game.Tile.Tile tile =  tile_obj.GetComponent<Game.Tile.Tile>();
                if(tile==null)
                {
                    Debug.LogError("沒有 tile_script");
                }
                tile.tileType = Game.Tile.TileType.Bamboo;
                tile.tileNumber = i;
                tile.cardFaceIndex = cardFaceIndex;
                tile.SetTableManager(this);
                tile.SetPlayerId(-1);
                tile.SetTilePool(underTilePoll);
                tile.SetHand(underHand);
                GenerateTileId(tile, j);
                Debug.Log(tile.id);
                tile_wall.Add(tile_obj);
                tile_obj.name = tile.id;
                SetTileFace(tile_obj);
            }
            ++cardFaceIndex; 
        }
        // Dot          34-42
        for (int i = 1; i <= 9; ++i)
        {
            for (int j = 1; j <= 4; ++j)
            {
                GameObject tile_obj = Instantiate(mahjongPrefab, startPosition + new Vector3(i * spacing, 0, 0), Quaternion.identity, transform_tileWall);
                Game.Tile.Tile tile =  tile_obj.GetComponent<Game.Tile.Tile>();
                if(tile==null)
                {
                    Debug.LogError("沒有 tile_script");
                }
                tile.tileType = Game.Tile.TileType.Dot;
                tile.tileNumber = i;
                tile.cardFaceIndex = cardFaceIndex;
                tile.SetTableManager(this);
                tile.SetPlayerId(-1);
                tile.SetTilePool(underTilePoll);
                tile.SetHand(underHand);
                GenerateTileId(tile, j);
                Debug.Log(tile.id);
                tile_wall.Add(tile_obj);
                tile_obj.name = tile.id;
                SetTileFace(tile_obj);
            }
            ++cardFaceIndex; 
        }
    }

    void Shuffle()
    {
        for(int i = 0; i < tile_wall.Count; ++i)
        {
            int j = Random.Range(0, tile_wall.Count);
            GameObject tmp = tile_wall[i];
            tile_wall[i] = tile_wall[j];
            tile_wall[j] = tmp;
        }
        Debug.Log("洗牌後的牌牆");
        for(int i = 0; i < tile_wall.Count; ++i)
        {

            Debug.Log(tile_wall[i]);
        }
    }

    // todo: just public for demo, it should be private
    public void GetTileFromTileWall(int playerId) 
    {
        Debug.Log("NowTop: " + tile_wall[0]);
        // todo: just for demo, should not change the playerId
        playerId = localPlayerId;
        players_tiles[playerId].Add(tile_wall[0]);
        tile_wall[0].GetComponent<Game.Tile.Tile>().SetPlayerId(playerId);
        tile_wall[0].GetComponent<RectTransform>().SetParent(transform_hand[playerId]);
        tile_wall[0].SetActive(true);
        tile_wall.RemoveAt(0);
        Debug.Log("NextTop: " + tile_wall[0]);
        
    }

    public void NextPlayer()
    {
        localPlayerId = (localPlayerId + 1) % 4;
        // Debug.Log("after: " + localPlayerId);
    }

    public int GetLocalPlayerId()
    {
        return localPlayerId;
    }
}