using System.Collections.Generic;
using UnityEngine;
using utilities.AI;


public abstract class PathFindStrategy : ScriptableObject
{
    public delegate void PathfindingFinished();

    public event PathfindingFinished OnPathfindingFinished;

    protected Point _start;
    protected Point _end;

    protected Vector3 _startPosition;
    protected Vector3 _endPosition;

    [HideInInspector] public AStarAgentStatus Status = AStarAgentStatus.Finished;

    [HideInInspector] public List<Point> CornerPoints;

    public virtual void Initialize() { }

    public abstract AStarAgentStatus Pathfinding(Vector3 from, Vector3 goal, float speed, ref List<Point> TotalPath, bool supressMovement = false);

    protected void FirePathFindFinished() => OnPathfindingFinished?.Invoke();

    protected float HeuristicFunction(Vector3 p1, Vector3 p2)
    {
        return (p2 - p1).sqrMagnitude;
    }

    protected List<Point> ReconstructPath(PointData start, PointData current, PointData[][][] dataSet)
    {
        CornerPoints = new List<Point>();
        List<Point> totalPath = new List<Point>();

        PointData currentPointData = dataSet[current.Coords.x][current.Coords.y][current.Coords.z];
        Point currentPoint = WorldManager.Instance.Grid[current.Coords.x][current.Coords.y][current.Coords.z];

        totalPath.Add(currentPoint);

        Point cameFromPoint = WorldManager.Instance.Grid[current.CameFrom.x][current.CameFrom.y][current.CameFrom.z];

        Vector3 direction = (currentPoint.Coords - cameFromPoint.Coords);
        direction = direction.normalized;

        CornerPoints.Add(currentPoint);

        int count = 0;
        while (current.CameFrom.x != -1 && count < 10000)
        {

            currentPoint = WorldManager.Instance.Grid[current.Coords.x][current.Coords.y][current.Coords.z];
            PointData cameFromPointData = dataSet[current.CameFrom.x][current.CameFrom.y][current.CameFrom.z];
            cameFromPoint = WorldManager.Instance.Grid[current.CameFrom.x][current.CameFrom.y][current.CameFrom.z];

            Vector3 dir = (currentPoint.Coords - cameFromPoint.Coords);
            if (dir != direction)
            {
                CornerPoints.Add(currentPoint);
                direction = dir;
            }

            totalPath.Add(cameFromPoint);
            current = dataSet[current.CameFrom.x][current.CameFrom.y][current.CameFrom.z];
        }

        currentPoint = WorldManager.Instance.Grid[current.Coords.x][current.Coords.y][current.Coords.z];
        CornerPoints.Add(currentPoint);

        return totalPath;
    }

    protected void Heapify(List<PointData> list, int i)
    {
        int parent = (i - 1) / 2;
        if (parent > -1)
        {
            if (list[i].FScore < list[parent].FScore)
            {
                PointData pom = list[i];
                list[i] = list[parent];
                list[parent] = pom;
                Heapify(list, parent);
            }
        }
    }

    protected void HeapifyDeletion(List<PointData> list, int i)
    {
        int smallest = i; //0
        int l = 2 * i + 1; //1
        int r = 2 * i + 2; //2

        if (l < list.Count && list[l].FScore < list[smallest].FScore)
        {
            smallest = l;
        }
        if (r < list.Count && list[r].FScore < list[smallest].FScore)
        {
            smallest = r;
        }
        if (smallest != i)
        {
            PointData pom = list[i];
            list[i] = list[smallest];
            list[smallest] = pom;

            // Recursively heapify the affected sub-tree
            HeapifyDeletion(list, smallest);
        }
    }
}
