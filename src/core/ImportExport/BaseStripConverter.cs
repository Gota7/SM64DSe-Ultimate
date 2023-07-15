using System;
using System.Collections.Generic;
using System.Linq;

namespace SM64DSe.ImportExport
{
    public abstract class BaseStripConverter
    {
        protected List<VertexLinked> m_Vertices;
        protected List<FaceLinked> m_Faces;
        protected List<FaceLinked> m_FacesToProcess;

        public BaseStripConverter(ModelBase.FaceListDef faceList)
        {
            verifyCorrectVertexCount(faceList);

            m_Vertices = new List<VertexLinked>();
            m_Faces = new List<FaceLinked>();
            m_FacesToProcess = new List<FaceLinked>();

            for (int i = 0; i < faceList.m_Faces.Count; i++)
            {
                if (IsDegenerateFace(faceList.m_Faces[i]))
                {
                    faceList.m_Faces.RemoveAt(i);
                }
            }

            for (int i = 0; i < faceList.m_Faces.Count; i++)
            {
                m_Faces.Add(new FaceLinked(faceList.m_Faces[i]));
            }

            for (int i = 0; i < m_Faces.Count; i++)
            {
                ModelBase.FaceDef face = m_Faces[i].m_Face;

                for (int j = 0; j < face.m_NumVertices; j++)
                {
                    VertexLinked vertex = new VertexLinked(face.m_Vertices[j]);

                    int index = m_Vertices.IndexOf(vertex);
                    if (index == -1)
                    {
                        m_Vertices.Add(vertex);
                        index = m_Vertices.Count - 1;
                    }

                    m_Vertices[index].m_LinkedFaces.Add(i);
                }
            }
        }
        
        protected abstract void verifyCorrectVertexCount(ModelBase.FaceListDef faceList);

        public abstract List<ModelBase.FaceListDef> Stripify(bool keepVertexOrderDuringStripping = false);

        protected FaceLinked DetermineBestNextNeighbour(FaceLinked face, List<FaceLinked> neighbours, int edge,
            bool tieBreak = false)
        {
            if (neighbours.Count == 0) return null;

            if (neighbours.Count == 1) return neighbours[0];

            neighbours.Sort((a, b) => GetNumLinked(a).CompareTo(GetNumLinked(b)));

            FaceLinked[] twoTop = new FaceLinked[2];
            twoTop[0] = neighbours[0]; twoTop[1] = neighbours[1];

            int[] numLinkedOfNeighbours = new int[] { GetNumLinked(twoTop[0]), GetNumLinked(twoTop[1]) };

            if (numLinkedOfNeighbours[0] < numLinkedOfNeighbours[1])
            {
                return twoTop[0];
            }
            else if (numLinkedOfNeighbours[1] < numLinkedOfNeighbours[0])
            {
                return twoTop[1];
            }
            else if (!tieBreak)
            {
                List<FaceLinked> firstLinked = GetLinked(twoTop[0])[(int)edge];
                List<FaceLinked> secondLinked = GetLinked(twoTop[1])[(int)edge];

                FaceLinked firstBest = DetermineBestNextNeighbour(twoTop[0], firstLinked, edge, true);
                FaceLinked secondBest = DetermineBestNextNeighbour(twoTop[1], secondLinked, edge, true);

                if (firstBest == null) { return twoTop[1]; }
                else if (secondBest == null) { return twoTop[0]; }
                else if (GetNumLinked(firstBest) < GetNumLinked(secondBest)) { return twoTop[0]; }
                else { return twoTop[1]; }
            }
            else if (tieBreak)
            {
                return twoTop[0];
            }
            else
            {
                return null;
            }
        }

