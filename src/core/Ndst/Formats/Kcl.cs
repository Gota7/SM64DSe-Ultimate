using System.Collections.Generic;
using System.IO;
using Ndst.Models;

namespace Ndst.Formats {

    // SM64DS KCL model.
    public class Kcl : IFormat {
        public uint VersionMaybe = 0x050000;
        public Fix20x12i BoundingBoxX;
        public Fix20x12i BoundingBoxY;
        public Fix20x12i BoundingBoxZ;
        public int MaskX;
        public int MaskY;
        public int MaskZ;
        public int CoordinateShift;
        public int ShiftY;
        public int ShiftZ;
        List<Fix20x12i[]> Vertices;
        List<Fix4x12i[]> Normals;
        List<Plane> Planes = new List<Plane>();

        // Plane with collision info.
        public class Plane {
            public Fix20x12i Length;
            public ushort VertexInd;
            public ushort NormalInd;
            public ushort NormalAInd;
            public ushort NormalBInd;
            public ushort NormalCInd;
            public ushort KclType;
        }

        public bool IsType(byte[] data) {
            return true;
        }

        public void Read(BinaryReader r, byte[] rawData) {

            // Header.
            long baseOff = r.BaseStream.Position;
            uint vertexOff = r.ReadUInt32();
            uint normalOff = r.ReadUInt32();
            uint planeOff = r.ReadUInt32() + 0x10;
            uint octreeOff = r.ReadUInt32();
            VersionMaybe = r.ReadUInt32();
            BoundingBoxX = r.ReadFix20x12i();
            BoundingBoxY = r.ReadFix20x12i();
            BoundingBoxZ = r.ReadFix20x12i();
            MaskX = r.ReadInt32();
            MaskY = r.ReadInt32();
            MaskZ = r.ReadInt32();
            CoordinateShift = r.ReadInt32();
            ShiftY = r.ReadInt32();
            ShiftZ = r.ReadInt32();
            int numNodes = ((~MaskX >> CoordinateShift) + 1) * ((~MaskY >> CoordinateShift) + 1) * ((~MaskZ >> CoordinateShift) + 1);

            // Get vertices.
            r.BaseStream.Position = baseOff + vertexOff;
            int numVertices = (int)(normalOff - vertexOff) / 0xC;
            Vertices = new List<Fix20x12i[]>();
            for (int i = 0; i < numVertices; i++) {
                Vertices.Add(new Fix20x12i[] { r.ReadFix20x12i(), r.ReadFix20x12i(), r.ReadFix20x12i() });
            }

            // Get normals.
            r.BaseStream.Position = baseOff + normalOff;
            int numNormals = (int)(planeOff - normalOff) / 0x6;
            Normals = new List<Fix4x12i[]>();
            for (int i = 0; i < numNormals; i++) {
                Normals.Add(new Fix4x12i[] { r.ReadFix4x12i(), r.ReadFix4x12i(), r.ReadFix4x12i() });
            }

            // Get planes.
            r.BaseStream.Position = baseOff + planeOff;
            int numPlanes = (int)(octreeOff - (planeOff + 0x10)) / 0x10;
            for (int i = 0; i < numPlanes; i++) {

                // Plane properties.
                Plane p = new Plane();
                p.Length = r.ReadFix20x12i();

                // Indices.
                p.VertexInd = r.ReadUInt16();
                p.NormalInd = r.ReadUInt16();
                p.NormalAInd = r.ReadUInt16();
                p.NormalBInd = r.ReadUInt16();
                p.NormalCInd = r.ReadUInt16();
                p.KclType = r.ReadUInt16();

                // Add plane.
                Planes.Add(p);

            }

        }

        public void Write(BinaryWriter w) {
            
        }

        // Extract as an OBJ.
        public void Extract(string path) {
            
            // Create folder.
            Directory.CreateDirectory(path); 

            // Create new model.
            Model m = new Model();
            m.AddNode(new Node() { Meshes = new List<int>() { 0 } });
            Mesh h = new Mesh();
            m.Meshes.Add(h);

            // Add each plane to the model.
            foreach (var p in Planes) {

                // Add indices.
                int indCount = 0;
                Face f = new Face();
                f.VertexIndices.Add(indCount);
                f.VertexIndices.Add(indCount + 1);
                f.VertexIndices.Add(indCount + 2);

                // Get all positions.
                Vec3 Vertex = ToVec20(Vertices[p.VertexInd]) / 15.625f;
                Vec3 Normal = ToVec4(Normals[p.NormalInd]) * 4;
                Vec3 NormalA = ToVec4(Normals[p.NormalAInd]) * 4;
                Vec3 NormalB = ToVec4(Normals[p.NormalBInd]) * 4;
                Vec3 NormalC = ToVec4(Normals[p.NormalCInd]) * 4;
                Vec3 CrossA = Normal % NormalA;
                Vec3 CrossB = Normal % NormalB;
                float DotA = CrossA * NormalC;
                float DotB = CrossB * NormalC;
                Vec3 v1 = Vertex;
                Vec3 v2 = v1 + CrossB * (DotB != 0f ? (float)(p.Length / 16000) / DotB : 0f);
                Vec3 v3 = v1 + CrossA * (DotA != 0f ? (float)(p.Length / 16000) / DotA : 0f);
                h.Vertices.Add(v1);
                h.Vertices.Add(v2);
                h.Vertices.Add(v3);
                
                // Add face.
                h.Faces.Add(f);

            }
            m.SaveModel(path + "/Model.obj", "Obj");

            // ToVec.
            Vec3 ToVec4(Fix4x12i[] val) {
                return new Vec3((float)val[0], (float)val[1], (float)val[2]);
            }
            Vec3 ToVec20(Fix20x12i[] val) {
                return new Vec3((float)val[0], (float)val[1], (float)val[2]);
            }

        }

        public void Pack(string path) {
            throw new System.NotImplementedException();
        }

        public string GetFormat() {
            return "Kcl";
        }

        public byte[] ContainedFile() {
            return null;
        }

        public bool IsOfFormat(string str) {
            return str.Equals("Kcl");
        }

        public string GetPathExtension() => "";

    }

}