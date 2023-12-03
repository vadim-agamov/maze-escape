using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Maze.Service;
using Modules.Extensions;
using UnityEngine;
using UnityEngine.Pool;

namespace Maze.Components
{
    public interface IGoalsFactoryComponent
    {
        GoalView Create(CellView cellView);
    }
    
    [Serializable]
    public class GoalItem
    {
        public string Id;
        public GoalView GoalView;
    }
    
    public class GoalsFactoryComponent: MonoBehaviour, IComponent, IGoalsFactoryComponent
    {
        [SerializeField]
        private GoalItem[] _goalsViews;

        [SerializeField] 
        private string[] _rewards;
        
        private readonly Dictionary<string, ObjectPool<GoalView>> _goalViewPools = new ();
        
        public static int RewardCount => Screen.width > Screen.height ? 3 : 1;

        UniTask IComponent.Initialize(Context context, IMazeService mazeService)
        {
            context.GoalsFactoryComponent = this;
            foreach (var goalItem in _goalsViews)
            {
                _goalViewPools.Add(goalItem.Id, new ObjectPool<GoalView>(
                    () => Instantiate(goalItem.GoalView, transform), 
                    actionOnGet: view => view.gameObject.SetActive(true),
                    actionOnRelease: view =>
                    {
                        view.transform.SetParent(transform);
                        view.gameObject.SetActive(false);
                    }));
            }
            return UniTask.CompletedTask;
        }

        GoalView IGoalsFactoryComponent.Create(CellView cellView)
        {
            var id = _goalsViews.Select(item => item.Id).ToList().Random();
            return Create(id, cellView);
        }

        private GoalView Create(string id, CellView cellView)
        {
            var pool = _goalViewPools[id];
            var goalView = pool.Get();
            goalView.transform.SetParent(cellView.transform);
            goalView.transform.localPosition = Vector3.zero;
            goalView.Pool = pool;
            goalView.Rewards = _rewards.Shuffle().Take(RewardCount);
            cellView.Goal = goalView;
            return goalView;
        }

        void IComponent.Tick()
        {
        }

        void IDisposable.Dispose()
        {
            foreach (var pool in _goalViewPools.Values)
            {
                pool.Dispose();
            }
            _goalViewPools.Clear();
        }
    }
}