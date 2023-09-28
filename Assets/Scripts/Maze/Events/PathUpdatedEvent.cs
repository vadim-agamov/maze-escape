using System.Collections.Generic;

namespace Maze.Events
{
    public struct PathUpdatedEvent
    {
        public IReadOnlyList<CellView> Cells;
    }
}