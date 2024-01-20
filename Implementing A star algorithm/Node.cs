using System;
using System.Collections.Generic;
using System.Text;

namespace Implementing_A_star_algorithm
{
    class Node
    {
        string name;
        Dictionary<Node, float> edges = new Dictionary<Node, float>();
        float latitude, longitude;

        public Node(string name, float latitude, float longitude)
        {
            this.name = name;
            this.latitude = latitude;
            this.longitude = longitude;
        }

        public void AddEdge(Node node, float weight)
        {
            edges[node] = weight;
            node.edges[this] = weight;
        } 

        public float GetLatitude()
        {
            return latitude;
        }

        public float GetLongitude()
        {
            return longitude;
        }

        public Dictionary<Node, float> GetEdges()
        {
            return edges;
        }

        public string GetName()
        {
            return name;
        }
    }
}
