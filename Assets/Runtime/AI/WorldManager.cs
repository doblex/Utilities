using System.Collections.Generic;
using UnityEngine;
using utilities.general.attributes;

namespace utilities.AI
{ 
    public class WorldManager : MonoBehaviour
    {
        public static WorldManager Instance { get; private set; }

        [SerializeField, Layer] protected int layer;

        [Header("PathFind options")]
        [SerializeField] int pathFindTries = 10;

        [Header("Obstacles")]
        [SerializeField] bool _DrawObstacles;
        [SerializeField] GameObject _GridPointPrefab;

        [Header("Dimensions")]
        [SerializeField] int GridWidth;
        [SerializeField] int GridHeight;
        [SerializeField] int GridLength;
    
        public Point[][][] Grid;

        [Header("Sizes")]
        public float PointDistance;
        public float PointSize;
        public float InvalidPointSize;
    
        Vector3 startPoint;
    
        float startTime;
    
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            InitializeGrid();
        }

        private void OnValidate()
        {
            layer = gameObject.layer;
        }

        private void AddNeighbour(Point p, Vector3Int neighbour)
        {
            if (neighbour.x > -1 && neighbour.x < GridWidth &&
               neighbour.y > -1 && neighbour.y < GridHeight &&
               neighbour.z > -1 && neighbour.z < GridLength)
            {
                p.Neighbours.Add(neighbour);
            }
        }
    
        private void InitializeGrid()
        {
            startPoint = new Vector3(-GridWidth, -GridHeight, -GridLength) / 2f * PointDistance + transform.position;
            GameObject gridParent = new GameObject("Grid");
            Grid = new Point[GridWidth][][];
    
            // Giro tutta la griglia
            for (int i = 0; i < GridWidth; i++)
            {
                Grid[i] = new Point[GridHeight][];
                for (int j = 0; j < GridHeight; j++)
                {
                    Grid[i][j] = new Point[GridLength];
                    for (int k = 0; k < GridLength; k++)
                    {
                        // mi creo il Point in base alle coordinate della griglia
                        Vector3 pos = startPoint + new Vector3(i, j, k) * PointDistance;
                        Grid[i][j][k] = new Point();
                        Grid[i][j][k].Coords = new Vector3Int(i, j, k);
                        Grid[i][j][k].WorldPosition = pos;

                        // check se c'� un ostacolo
                        LayerMask obstacleLayerMask = 1 << layer;

                        Collider[] colliders = Physics.OverlapBox(Grid[i][j][k].WorldPosition, Vector3.one * PointDistance / 2f, Quaternion.identity, obstacleLayerMask);

                        if (colliders.Length > 0)
                        {
                            Grid[i][j][k].Invalid = true;
    
                            if (_DrawObstacles)
                                Instantiate(_GridPointPrefab, Grid[i][j][k].WorldPosition, Quaternion.identity, gridParent.transform).transform.localScale = Vector3.one * InvalidPointSize;
                        }
    
                        // aggiungo i vicini al punto
                        for (int p = -1; p <= 1; p++)
                        {
                            for (int q = -1; q <= 1; q++)
                            {
                                for (int g = -1; g <= 1; g++)
                                {
                                    if (i == p && g == q && k == g)
                                    {
                                        continue;
                                    }
                                    AddNeighbour(Grid[i][j][k], new Vector3Int(i + p, j + q, k + g));
                                }
                            }
                        }
                    }
                }
            }
        }
    
        private void OnDrawGizmos()
        {
            // disegno i quadrato della griglia
            Gizmos.DrawWireCube(transform.position, new Vector3(GridWidth, GridHeight, GridLength) * PointDistance);
        }
    
        public Point GetClosestPointWorldSpace(Vector3 position)
        {
            float sizeX = PointDistance * GridWidth;
            float sizeY = PointDistance * GridHeight;
            float sizeZ = PointDistance * GridLength;
    
            Vector3 pos = position - startPoint;
            float percentageX = Mathf.Clamp01(pos.x / sizeX);
            float percentageY = Mathf.Clamp01(pos.y / sizeY);
            float percentageZ = Mathf.Clamp01(pos.z / sizeZ);
            int x = Mathf.Clamp(Mathf.RoundToInt(percentageX * GridWidth), 0, GridWidth - 1);
            int y = Mathf.Clamp(Mathf.RoundToInt(percentageY * GridHeight), 0, GridHeight - 1);
            int z = Mathf.Clamp(Mathf.RoundToInt(percentageZ * GridLength), 0, GridLength - 1);
            Point result = Grid[x][y][z];

            int tries = 0;

            while (result.Invalid)
            {
                if (tries > pathFindTries)
                {
                    Debug.LogError("No valid point found");
                    return null;
                }
                tries++;
                int step = 1;
                List<Point> freePoints = new List<Point>();
                for (int p = -step; p <= step; p++)
                {
                    for (int q = -step; q <= step; q++)
                    {
                        for (int g = -step; g <= step; g++)
                        {
                            if (x == p && y == q && z == g)
                            {
                                continue;
                            }
                            int i = x + p;
                            int j = y + q;
                            int k = z + g;
                            if (i > -1 && i < GridWidth &&
                                j > -1 && j < GridHeight &&
                                k > -1 && k < GridLength)
                            {
                                if (!Grid[x + p][y + q][z + g].Invalid)
                                {
                                    freePoints.Add(Grid[x + p][y + q][z + g]);
                                }
                            }
                        }
                    }
                }
                float distance = Mathf.Infinity;
                for (int i = 0; i < freePoints.Count; i++)
                {
                    float dist = (freePoints[i].WorldPosition - position).sqrMagnitude;
                    if (dist < distance)
                    {
                        result = freePoints[i];
                        dist = distance;
                    }
                }
            }
            return result;
        }
    
        public List<Point> GetFreePoints()
        {
            List<Point> freePoints = new List<Point>();
            for (int i = 0; i < Grid.Length; i++)
            {
                for (int j = 0; j < Grid[i].Length; j++)
                {
                    for (int k = 0; k < Grid[i][j].Length; k++)
                    {
                        if (!Grid[i][j][k].Invalid)
                        {
                            freePoints.Add(Grid[i][j][k]);
                        }
                    }
                }
            }
            return freePoints;
        }
    }
}
