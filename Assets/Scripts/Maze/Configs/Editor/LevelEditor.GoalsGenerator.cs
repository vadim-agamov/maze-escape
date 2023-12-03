using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Maze.Configs.Editor
{
    public partial class LevelEditor
    {
        private string _goalsAmountString;
        private int _goalsAmount;
        
        
        private int _goalsLengthMin = 5;
        private string _goalsLengthMinString;
            
        private int _goalsLengthMax;
        private string _goalsLengthMaxString;
        
        private List<PathGoal> _goals = new ();
        private int Complexity => _goals.Sum(x => x.Length);
        
        private void GenerateGoals()
        {
            _goals.Clear();
            var start = GetStart();
            var prevGoal = (Row: start.Row, Col: start.Col);
            for (var i = 0; i < _goalsAmount; i++)
            {
                prevGoal = GenerateNextGoal(prevGoal);
            }
        }
        
        private (int Row, int Col) GenerateNextGoal((int Row, int Col) p)
        {
            var desiredPathLength = Random.Range(_goalsLengthMin, _goalsLengthMax);
            var goalFinder = new GoalFinder(_cells, desiredPathLength);
            var (row, col, length) = goalFinder.Execute(p.Row, p.Col);
            _goals.Add(new PathGoal{Row = row, Col = col, Length = length});
            return (row, col);
        }
    }
}