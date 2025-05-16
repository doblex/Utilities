using System.Collections.Generic;
using UnityEngine;
using utilities.AI;

[CreateAssetMenu(menuName = "BasepathFind", fileName = "PathFindStrats/BasepathFind")]
public class BasepathFind : PathFindStrategy
{
    public override AStarAgentStatus Pathfinding(Vector3 from, Vector3 goal, float speed ,ref List<Point> TotalPath, bool supressMovement = false)
    {
        //Mi setto le posizioni di partenza e arrivo
        _startPosition = from;
        _endPosition = goal;
        _start = WorldManager.Instance.GetClosestPointWorldSpace(from);
        _end = WorldManager.Instance.GetClosestPointWorldSpace(goal);

        if (_start == null || _end == null)
        {
            Status = AStarAgentStatus.Invalid;
            return Status;
        }

        //controllo se sono validi i punti
        if (_start == _end || _start.Invalid || _end.Invalid)
        {
            Status = AStarAgentStatus.Invalid;
            return Status;
        }

        //Mi creo una matrice di PointData per memorizzare i dati dei punti
        PointData[][][] dataSet = new PointData[WorldManager.Instance.Grid.Length][][];
        for (int i = 0; i < dataSet.Length; i++)
        {
            dataSet[i] = new PointData[WorldManager.Instance.Grid[i].Length][];
            for (int j = 0; j < dataSet[i].Length; j++)
            {
                dataSet[i][j] = new PointData[WorldManager.Instance.Grid[i][j].Length];
            }
        }

        List<PointData> openSet = new List<PointData>();

        //Setto il punto di partenza all interno della matrice
        PointData startPoint = new PointData(_start);
        dataSet[_start.Coords.x][_start.Coords.y][_start.Coords.z] = startPoint;
        startPoint.GScore = 0;

        startPoint.TimeToReach = 0;

        // il punto di partenza è sempre nell openset
        openSet.Add(startPoint);

        while (openSet.Count > 0)
        {

            PointData current = openSet[0];

            if (current.Coords == _end.Coords)
            {
                TotalPath = ReconstructPath(startPoint, current, dataSet);
                if (!supressMovement)
                {
                    Status = AStarAgentStatus.InProgress;
                    FirePathFindFinished();
                }
                return Status;
            }

            openSet.RemoveAt(0);
            HeapifyDeletion(openSet, 0);

            Point currentPoint = WorldManager.Instance.Grid[current.Coords.x][current.Coords.y][current.Coords.z];

            //controllo i vicini del punto corrente
            for (int i = 0; i < currentPoint.Neighbours.Count; i++)
            {
                // mi tiro fuori il vicino dalle sue coordinate
                Vector3Int indexes = currentPoint.Neighbours[i];
                Point neighbour = WorldManager.Instance.Grid[indexes.x][indexes.y][indexes.z];
                PointData neighbourData = dataSet[indexes.x][indexes.y][indexes.z];

                bool neighbourPassed = true;
                if (neighbourData == null)
                {
                    neighbourData = new PointData(neighbour);
                    dataSet[indexes.x][indexes.y][indexes.z] = neighbourData;
                    neighbourPassed = false;
                }

                float distance = (currentPoint.WorldPosition - neighbour.WorldPosition).magnitude;
                float timeToReach = current.TimeToReach + distance / speed;

                if (!neighbour.Invalid)
                {
                    float tenativeScore = current.GScore + WorldManager.Instance.PointDistance;
                    if (tenativeScore < neighbourData.GScore)
                    {
                        neighbourData.CameFrom = current.Coords;
                        neighbourData.GScore = tenativeScore;
                        neighbourData.FScore = neighbourData.GScore + HeuristicFunction(neighbour.WorldPosition, _end.WorldPosition);
                        neighbourData.TimeToReach = timeToReach;
                        if (!neighbourPassed)
                        {
                            openSet.Add(neighbourData);
                            Heapify(openSet, openSet.Count - 1);
                        }
                    }
                }
            }
        }
        Status = AStarAgentStatus.Invalid;
        return Status;
    }
}
