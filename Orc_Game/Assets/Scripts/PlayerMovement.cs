using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Tilemaps;
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    public Tilemap ground;
    public float speed = 10;

    private Vector3 target;
    private Vector3 currentTileLocal;
    private List<Vector3> tLocal = new List<Vector3>();
    private List<Vector3> tGlobal = new List<Vector3>();

    public float movementTimer = 1f;
    private float startMovementTimer;

    //A* vars
    private List<(Vector3, int)> start;
    private class Node
    {
        public Node(Vector3 pos, Vector3 parent, float localCost, float globalCost, bool visited, List<Node> neighbors)
        {
            this.pos = pos;
            this.parent = parent;
            this.localCost = localCost;
            this.globalCost = globalCost;
            this.visited = visited;
            this.neighbors = neighbors;
        }

        public Vector3 pos {get; set; }
        public Vector3 parent {get; set; }
        public float localCost {get; set; }
        public float globalCost {get; set; }
        public bool visited {get; set; }
        public List<Node> neighbors {get; set; }
    }
    private List<Node> path = new List<Node>();


    private List<Node> graph = new List<Node>();
    private Dictionary<Vector3, Node> mapping = new Dictionary<Vector3, Node>();
    private void Awake()
    {
        GetTileMap();

        InitializeGraph();
        
    }


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = transform.position;
        currentTileLocal = ground.WorldToCell(target);
        startMovementTimer = movementTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleInput();
        }

        if (path.Count != 0)
        {
            if (movementTimer < 0f)
            {
                transform.position = ground.CellToWorld(new Vector3Int((int)path[0].pos.x, (int)path[0].pos.y, 0));
                path.RemoveAt(0);
                movementTimer = startMovementTimer;
            }
            else
            {
                movementTimer -= Time.deltaTime;
            }
        }
        /*transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);
        currentTileLocal = ground.WorldToCell(target);*/
    }

    void HandleInput()
    {
        Vector2 _target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPos = ground.WorldToCell(_target);
        gridPos.z = 0;
        /*
        Debug.Log("tile at: " + gridPos + " Has Tile: " + ground.HasTile(gridPos));
        */
        if (ground.HasTile(gridPos))
        {
            target = _target;
            Vector3Int goal = ground.WorldToCell(target);
            goal.z = 0;
            Vector3Int playerPos = ground.WorldToCell(transform.position);
            playerPos.z = 0;
            path = AStar(playerPos, goal);
            InitializeGraph();
   
            /*foreach (var node in path)
            {
                transform.position = ground.CellToWorld(new Vector3Int((int)node.pos.x, (int)node.pos.y, 0));
                Debug.Log("path: " + node.pos);
            }*/
        }
        
    }
    
    /// <summary>
    /// retrieves reference to all tiles in map.
    /// </summary>
    void GetTileMap()
    {
        foreach (var pos in ground.cellBounds.allPositionsWithin)
        {
            Vector3Int tilePosLocal = new Vector3Int(pos.x, pos.y, pos.z);
            Vector3 global = ground.CellToWorld(tilePosLocal);
            if (ground.HasTile(tilePosLocal))
            {
                tLocal.Add(tilePosLocal);
                tGlobal.Add(global);
            }
        }
    }


    List<Node> AStar(Vector3 start, Vector3 goal)
    {
        Debug.Log("goal: " + goal);
        List<Node> path = new List<Node>();
        List<Node> frontier = new List<Node>();
        Node S = mapping[start];
        S.localCost = 0;
        S.globalCost = 0;
        Debug.Log("localCost should be 0: " + mapping[start].localCost);
        /*
        path.Add(mapping[start]);
        */
        frontier.Add(mapping[start]);
        Debug.Log(frontier.Count);
        while (frontier.Count > 0)
        {
            Debug.Log("in while loop");

            Node current = frontier[0];
            

            foreach (var neighbor in current.neighbors)
            {
                Node n = neighbor;

                if (n.visited == false && current.localCost < n.localCost)
                {
                    n.parent = current.pos;
                    n.localCost = current.localCost + 1;
                    n.globalCost = n.localCost + DistanceBetweenTiles(n.pos, goal);

                    frontier.Add(n);
                    /*
                    Debug.Log("Frontier size: "+ frontier.Count);
                    */
                }
            }
            

            current.visited = true;
            frontier.RemoveAt(0);
        }

        Node temp = mapping[goal];
        while (temp != S)
        {
            path.Insert(0, mapping[temp.pos]);
            Debug.Log(path.Count);
            temp = mapping[temp.parent];
        }
        
        return path;
    }
    private void InitializeGraph()
    {
        foreach (var tile in tLocal)
        {
            graph.Add(new Node(tile, Vector3.negativeInfinity, float.PositiveInfinity, float.PositiveInfinity, false, new List<Node>()));
            mapping[tile] = graph[graph.Count - 1];
        }
        CreateGraph();
    }
    
    private void CreateGraph()
    {
        List<(int, int)> direction = new List<(int, int)>()
        {
            (0,1),
            (1,0),
            (0,-1),
            (-1,0)
        };

        foreach (var node in graph)
        {
            foreach (var dir in direction)
            {
                Vector3Int tile = new Vector3Int((int)node.pos.x + dir.Item1, (int)node.pos.y + dir.Item2, 0);
                if (ground.HasTile(tile))
                {    
                    Vector3 neighbor = node.pos + new Vector3(dir.Item1, dir.Item2, 0);
                    node.neighbors.Add(mapping[neighbor]);
                }
            }
        }
    }
    //Hueristic for A*
    float DistanceBetweenTiles(Vector3 x, Vector3 y)
    {
        return Vector3.Distance(x, y);
    }
}
