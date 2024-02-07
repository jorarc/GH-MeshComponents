using System;
using System.Collections;
using System.Collections.Generic;
using Rhino;
using Rhino.Geometry;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using System.Threading.Tasks;
using System.Linq;

public class Script_Instance : GH_ScriptInstance
{
  /// <summary>
  /// This procedure contains the user code. Input parameters are provided as regular arguments,
  /// Output parameters as ref arguments. You don't have to assign output parameters,
  /// they will have a default value.
  /// </summary>
  private void RunScript(Mesh mesh, Point3d startPoint, ref object U)
  {
    int nVertex = mesh.Vertices.Count;
    double[] distances = new double[nVertex];
    bool[] visited = new bool[nVertex];
    List<int>[] connectedIndices = new List<int>[nVertex];

    // Initialize distances and connectedIndices
    for (int i = 0; i < nVertex; i++)
    {
      distances[i] = double.MaxValue;
      connectedIndices[i] = new List<int>();
    }

    // Find nearest vertex to the startPoint and set its distance to 0
    int startIndex = FindNearestVertexIndex(mesh, startPoint);
    distances[startIndex] = 0.0;

    // Populate connected indices
    for (int i = 0; i < mesh.TopologyVertices.Count; i++)
    {
      connectedIndices[i] = mesh.TopologyVertices.ConnectedTopologyVertices(i).ToList();
    }

    // Dijkstra's Algorithm
    PriorityQueue<int, double> vertexQueue = new PriorityQueue<int, double>();
    vertexQueue.Enqueue(startIndex, 0.0);

    while (vertexQueue.Count > 0)
    {
      int i = vertexQueue.Dequeue();
      if (visited[i]) continue;
      visited[i] = true;

      foreach (int j in connectedIndices[i])
      {
        double tentativeDistance = distances[i] + mesh.Vertices[i].DistanceTo(mesh.Vertices[j]);
        if (tentativeDistance < distances[j])
        {
          distances[j] = tentativeDistance;
          vertexQueue.Enqueue(j, tentativeDistance);
        }
      }
    }

    // Convert distances to GH_Number and return
    U = distances.Select(d => new GH_Number(d)).ToArray();

  }

  // <Custom additional code> 
  private static int FindNearestVertexIndex(Mesh mesh, Point3d point)
  {
    double minDistance = double.MaxValue;
    int nearestIndex = -1;

    for (int i = 0; i < mesh.Vertices.Count; i++)
    {
      double distance = point.DistanceTo(mesh.Vertices[i]);
      if (distance < minDistance)
      {
        minDistance = distance;
        nearestIndex = i;
      }
    }

    return nearestIndex;
  }

  public class PriorityQueueNode<TElement, TPriority> where TPriority : IComparable<TPriority>
  {
    public TElement Element { get; set; }
    public TPriority Priority { get; set; }

    public PriorityQueueNode(TElement element, TPriority priority)
    {
      Element = element;
      Priority = priority;
    }
  }

  public class PriorityQueue<TElement, TPriority> where TPriority : IComparable<TPriority>
  {
    private List<PriorityQueueNode<TElement, TPriority>> elements = new List<PriorityQueueNode<TElement, TPriority>>();

    public int Count
    {
      get
      {
        return elements.Count;
      }
    }

    public void Enqueue(TElement element, TPriority priority)
    {
      var node = new PriorityQueueNode<TElement, TPriority>(element, priority);
      elements.Add(node);
      int ci = elements.Count - 1; // Child index; start at end
      while (ci > 0)
      {
        int pi = (ci - 1) / 2; // Parent index
        if (elements[ci].Priority.CompareTo(elements[pi].Priority) >= 0) break; // Child item is larger than (or equal) parent so we're done
        var tmp = elements[ci]; elements[ci] = elements[pi]; elements[pi] = tmp;
        ci = pi;
      }
    }

    public TElement Dequeue()
    {
      // Assumes pq is not empty
      int li = elements.Count - 1; // Last index (before removal)
      var frontItem = elements[0];   // Fetch the front
      elements[0] = elements[li];
      elements.RemoveAt(li);

      --li; // Last index (after removal)
      int pi = 0; // Parent index. Start at front of pq
      while (true)
      {
        int ci = pi * 2 + 1; // Left child index of parent
        if (ci > li) break;  // No children so done
        int rc = ci + 1;     // Right child
        if (rc <= li && elements[rc].Priority.CompareTo(elements[ci].Priority) < 0) // If there is a right child and it is smaller than left child, use the right child instead
          ci = rc;
        if (elements[pi].Priority.CompareTo(elements[ci].Priority) <= 0) break; // Parent is smaller than (or equal to) smallest child so done
        var tmp = elements[pi]; elements[pi] = elements[ci]; elements[ci] = tmp; // Swap parent and child
        pi = ci;
      }
      return frontItem.Element;
    }
  }

  // </Custom additional code> 
}