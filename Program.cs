/*
    Copyright 2012 Kuribo64

    This file is part of SM64DSe.

    SM64DSe is free software: you can redistribute it and/or modify it under
    the terms of the GNU General Public License as published by the Free
    Software Foundation, either version 3 of the License, or (at your option)
    any later version.

    SM64DSe is distributed in the hope that it will be useful, but WITHOUT ANY 
    WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS 
    FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along 
    with SM64DSe. If not, see http://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using System.IO;

namespace SM64DSe
{
    static class Program
    {
        public static string AppTitle = "SM64DS Editor ULTIMATE";
        public static string AppVersion = "v2.3.7";
        public static string AppDate = "May 15, 2020";

        public static string ServerURL = "http://kuribo64.net/";

        public static string m_ROMPath;
        public static NitroROM m_ROM;

        public static List<LevelEditorForm> m_LevelEditors;

        public static Dictionary<string, int> shaderPrograms = new Dictionary<string, int>();

        //code from https://www.codeproject.com/Articles/1167387/OpenGL-with-OpenTK-in-Csharp-Part-Compiling-Shader
        private static void LoadShader(string name, string vertShaderName, string fragShaderName)
        {
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader,
            File.ReadAllText(@"Shaders\"+ vertShaderName + ".vert"));
            GL.CompileShader(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader,
            File.ReadAllText(@"Shaders\"+ fragShaderName + ".frag"));
            GL.CompileShader(fragmentShader);

            int program = GL.CreateProgram();
            GL.AttachShader(program, vertexShader);
            GL.AttachShader(program, fragmentShader);
            GL.LinkProgram(program);

            GL.DetachShader(program, vertexShader);
            GL.DetachShader(program, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            if (shaderPrograms.ContainsKey(name))
                shaderPrograms[name] = program;
            else
                shaderPrograms.Add(name, program);
        }

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(args));
        }
    }
}
