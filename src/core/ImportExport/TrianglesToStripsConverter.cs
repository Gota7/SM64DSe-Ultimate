/* Attempts to convert a list of separate triangles to triangle strips wherever possible.
 * 
 * Based on the SGI / "tomesh" algorithm with some modifications.
 * 
 * See comments for details.
 *  
 *   Triangle Strips   
 *    v2___v4____v6
 *    /|\  |\    /\     
 * v0( | \ | \  /  \    
 *    \|__\|__\/____\   
 *    v1   v3  v5   v7
 * 
 * The vertices are normally arranged anti-clockwise, except that: in triangle-strips each second polygon uses clockwise arranged vertices.
 * 
 * Edge AB: v0 - v1
 * Edge BC: v1 - v2
 * Edge CA: v2 - v0
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace SM64DSe.ImportExport
{
    public class TrianglesToStripsConverter : BaseStripConverter
    {
        public TrianglesToStripsConverter(ModelBase.FaceListDef faceList) : base(faceList) {}

        protected override void verifyCorrectVertexCount(ModelBase.FaceListDef faceList)
        {
            if (faceList.m_Type != ModelBase.PolyListType.Triangles)
            {
                bool tris = true;
                for (int i = 0; i < faceList.m_Faces.Count; i++)
                {
                    tris = (faceList.m_Faces[i].m_NumVertices == 3);
                    if (!tris)
                        throw new ArgumentException("The provided FaceListDef must be triangulated.");
                }
                faceList.m_Type = ModelBase.PolyListType.Triangles;
            }
        }

        public override List<ModelBase.FaceListDef> Stripify(bool keepVertexOrderDuringStripping = false)
        {
            for (int i = 0; i < m_Faces.Count; i++)
            {
                ModelBase.FaceDef triangle = m_Faces[i].m_Face;

                List<int> vertexALinked = GetVertex(triangle.m_Vertices[0]).m_LinkedFaces;
                List<int> vertexBLinked = GetVertex(triangle.m_Vertices[1]).m_LinkedFaces;
                List<int> vertexCLinked = GetVertex(triangle.m_Vertices[2]).m_LinkedFaces;

                // Shares edge AB if both triangles reference vertices A and B
                var edgeAB = vertexALinked.Intersect(vertexBLinked).Except(new int[] { i });
                var edgeBC = vertexBLinked.Intersect(vertexCLinked).Except(new int[] { i });
                var edgeCA = vertexCLinked.Intersect(vertexALinked).Except(new int[] { i });

                if (edgeAB.Count() > 0)
                    m_Faces[i].m_LinkedFaces[(int)TriangleEdge.Edge_AB].AddRange(edgeAB);
                if (edgeBC.Count() > 0)
                    m_Faces[i].m_LinkedFaces[(int)TriangleEdge.Edge_BC].AddRange(edgeBC);
                if (edgeCA.Count() > 0)
                    m_Faces[i].m_LinkedFaces[(int)TriangleEdge.Edge_CA].AddRange(edgeCA);
            }

            m_FacesToProcess.AddRange(m_Faces);

            // Sort by number of neighbours
            if (!keepVertexOrderDuringStripping)
            {
                m_FacesToProcess.Sort((a, b) => GetNumLinked(a).CompareTo(GetNumLinked(b)));
            }

            List<ModelBase.FaceListDef> tStrips = new List<ModelBase.FaceListDef>();

            // For storing triangles that have no neighbours or don't end up in a strip
            ModelBase.FaceListDef separateTriangles = new ModelBase.FaceListDef(ModelBase.PolyListType.Triangles);

            FaceLinked currentTriangle = null;

            while (m_FacesToProcess.Count > 0)
            {
                while (true)
                {
                    if (m_FacesToProcess.Count <= 0)
                    {
                        currentTriangle = null;
                        break;
                    }

                    currentTriangle = m_FacesToProcess[0];
                    m_FacesToProcess.RemoveAt(0);
                    if (currentTriangle.m_Marked)
                        continue;
                    else
                        break;
                }
                if (currentTriangle == null) break;

                List<FaceLinked>[] linked = GetLinked(currentTriangle);
                int numLinked = GetNumLinked(currentTriangle);

                currentTriangle.m_Marked = true;

                if (numLinked == 0)
                {
                    separateTriangles.m_Faces.Add(currentTriangle.m_Face);
                    continue;
                }

                // For each face build a strip off each of its edges and keep the longest one, discarding the 
                // rest. Not part of SGI.
                Tuple<ModelBase.FaceListDef, List<int>> tStripAB = 
                    GetStripAndIndicesForStartingEvenEdge(currentTriangle, TriangleEdge.Edge_AB);
                Tuple<ModelBase.FaceListDef, List<int>> tStripBC =
                    GetStripAndIndicesForStartingEvenEdge(currentTriangle, TriangleEdge.Edge_BC);
                Tuple<ModelBase.FaceListDef, List<int>> tStripCA =
                    GetStripAndIndicesForStartingEvenEdge(currentTriangle, TriangleEdge.Edge_CA);

                List<Tuple<ModelBase.FaceListDef, List<int>>> candidates = new List<Tuple<ModelBase.FaceListDef, List<int>>>() 
                    { tStripAB, tStripBC, tStripCA };

                List<int> stripLengths = new List<int>();
                foreach (Tuple<ModelBase.FaceListDef, List<int>> strip in candidates)
                    stripLengths.Add(strip.Item1.m_Faces.Count);
                int longestStripIndex = stripLengths.IndexOf(stripLengths.Max());

                Tuple<ModelBase.FaceListDef, List<int>> longestStrip = candidates[longestStripIndex];

                if (longestStrip.Item1.m_Faces.Count == 0)
                {
                    separateTriangles.m_Faces.Add(currentTriangle.m_Face);
                    continue;
                }

                foreach (int tri in longestStrip.Item2)
                    m_Faces[tri].m_Marked = true;

                tStrips.Add(longestStrip.Item1);
            }

            if (separateTriangles.m_Faces.Count > 0) tStrips.Add(separateTriangles);

            return tStrips;
        }

        Tuple<ModelBase.FaceListDef, List<int>> GetStripAndIndicesForStartingEvenEdge(
            FaceLinked start, TriangleEdge startForwardEdge)
        {
            List<int> stripIndices = new List<int>();
            List<TriangleRotation> stripRotations = new List<TriangleRotation>();

            List<FaceLinked> linked = GetLinked(start)[(int)startForwardEdge];
            FaceLinked bestNeighbour = DetermineBestNextNeighbour(start, linked, (int)startForwardEdge);

            if (bestNeighbour == null) 
                return new Tuple<ModelBase.FaceListDef, List<int>>(new ModelBase.FaceListDef(), new List<int>());

            TriangleRotation startRotation = (TriangleRotation)((int)(startForwardEdge - TriangleEdge.Edge_BC + 3) % 3);

            FaceLinked t = start;
            TriangleEdge currentEdge = startForwardEdge;
            TriangleRotation currentRotation = startRotation;
            bool even = true;
            int index_t = -1;
            while (t != null && !stripIndices.Contains((index_t = m_Faces.IndexOf(t))))
            {
                stripIndices.Add(index_t);
                stripRotations.Add(currentRotation);

                linked = GetLinked(t)[(int)currentEdge];
                bestNeighbour = DetermineBestNextNeighbour(t, linked, (int)currentEdge);

                t = bestNeighbour;

                even = !even;

                if (t != null)
                {
                    // Determine rotation and the edge to be used to get the next face

                    ModelBase.FaceDef triangleC_CW = new ModelBase.FaceDef(3);
                    if (even)
                    {
                        triangleC_CW.m_Vertices[0] = t.m_Face.m_Vertices[0];
                        triangleC_CW.m_Vertices[1] = t.m_Face.m_Vertices[1];
                        triangleC_CW.m_Vertices[2] = t.m_Face.m_Vertices[2];
                    }
                    else
                    {
                        triangleC_CW.m_Vertices[0] = t.m_Face.m_Vertices[2];
                        triangleC_CW.m_Vertices[1] = t.m_Face.m_Vertices[1];
                        triangleC_CW.m_Vertices[2] = t.m_Face.m_Vertices[0];
                    }

                    // The edge of the vertices which match the preceding triangle's
                    TriangleEdge linkBackEdge = TriangleEdge.Edge_AB;
                    // The vertices which match the preceding triangle's
                    ModelBase.VertexDef[] currentMatchedEdge = new ModelBase.VertexDef[2];
                    FaceLinked previous = m_Faces[stripIndices[stripIndices.Count - 1]];
                    currentMatchedEdge[0] = previous.m_Face.m_Vertices[(int)(currentEdge + 0) % 3];
                    currentMatchedEdge[1] = previous.m_Face.m_Vertices[(int)(currentEdge + 1) % 3];
                    // Find the edge in the current triangle which if odd has been made CW which matches 
                    // that from the preceding triangle. This will be set as the current triangle's first, 
                    // or 'AB' edge and the next edge (next two vertices) will be used to match the next 
                    // triangle.
                    for (int i = 0; i < 3; i++)
                    {
                        ModelBase.VertexDef[] edge = new ModelBase.VertexDef[2];
                        edge[0] = triangleC_CW.m_Vertices[(i + 0) % 3];
                        edge[1] = triangleC_CW.m_Vertices[(i + 1) % 3];
                        if (edge.Except(currentMatchedEdge).Count() == 0)
                        {
                            linkBackEdge = (TriangleEdge)i;
                            break;
                        }
                    }

                    TriangleEdge nextEdgeNoC_CW = (TriangleEdge)((int)(linkBackEdge + 1) % 3);

                    TriangleEdge nextEdge = nextEdgeNoC_CW;
                    if (!even)
                    {
                        // If odd, nextEdgeNoC_CW points to the edge to be used if written CW, however 
                        // all triangles have been read in as CCW so need to get the corresponding edge 
                        // in CCW version.
                        ModelBase.VertexDef[] nextEdgeNoC_CW_Vertices = new ModelBase.VertexDef[2];
                        nextEdgeNoC_CW_Vertices[0] = triangleC_CW.m_Vertices[(int)(nextEdgeNoC_CW + 0) % 3];
                        nextEdgeNoC_CW_Vertices[1] = triangleC_CW.m_Vertices[(int)(nextEdgeNoC_CW + 1) % 3];
                        for (int i = 0; i < 3; i++)
                        {
                            ModelBase.VertexDef[] ccwEdge = new ModelBase.VertexDef[2];
                            ccwEdge[0] = t.m_Face.m_Vertices[(i + 0) % 3];
                            ccwEdge[1] = t.m_Face.m_Vertices[(i + 1) % 3];
                            if (nextEdgeNoC_CW_Vertices.Except(ccwEdge).Count() == 0)
                            {
                                nextEdge = (TriangleEdge)i;
                                break;
                            }
                        }
                    }

                    // Now we need to determine the required rotation of the current triangle so that for 
                    // even triangles the new vertex in at index 0 and for odd triangles it occurs at 
                    // index 2.
                    ModelBase.VertexDef uniqueVertex = t.m_Face.m_Vertices.Except(previous.m_Face.m_Vertices).ElementAt(0);
                    int uniqueVertexIndex = Array.IndexOf(t.m_Face.m_Vertices, uniqueVertex);
                    TriangleRotation requiredRotation =
                        (even) ? (TriangleRotation)((uniqueVertexIndex - 2 + 3) % 3) : 
                        (TriangleRotation)(uniqueVertexIndex);

                    currentRotation = requiredRotation;
                    currentEdge = nextEdge;

                    // To best understand how this works, debug and step-through how the following model is handled:
                    //
                    // An example:
                    // Faces as defined in model (all Counter-Clockwise (CCW)):
                    // f 1 2 3 
                    // f 4 1 3
                    // f 4 5 1
                    // Build strip from edge CA. 
                    // # 2 3 1 (LS)      <- Need to Left Shift vertices so that CA is the second edge (in a tri. strip it's 
                    //                      always the second edge that's shared - see diagram at top).
                    // #   3 1 4 (4 1 3) <- For odd faces the new vertex must be at index [0]
                    //                      No Rot required, link-back CW: AB, CW forward: AB + 1 = BC, 
                    //                      CCW forward: edge in CCW that contains vertices in (CW forward) = AB
                    //                      The next triangle is the one that shares the CCW edge AB (vertices 1 and 4)
                    // #     1 4 5 (RS)  <- Even face the new vertex needs to be in index [2] so need to Right Shift vertices
                    //                      Repeat steps as for above face but don't need to worry about converting between CCW and CW order 
                }

            }

            ModelBase.FaceListDef tStrip = new ModelBase.FaceListDef(ModelBase.PolyListType.TriangleStrip);

            for (int i = 0; i < stripIndices.Count; i++)
            {
                TriangleRotation requiredRotation = (TriangleRotation)stripRotations[i];

                ModelBase.FaceDef rotated = new ModelBase.FaceDef(3);
                rotated.m_Vertices[0] = m_Faces[stripIndices[i]].m_Face.m_Vertices[((int)(0 + requiredRotation) % 3)];
                rotated.m_Vertices[1] = m_Faces[stripIndices[i]].m_Face.m_Vertices[((int)(1 + requiredRotation) % 3)];
                rotated.m_Vertices[2] = m_Faces[stripIndices[i]].m_Face.m_Vertices[((int)(2 + requiredRotation) % 3)];

                tStrip.m_Faces.Add(rotated);
            }

            return new Tuple<ModelBase.FaceListDef, List<int>>(tStrip, stripIndices);
        }

        // Return true if two of a triangle's vertices are the same
        protected override bool IsDegenerateFace(ModelBase.FaceDef triangle)
        {
            List<ModelBase.VertexDef> verts = triangle.m_Vertices.ToList();
            return (verts.Count() != verts.Distinct().Count());
        }

        enum TriangleEdge
        {
            Edge_AB = 0,
            Edge_BC = 1, 
            Edge_CA = 2
        };

        enum TriangleRotation
        {
            TRot_None = 0,
            TRot_LS = 1,
            TRot_LSLS = 2
        };
    }
}
