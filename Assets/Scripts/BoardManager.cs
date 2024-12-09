using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;


public class BoardManager : MonoBehaviour
{
    public class CellData
    {
        public bool Passable;
        public CellObject ContainedObject;
        
    }
    private List<Vector2Int> m_EmptyCellsList;
    public FoodObject FoodPrefab;
    public FoodObject FoodPrefab2;
    public WallObject Enemy;
    private CellData[,] m_BoardData;
    private Tilemap m_Tilemap;
    private Grid m_Grid;
    public WallObject WallPrefab;
    public ExitCellObject ExitCellPrefab;
    public int Width;
    public int Height;
    public Tile[] GroundTiles;
    public Tile[] BlockingTiles;
    public Enemy[] EnemyPreFabs;
    public int MinEnemyCount = 3;
    public int MaxEnemyCount = 5;

    public void Init()
    {
        m_Tilemap = GetComponentInChildren<Tilemap>();
        m_Grid = GetComponentInChildren<Grid>();
        //Initialize the list
        m_EmptyCellsList = new List<Vector2Int>();

        m_BoardData = new CellData[Width, Height];


        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                Tile tile;
                m_BoardData[x, y] = new CellData();

                if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                {
                    tile = BlockingTiles[Random.Range(0, BlockingTiles.Length)];
                    m_BoardData[x, y].Passable = false;
                }
                else
                {
                    tile = GroundTiles[Random.Range(0, GroundTiles.Length)];
                    m_BoardData[x, y].Passable = true;

                    //this is a passable empty cell, add it to the list!
                    m_EmptyCellsList.Add(new Vector2Int(x, y));
                }

                m_Tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
        m_EmptyCellsList.Remove(new Vector2Int(1, 1));

        Vector2Int endCoord = new Vector2Int(Width - 2, Height - 2);
        AddObject(Instantiate(ExitCellPrefab), endCoord);
        m_EmptyCellsList.Remove(endCoord);

        GenerateEnemy();
        GenerateWall();
        GenerateFood();
        GenerateFood2();
        SpawnEnemies();


    }
    void AddObject(CellObject obj, Vector2Int coord)
    {
        CellData data = m_BoardData[coord.x, coord.y];
        obj.transform.position = CellToWorld(coord);
        data.ContainedObject = obj;
        obj.Init(coord);
    }
    void GenerateWall()
    {
        int wallCount = Random.Range(6, 20);
        for (int i = 0; i < wallCount; ++i)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);
            WallObject newWall = Instantiate(WallPrefab);
            AddObject(newWall, coord);
        }
    }
    private void SpawnEnemies()
    {
        // Rastgele düþman sayýsýný belirle
        int enemyCount = Random.Range(MinEnemyCount, MaxEnemyCount + 1);

        for (int i = 0; i < enemyCount; i++)
        {
            if (m_EmptyCellsList.Count == 0) break;

            // Rastgele bir hücre seç
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int randomCell = m_EmptyCellsList[randomIndex];

            // Hücreyi kullanýldý olarak iþaretle
            m_EmptyCellsList.RemoveAt(randomIndex);

            // Düþmaný oluþtur
            Enemy selectedEnemyPrefab = EnemyPreFabs[Random.Range(0, EnemyPreFabs.Length)];
            GameObject enemy = Instantiate(selectedEnemyPrefab.gameObject, CellToWorld(randomCell), Quaternion.identity);
            enemy.GetComponent<Enemy>().Init(randomCell);


            // Enemy script'ini baþlat
            enemy.GetComponent<Enemy>().Init(randomCell);
        }
    }
    public Tile GetCellTile(Vector2Int cellIndex)
    {
        return m_Tilemap.GetTile<Tile>(new Vector3Int(cellIndex.x, cellIndex.y, 0));
    }
    public void SetCellTile(Vector2Int cellIndex, Tile tile)
    {
        m_Tilemap.SetTile(new Vector3Int(cellIndex.x, cellIndex.y, 0), tile);
    }

    public void Clean()
    {
        //no board data, so exit early, nothing to clean
        if (m_BoardData == null)
            return;


        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                var cellData = m_BoardData[x, y];

                if (cellData.ContainedObject != null)
                {
                    //CAREFUL! Destroy the GameObject NOT just cellData.ContainedObject
                    //Otherwise what you are destroying is the JUST CellObject COMPONENT
                    //and not the whole gameobject with sprite
                    Destroy(cellData.ContainedObject.gameObject);
                }

                SetCellTile(new Vector2Int(x, y), null);
            }
        }
    }
    void GenerateFood2()
    {
        int foodCount = 5;
        for (int i = 0; i < foodCount; ++i)
        {
            int randomIndex2 = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord2 = m_EmptyCellsList[randomIndex2];

            m_EmptyCellsList.RemoveAt(randomIndex2);
            FoodObject newFood = Instantiate(FoodPrefab2);
            AddObject(newFood, coord2);
        }
    }
    void GenerateFood()
    {
        int foodCount = 5;
        for (int i = 0; i < foodCount; ++i)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);
            FoodObject newFood = Instantiate(FoodPrefab);
            AddObject(newFood, coord);
        }
    }
    void GenerateEnemy()
    {
        int wallCount = Random.Range(6, 20);
        for (int i = 0; i < wallCount; ++i)
        {
            int randomIndex3 = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord2 = m_EmptyCellsList[randomIndex3];

            m_EmptyCellsList.RemoveAt(randomIndex3);
            WallObject newWall = Instantiate(WallPrefab);
            AddObject(newWall, coord2);
        }
    }
    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return m_Grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }

    public CellData GetCellData(Vector2Int cellIndex)
    {
        if (cellIndex.x < 0 || cellIndex.x >= Width
            || cellIndex.y < 0 || cellIndex.y >= Height)
        {
            return null;
        }

        return m_BoardData[cellIndex.x, cellIndex.y];
    }

}
