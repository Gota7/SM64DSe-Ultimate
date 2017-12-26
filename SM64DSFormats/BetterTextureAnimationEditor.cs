using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace SM64DSe.SM64DSFormats
{
    public partial class BetterTextureAnimationEditor : Form
    {
        private LevelEditorForm m_parent;
        private BMD m_BMD;
        private Image m_previewTexture;

        private Dictionary<string, LevelTexAnim.Def> m_entries;
        private List<LevelTexAnim> m_animations;
        private LevelTexAnim.Def m_animEntry;
        private int m_animId;

        private Dictionary<string, LevelTexAnim.Def>[] m_unsavedEntries;

        private float m_translationX = 0;
        private float m_translationY = 0;
        private float m_rotation = 0;
        private float m_scale = 1;
        
        private System.Windows.Forms.Timer m_texAnimTimer;
        private int m_texAnimFrame = 0;

        private int m_timelineFrame = 0;
        private int m_timelinePart = 0;
        private int m_timelineParts = 1;
        private float m_valueCeiling = 1;
        private float m_valueFloor = 0;

        private bool m_settingValue;
        private List<float> m_animationValues;
        private bool m_settingKeyframeValue;
        private SortedDictionary<int, float> m_keyFrames;

        private enum AnimationProperty
        {
            TRANSLATION_X = 0,
            TRANSLATION_Y = 1,
            ROTATION = 2,
            SCALE = 3
        }

        private float m_zoomFactor = 1;
        public float ZoomFactor
        {
            get
            {
                return m_zoomFactor;
            }
            set
            {
                m_zoomFactor = Math.Min(Math.Max(0.1f,value),4);
                textureView.Refresh();
            }
        }

        //all timer commands
        private void InitTimer()
        {
            m_texAnimTimer = new System.Windows.Forms.Timer();
            m_texAnimTimer.Interval = (int)(1000f / 30f);
            m_texAnimTimer.Tick += new EventHandler(m_AnimationTimer_Tick);
        }

        private void StartAnimation()
        {
            m_texAnimFrame = 0;
            m_texAnimTimer.Enabled = true;
            m_texAnimTimer.Start();

            SetupGUIForPlayMode();

        }

        private void PauseAnimation()
        {
            m_texAnimTimer.Enabled = false;
            m_texAnimTimer.Stop();

            SetupGUIForPauseMode();
        }

        private void ResumeAnimation()
        {
            m_texAnimTimer.Enabled = true;
            m_texAnimTimer.Start();

            SetupGUIForPlayMode();
        }

        private void StopAnimation()
        {
            m_texAnimTimer.Stop();
            m_texAnimTimer.Enabled = false;
            m_texAnimFrame = 0;
            if (m_animEntry != null)
            {
                m_translationX = LevelTexAnim.AnimationValue(m_animEntry.m_TranslationXValues, 0, 0);
                m_translationY = LevelTexAnim.AnimationValue(m_animEntry.m_TranslationYValues, 0, 0);
                m_rotation = LevelTexAnim.AnimationValue(m_animEntry.m_RotationValues, 0, 0);
                m_scale = LevelTexAnim.AnimationValue(m_animEntry.m_ScaleValues, 0, 0);
            }
            else
            {
                m_translationX = 0;
                m_translationY = 0;
                m_rotation = 0;
                m_scale = 1;
            }

            SetupGUIForPauseMode();
            UpdateAnimation();
        }

        private void UpdateAnimation()
        {
            if (m_animEntry != null)
            {
                m_translationX = LevelTexAnim.AnimationValue(m_animEntry.m_TranslationXValues, m_texAnimFrame, (int)m_animations[m_animId].m_NumFrames);
                m_translationY = LevelTexAnim.AnimationValue(m_animEntry.m_TranslationYValues, m_texAnimFrame, (int)m_animations[m_animId].m_NumFrames);
                m_rotation = LevelTexAnim.AnimationValue(m_animEntry.m_RotationValues, m_texAnimFrame, (int)m_animations[m_animId].m_NumFrames);
                m_scale = LevelTexAnim.AnimationValue(m_animEntry.m_ScaleValues, m_texAnimFrame, (int)m_animations[m_animId].m_NumFrames);
            }
            if (valueSettingPanel2.Visible)
            {
                if ((AnimationProperty)cbSelectProperty.SelectedIndex==AnimationProperty.TRANSLATION_X)
                    m_translationX = InterpolatedValue(m_keyFrames, m_texAnimFrame) * (float)numScaling.Value;
                else if ((AnimationProperty)cbSelectProperty.SelectedIndex == AnimationProperty.TRANSLATION_Y)
                    m_translationY = InterpolatedValue(m_keyFrames, m_texAnimFrame) * (float)numScaling.Value;
                else if ((AnimationProperty)cbSelectProperty.SelectedIndex == AnimationProperty.ROTATION)
                    m_rotation = InterpolatedValue(m_keyFrames, m_texAnimFrame) * (float)numScaling.Value;
                else if ((AnimationProperty)cbSelectProperty.SelectedIndex == AnimationProperty.SCALE)
                    m_scale = InterpolatedValue(m_keyFrames, m_texAnimFrame) * (float)numScaling.Value;
            }
            m_texAnimFrame %= (int)m_animations[m_animId].m_NumFrames;
            m_timelineFrame = m_texAnimFrame % 120;
            m_timelinePart = (m_texAnimFrame / 120) % m_timelineParts;

            if (!m_texAnimTimer.Enabled)
            {
                //decide which buttons should be disabled
                btnStepLeft.Enabled = (m_texAnimFrame > 0);
                btnStepRight.Enabled = true;

                btnPrevPart.Enabled = (m_timelinePart > 0);
                btnNextPart.Enabled = (m_timelinePart < m_timelineParts);

                if (m_animEntry != null)
                {
                    m_settingValue = true;
                    numValue.Value = (decimal)m_animationValues[Math.Min(m_animationValues.Count - 1, m_texAnimFrame)];
                    m_settingValue = false;
                }
                if (valueSettingPanel2.Visible)
                {
                    m_settingKeyframeValue = true;
                    numKeyframeValue.Value = (decimal)InterpolatedValue(m_keyFrames, m_texAnimFrame);
                    m_settingKeyframeValue = false;
                    btnDeleteKeyframe.Enabled = m_keyFrames.ContainsKey(m_texAnimFrame);
                }
            }
            textureView.Refresh();
            timelinePanel.Refresh();
        }

        //deactivates most interaction cntrols
        private void SetupGUIForPlayMode()
        {
            numValue.Enabled = false;
            btnSaveAnim.Enabled = false;
            btnCreateNew.Enabled = false;
            btnSetKeyframes.Enabled = false;
            checkSetAll.Enabled = false;
            btnDeleteAnim.Enabled = false;

            //disable all frame related controls
            btnStepLeft.Enabled = false;
            btnStepRight.Enabled = false;

            btnPrevPart.Enabled = false;
            btnNextPart.Enabled = false;

            btnGenerateAnim.Enabled = false;
            numKeyframeValue.Enabled = false;
            btnReset.Enabled = false;
            btnCancelKeyFrames.Enabled = false;
            btnDeleteKeyframe.Enabled = false;
        }

        //activates most interaction controls
        private void SetupGUIForPauseMode()
        {
            numValue.Enabled = (m_animEntry != null);
            
            btnSaveAnim.Enabled = (m_animEntry != null);
            btnCreateNew.Enabled = true;
            btnSetKeyframes.Enabled = (m_animEntry != null);
            checkSetAll.Enabled = (m_animEntry != null);
            btnDeleteAnim.Enabled = (m_animEntry != null);

            btnGenerateAnim.Enabled = true;
            numKeyframeValue.Enabled = true;
            btnReset.Enabled = true;
            btnCancelKeyFrames.Enabled = true;
        }

        //prepare GUI for new selected frame

        //increase the frame on every timer tick
        private void m_AnimationTimer_Tick(object sender, EventArgs e)
        {
            m_texAnimFrame++;
            UpdateAnimation();
        }


        //---CONSTRUCTOR---
        public BetterTextureAnimationEditor(LevelEditorForm parent)
        {
            //setup GUI
            InitializeComponent();
            typeof(Panel).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, textureView, new object[] { true });
            typeof(Panel).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, timelinePanel, new object[] { true });
            InitTimer();
            textureView.MouseWheel += textureView_MouseWheel;
            numScaling.Tag = 1;
            valueSettingPanel2.Location = valueSettingPanel1.Location;
            valueSettingPanel1.BringToFront();

            //set private values
            m_parent = parent;
            m_BMD = m_parent.m_LevelModel;
            m_previewTexture = new Bitmap(64,64);

            m_animId = 0;
            m_animations = new List<LevelTexAnim>();

            //load Animations
            m_unsavedEntries = new Dictionary<string, LevelTexAnim.Def>[m_BMD.m_ModelChunks.Length];
            m_entries = new Dictionary<string, LevelTexAnim.Def>();
            int i = 0;
            foreach (BMD.ModelChunk chunk in m_BMD.m_ModelChunks)
            {
                LevelTexAnim animForArea = m_parent.m_Level.m_TexAnims[i];
                TreeNode chunkNode = tvMaterials.Nodes.Add(chunk.m_Name, "Area " + i + " (" + m_parent.m_Level.m_TexAnims[i].m_NumFrames + " frames)");
                chunkNode.Tag = i;

                m_unsavedEntries[i] = new Dictionary<string, LevelTexAnim.Def>();
                m_animations.Add(animForArea);
                List<LevelTexAnim.Def> newEntries = new List<LevelTexAnim.Def>();

                foreach (LevelTexAnim.Def entry in animForArea.m_Defs)
                {
                    LevelTexAnim.Def newEntry = new LevelTexAnim.Def()
                    {
                        m_DefaultScale = entry.m_DefaultScale,
                        m_MaterialName = entry.m_MaterialName,
                        m_TranslationXValues = new List<float>(entry.m_TranslationXValues),
                        m_TranslationYValues = new List<float>(entry.m_TranslationYValues),
                        m_RotationValues = new List<float>(entry.m_RotationValues),
                        m_ScaleValues = new List<float>(entry.m_ScaleValues),
                    };

                    m_entries.Add(entry.m_MaterialName, newEntry);
                }

                if (animForArea.m_NumFrames>0)
                {
                    //populate AnimationNode
                    int i2 = 0;
                    foreach (BMD.MaterialGroup matGroup in chunk.m_MatGroups)
                    {
                        if (matGroup.m_Texture != null)
                        {
                            bool hasAnimation = false;
                            foreach (LevelTexAnim.Def entry in m_animations[i].m_Defs)
                            {
                                if (entry.m_MaterialName == matGroup.m_Name)
                                    hasAnimation = true;
                            }
                            TreeNode matGroupNode = chunkNode.Nodes.Add(matGroup.m_Name, matGroup.m_Name + (hasAnimation ? "[ANIMATED]" : ""));
                            matGroupNode.Tag = i2;
                        }
                        i2++;
                    }
                }
                i++;
            }
        }

        //Prepare for new animation values
        private void InitializeTimeLine(AnimationProperty property)
        {
            m_valueFloor = 0;
            m_valueCeiling = 0;
            if (property == AnimationProperty.TRANSLATION_X)
                m_animationValues = m_animEntry.m_TranslationXValues;
            else if (property == AnimationProperty.TRANSLATION_Y)
                m_animationValues = m_animEntry.m_TranslationYValues;
            else if (property == AnimationProperty.ROTATION)
            {
                m_animationValues = new List<float>();
                foreach (float value in m_animEntry.m_RotationValues)
                {
                    m_animationValues.Add(Helper.Wrap(value,360f));
                }
            }
            else
                m_animationValues = m_animEntry.m_ScaleValues;
            foreach (float value in m_animationValues)
            {
                if (value < m_valueFloor)
                    m_valueFloor = value;
                else if (value > m_valueCeiling)
                    m_valueCeiling = value;
            }
            if (m_animationValues.Count == 0)
                m_animationValues.Add((property == AnimationProperty.SCALE) ? 1 : 0);
            UpdateAnimation();
        }


        //Painting
        private void textureView_Paint(object sender, PaintEventArgs e)
        {
            float width = e.ClipRectangle.Width;
            float height = e.ClipRectangle.Height;

            Rectangle m_UVwindowRect = new Rectangle(0, 0, (int)width - valueSettingPanel1.Width, (int)height);

            Graphics gfx = e.Graphics;
            gfx.Clear(Color.White);
            TextureBrush texBrush = new TextureBrush(m_previewTexture);

            //UV Preview
            texBrush.TranslateTransform(m_UVwindowRect.Width / 2f, m_UVwindowRect.Height / 2f);

            float texWidth = m_previewTexture.Width;
            float texHeight = m_previewTexture.Height;
            if (texWidth > texHeight)
                texBrush.ScaleTransform(m_zoomFactor*32/texWidth, m_zoomFactor * 32 / texWidth);
            else
                texBrush.ScaleTransform(m_zoomFactor * 32 / texHeight, m_zoomFactor * 32 / texHeight);
            gfx.FillRectangle(texBrush, m_UVwindowRect);
            
            //UV Rectangle
            Pen pen = new Pen(Color.Gold, 2);
            double angle = m_rotation * 0.0174533f;
            RectangleF rect = new RectangleF(-32 * m_scale, -32 * m_scale, 64 * m_scale, 64 * m_scale);
            PointF center = new PointF(m_UVwindowRect.Width / 2f + m_translationX * 64 * m_zoomFactor, m_UVwindowRect.Height / 2f - m_translationY * 64 * m_zoomFactor);

            gfx.DrawPolygon(pen, new PointF[] {
                new PointF(center.X + ((float)Math.Sin(-angle)*rect.Top    + (float)Math.Cos(angle)*rect.Left)*m_zoomFactor,
                           center.Y + ((float)Math.Cos(-angle)*rect.Top    + (float)Math.Sin(angle)*rect.Left)*m_zoomFactor),

                new PointF(center.X + ((float)Math.Sin(-angle)*rect.Top    + (float)Math.Cos(angle)*rect.Right)*m_zoomFactor,
                           center.Y + ((float)Math.Cos(-angle)*rect.Top    + (float)Math.Sin(angle)*rect.Right)*m_zoomFactor),

                new PointF(center.X + ((float)Math.Sin(-angle)*rect.Bottom + (float)Math.Cos(angle)*rect.Right)*m_zoomFactor,
                           center.Y + ((float)Math.Cos(-angle)*rect.Bottom + (float)Math.Sin(angle)*rect.Right)*m_zoomFactor),

                new PointF(center.X + ((float)Math.Sin(-angle)*rect.Bottom + (float)Math.Cos(angle)*rect.Left)*m_zoomFactor,
                           center.Y + ((float)Math.Cos(-angle)*rect.Bottom + (float)Math.Sin(angle)*rect.Left)*m_zoomFactor)
            });

            //off bounds cover
            gfx.FillRectangle(SystemBrushes.Control, new Rectangle((int)width - valueSettingPanel1.Width, 0, valueSettingPanel1.Width, (int)height));

            //small Texture Preview Window
            texBrush.ResetTransform();
            if (texWidth > texHeight)
                texBrush.ScaleTransform(32 / texWidth / m_scale, 32 / texWidth / m_scale);
            else
                texBrush.ScaleTransform(32 / texHeight / m_scale, 32 / texHeight / m_scale);
            texBrush.TranslateTransform(width - 32 - m_translationX * 64, 32 + m_translationY * 64);
            texBrush.RotateTransform(-m_rotation);
            gfx.FillRectangle(texBrush, width - 64, 0, 64, 64);
        }

        private void timelinePanel_Paint(object sender, PaintEventArgs e)
        {
            float width = e.ClipRectangle.Width;
            float height = e.ClipRectangle.Height;

            Graphics gfx = e.Graphics;
            gfx.Clear(Color.FromArgb(60, 60, 60));

            if (m_animEntry == null)
                return;

            if (m_timelinePart < 0 || m_timelinePart * 120 > m_animations[m_animId].m_NumFrames)
                return;

            if (valueSettingPanel1.Visible)
            {
                int framesInPart = Math.Min((int)m_animations[m_animId].m_NumFrames - m_timelinePart * 120, 120);

                Brush brush = new SolidBrush(Color.FromArgb(30, 30, 30));
                gfx.FillRectangle(brush, 0, 15, width, height - 15);

                brush = new SolidBrush(Color.DarkRed);
                gfx.FillRectangle(brush, 0, 15, width * framesInPart / 120, height - 15);

                //draw Values Display
                Pen pen = new Pen(Color.Red, 3);
                for (int i = 0; i < framesInPart; i++)
                {
                    float x = width * i / 120;
                    float y;
                    if (m_valueCeiling == m_valueFloor)
                        y = height * 0.5f;
                    else
                    {
                        int index = Math.Min(m_timelinePart * 120 + i, m_animationValues.Count - 1);
                        y = height - (height - 15) * ((m_animationValues[index] - m_valueFloor) / (m_valueCeiling - m_valueFloor));
                    }
                    gfx.DrawLine(pen, x, y, x, height);
                }

                //draw Cursor
                pen = new Pen(Color.FromArgb(100, Color.Gold), 2);
                float cursorX = width * m_timelineFrame / 120;
                gfx.DrawLine(pen, cursorX, 15, cursorX, height);

                //draw Part:Frame Display

                brush = new SolidBrush(Color.Gold);
                gfx.DrawString(m_timelinePart + " : " + m_timelineFrame, SystemFonts.DefaultFont, brush, Point.Empty);
            }
            else if (valueSettingPanel2.Visible)
            {
                gfx.Clear(Color.FromArgb(0, 30, 60));

                //draw graph
                List<PointF> graphPoints = new List<PointF>();
                foreach (KeyValuePair<int, float> entry in m_keyFrames)
                {
                    if (cbSelectInterpolation.SelectedIndex == 1 && graphPoints.Count > 0)
                        graphPoints.Add(new PointF(width * entry.Key / m_animations[m_animId].m_NumFrames, graphPoints.Last().Y));
                    graphPoints.Add(new PointF(width * entry.Key / m_animations[m_animId].m_NumFrames, height - entry.Value * height));
                }
                graphPoints.Add(new PointF(width, height - m_keyFrames.Last().Value * height));
                graphPoints.Add(new PointF(width, height + 2));
                graphPoints.Add(new PointF(0, height + 2));

                Brush brush = new SolidBrush(Color.FromArgb(0, 60, 120));
                gfx.FillPolygon(brush, graphPoints.ToArray());
                Pen pen = new Pen(Color.FromArgb(0, 200, 255), 2);
                gfx.DrawPolygon(pen, graphPoints.ToArray());

                //draw Cursor
                pen = new Pen(Color.FromArgb(100, Color.Gold), 2);
                float cursorX = width * m_texAnimFrame / m_animations[m_animId].m_NumFrames;
                gfx.DrawLine(pen, cursorX, 0, cursorX, height);
            }
        }

        //reinitialize for selected material
        private void tvMaterials_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode selected = tvMaterials.SelectedNode;
            if (selected.Level == 1)
            {
                int chunkId = (int)selected.Parent.Tag;
                int matId = (int)selected.Tag;
                m_previewTexture = m_BMD.m_ModelChunks[chunkId].m_MatGroups[matId].m_Texture.ToBitmap();
                BMD.MaterialGroup matGroup = m_BMD.m_ModelChunks[chunkId].m_MatGroups[matId];

                m_animEntry = null;
                m_animId = chunkId;
                m_timelineParts = (int)Math.Ceiling(m_animations[m_animId].m_NumFrames / 120f);
                m_timelinePart = 0;
                m_timelineFrame = 0;
                if (m_entries.ContainsKey(matGroup.m_Name))
                {
                    m_animEntry = m_entries[matGroup.m_Name];
                    cbSelectProperty.SelectedIndex = 0;
                    InitializeTimeLine(AnimationProperty.TRANSLATION_X);
                    cbSelectProperty.Enabled = true;
                    if (m_animations[m_animId].m_NumFrames == 1)
                    {
                        StartAnimation();
                        btnPlay.Enabled = false;
                        btnStop.Enabled = false;
                    }
                    else
                    {
                        StopAnimation();
                        btnPlay.Enabled = true;
                        btnStop.Enabled = true;
                    }
                }
                else
                {
                    StopAnimation();
                    textureView.Refresh();
                    timelinePanel.Refresh();
                    cbSelectProperty.Enabled = false;
                }
                btnCreateAnimData.Enabled = false;
                btnDeleteAnimData.Enabled = false;
            }
            else if (selected.Level == 0)
            {
                btnCreateAnimData.Enabled = true;
                btnDeleteAnimData.Enabled = true;
            }
        }

        //for zooming
        private void textureView_MouseWheel(object sender, MouseEventArgs e)
        {
            ZoomFactor += e.Delta * 0.0001f * ZoomFactor*ZoomFactor;
        }
        
        //load the values for the selected property in the Timeline
        private void cbSelectProperty_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitializeTimeLine((AnimationProperty)cbSelectProperty.SelectedIndex);
        }

        //set the frame position to the clicked position
        private void timelinePanel_MouseDown(object sender, MouseEventArgs e)
        {
            timelinePanel_AllMouseEvents(e);
        }

        private void timelinePanel_MouseMove(object sender, MouseEventArgs e)
        {
            timelinePanel_AllMouseEvents(e);
        }

        private void timelinePanel_AllMouseEvents(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (valueSettingPanel1.Visible)
            {
                int x = (int)Math.Round(120 * e.Location.X / (float)timelinePanel.Width);
                int framesInPart = Math.Min((int)m_animations[m_animId].m_NumFrames - m_timelinePart * 120, 120);

                m_timelineFrame = Math.Min(Math.Max(0, x), framesInPart - 1);

                m_texAnimFrame = m_timelinePart * 120 + m_timelineFrame;
                
            }
            else if (valueSettingPanel2.Visible)
            {
                int x = (int)Math.Round(m_animations[m_animId].m_NumFrames * e.Location.X / (float)timelinePanel.Width);
                m_texAnimFrame = Math.Min(Math.Max(0, x), (int)m_animations[m_animId].m_NumFrames - 1);
            }
            UpdateAnimation();
        }


        //player buttons
        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (m_texAnimTimer.Enabled)
            {
                PauseAnimation();
            }
            else
            {
                ResumeAnimation();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopAnimation();
        }

        private void btnStepLeft_Click(object sender, EventArgs e)
        {
            m_texAnimFrame--;
            UpdateAnimation();
        }

        private void btnStepRight_Click(object sender, EventArgs e)
        {
            m_texAnimFrame++;
            UpdateAnimation();
        }

        private void btnPrevPart_Click(object sender, EventArgs e)
        {
            m_texAnimFrame = Math.Max(m_texAnimFrame-120,0);
            UpdateAnimation();
        }

        private void btnNextPart_Click(object sender, EventArgs e)
        {
            m_texAnimFrame = Math.Min(m_texAnimFrame + 120, (int)m_animations[m_animId].m_NumFrames-1);
            UpdateAnimation();
        }


        //changing selected value
        private void numValue_ValueChanged(object sender, EventArgs e)
        {
            if (m_settingValue) return;
            if (m_animationValues.Count < m_animations[m_animId].m_NumFrames)
            {
                float value;
                if (m_animationValues.Count > 0)
                    value = m_animationValues.Last();
                else if ((AnimationProperty)cbSelectProperty.SelectedIndex == AnimationProperty.SCALE)
                    value = 1;
                else
                    value = 0;
                for (int i = m_animationValues.Count; i < m_animations[m_animId].m_NumFrames; i++)
                    m_animationValues.Add(value);
            }
            if (checkSetAll.Checked)
            {
                for(int i = 0;i< m_animationValues.Count; i++)
                {
                    m_animationValues[i] = (float)numValue.Value;
                }
            }
            else
            {
                m_animationValues[m_texAnimFrame] = (float)numValue.Value;
            }
            InitializeTimeLine((AnimationProperty)cbSelectProperty.SelectedIndex);
            ApplyChanges((AnimationProperty)cbSelectProperty.SelectedIndex);
            UpdateAnimation();
        }

        private void numKeyframeValue_ValueChanged(object sender, EventArgs e)
        {
            if (m_settingKeyframeValue) return;
            if (m_keyFrames.ContainsKey(m_texAnimFrame))
                m_keyFrames[m_texAnimFrame] = (float)numKeyframeValue.Value;
            else
                m_keyFrames.Add(m_texAnimFrame, (float)numKeyframeValue.Value);
            UpdateAnimation();
        }


        //saving / deleting
        private void ApplyChanges(AnimationProperty property)
        {
            if (property == AnimationProperty.TRANSLATION_X)
                m_animEntry.m_TranslationXValues = m_animationValues;
            else if (property == AnimationProperty.TRANSLATION_Y)
                m_animEntry.m_TranslationYValues = m_animationValues;
            else if (property == AnimationProperty.ROTATION)
            {
                m_animEntry.m_RotationValues = new List<float>();
                foreach (float value in m_animationValues)
                {
                    float wrappedValue = Helper.Wrap(value,360);
                    if (wrappedValue<=180)
                        m_animEntry.m_RotationValues.Add(wrappedValue);
                    else
                        m_animEntry.m_RotationValues.Add(wrappedValue-360);
                }
            }
            else
                m_animEntry.m_ScaleValues = m_animationValues;

            if (m_unsavedEntries[m_animId].ContainsKey(m_animEntry.m_MaterialName))
                m_unsavedEntries[m_animId][m_animEntry.m_MaterialName] = m_animEntry;
            else
                m_unsavedEntries[m_animId].Add(m_animEntry.m_MaterialName, m_animEntry);
        }

        private void SaveAnimation(int area, string materialName)
        {
            //Console.WriteLine(area);
            //Console.WriteLine(materialName);
            List<LevelTexAnim.Def> newEntries = new List<LevelTexAnim.Def>();



            foreach (LevelTexAnim.Def entry in m_parent.m_Level.m_TexAnims[area].m_Defs)
            {
                if (entry.m_MaterialName == materialName) {
                    if (m_unsavedEntries[area][materialName] != null)
                        if(m_unsavedEntries[area][materialName].m_RotationValues.Count>0)
                            newEntries.Add(OptimizeEntry(m_unsavedEntries[area][materialName]));
                    m_unsavedEntries[area].Remove(materialName);
                }
                else
                {
                    newEntries.Add(entry);
                }
            }
            if (m_unsavedEntries[area].ContainsKey(materialName)) {
                if (m_unsavedEntries[area][materialName] != null)
                    if (m_unsavedEntries[area][materialName].m_RotationValues.Count > 0)
                        newEntries.Add(OptimizeEntry(m_unsavedEntries[area][materialName]));
                m_unsavedEntries[area].Remove(materialName);
            }
            m_parent.m_Level.m_TexAnims[area].m_Defs = newEntries;
        }

        private LevelTexAnim.Def OptimizeEntry(LevelTexAnim.Def animEntry)
        {
            animEntry.m_TranslationXValues = OptimizeValues(animEntry.m_TranslationXValues);
            animEntry.m_TranslationYValues = OptimizeValues(animEntry.m_TranslationYValues);
            animEntry.m_RotationValues = OptimizeValues(animEntry.m_RotationValues);
            animEntry.m_ScaleValues = OptimizeValues(animEntry.m_ScaleValues);
            return animEntry;
        }

        private List<float> OptimizeValues(List<float> values)
        {
            if (values.Count == 0)
                return values;
            float firstNumber = values[0];
            bool isConstant = true;
            for (int i = 1; i < values.Count; i++)
            {
                if (values[i] != firstNumber)
                    isConstant = false;
            }
            if (isConstant)
            {
                List<float> optimizedValues = new List<float>();
                optimizedValues.Add(firstNumber);
                return optimizedValues;
            }
            else
                return values;

        }

        private void btnSaveAnim_Click(object sender, EventArgs e)
        {
            SaveAnimation(m_animId, m_animEntry.m_MaterialName);
        }

        private void btnDeleteAnim_Click(object sender, EventArgs e)
        {
            m_animEntry.m_RotationValues = new List<float>();

            if (m_unsavedEntries[m_animId].ContainsKey(m_animEntry.m_MaterialName))
                m_unsavedEntries[m_animId][m_animEntry.m_MaterialName] = m_animEntry;
            else
                m_unsavedEntries[m_animId].Add(m_animEntry.m_MaterialName, m_animEntry);

            SaveAnimation(m_animId, m_animEntry.m_MaterialName);

            m_entries.Remove(m_animEntry.m_MaterialName);
            m_animEntry = null;

            TreeNode node = tvMaterials.SelectedNode;
            node.Text = node.Text.Substring(0, node.Text.Length - 10);
            SetupGUIForPauseMode();
        }


        //Generating
        private void btnCreateNew_Click(object sender, EventArgs e)
        {
            LevelTexAnim.Def newEntry = new LevelTexAnim.Def()
            {
                m_DefaultScale = 1,
                m_MaterialName = tvMaterials.SelectedNode.Name,
                m_TranslationXValues = new List<float>(),
                m_TranslationYValues = new List<float>(),
                m_RotationValues = new List<float>(),
                m_ScaleValues = new List<float>()
            };
            newEntry.m_RotationValues.Add(0);
            newEntry.m_ScaleValues.Add(1);

            if (m_entries.ContainsKey(tvMaterials.SelectedNode.Name))
                m_entries[tvMaterials.SelectedNode.Name] = newEntry;
            else
                m_entries.Add(tvMaterials.SelectedNode.Name, newEntry);
            m_animEntry = newEntry;

            if (m_unsavedEntries[m_animId].ContainsKey(m_animEntry.m_MaterialName))
                m_unsavedEntries[m_animId][m_animEntry.m_MaterialName] = m_animEntry;
            else
                m_unsavedEntries[m_animId].Add(m_animEntry.m_MaterialName, m_animEntry);

            cbSelectProperty.Enabled = true;
            cbSelectProperty.SelectedIndex = 0;
            InitializeTimeLine(AnimationProperty.TRANSLATION_X);

            TreeNode node = tvMaterials.SelectedNode;
            if (!node.Text.EndsWith("[ANIMATED]"))
                node.Text = node.Text + "[ANIMATED]";
            SetupGUIForPauseMode();
        }

        private void btnSetKeyframes_Click(object sender, EventArgs e)
        {
            m_keyFrames = new SortedDictionary<int, float>();
            m_keyFrames.Add(0,0);
            cbSelectInterpolation.SelectedIndex = 0;
            //switch panels
            valueSettingPanel1.Visible = false;
            valueSettingPanel2.Visible = true;
            valueSettingPanel2.BringToFront();
            StopAnimation();
            tvMaterials.Enabled = false;
            btnNextPart.Visible = false;
            btnPrevPart.Visible = false;
        }

        private void btnGenerateAnim_Click(object sender, EventArgs e)
        {
            AnimationProperty property = (AnimationProperty)cbSelectProperty.SelectedIndex;
            //generate values out of keyframes
            if (property == AnimationProperty.TRANSLATION_X)
            {
                m_animEntry.m_TranslationXValues = new List<float>();
                for (int i = 0; i < m_animations[m_animId].m_NumFrames; i++)
                    m_animEntry.m_TranslationXValues.Add(InterpolatedValue(m_keyFrames, i) * (float)numScaling.Value);
            }
            else if (property == AnimationProperty.TRANSLATION_Y)
            {
                m_animEntry.m_TranslationYValues = new List<float>();
                for (int i = 0; i < m_animations[m_animId].m_NumFrames; i++)
                    m_animEntry.m_TranslationYValues.Add(InterpolatedValue(m_keyFrames, i) * (float)numScaling.Value);
            }
            else if (property == AnimationProperty.ROTATION)
            {
                m_animEntry.m_RotationValues = new List<float>();
                for (int i = 0; i < m_animations[m_animId].m_NumFrames; i++)
                {
                    float wrappedValue = Helper.Wrap(InterpolatedValue(m_keyFrames, i) * (float)numScaling.Value, 360);
                    if (wrappedValue <= 180)
                        m_animEntry.m_RotationValues.Add(wrappedValue);
                    else
                        m_animEntry.m_RotationValues.Add(wrappedValue - 360);
                }
            }
            else
            {
                m_animEntry.m_ScaleValues = new List<float>();
                for (int i = 0; i < m_animations[m_animId].m_NumFrames; i++)
                    m_animEntry.m_ScaleValues.Add(InterpolatedValue(m_keyFrames, i) * (float)numScaling.Value);
            }

            //add to unsaved
            if (m_unsavedEntries[m_animId].ContainsKey(m_animEntry.m_MaterialName))
                m_unsavedEntries[m_animId][m_animEntry.m_MaterialName] = m_animEntry;
            else
                m_unsavedEntries[m_animId].Add(m_animEntry.m_MaterialName, m_animEntry);
            
            //switch panels
            valueSettingPanel1.Visible = true;
            valueSettingPanel2.Visible = false;
            valueSettingPanel1.BringToFront();
            StopAnimation();
            tvMaterials.Enabled = true;
            btnNextPart.Visible = true;
            btnPrevPart.Visible = true;

            InitializeTimeLine(property);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            m_keyFrames = new SortedDictionary<int, float>();
            m_keyFrames.Add(0, 0);
            StopAnimation();
        }


        //delete the keyframe for this frame if there is one
        private void btnDeleteKeyframe_Click(object sender, EventArgs e)
        {
            if (m_texAnimFrame == 0)
                m_keyFrames[0] = 0;
            else
                m_keyFrames.Remove(m_texAnimFrame);
            UpdateAnimation();
        }

        //cancel setting keyframes
        private void btnCancelKeyFrames_Click(object sender, EventArgs e)
        {
            //switch panels
            valueSettingPanel1.Visible = true;
            valueSettingPanel2.Visible = false;
            valueSettingPanel1.BringToFront();
            StopAnimation();
            tvMaterials.Enabled = true;
            btnNextPart.Visible = true;
            btnPrevPart.Visible = true;
        }

        //interpolating value between keyframes
        private float InterpolatedValue(SortedDictionary<int,float> keyframes, int frame)
        {
            float x1,y1,x2,y2;
            for (int i = 1; i < keyframes.Count; i++)
            {
                x1 = keyframes.ElementAt(i-1).Key;
                x2 = keyframes.ElementAt(i).Key;
                if (x1 <= frame && frame <= x2)
                {
                    y1 = keyframes.ElementAt(i - 1).Value;
                    if (cbSelectInterpolation.SelectedIndex == 1) return y1;
                    y2 = keyframes.ElementAt(i).Value;

                    return y1 + (frame - x1) * (y2 - y1) / (x2 - x1);
                }
            }
            return keyframes.Last().Value;
        }

        private void numScaling_ValueChanged(object sender, EventArgs e)
        {
            if (numScaling.Value == 0)
            {
                if ((int)numScaling.Tag < 0)
                    numScaling.Value = 1;
                else if ((int)numScaling.Tag > 0)
                    numScaling.Value = -1;
                numScaling.Tag = (int)numScaling.Value;
            }
            UpdateAnimation();
        }

        private void cbSelectInterpolation_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAnimation();
        }
        

        //ask for save before quitting
        private void BetterTextureAnimationEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            int unsavedAnims = 0;
            foreach(Dictionary<string, LevelTexAnim.Def> areaEntries in m_unsavedEntries)
                unsavedAnims+= areaEntries.Count;

            if (unsavedAnims==0) return;

            DialogResult result = MessageBox.Show(this, unsavedAnims+" files are unsaved, would you like to save them?", "Save before quitting", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Yes) SaveAll();
            else if (result == DialogResult.Cancel) e.Cancel = true;

            for (int area = 0; area < m_parent.m_Level.m_TexAnims.Count; area++)
            {
                LevelTexAnim animation = m_parent.m_Level.m_TexAnims[area];
                if (animation.m_NumDefs == 0)
                    animation.m_NumFrames = 0;

            }
        }

        private void SaveAll()
        {
            for (int area = 0; area < m_unsavedEntries.Length; area++)
            {
                List<LevelTexAnim.Def> newEntries = new List<LevelTexAnim.Def>();

                foreach (LevelTexAnim.Def entry in m_parent.m_Level.m_TexAnims[area].m_Defs)
                {
                    if (m_unsavedEntries[area].ContainsKey(entry.m_MaterialName))
                    {
                        if (m_unsavedEntries[area][entry.m_MaterialName].m_RotationValues.Count > 0)
                        {
                            newEntries.Add(OptimizeEntry(m_unsavedEntries[area][entry.m_MaterialName]));
                            m_unsavedEntries[area].Remove(entry.m_MaterialName);
                        }
                    }
                    else
                    {
                        newEntries.Add(entry);
                    }
                }
                foreach (KeyValuePair<string, LevelTexAnim.Def> entry in m_unsavedEntries[area])
                {
                    if (m_unsavedEntries[area][entry.Key].m_RotationValues.Count > 0)
                        newEntries.Add(OptimizeEntry(entry.Value));
                }
                m_parent.m_Level.m_TexAnims[area].m_Defs = newEntries;

                m_unsavedEntries[area].Clear();
            }

        }


        //manage animation data per area
        private void btnCreateAnimData_Click(object sender, EventArgs e)
        {
            int area = (int)tvMaterials.SelectedNode.Tag;
            DialogResult result = new CreateNewLevelAnimationDialog(m_parent.m_Level, area).ShowDialog();
            if (result == DialogResult.OK)
            {
                TreeNode selected = tvMaterials.SelectedNode;
                selected.Text = "Area " + area + " (" + m_parent.m_Level.m_TexAnims[area].m_NumFrames + " frames)";
                selected.Nodes.Clear();
                int i = (int)selected.Tag;
                int i2 = 0;
                foreach (BMD.MaterialGroup matGroup in m_parent.m_LevelModel.m_ModelChunks[i].m_MatGroups)
                {
                    if (matGroup.m_Texture != null)
                    {
                        bool hasAnimation = false;
                        foreach (LevelTexAnim.Def entry in m_animations[i].m_Defs)
                        {
                            if (entry.m_MaterialName == matGroup.m_Name)
                                hasAnimation = true;
                        }
                        TreeNode matGroupNode = tvMaterials.SelectedNode.Nodes.Add(matGroup.m_Name, matGroup.m_Name + (hasAnimation ? "[ANIMATED]" : ""));
                        matGroupNode.Tag = i2;
                    }
                    i2++;
                }
                btnDeleteAnimData.Enabled = true;
            }
        }

        private void btnDeleteAnimData_Click(object sender, EventArgs e)
        {
            TreeNode selected = tvMaterials.SelectedNode;
            int area = (int)selected.Tag;
            LevelTexAnim texAnim = m_parent.m_Level.m_TexAnims[area];
            texAnim.m_Defs = new List<LevelTexAnim.Def>();
            texAnim.m_NumFrames = 0;
            selected.Nodes.Clear();
            selected.Text = "Area " + area + " (0 frames)";
            btnDeleteAnimData.Enabled = false;
        }
    }
}
