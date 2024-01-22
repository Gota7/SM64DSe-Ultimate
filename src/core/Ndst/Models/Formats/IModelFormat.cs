namespace Ndst.Models {

    // A 3d model format.
    public interface IModelFormat {
        string FormatName();
        bool IsOfFormat(string filePath);
        Model FromFile(string filePath);
        void WriteFile(string filePath, Model m);
    }

}