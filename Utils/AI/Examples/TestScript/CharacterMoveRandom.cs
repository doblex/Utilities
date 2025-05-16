using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace utilities.AI.Examples
{

    [RequireComponent(typeof(PathFind))]
    public class CharacterMoveRandom : MonoBehaviour
    {
        PathFind _PathFind;

        private void Start()
        {
            _PathFind = GetComponent<PathFind>();
            StartCoroutine(Coroutine_MoveRandom());
        }

        IEnumerator Coroutine_MoveRandom()
        {
            List<Point> freePoints = WorldManager.Instance.GetFreePoints();
            Point start = freePoints[Random.Range(0, freePoints.Count)];
            transform.position = start.WorldPosition;
            while (true)
            {
                Point p = freePoints[Random.Range(0, freePoints.Count)];
                _PathFind.Pathfinding(p.WorldPosition);
                while (_PathFind.PathFindStatus != AStarAgentStatus.Finished)
                {
                    yield return null;
                }
            }
        }
    }
}