using System;
using System.Collections.Generic;
using System.Diagnostics;

public class AStar
{
    SearchNode start;
    AStarFrontier frontier;

    public AStar(State start, int width, int height, Cell goal)
    {
        State.width = width;
        State.height = height;
        State.goal = goal;

        this.start = new SearchNode(start, null, Direction.Left, 0);
        this.frontier = new AStarFrontier();
        this.frontier.Insert(this.start);
    }

    public SearchResult Search()
    {
        Stopwatch timer = Stopwatch.StartNew();
        float maxTime = 1_000; // ms

        // Set up explored set of states in order not to repeat ourselves.
        HashSet<State> explored = new HashSet<State>();
        HashSet<Cell> exploredCells = new HashSet<Cell>();

        while (!frontier.IsEmpty)
        {
            SearchNode node = frontier.Pop();

            if (timer.ElapsedMilliseconds > maxTime) break;

            // Are we done?
            if (node.State.IsGoal)
            {
                // Return path starting from the beginning of the parent link chain.
                var path = node.Path();

                long elapsedMs = timer.ElapsedMilliseconds;

                UnityEngine.Debug.Log($"Found path of length={path.Count} by exploring {explored.Count} states in {elapsedMs} ms");

                SearchResult res = new SearchResult(path, elapsedMs, explored.Count);

                return res;
            };

            explored.Add(node.State);
            exploredCells.Add(node.State.Head);

            foreach (Direction action in node.State.Actions())
            {
                var childNode = new SearchNode(node, action);
                if (!exploredCells.Contains(childNode.State.Head) && !explored.Contains(childNode.State) && !frontier.Contains(childNode.State))
                {
                    frontier.Insert(childNode);
                }
                else if (frontier.Contains(childNode.State))
                {
                    frontier.UpdatePriorityIfNeeded(childNode);
                }
            }
        }

        // No solution found:
        return new SearchResult(null, timer.ElapsedMilliseconds, explored.Count);
    }
}

public class SearchNode
{
    public State State { get; }
    public SearchNode Parent { get; }
    public Direction Action { get; }
    public int PathLength { get; }

    public SearchNode(State state, SearchNode parent, Direction action, int pathLength)
    {
        State = state;
        Parent = parent;
        Action = action;
        PathLength = pathLength;
    }

    public SearchNode(SearchNode parent, Direction action)
    {
        State = State.ApplyAction(action, parent.State);
        Parent = parent;
        Action = action;
        PathLength = parent.PathLength + 1;
    }

    public LinkedList<Direction> Path()
    {
        var res = new LinkedList<Direction>();
        var curr = this;
        while (curr.Parent != null)
        {
            res.AddFirst(curr.Action);
            curr = curr.Parent;
        }
        return res;
    }
}

struct Prio<D> : IComparable<Prio<D>> where D : class
{
    public readonly D data;
    public readonly float priority;

    public Prio(D data, float priority)
    {
        this.data = data;
        this.priority = priority;
    }

    public int CompareTo(Prio<D> that)
    {
        return this.priority.CompareTo(that.priority);
    }

    public bool Equals(Prio<D> that)
    {
        return this.priority == that.priority;
    }
}

public class AStarFrontier
{
    readonly C5.IPriorityQueue<Prio<SearchNode>> nodes;
    readonly IDictionary<State, C5.IPriorityQueueHandle<Prio<SearchNode>>> handles;

    public AStarFrontier()
    {
        nodes = new C5.IntervalHeap<Prio<SearchNode>>();
        handles = new Dictionary<State, C5.IPriorityQueueHandle<Prio<SearchNode>>>();
    }

    public void Insert(SearchNode node)
    {
        float priority = NodePriority(node);
        Prio<SearchNode> nodeWithPriority = new Prio<SearchNode>(node, priority);
        C5.IPriorityQueueHandle<Prio<SearchNode>> handle = null;
        nodes.Add(ref handle, nodeWithPriority);
        handles[node.State] = handle;
    }

    public void UpdatePriorityIfNeeded(SearchNode node)
    {
        C5.IPriorityQueueHandle<Prio<SearchNode>> handle = handles[node.State];
        Prio<SearchNode> existingNode = nodes[handle];
        float newPriority = NodePriority(node);
        if (newPriority < existingNode.priority)
        {
            nodes[handle] = new Prio<SearchNode>(node, newPriority);
        }
    }

    public SearchNode Pop()
    {
        Prio<SearchNode> min = nodes.DeleteMin();
        SearchNode node = min.data;
        handles.Remove(node.State);
        return node;
    }

    public bool IsEmpty
    {
        get => nodes.IsEmpty;
    }

    public bool Contains(State state)
    {
        return handles.ContainsKey(state);
    }

    static float NodePriority(SearchNode node)
    {
        // We multiple heuristics by 1.001 to break the ties between similar paths,
        // so that we need to explore only one of them.
        // See https://theory.stanford.edu/~amitp/GameProgramming/Heuristics.html#breaking-ties.
        // This has a nice side effect that AI avoids "ladders" like paths and
        // prefers straight human-like patterns.
        return (float) node.PathLength + 1.001f * Heuristic(node);
    }

    static float Heuristic(SearchNode node)
    {
        // We use Manhattan distance to the goal as our estimate:
        int drow = Math.Abs(State.goal.Row - node.State.Head.Row);
        int dcol = Math.Abs(State.goal.Col - node.State.Head.Col);
        return (float) drow + dcol;
    }
}

public class SearchResult
{
    public LinkedList<Direction> Path;
    public long ElapsedMs;
    public int ExploredStates;

    public SearchResult(LinkedList<Direction> path, long elapsedMs, int exploredStates)
    {
        Path = path;
        ElapsedMs = elapsedMs;
        ExploredStates = exploredStates;
    }
}
