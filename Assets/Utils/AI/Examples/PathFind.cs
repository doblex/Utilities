using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace utilities.AI.Examples
{
    public class PathFind : MonoBehaviour
    {
        [SerializeField] PathFindStrategy _PathFindStrategy;

        public AStarAgentStatus PathFindStatus { get => _PathFindStrategy.Status; }

        public float Speed;
        public float TurnSpeed;

        [HideInInspector] public List<Point> TotalPath;


        [SerializeField] bool _DebugPath;
        [SerializeField] Color _DebugPathColor;

        public bool CurvePath;

        [SerializeField] float _CornerSmooth;

        private void Awake()
        {
            if (_PathFindStrategy == null)
            {
                Debug.LogError("PathFindStrategy is null");
                return;
            }

            _PathFindStrategy.Initialize();
        }

        private void Start()
        {
            _PathFindStrategy.OnPathfindingFinished += StartMoving;
        }

        public void Pathfinding(Vector3 goal)
        {
            AStarAgentStatus returnStatus = _PathFindStrategy.Pathfinding(transform.position, goal, Speed, ref TotalPath);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            _PathFindStrategy.OnPathfindingFinished -= StartMoving;
        }

        private void StartMoving()
        {
            StopAllCoroutines();
            StartCoroutine(Coroutine_CharacterFollowPath());
        }

        IEnumerator Coroutine_CharacterFollowPath()
        {
            _PathFindStrategy.Status = AStarAgentStatus.InProgress;
            for (int i = TotalPath.Count - 1; i >= 0; i--)
            {
                SetPathColor();
                float length = (transform.position - TotalPath[i].WorldPosition).magnitude;
                float l = 0;
                while (l < length)
                {
                    SetPathColor();
                    Vector3 forwardDirection = (TotalPath[i].WorldPosition - transform.position).normalized;
                    if (CurvePath)
                    {
                        transform.position += transform.forward * Time.deltaTime * Speed;
                        transform.forward = Vector3.Slerp(transform.forward, forwardDirection, Time.deltaTime * TurnSpeed);
                    }
                    else
                    {
                        transform.forward = forwardDirection;
                        transform.position = Vector3.MoveTowards(transform.position, TotalPath[i].WorldPosition, Time.deltaTime * Speed);
                    }
                    l += Time.deltaTime * Speed;
                    yield return new WaitForFixedUpdate();
                }
            }
            _PathFindStrategy.Status = AStarAgentStatus.Finished;
        }

        public void SetPathColor()
        {
            if (_DebugPath)
            {
                if (TotalPath != null)
                {
                    for (int j = TotalPath.Count - 2; j >= 0; j--)
                    {
                        Debug.DrawLine(TotalPath[j + 1].WorldPosition, TotalPath[j].WorldPosition, _DebugPathColor, 1);
                    }
                }
            }
        }
    }
}
