using System;
using System.Collections.Generic;
using System.Text;

namespace Implementing_A_star_algorithm
{
    class Graph
    {
        HashSet<Node> allNodes = new HashSet<Node>();

        public Node AddNode(string name, float latitude, float longitude)
        {
            Node newNode = new Node(name, latitude, longitude);
            allNodes.Add(newNode);
            return newNode;
        }

        public List<Node> AStar(Node start, Node goal)
        {
            HashSet<Node> openSet = new HashSet<Node>();
            openSet.Add(start);

            Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();

            Dictionary<Node, float> gScore = new Dictionary<Node, float>();
            foreach(Node node in allNodes)
            {
                gScore[node] = float.PositiveInfinity;
            }
            gScore[start] = 0;

            Dictionary<Node, float> fScore = new Dictionary<Node, float>();
            foreach (Node node in allNodes)
            {
                fScore[node] = float.PositiveInfinity;
            }
            fScore[start] = Heuristic(start, goal);
            
            while(openSet.Count>0)
            {
                //Find the node in open set having the lowest fScore value
                Node current = null;
                float lowestValue = float.PositiveInfinity;
                foreach(Node node in openSet)
                {
                    if(fScore[node]<lowestValue)
                    {
                        current = node;
                        lowestValue = fScore[node];
                    }
                }

                //Check if we reached our goal
                if(current == goal)
                {
                    return ReconstructPath(cameFrom, current);
                }

                openSet.Remove(current);

                foreach(var edge in current.GetEdges())
                {
                    //Tentative_gScore is the distance from start to the neighbor through current
                    float tentativeGScore = gScore[current] + edge.Value;
                    if(tentativeGScore < gScore[edge.Key])
                    {
                        cameFrom[edge.Key] = current;
                        gScore[edge.Key] = tentativeGScore;
                        fScore[edge.Key] = tentativeGScore + Heuristic(edge.Key, goal);
                        if(!openSet.Contains(edge.Key))
                        {
                            openSet.Add(edge.Key);
                        }
                    }
                }
            }
            return new List<Node>();
        }

        private List<Node> ReconstructPath(Dictionary<Node, Node> cameFrom, Node current)
        {
            List<Node> path = new List<Node>();
            while(cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Insert(0, current);
            }
            return path;
        }

        float Heuristic(Node node, Node goal)
        {
            float latDistance = node.GetLatitude() - goal.GetLatitude();
            float longDistance = node.GetLongitude() - goal.GetLongitude();
            float distanceSquared = latDistance * latDistance + longDistance * longDistance;
            return (float)Math.Sqrt(distanceSquared);
        }

    }
}
