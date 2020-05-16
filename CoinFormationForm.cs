using Microsoft.Win32;
using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SM64DSe {

    /// <summary>
    /// Make coin formation adding not a pain.
    /// </summary>
    public partial class CoinFormationForm : Form {

        /// <summary>
        /// Level editor.
        /// </summary>
        private LevelEditorForm LE;

        /// <summary>
        /// The main object.
        /// </summary>
        private LevelObject Obj;

        /// <summary>
        /// Center.
        /// </summary>
        private Vector3 Center;

        /// <summary>
        /// Slaves.
        /// </summary>
        private List<LevelObject> Slaves = new List<LevelObject>();

        /// <summary>
        /// Initialized.
        /// </summary>
        private bool Initialized = false;

        /// <summary>
        /// If ok.
        /// </summary>
        private bool Ok = false;

        /// <summary>
        /// If quitting is forced.
        /// </summary>
        private bool ForceQuitted = false;

        /// <summary>
        /// Create a new coin formation placer.
        /// </summary>
        /// <param name="le">Level editor form.</param>
        public CoinFormationForm(LevelEditorForm le) {
            InitializeComponent();
            this.FormClosing += OnClosing;
            LE = le;
        }

        /// <summary>
        /// Show this for an object.
        /// </summary>
        /// <param name="obj">Object.</param>
        public void ShowForObject(LevelObject obj) {
            TopMost = true;
            formationType.SelectedIndex = 0;
            Center = obj.Position;
            val_posX.Value = (decimal)Center.X;
            val_posY.Value = (decimal)Center.Y;
            val_posZ.Value = (decimal)Center.Z;
            Obj = obj;
            Initialized = true;
            UpdateObjects();
            Show();
        }

        /// <summary>
        /// Update objects.
        /// </summary>
        private void UpdateObjects() {

            //Make sure initialized.
            if (!Initialized) {
                return;
            }

            //Add objects if needed.
            while (Slaves.Count < numCoins.Value) {
                Slaves.Add(LE.AddObject(LevelObject.Type.SIMPLE, 37, Obj.m_Layer, Obj.m_Area));
                if (Obj.Parameters != null) Array.Copy(Obj.Parameters, Slaves.Last().Parameters, Obj.Parameters.Length);
                Slaves.Last().GenerateProperties();
            }

            //Remove objects if needed.
            while (Slaves.Count > numCoins.Value) {
                LE.RemoveObject(Slaves.Last());
                Slaves.Remove(Slaves.Last());
            }

            //Line.
            if (formationType.SelectedIndex == 0) {

                //Get the total length.
                double dist = (double)distance.Value;
                double len = (Slaves.Count - 1) * dist;

                //Get the starting position.
                Vector3 dir = -new Vector3((float)(Math.Cos(HA) * Math.Cos(VA)), (float)Math.Sin(VA), (float)(Math.Sin(HA) * Math.Cos(VA)));
                dir.Normalize();
                Vector3 start = Center - dir * (float)len / 2;

                //Set objects.
                for (int i = 0; i < Slaves.Count; i++) {
                    Slaves[i].Position = start + Scale(dist * i, dir);
                }

            }

            //Ring.
            else {

                //Get distance.
                float dist = (float)distance.Value;

                //Delta angle change.
                double da = Math.PI * 2 / Slaves.Count;

                //Get starting angles.
                double ang = 0;

                //Figure out orthonormal basis.
                Vector3 a = new Vector3((float)(Math.Cos(HA) * Math.Cos(VA)), (float)Math.Sin(VA), (float)(Math.Sin(HA) * Math.Cos(VA)));
                Vector3 b = Vector3.Cross(a, Vector3.UnitY);
                a.Normalize();
                b.Normalize();

                //Set objects.
                for (int i = 0; i < Slaves.Count; i++) {
                    Slaves[i].Position = Center + (float)(dist * Math.Cos(ang)) * a + (float)(dist * Math.Sin(ang)) * b;
                    ang += da;
                }

            }

            //Refresh.
            LE.RefreshObjects(Obj.m_Layer);

        }

        /// <summary>
        /// Scale the coin formation.
        /// </summary>
        /// <param name="s">Scale.</param>
        /// <param name="val">Value to scale.</param>
        /// <returns>Scaled value.</returns>
        private static Vector3 Scale(double s, Vector3 val) {
            return new Vector3((float)(val.X * s), (float)(val.Y * s), (float)(val.Z * s));
        }

        /// <summary>
        /// Horizontal angle.
        /// </summary>
        /// <returns>The horizontal angle.</returns>
        private double HA =>(double)horizontalRotation.Value * Math.PI / 180;

        /// <summary>
        /// Vertical angle.
        /// </summary>
        /// <returns>The vertical angle.</returns>
        private double VA => (double)verticalRotation.Value * Math.PI / 180;

        private void numCoins_ValueChanged(object sender, EventArgs e) {
            UpdateObjects();
        }

        private void distance_ValueChanged(object sender, EventArgs e) {
            UpdateObjects();
        }

        private void horizontalRotation_ValueChanged(object sender, EventArgs e) {
            UpdateObjects();
        }

        private void verticalRotation_ValueChanged(object sender, EventArgs e) {
            UpdateObjects();
        }

        private void formationType_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateObjects();
        }

        private void val_posX_ValueChanged(object sender, EventArgs e) {
            Center.X = (float)val_posX.Value;
            UpdateObjects();
        }

        private void val_posY_ValueChanged(object sender, EventArgs e) {
            Center.Y = (float)val_posY.Value;
            UpdateObjects();
        }

        private void val_posZ_ValueChanged(object sender, EventArgs e) {
            Center.Z = (float)val_posZ.Value;
            UpdateObjects();
        }

        private void ok_Click(object sender, EventArgs e) {
            Ok = true;
            Close();
        }

        private void OnClosing(object sender, EventArgs e) {
            if (!Ok) {
                foreach (var s in Slaves) {
                    LE.RemoveObject(s);
                }
                Slaves.Clear();
            } else if (!ForceQuitted) {
                try { LE.RemoveObject(Obj); } catch { }
            }
            LE.RefreshObjects(Obj.m_Layer);
        }

        /// <summary>
        /// Force quit.
        /// </summary>
        public void ForceQuit() {
            ForceQuitted = true;
            Close();
        }

    }

}