        protected List<FaceLinked>[] GetLinked(FaceLinked face)
        {
            List<FaceLinked>[] linked = new List<FaceLinked>[4];

            for (int i = 0; i < face.VertexCount; i++)
            {
                List<int> linkedIndices = face.m_LinkedFaces[i];
                for (int j = 0; j < linkedIndices.Count; j++)
                {
                    // Make sure it doesn't count itself as a neighbour
                    if (m_Faces[m_Faces.IndexOf(face)].Equals(linkedIndices[j]))
                    {
                        linkedIndices.RemoveAt(j);
                        continue;
                    }
                    // Ignore marked faces (already in a strip)
                    if (m_Faces[linkedIndices[j]].m_Marked)
                    {
                        linkedIndices.RemoveAt(j);
                        continue;
                    }
                    // Ignore faces which share all vertices
                    if (m_Faces[linkedIndices[j]].m_Face.m_Vertices.Except(face.m_Face.m_Vertices).Count() == 0)
                    {
                        linkedIndices.RemoveAt(j);
                        continue;
                    }
                }
                linked[i] = new List<FaceLinked>();
                for (int j = 0; j < linkedIndices.Count; j++)
                {
                    linked[i].Add(m_Faces[linkedIndices[j]]);
                }
            }

            return linked;
        }

        protected int GetNumLinked(FaceLinked face)
        {
            List<FaceLinked>[] linked = GetLinked(face);
            int count = 0;
            for (int i = 0; i < face.VertexCount; i++)
            {
                count += linked[i].Count;
            }
            return count;
        }

        protected abstract bool IsDegenerateFace(ModelBase.FaceDef face);

        protected VertexLinked GetVertex(ModelBase.VertexDef vertexDef)
        {
            return m_Vertices[m_Vertices.IndexOf(new VertexLinked(vertexDef))];
        }

        // Holds a vertex and a list of the indices of the faces that reference it
        protected class VertexLinked
        {
            Tuple<ModelBase.VertexDef, List<int>> m_VertexLinkedFaceIndices;

            public VertexLinked(ModelBase.VertexDef vertex)
            {
                m_VertexLinkedFaceIndices = new Tuple<ModelBase.VertexDef, List<int>>(vertex, new List<int>());
            }

            public ModelBase.VertexDef m_Vertex
            {
                get
                {
                    return m_VertexLinkedFaceIndices.Item1;
                }
            }

            public List<int> m_LinkedFaces
            {
                get
                {
                    return m_VertexLinkedFaceIndices.Item2;
                }
            }

            public override bool Equals(object obj)
            {
                var item = obj as VertexLinked;

                if (item == null) return false;

                return m_Vertex.Equals(item.m_Vertex);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 13;
                    hash = hash * 7 + m_Vertex.GetHashCode();
                    return hash;
                }
            }
        }

        // Holds a face and the indices of its neighbours
        protected class FaceLinked
        {
            int m_VertexCount;
            Tuple<ModelBase.FaceDef, List<int>[]> m_FaceLinkedNeighbours;
            public bool m_Marked;

            public FaceLinked(ModelBase.FaceDef face)
            {
                m_VertexCount = face.m_NumVertices;
                List<int>[] linkedNeighbourIndices = new List<int>[m_VertexCount];
                for (int i = 0; i < m_VertexCount; i++) { linkedNeighbourIndices[i] = null; }
                m_FaceLinkedNeighbours = new Tuple<ModelBase.FaceDef, List<int>[]>(face, linkedNeighbourIndices);
                for (int i = 0; i < m_VertexCount; i++)
                {
                    m_LinkedFaces[i] = new List<int>();
                }
                m_Marked = false;
            }

            public int VertexCount
            {
                get
                {
                    return m_VertexCount;
                }
            }

            public ModelBase.FaceDef m_Face
            {
                get
                {
                    return m_FaceLinkedNeighbours.Item1;
                }
            }

            public List<int>[] m_LinkedFaces
            {
                get
                {
                    return m_FaceLinkedNeighbours.Item2;
                }
            }

            public override bool Equals(object obj)
            {
                var item = obj as FaceLinked;

                if (item == null) return false;

                return m_Face.Equals(item.m_Face);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 13;
                    hash = hash * 7 + m_Face.GetHashCode();
                    return hash;
                }
            }
        }
    }
}
