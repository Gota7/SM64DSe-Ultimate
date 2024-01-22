/* Attempts to convert a list of separate quads to quadrilateral strips.
 * 
 *   Separate Quads          Quadliteral Strips         Prohibited Quads
 *  v0__v3                 v0__v2____v4     v10__    v0__v3     v4
 *   /  \   v4____v7        /  \     |\ _____ / /v11   \/       |\
 *  /    \   |    \        /    \    | |v6 v8| /       /\     v5| \
 * /______\  |_____\      /______\___|_|_____|/       /__\     /___\
 * v1    v2  v5    v6     v1    v3  v5 v7   v9       v2   v1   v6   v7
 * 
 * The vertices are normally arranged anti-clockwise, except that .. quad-strips are sorts of "up-down" 
 * arranged (whereas "up" and "down" may be anywhere due to rotation). Other arrangements may result in 
 * quads with crossed lines, or may swap the front and back sides of the polygon (above examples are 
 * showing the front sides).
 * 
 * ModelBase Face:
 * Edge AB: v0 - v1
 * Edge BC: v1 - v2
 * Edge CD: v2 - v3
 * Edge DA: v3 - v0
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace SM64DSe.ImportExport
{
    public class QuadsToStripsConverter : BaseStripConverter
    {
        public QuadsToStripsConverter(ModelBase.FaceListDef faceList) : base(faceList) {}

        protected override void verifyCorrectVertexCount(ModelBase.FaceListDef faceList)
        {
            for (int i = 0; i < faceList.m_Faces.Count; i++)
            {
                if (faceList.m_Faces[i].m_NumVertices != 4)
                {
                    throw new ArgumentException("The provided FaceListDef must contain only quads.");
                }
            }
        }

        public override List<ModelBase.FaceListDef> Stripify(bool keepVertexOrderDuringStripping = false)
        {
            for (int i = 0; i < m_Faces.Count; i++)
            {
                ModelBase.FaceDef face = m_Faces[i].m_Face;

                List<int> vertexALinked = GetVertex(face.m_Vertices[0]).m_LinkedFaces;
                List<int> vertexBLinked = GetVertex(face.m_Vertices[1]).m_LinkedFaces;
                List<int> vertexCLinked = GetVertex(face.m_Vertices[2]).m_LinkedFaces;
                List<int> vertexDLinked = GetVertex(face.m_Vertices[3]).m_LinkedFaces;

                // Shares edge AB if both faces reference vertices A and B
                var edgeAB = vertexALinked.Intersect(vertexBLinked).Except(new int[] { i });
                var edgeBC = vertexBLinked.Intersect(vertexCLinked).Except(new int[] { i });
                var edgeCD = vertexCLinked.Intersect(vertexDLinked).Except(new int[] { i });
                var edgeDA = vertexDLinked.Intersect(vertexALinked).Except(new int[] { i });

                if (edgeAB.Count() > 0)
                    m_Faces[i].m_LinkedFaces[(int)QuadEdge.Edge_AB].AddRange(edgeAB);
                if (edgeBC.Count() > 0)
                    m_Faces[i].m_LinkedFaces[(int)QuadEdge.Edge_BC].AddRange(edgeBC);
                if (edgeCD.Count() > 0)
                    m_Faces[i].m_LinkedFaces[(int)QuadEdge.Edge_CD].AddRange(edgeCD);
                if (edgeDA.Count() > 0)
                    m_Faces[i].m_LinkedFaces[(int)QuadEdge.Edge_DA].AddRange(edgeDA);
            }

            m_FacesToProcess.AddRange(m_Faces);

            // Sort by number of neighbours
            if (!keepVertexOrderDuringStripping)
            {
                m_FacesToProcess.Sort((a, b) => GetNumLinked(a).CompareTo(GetNumLinked(b)));
            }

            List<ModelBase.FaceListDef> fStrips = new List<ModelBase.FaceListDef>();

            // For storing faces that have no neighbours or don't end up in a strip
            ModelBase.FaceListDef separateFaces = new ModelBase.FaceListDef(ModelBase.PolyListType.Polygons);

            FaceLinked currentFace = null;

            while (m_FacesToProcess.Count > 0)
            {
                while (true)
                {
                    if (m_FacesToProcess.Count <= 0)
                    {
                        currentFace = null;
                        break;
                    }

                    currentFace = m_FacesToProcess[0];
                    m_FacesToProcess.RemoveAt(0);
                    if (currentFace.m_Marked)
                        continue;
                    else
                        break;
                }
                if (currentFace == null) break;

                List<FaceLinked>[] linked = GetLinked(currentFace);
                int numLinked = GetNumLinked(currentFace);

                currentFace.m_Marked = true;

                if (numLinked == 0)
                {
                    separateFaces.m_Faces.Add(currentFace.m_Face);
                    continue;
                }

                // For each face build a strip off each of its edges and keep the longest one, discarding the rest
                Tuple<ModelBase.FaceListDef, List<int>> qStripAB =
                    GetStripAndIndicesForStartingEvenEdge(currentFace, QuadEdge.Edge_AB);
                Tuple<ModelBase.FaceListDef, List<int>> qStripBC =
                    GetStripAndIndicesForStartingEvenEdge(currentFace, QuadEdge.Edge_BC);
                Tuple<ModelBase.FaceListDef, List<int>> qStripCD =
                    GetStripAndIndicesForStartingEvenEdge(currentFace, QuadEdge.Edge_CD);
                Tuple<ModelBase.FaceListDef, List<int>> qStripDA =
                    GetStripAndIndicesForStartingEvenEdge(currentFace, QuadEdge.Edge_DA);

                List<Tuple<ModelBase.FaceListDef, List<int>>> candidates = new List<Tuple<ModelBase.FaceListDef, List<int>>>()
                    { qStripAB, qStripBC, qStripCD, qStripDA };

                List<int> stripLengths = new List<int>();
                foreach (Tuple<ModelBase.FaceListDef, List<int>> strip in candidates)
                    stripLengths.Add(strip.Item1.m_Faces.Count);
                int longestStripIndex = stripLengths.IndexOf(stripLengths.Max());

                Tuple<ModelBase.FaceListDef, List<int>> longestStrip = candidates[longestStripIndex];

                if (longestStrip.Item1.m_Faces.Count == 0)
                {
                    separateFaces.m_Faces.Add(currentFace.m_Face);
                    continue;
                }

                foreach (int faceIdx in longestStrip.Item2)
                    m_Faces[faceIdx].m_Marked = true;

                fStrips.Add(longestStrip.Item1);
            }

            if (separateFaces.m_Faces.Count > 0) fStrips.Add(separateFaces);

            return fStrips;
        }

        Tuple<ModelBase.FaceListDef, List<int>> GetStripAndIndicesForStartingEvenEdge(
            FaceLinked start, QuadEdge startForwardEdge)
        {
            List<int> stripIndices = new List<int>();
            List<QuadRotation> stripRotations = new List<QuadRotation>();

            List<FaceLinked> linked = GetLinked(start)[(int)startForwardEdge];
            FaceLinked bestNeighbour = DetermineBestNextNeighbour(start, linked, (int)startForwardEdge);

            if (bestNeighbour == null) 
                return new Tuple<ModelBase.FaceListDef, List<int>>(new ModelBase.FaceListDef(), new List<int>());

            // The rotation required so that the "start forward edge" (the linking edge) is made up 
            // of the 3rd and 4th vertices (the 3rd edge)
            // AB(0): 2; BC(1): 3; CD(2): 0; DA(3): 1
            QuadRotation startRotation = (QuadRotation)((int)(startForwardEdge + 2) % 4);

            FaceLinked f = start;
            QuadEdge currentEdge = startForwardEdge;
            QuadRotation currentRotation = startRotation;
            int index_f = -1;
            while (f != null && !stripIndices.Contains((index_f = m_Faces.IndexOf(f))))
            {
                stripIndices.Add(index_f);
                stripRotations.Add(currentRotation);

                linked = GetLinked(f)[(int)currentEdge];
                bestNeighbour = DetermineBestNextNeighbour(f, linked, (int)currentEdge);

                f = bestNeighbour;

                if (f != null)
                {
                    // Determine rotation and the edge to be used to get the next face

                    // The edge of the vertices which match the preceding triangle's
                    QuadEdge linkBackEdge = QuadEdge.Edge_AB;
                    // The vertices which match the preceding triangle's
                    ModelBase.VertexDef[] currentMatchedEdge = new ModelBase.VertexDef[2];
                    FaceLinked previous = m_Faces[stripIndices[stripIndices.Count - 1]];
                    currentMatchedEdge[0] = previous.m_Face.m_Vertices[(int)(currentEdge + 0) % 4];
                    currentMatchedEdge[1] = previous.m_Face.m_Vertices[(int)(currentEdge + 1) % 4];
                    
                    // Find the edge in the current face which matches that from the preceding face.
                    for (int i = 0; i < 4; i++)
                    {
                        ModelBase.VertexDef[] edge = new ModelBase.VertexDef[2];
                        edge[0] = f.m_Face.m_Vertices[(i + 0) % 4];
                        edge[1] = f.m_Face.m_Vertices[(i + 1) % 4];
                        if (edge.Except(currentMatchedEdge).Count() == 0)
                        {
                            linkBackEdge = (QuadEdge)i;
                            break;
                        }
                    }

                    // The next edge is the edge "opposite" the linking edge
                    QuadEdge nextEdge = (QuadEdge)((int)(linkBackEdge + 2) % 4);

                    // Required rotation is the rotation required so that the linking edge is made up of the 
                    // 1st and 2nd vertices, the next linking edge being the 3rd and 4th vertices
                    QuadRotation requiredRotation = (QuadRotation)((int)(nextEdge + 2) % 4);

                    currentRotation = requiredRotation;
                    currentEdge = nextEdge;
                }

            }

            ModelBase.FaceListDef qStrip = new ModelBase.FaceListDef(ModelBase.PolyListType.QuadrilateralStrip);

            for (int i = 0; i < stripIndices.Count; i++)
            {
                QuadRotation requiredRotation = (QuadRotation)stripRotations[i];

                ModelBase.FaceDef rotated = new ModelBase.FaceDef(4);
                rotated.m_Vertices[0] = m_Faces[stripIndices[i]].m_Face.m_Vertices[((int)(0 + requiredRotation) % 4)];
                rotated.m_Vertices[1] = m_Faces[stripIndices[i]].m_Face.m_Vertices[((int)(1 + requiredRotation) % 4)];
                rotated.m_Vertices[2] = m_Faces[stripIndices[i]].m_Face.m_Vertices[((int)(2 + requiredRotation) % 4)];
                rotated.m_Vertices[3] = m_Faces[stripIndices[i]].m_Face.m_Vertices[((int)(3 + requiredRotation) % 4)];

                qStrip.m_Faces.Add(rotated);
            }

            return new Tuple<ModelBase.FaceListDef, List<int>>(qStrip, stripIndices);
        }

        // Return true if at least two of a quads's vertices are the same
        protected override bool IsDegenerateFace(ModelBase.FaceDef quad)
        {
            List<ModelBase.VertexDef> verts = quad.m_Vertices.ToList();
            return verts.Distinct().Count() < 3;
        }

        enum QuadEdge
        {
            Edge_AB = 0,
            Edge_BC = 1, 
            Edge_CD = 2,
            Edge_DA = 3
        };

        // LS: left shift, A -> D, B -> A etc.
        enum QuadRotation
        {
            QRot_None = 0,
            QRot_LS = 1,
            QRot_LSLS = 2,
            QRot_LSLSLS = 3
        };
    }
}
