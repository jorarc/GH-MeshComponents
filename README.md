# Geodesic Distance
This documentation covers the C# script used in Grasshopper for calculating approximate geodesic distances from a specified point on a mesh to all other vertices. The script is designed to work within the Grasshopper environment and is optimized for performance with large meshes.

The script implements a version of **Dijkstra's algorithm** using a custom priority queue. It is tailored for use with [RhinoCommon's mesh data structures](https://developer.rhino3d.com/api/rhinocommon/rhino.geometry.mesh?version=8.x) in Grasshopper and is designed to calculate distances efficiently, even for large meshes.
<p align="center">
  <img src="https://github.com/jorarc/GH-MeshComponents/assets/124450741/a6dc4d7e-c133-4b92-9471-56b07519da88" alt="image description">
</p>

- Input: **Mesh (`Mesh`)**:
    - 🟦 Type: Rhino.Geometry.Mesh
    - 📝 Description: The input mesh on which geodesic distances are to be calculated.
- Input: **StartPoint (`Point3d`)**:
    - 🔵 Type: Rhino.Geometry.Point3d
    - 📝 Description: The starting point on the mesh from which distances to all vertices are calculated.
- Output: **U (`object`)**
    - 🟩 Type: Array of Grasshopper.Kernel.Types.GH_Number
    - 📝 Description: An array containing the calculated geodesic distances from the **`StartPoint`** to each vertex in the **`Mesh`**. The distances are represented as Grasshopper numbers for compatibility with other Grasshopper components.
