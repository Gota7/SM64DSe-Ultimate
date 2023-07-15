using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using OpenTK;
using OpenTK.Graphics;

namespace SM64DSe
{
    public partial class ParticleViewerForm : Form
    {
        private int[] m_DisplayLists;
        private float m_ScaleRef;

        private Particle.Manager m_ParticleManager;
        public List<Particle.System.Def> m_SysDefs;
        private List<Particle.Texture> m_Textures;

        private Thread m_Runner;
        private object m_Locker = new object();
        private System.Windows.Forms.Timer m_CloseTimer;

        private bool m_Running;
        private bool m_CanClose;

        private bool m_Frozen;

        const bool externalSPT = true;
        private int builtInTextures;
        private int totalTextures;
        private string m_Name;
        private NitroFile m_ParticleArchiveFile;
        private ROMFileSelect m_ROMFileSelect = new ROMFileSelect();

        public ParticleViewerForm(string fileName)
        {
            InitializeComponent();
            m_ScaleRef = 1.0f;

            m_Name = fileName;

            m_DisplayLists = new int[1];
            m_DisplayLists[0] = 0;

            glModelView.Initialise();
            glModelView.ProvidePostDisplayLists(m_DisplayLists);
            glModelView.ProvideScaleRef(ref m_ScaleRef);
            glModelView.SetShowMarioReference(true);
            glModelView.Refresh();

            m_ParticleManager = new Particle.Manager();
            m_SysDefs = new List<Particle.System.Def>();
            m_Textures = new List<Particle.Texture>();
        }

        public ParticleViewerForm()
            : this(null) { }

        private void LoadMainSysDefs()
        {
            bool autorw = Program.m_ROM.CanRW();
            if (!autorw)
                Program.m_ROM.BeginRW();

            // externalSPT = Program.m_ROM.Read32(Particle.EXTERNAL_SPT_MARK_ROM_ADDR) == Particle.EXTERNAL_SPT_MARK_VALUE;

            if (!autorw)
                Program.m_ROM.EndRW();

            int numSysDefs = m_ParticleArchiveFile.Read16(0x8);
            builtInTextures  = totalTextures = m_ParticleArchiveFile.Read16(0xa);

            if (externalSPT)
            {
                totalTextures = m_ParticleArchiveFile.Read8(0xa);
                builtInTextures = m_ParticleArchiveFile.Read8(0xb);
            }

            LoadTextures();
            LoadSysDefs(numSysDefs);
        }

        private void LoadSysDefs(int numSysDefs)
        {
            lbxSysDef.Items.Clear();
            m_SysDefs.Capacity = numSysDefs;
            uint offset = 0x20;
            for (int i = 0; i < numSysDefs; ++i)
            {
                //Console.WriteLine($"MainInfo 0x{Convert.ToString(i, 16)} started (0x{Convert.ToString(offset, 16)})");
                Particle.System.Def sysDef = new Particle.System.Def();
                Particle.MainInfo info = new Particle.MainInfo();

                uint flags = m_ParticleArchiveFile.Read32(offset);
                int texRepeatFlags = m_ParticleArchiveFile.Read8(offset + 0x37);
                info.m_SpawnShape = (Particle.MainInfo.SpawnShape)(flags & 0x7);
                info.m_DrawMode = (Particle.MainInfo.DrawMode)(flags >> 4 & 0x3);
                info.m_Plane = (Particle.MainInfo.Plane)(flags >> 6 & 0x3);
                info.m_Rotate = (flags >> 12 & 1) != 0;
                info.m_RandomInitAng = (flags >> 13 & 1) != 0;
                info.m_SelfDestruct = (flags >> 14 & 1) != 0;
                info.m_FollowSystem = (flags >> 15 & 1) != 0;
                info.m_WeirdAxis = (flags >> 17 & 1) != 0;
                info.m_HorzIf3D = (flags >> 19 & 1) != 0;
                info.m_Rate = (int)m_ParticleArchiveFile.Read32(offset + 0x04) / 4096.0f;
                info.m_StartHorzDist = (int)m_ParticleArchiveFile.Read32(offset + 0x08) / 512.0f;
                info.m_Dir = new Vector3(
                    (short)m_ParticleArchiveFile.Read16(offset + 0x0c) / 4096.0f,
                    (short)m_ParticleArchiveFile.Read16(offset + 0x0e) / 4096.0f,
                    (short)m_ParticleArchiveFile.Read16(offset + 0x10) / 4096.0f
                    );
                info.m_Color = Helper.BGR15ToColor(m_ParticleArchiveFile.Read16(offset + 0x12));
                info.m_HorzSpeed = (int)m_ParticleArchiveFile.Read32(offset + 0x14) / 512.0f;
                info.m_VertSpeed = (int)m_ParticleArchiveFile.Read32(offset + 0x18) / 512.0f;
                info.m_Scale = (int)m_ParticleArchiveFile.Read32(offset + 0x1c) / 512.0f;
                info.m_HorzScaleMult = (short)m_ParticleArchiveFile.Read16(offset + 0x20) / 4096.0f;
                info.m_MinAngleSpeed = (short)m_ParticleArchiveFile.Read16(offset + 0x24);
                info.m_MaxAngleSpeed = (short)m_ParticleArchiveFile.Read16(offset + 0x26);
                info.m_Frames = m_ParticleArchiveFile.Read16(offset + 0x28);
                info.m_Lifetime = m_ParticleArchiveFile.Read16(offset + 0x2a);
                info.m_ScaleRand = m_ParticleArchiveFile.Read8(offset + 0x2c);
                info.m_LifetimeRand = m_ParticleArchiveFile.Read8(offset + 0x2d);
                info.m_SpeedRand = m_ParticleArchiveFile.Read8(offset + 0x2e);
                info.m_SpawnPeriod = m_ParticleArchiveFile.Read8(offset + 0x30);
                info.m_Alpha = m_ParticleArchiveFile.Read8(offset + 0x31);
                info.m_SpeedFalloff = m_ParticleArchiveFile.Read8(offset + 0x32);
                info.m_Sprite = m_Textures[m_ParticleArchiveFile.Read8(offset + 0x33)];
                info.m_AltLength = m_ParticleArchiveFile.Read8(offset + 0x34);
                info.m_VelStretchFactor = (short)m_ParticleArchiveFile.Read16(offset + 0x35) / 4096.0f;
                info.m_LogTexRepeatHorz = texRepeatFlags >> 0 & 0x3;
                info.m_LogTexRepeatVert = texRepeatFlags >> 2 & 0x3;
                sysDef.m_MainInfo = info;
                offset += 0x38;

                if ((flags >> 8 & 1) != 0)
                {
                    //Console.WriteLine($"ScaleTransition 0x{Convert.ToString(i, 16)} started (0x{Convert.ToString(offset, 16)})");
                    Particle.ScaleTransition scaleTrans = new Particle.ScaleTransition();
                    scaleTrans.m_Start = m_ParticleArchiveFile.Read16(offset + 0x00) / 4096.0f;
                    scaleTrans.m_Middle = m_ParticleArchiveFile.Read16(offset + 0x02) / 4096.0f;
                    scaleTrans.m_End = m_ParticleArchiveFile.Read16(offset + 0x04) / 4096.0f;
                    scaleTrans.m_Trans1End = m_ParticleArchiveFile.Read8(offset + 0x06);
                    scaleTrans.m_Trans2Start = m_ParticleArchiveFile.Read8(offset + 0x07);
                    scaleTrans.m_UseAltLength = (m_ParticleArchiveFile.Read8(offset + 0x08) & 1) != 0;
                    sysDef.m_ScaleTrans = scaleTrans;
                    offset += 0x0c;
                }
                if ((flags >> 9 & 1) != 0)
                {
                    //Console.WriteLine($"ColorTransition 0x{Convert.ToString(i, 16)} started (0x{Convert.ToString(offset, 16)})");
                    Particle.ColorTransition colorTrans = new Particle.ColorTransition();
                    uint interpFlags = m_ParticleArchiveFile.Read8(offset + 0x08);
                    colorTrans.m_Start = Helper.BGR15ToColor(m_ParticleArchiveFile.Read16(offset + 0x00));
                    colorTrans.m_End = Helper.BGR15ToColor(m_ParticleArchiveFile.Read16(offset + 0x02));
                    colorTrans.m_Trans1Start = m_ParticleArchiveFile.Read8(offset + 0x04);
                    colorTrans.m_Trans2Start = m_ParticleArchiveFile.Read8(offset + 0x05);
                    colorTrans.m_Trans2End = m_ParticleArchiveFile.Read8(offset + 0x06);
                    colorTrans.m_UseAsOptions = (interpFlags >> 0 & 1) != 0;
                    colorTrans.m_UseAltLength = (interpFlags >> 1 & 1) != 0;
                    colorTrans.m_SmoothTrans = (interpFlags >> 2 & 1) != 0;
                    sysDef.m_ColorTrans = colorTrans;
                    offset += 0x0c;
                }
                if ((flags >> 10 & 1) != 0)
                {
                    //Console.WriteLine($"AlphaTransition 0x{Convert.ToString(i, 16)} started (0x{Convert.ToString(offset, 16)})");
                    Particle.AlphaTransition alphaTrans = new Particle.AlphaTransition();
                    uint alphas = m_ParticleArchiveFile.Read16(offset + 0x00);
                    alphaTrans.m_Start = (int)(alphas >> 0 & 0x1f);
                    alphaTrans.m_Middle = (int)(alphas >> 5 & 0x1f);
                    alphaTrans.m_End = (int)(alphas >> 10 & 0x1f);
                    alphaTrans.m_Flicker = m_ParticleArchiveFile.Read8(offset + 0x02);
                    alphaTrans.m_UseAltLength = (m_ParticleArchiveFile.Read8(offset + 0x03) & 1) != 0;
                    alphaTrans.m_Trans1End = m_ParticleArchiveFile.Read8(offset + 0x04);
                    alphaTrans.m_Trans2Start = m_ParticleArchiveFile.Read8(offset + 0x05);
                    sysDef.m_AlphaTrans = alphaTrans;
                    offset += 0x08;
                }
                if ((flags >> 11 & 1) != 0)
                {
                    //Console.WriteLine($"TextureSequence 0x{Convert.ToString(i, 16)} started (0x{Convert.ToString(offset, 16)})");
                    Particle.TextureSequence texSeq = new Particle.TextureSequence();
                    texSeq.m_Sprites = new Particle.Texture[8];
                    int numSprites = m_ParticleArchiveFile.Read8(offset + 0x08);
                    uint interpFlags = m_ParticleArchiveFile.Read8(offset + 0x0a);
                    for (uint j = 0; j < numSprites; ++j)
                        texSeq.m_Sprites[j] = m_Textures[m_ParticleArchiveFile.Read8(offset + j)];
                    texSeq.m_NumSprites = numSprites;
                    texSeq.m_Interval = m_ParticleArchiveFile.Read8(offset + 0x09);
                    texSeq.m_UseAsOptions = (interpFlags >> 0 & 1) != 0;
                    texSeq.m_UseAltLength = (interpFlags >> 1 & 1) != 0;
                    sysDef.m_TexSeq = texSeq;
                    offset += 0x0c;
                }
                if ((flags >> 16 & 1) != 0)
                {
                    //Console.WriteLine($"Glitter 0x{Convert.ToString(i, 16)} started (0x{Convert.ToString(offset, 16)})");
                    Particle.Glitter glitter = new Particle.Glitter();
                    int gFlags = m_ParticleArchiveFile.Read16(offset + 0x00);
                    int gTexRepeatFlags = m_ParticleArchiveFile.Read8(offset + 0x10);
                    glitter.m_HasEffects = (gFlags & 1) != 0;
                    glitter.m_HasScaleTrans = (gFlags >> 1 & 1) != 0;
                    glitter.m_HasAlphaTrans = (gFlags >> 2 & 1) != 0;
                    glitter.m_AngleCopyMode = (Particle.Glitter.AngleCopyMode)(gFlags >> 3 & 0x3);
                    glitter.m_FollowSystem = (gFlags >> 5 & 1) != 0;
                    glitter.m_UseGlitterColor = (gFlags >> 6 & 1) != 0;
                    glitter.m_DrawMode = (Particle.MainInfo.DrawMode)(gFlags >> 7 & 0x3);
                    glitter.m_SpeedRand = m_ParticleArchiveFile.Read16(offset + 0x02) / 512.0f;
                    glitter.m_Scale = (short)m_ParticleArchiveFile.Read16(offset + 0x04) / 4096.0f; //multiplier
                    glitter.m_Lifetime = m_ParticleArchiveFile.Read16(offset + 0x06);
                    glitter.m_SpeedMult = m_ParticleArchiveFile.Read8(offset + 0x08);
                    glitter.m_ScaleMult = m_ParticleArchiveFile.Read8(offset + 0x09);
                    glitter.m_Color = Helper.BGR15ToColor(m_ParticleArchiveFile.Read16(offset + 0x0a));
                    glitter.m_Rate = m_ParticleArchiveFile.Read8(offset + 0x0c);
                    glitter.m_Wait = m_ParticleArchiveFile.Read8(offset + 0x0d);
                    glitter.m_Period = m_ParticleArchiveFile.Read8(offset + 0x0e);
                    glitter.m_Sprite = m_Textures[m_ParticleArchiveFile.Read8(offset + 0x0f)];
                    glitter.m_LogTexRepeatHorz = gTexRepeatFlags >> 0 & 0x3;
                    glitter.m_LogTexRepeatVert = gTexRepeatFlags >> 2 & 0x3;
                    sysDef.m_Glitter = glitter;
                    offset += 0x14;
                }

                sysDef.m_Effects = new List<Particle.Effect>();
                if ((flags >> 24 & 1) != 0)
                {
                    //Console.WriteLine($"Acceleration 0x{Convert.ToString(i, 16)} started (0x{Convert.ToString(offset, 16)})");
                    Particle.Acceleration accel = new Particle.Acceleration();
                    accel.m_Accel = new Vector3(
                        (short)m_ParticleArchiveFile.Read16(offset + 0x00) / 512.0f,
                        (short)m_ParticleArchiveFile.Read16(offset + 0x02) / 512.0f,
                        (short)m_ParticleArchiveFile.Read16(offset + 0x04) / 512.0f
                        );
                    sysDef.m_Effects.Add(accel);
                    offset += 0x08;
                }
                if ((flags >> 25 & 1) != 0)
                {
                    //Console.WriteLine($"Jitter 0x{Convert.ToString(i, 16)} started (0x{Convert.ToString(offset, 16)})");
                    Particle.Jitter jitter = new Particle.Jitter();
                    jitter.m_Mag = new Vector3(
                        (short)m_ParticleArchiveFile.Read16(offset + 0x00) / 512.0f,
                        (short)m_ParticleArchiveFile.Read16(offset + 0x02) / 512.0f,
                        (short)m_ParticleArchiveFile.Read16(offset + 0x04) / 512.0f
                        );
                    jitter.m_Period = m_ParticleArchiveFile.Read16(offset + 0x06);
                    sysDef.m_Effects.Add(jitter);
                    offset += 0x08;
                }
                if ((flags >> 26 & 1) != 0)
                {
                    //Console.WriteLine($"Converge 0x{Convert.ToString(i, 16)} started (0x{Convert.ToString(offset, 16)})");
                    Particle.Converge converge = new Particle.Converge();
                    converge.m_Offset = new Vector3(
                        (int)m_ParticleArchiveFile.Read32(offset + 0x00) / 512.0f,
                        (int)m_ParticleArchiveFile.Read32(offset + 0x04) / 512.0f,
                        (int)m_ParticleArchiveFile.Read32(offset + 0x08) / 512.0f
                        );
                    converge.m_Mag = (short)m_ParticleArchiveFile.Read16(offset + 0x0c) / 4096.0f;
                    sysDef.m_Effects.Add(converge);
                    offset += 0x10;
                }
                if ((flags >> 27 & 1) != 0)
                {
                    //Console.WriteLine($"Turn 0x{Convert.ToString(i, 16)} started (0x{Convert.ToString(offset, 16)})");
                    Particle.Turn turn = new Particle.Turn();
                    turn.m_AngleSpeed = (short)m_ParticleArchiveFile.Read16(offset + 0x00);
                    turn.m_Axis = (Particle.Turn.Axis)m_ParticleArchiveFile.Read8(offset + 0x02);
                    sysDef.m_Effects.Add(turn);
                    offset += 0x04;
                }
                if ((flags >> 28 & 1) != 0) //not used by any of the 0x141 particle system defs
                {
                    //Console.WriteLine($"LimitPlane 0x{Convert.ToString(i, 16)} started (0x{Convert.ToString(offset, 16)})");
                    Particle.LimitPlane limit = new Particle.LimitPlane();
                    limit.m_PosY = (int)m_ParticleArchiveFile.Read32(offset + 0x00) / 512.0f;
                    limit.m_ReverseSpeedMult = (short)m_ParticleArchiveFile.Read16(offset + 0x04) / 4096.0f;
                    limit.m_Behavior = (Particle.LimitPlane.Behavior)(m_ParticleArchiveFile.Read8(offset + 0x06));
                    sysDef.m_Effects.Add(limit);
                    offset += 0x08;
                }
                if ((flags >> 29 & 1) != 0)
                {
                    //Console.WriteLine($"RadiusConverge 0x{Convert.ToString(i, 16)} started (0x{Convert.ToString(offset, 16)})");
                    Particle.RadiusConverge converge = new Particle.RadiusConverge();
                    converge.m_Offset = new Vector3(
                        (int)m_ParticleArchiveFile.Read32(offset + 0x00) / 512.0f,
                        (int)m_ParticleArchiveFile.Read32(offset + 0x04) / 512.0f,
                        (int)m_ParticleArchiveFile.Read32(offset + 0x08) / 512.0f
                        );
                    converge.m_Mag = (short)m_ParticleArchiveFile.Read16(offset + 0x0c) / 4096.0f;
                    sysDef.m_Effects.Add(converge);
                    offset += 0x10;
                }

                ParticleSysDefProperties.GenerateProperties(sysDef, m_Textures);
                m_SysDefs.Add(sysDef);
                lbxSysDef.Items.Add($"particle{Convert.ToString(i, 16).PadLeft(3, '0')}.spd");
            }
        }

        private void LoadTextures()
        {
            m_Textures.Capacity = totalTextures;
            uint offset = m_ParticleArchiveFile.Read32(0x18);

            // load the internal textures
            for (int i = 0; i < builtInTextures; ++i)
            {
                //Console.WriteLine($"offset = 0x{Convert.ToString(offset, 16)}");
                if (m_ParticleArchiveFile.Read32(offset) != 0x53505420)
                {
                    MessageBox.Show(string.Format("Tried to read bad SPT file: Texture {0}", i),
                        "Bad texture");
                    Close();
                    return;
                }

                uint flags = m_ParticleArchiveFile.Read32(offset + 0x04);
                uint texelArrSize = m_ParticleArchiveFile.Read32(offset + 0x08);
                uint palOffset = m_ParticleArchiveFile.Read32(offset + 0x0c);
                uint palSize = m_ParticleArchiveFile.Read32(offset + 0x10);
                uint totalSize = m_ParticleArchiveFile.Read32(offset + 0x1c);

                byte[] texels = m_ParticleArchiveFile.ReadBlock(offset + 0x20, texelArrSize);
                byte[] palette = m_ParticleArchiveFile.ReadBlock(offset + palOffset, palSize);

                int width = 1 << (((int)flags >> 4 & 0xf) + 3);
                int height = 1 << (((int)flags >> 8 & 0xf) + 3);
                bool color0Transp = ((flags & 0x8) | (flags & 0x10000)) != 0;
                int type = (int)flags & 0x7;
                Particle.Texture.RepeatMode repeatX = (flags & 0x4000) != 0 ?
                    Particle.Texture.RepeatMode.FLIP : (flags & 0x1000) != 0 ?
                    Particle.Texture.RepeatMode.REPEAT :
                    Particle.Texture.RepeatMode.CLAMP;
                Particle.Texture.RepeatMode repeatY = (flags & 0x8000) != 0 ?
                    Particle.Texture.RepeatMode.FLIP : (flags & 0x2000) != 0 ?
                    Particle.Texture.RepeatMode.REPEAT :
                    Particle.Texture.RepeatMode.CLAMP;

                m_Textures.Add(new Particle.Texture(texels, palette, width, height,
                    (byte)(color0Transp ? 1 : 0), type, repeatX, repeatY, i));
                offset += totalSize;
            }

            // load external (dummy) textures if the rom has the necessary patch
            if (externalSPT && builtInTextures != totalTextures)
            {
                for (int i = builtInTextures; i < totalTextures; ++i)
                {
                    m_Textures.Add(new Particle.Texture());
                }
            }

            ParticleSysDefProperties.highestTextureID = m_Textures.Count - 1;
        }

        private void Run()
        {
            m_Running = true;
            m_CanClose = false;

            //IGraphicsContext context = new GraphicsContext(glModelView.GraphicsMode, glModelView.WindowInfo);
            //context.MakeCurrent(glModelView.WindowInfo);

            Stopwatch stopwatch = Stopwatch.StartNew();
            long leftoverTicks = 0;
            long requiredTicks = Stopwatch.Frequency / 30;

            while (m_Running)
            {
                if (stopwatch.ElapsedTicks + leftoverTicks >= requiredTicks)
                {
                    leftoverTicks = stopwatch.ElapsedTicks + leftoverTicks - requiredTicks;
                    if (leftoverTicks > requiredTicks)
                        leftoverTicks = requiredTicks;

                    stopwatch.Restart();
                    //GL stuff needs to happen on the main thread.
                    Invoke(new Action(delegate () { UpdateAndRenderParticles(); }));
                }
            }

            m_CanClose = true;
        }

        private void UpdateAndRenderParticles()
        {
            m_ParticleManager.Update();
            m_ParticleManager.Render(m_DisplayLists, glModelView.CamMatrix);
            glModelView.Refresh();
        }

        private void m_CloseTimer_Tick(object sender, EventArgs e)
        {
            Close();
        }

        private void ParticleViewerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_Running = false;
            if (!m_CanClose)
                e.Cancel = true;

            //wait for the other thread to finish without blocking this thread,
            //as the other thread could be calling Invoke()
            if (m_CloseTimer == null)
            {
                m_CloseTimer = new System.Windows.Forms.Timer();
                m_CloseTimer.Interval = 55; //minimum precision
                m_CloseTimer.Tick += new EventHandler(m_CloseTimer_Tick);
                m_CloseTimer.Start();
            }
        }

        private void ParticleViewerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (m_Runner.IsAlive)
                m_Runner.Join();
            glModelView.PrepareForClose();
            m_ParticleManager.RemoveLists();
            foreach (Particle.Texture texture in m_Textures)
                texture.Unload();
        }

        private void ParticleViewerForm_Load(object sender, EventArgs e)
        {
            if (m_Name == null)
            {
                m_ROMFileSelect.ReInitialize("Select a SPA file to load", new String[] { ".spa" });
                DialogResult result = m_ROMFileSelect.ShowDialog();
                if (result != DialogResult.OK)
                    Close();
                else
                {
                    m_Name = m_ROMFileSelect.m_SelectedFile;
                }
            }

            try
            {
                m_ParticleArchiveFile = Program.m_ROM.GetFileFromName(m_Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }

            LoadMainSysDefs();

            foreach (Particle.Texture texture in m_Textures)
                texture.Load();
            m_Runner = new Thread(Run);
            m_Runner.Start();
        }

        private void btnDisplay_Click(object sender, EventArgs e)
        {
            int sysDefID = lbxSysDef.SelectedIndex;
            if (sysDefID < 0 || sysDefID >= m_SysDefs.Count)
            {
                MessageBox.Show(string.Format("Invalid particle system ID. Must be between 0x0 and 0x{0:x}, "
                    + "including the former, excluding the latter.", m_SysDefs.Count), "Bad ID");
                return;
            }

            foreach (Particle.System system in m_ParticleManager.m_Systems)
            {
                system.m_Paused = false;
                system.m_Stopped = true;
                m_Frozen = false;
                btnFreeze.Text = "Freeze";
            }
            m_ParticleManager.AddSystem(m_SysDefs[sysDefID], Vector3.Zero, null);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            foreach (Particle.System system in m_ParticleManager.m_Systems)
            {
                system.m_Paused = false;
                system.m_Stopped = true;
                m_Frozen = false;
                btnFreeze.Text = "Freeze";
            }
        }

        private void btnFreeze_Click(object sender, EventArgs e)
        {
            m_Frozen = !m_Frozen;
            foreach (Particle.System system in m_ParticleManager.m_Systems)
                system.m_Paused = m_Frozen;
            btnFreeze.Text = m_Frozen ? "Unfreeze" : "Freeze";
        }

        private void lbxSysDef_SelectedIndexChanged(object sender, EventArgs e)
        {
            int sysDefID = lbxSysDef.SelectedIndex;
            if (sysDefID < 0 || sysDefID >= m_SysDefs.Count)
                pgSysDefProps.SelectedObject = null;
            else
                pgSysDefProps.SelectedObject = m_SysDefs[sysDefID].m_Properties;
        }

        private void pgSysDefProps_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            object value = e.ChangedItem.Value;
            if (value is string)
            {
                if (e.ChangedItem.PropertyDescriptor.Converter is ItemListTypeConverter)
                {
                    List<string> items = null;
                    foreach (Attribute attr in e.ChangedItem.PropertyDescriptor.Attributes)
                        if (attr is ItemListAttribute)
                            items = ((ItemListAttribute)attr).m_Items;
                    value = items.IndexOf((string)value);
                }
                else if (e.ChangedItem.PropertyDescriptor.Converter is BoolTypeConverter)
                {
                    value = !((string)value).Equals("False", StringComparison.CurrentCultureIgnoreCase);
                }
            }

            int sysDefID = lbxSysDef.SelectedIndex;
            if (sysDefID < 0 || sysDefID >= m_SysDefs.Count)
            {
                MessageBox.Show("BUG! This shouldn't happen. There should be a valid particle " +
                    "system ID in the text box.", "Impossible Bug is Possible");
            }
            Particle.System.Def sysDef = m_SysDefs[sysDefID];
            //List<Particle.System> usedSystems = m_ParticleManager.m_Systems.Where(x => x.m_Def == sysDef).ToList();

            Console.WriteLine($"{value.GetType()} value = {value}");
            Console.WriteLine($"Category = {e.ChangedItem.PropertyDescriptor.Category}");
            Console.WriteLine($"Label = {e.ChangedItem.Label}");

            // General
            if (e.ChangedItem.PropertyDescriptor.Category == "General")
            {
                Particle.MainInfo info = sysDef.m_MainInfo;

                if (e.ChangedItem.Label == "Spawn Shape")
                    info.m_SpawnShape = (Particle.MainInfo.SpawnShape)(int)value;
                else if (e.ChangedItem.Label == "Draw Mode")
                    info.m_DrawMode = (Particle.MainInfo.DrawMode)(int)value;
                else if (e.ChangedItem.Label == "Plane")
                    info.m_Plane = (Particle.MainInfo.Plane)(int)value;
                else if (e.ChangedItem.Label == "Rotate")
                    info.m_Rotate = (bool)value;
                else if (e.ChangedItem.Label == "Random Initial Angle")
                    info.m_RandomInitAng = (bool)value;
                else if (e.ChangedItem.Label == "Self Destruct")
                    info.m_SelfDestruct = (bool)value;
                else if (e.ChangedItem.Label == "Follow System")
                    info.m_FollowSystem = (bool)value;
                else if (e.ChangedItem.Label == "Rotate Around Weird Axis")
                    info.m_WeirdAxis = (bool)value;
                else if (e.ChangedItem.Label == "Horizontal if 3D")
                    info.m_HorzIf3D = (bool)value;
                else if (e.ChangedItem.Label == "Rate")
                    info.m_Rate = (float)value;
                else if (e.ChangedItem.Label == "Distance")
                    info.m_StartHorzDist = (float)value;
                else if (e.ChangedItem.Label == "Direction X")
                    info.m_Dir.X = (float)value;
                else if (e.ChangedItem.Label == "Direction Y")
                    info.m_Dir.Y = (float)value;
                else if (e.ChangedItem.Label == "Direction Z")
                    info.m_Dir.Z = (float)value;
                else if (e.ChangedItem.Label == "Color")
                    info.m_Color = (Color)value;
                else if (e.ChangedItem.Label == "Horizontal Speed")
                    info.m_HorzSpeed = (float)value;
                else if (e.ChangedItem.Label == "Vertical Speed")
                    info.m_VertSpeed = (float)value;
                else if (e.ChangedItem.Label == "Scale")
                    info.m_Scale = (float)value;
                else if (e.ChangedItem.Label == "Horizontal Scale Multiplier")
                    info.m_HorzScaleMult = (float)value;
                else if (e.ChangedItem.Label == "Minimum Angular Speed")
                    info.m_MinAngleSpeed = (short)value;
                else if (e.ChangedItem.Label == "Maximum Angular Speed")
                    info.m_MaxAngleSpeed = (short)value;
                else if (e.ChangedItem.Label == "Frames")
                    info.m_Frames = (int)value;
                else if (e.ChangedItem.Label == "Lifetime")
                    info.m_Lifetime = (int)value;
                else if (e.ChangedItem.Label == "Scale Randomness")
                    info.m_ScaleRand = (int)value;
                else if (e.ChangedItem.Label == "Lifetime Randomness")
                    info.m_LifetimeRand = (int)value;
                else if (e.ChangedItem.Label == "Speed Randomness")
                    info.m_SpeedRand = (int)value;
                else if (e.ChangedItem.Label == "Period")
                    info.m_SpawnPeriod = (int)value;
                else if (e.ChangedItem.Label == "Opacity")
                    info.m_Alpha = (int)value;
                else if (e.ChangedItem.Label == "Speed Falloff")
                    info.m_SpeedFalloff = (int)value;
                else if (e.ChangedItem.Label == "Sprite ID")
                    info.m_Sprite = m_Textures[(int)value];
                else if (e.ChangedItem.Label == "Alternate Length")
                    info.m_AltLength = (int)value;
                else if (e.ChangedItem.Label == "Velocity Stretch Factor")
                    info.m_VelStretchFactor = (float)value;
                else if (e.ChangedItem.Label == "Texture Repeat X")
                    info.m_LogTexRepeatHorz = (int)value;
                else if (e.ChangedItem.Label == "Texture Repeat Y")
                    info.m_LogTexRepeatVert = (int)value;
                else if (e.ChangedItem.Label == "Has Scale Transition")
                {
                    if ((bool)value)
                        sysDef.m_ScaleTrans = new Particle.ScaleTransition();
                    else
                        sysDef.m_ScaleTrans = null;

                    // regenerate the properties to add / remove the category to the property grid
                    ParticleSysDefProperties.GenerateProperties(sysDef, m_Textures);
                    pgSysDefProps.SelectedObject = m_SysDefs[sysDefID].m_Properties;
                }
                else if (e.ChangedItem.Label == "Has Color Transition")
                {
                    if ((bool)value)
                        sysDef.m_ColorTrans = new Particle.ColorTransition();
                    else
                        sysDef.m_ColorTrans = null;

                    // regenerate the properties to add / remove the category to the property grid
                    ParticleSysDefProperties.GenerateProperties(sysDef, m_Textures);
                    pgSysDefProps.SelectedObject = m_SysDefs[sysDefID].m_Properties;
                }
                else if (e.ChangedItem.Label == "Has Alpha Transition")
                {
                    if ((bool)value)
                        sysDef.m_AlphaTrans = new Particle.AlphaTransition();
                    else
                        sysDef.m_AlphaTrans = null;

                    // regenerate the properties to add / remove the category to the property grid
                    ParticleSysDefProperties.GenerateProperties(sysDef, m_Textures);
                    pgSysDefProps.SelectedObject = m_SysDefs[sysDefID].m_Properties;
                }
                else if (e.ChangedItem.Label == "Has Texture Sequence")
                {
                    if ((bool)value)
                    {
                        sysDef.m_TexSeq = new Particle.TextureSequence();
                        sysDef.m_TexSeq.m_NumSprites = 1;
                        sysDef.m_TexSeq.m_Sprites = new Particle.Texture[8];
                        sysDef.m_TexSeq.m_Sprites[0] = m_Textures[0];
                    }
                    else
                        sysDef.m_TexSeq = null;

                    // regenerate the properties to add / remove the category to the property grid
                    ParticleSysDefProperties.GenerateProperties(sysDef, m_Textures);
                    pgSysDefProps.SelectedObject = m_SysDefs[sysDefID].m_Properties;
                }
                else if (e.ChangedItem.Label == "Has Glitter")
                {
                    if ((bool)value)
                    {
                        sysDef.m_Glitter = new Particle.Glitter();
                        sysDef.m_Glitter.m_Sprite = m_Textures[0];
                    }
                    else
                        sysDef.m_Glitter = null;

                    // regenerate the properties to add / remove the category to the property grid
                    ParticleSysDefProperties.GenerateProperties(sysDef, m_Textures);
                    pgSysDefProps.SelectedObject = m_SysDefs[sysDefID].m_Properties;
                }
                else if (e.ChangedItem.Label == "Has Acceleration Effect")
                {
                    if ((bool)value)
                        sysDef.m_Effects.Add(new Particle.Acceleration());
                    else
                        sysDef.m_Effects.Remove(sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.Acceleration)]);

                    // regenerate the properties to add / remove the category to the property grid
                    ParticleSysDefProperties.GenerateProperties(sysDef, m_Textures);
                    pgSysDefProps.SelectedObject = m_SysDefs[sysDefID].m_Properties;
                }
                else if (e.ChangedItem.Label == "Has Jitter Effect")
                {
                    if ((bool)value)
                        sysDef.m_Effects.Add(new Particle.Jitter());
                    else
                        sysDef.m_Effects.Remove(sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.Jitter)]);

                    // regenerate the properties to add / remove the category to the property grid
                    ParticleSysDefProperties.GenerateProperties(sysDef, m_Textures);
                    pgSysDefProps.SelectedObject = m_SysDefs[sysDefID].m_Properties;
                }
                else if (e.ChangedItem.Label == "Has Converge Effect")
                {
                    if ((bool)value)
                        sysDef.m_Effects.Add(new Particle.Converge());
                    else
                        sysDef.m_Effects.Remove(sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.Converge)]);

                    // regenerate the properties to add / remove the category to the property grid
                    ParticleSysDefProperties.GenerateProperties(sysDef, m_Textures);
                    pgSysDefProps.SelectedObject = m_SysDefs[sysDefID].m_Properties;
                }
                else if (e.ChangedItem.Label == "Has Turn Effect")
                {
                    if ((bool)value)
                        sysDef.m_Effects.Add(new Particle.Turn());
                    else
                        sysDef.m_Effects.Remove(sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.Turn)]);

                    // regenerate the properties to add / remove the category to the property grid
                    ParticleSysDefProperties.GenerateProperties(sysDef, m_Textures);
                    pgSysDefProps.SelectedObject = m_SysDefs[sysDefID].m_Properties;
                }
                else if (e.ChangedItem.Label == "Has Limit Plane Effect")
                {
                    if ((bool)value)
                        sysDef.m_Effects.Add(new Particle.LimitPlane());
                    else
                        sysDef.m_Effects.Remove(sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.LimitPlane)]);

                    // regenerate the properties to add / remove the category to the property grid
                    ParticleSysDefProperties.GenerateProperties(sysDef, m_Textures);
                    pgSysDefProps.SelectedObject = m_SysDefs[sysDefID].m_Properties;
                }
                else if (e.ChangedItem.Label == "Has Radius Converge Effect")
                {
                    if ((bool)value)
                        sysDef.m_Effects.Add(new Particle.RadiusConverge());
                    else
                        sysDef.m_Effects.Remove(sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.RadiusConverge)]);

                    // regenerate the properties to add / remove the category to the property grid
                    ParticleSysDefProperties.GenerateProperties(sysDef, m_Textures);
                    pgSysDefProps.SelectedObject = m_SysDefs[sysDefID].m_Properties;
                }
            }
            else if (e.ChangedItem.PropertyDescriptor.Category == "Scale Transition")
            {
                Particle.ScaleTransition scaleTrans = sysDef.m_ScaleTrans;

                if (e.ChangedItem.Label == "Scale Multiplier 1")
                    scaleTrans.m_Start = (float)value;
                else if (e.ChangedItem.Label == "Scale Multiplier 2")
                    scaleTrans.m_Middle = (float)value;
                else if (e.ChangedItem.Label == "Scale Multiplier 3")
                    scaleTrans.m_End = (float)value;
                else if (e.ChangedItem.Label == "Transition 1-2 End (Scale)")
                    scaleTrans.m_Trans1End = (int)value;
                else if (e.ChangedItem.Label == "Transition 2-3 Start (Scale)")
                    scaleTrans.m_Trans2Start = (int)value;
                else if (e.ChangedItem.Label == "Use Alternate Length (Scale)")
                    scaleTrans.m_UseAltLength = (bool)value;
            }
            else if (e.ChangedItem.PropertyDescriptor.Category == "Color Transition")
            {
                Particle.ColorTransition colorTrans = sysDef.m_ColorTrans;

                if (e.ChangedItem.Label == "Color 1")
                    colorTrans.m_Start = (Color)value;
                else if (e.ChangedItem.Label == "Color 3")
                    colorTrans.m_End = (Color)value;
                else if (e.ChangedItem.Label == "Transition 1 - 2 Start")
                    colorTrans.m_Trans1Start = (int)value;
                else if (e.ChangedItem.Label == "Transition 2 - 3 Start")
                    colorTrans.m_Trans2Start = (int)value;
                else if (e.ChangedItem.Label == "Transition 2 - 3 End")
                    colorTrans.m_Trans2End = (int)value;
                else if (e.ChangedItem.Label == "Use As Options (Color)")
                    colorTrans.m_UseAsOptions = (bool)value;
                else if (e.ChangedItem.Label == "Use Alternate Length")
                    colorTrans.m_UseAltLength = (bool)value;
                else if (e.ChangedItem.Label == "Smooth Transition")
                    colorTrans.m_SmoothTrans = (bool)value;
            }
            else if (e.ChangedItem.PropertyDescriptor.Category == "Alpha Transition")
            {
                Particle.AlphaTransition alphaTrans = sysDef.m_AlphaTrans;

                if (e.ChangedItem.Label == "Opacity 1")
                    alphaTrans.m_Start = (int)value;
                else if (e.ChangedItem.Label == "Opacity 2")
                    alphaTrans.m_Middle = (int)value;
                else if (e.ChangedItem.Label == "Opacity 3")
                    alphaTrans.m_End = (int)value;
                else if (e.ChangedItem.Label == "Flicker")
                    alphaTrans.m_Flicker = (int)value;
                else if (e.ChangedItem.Label == "Transition 1-2 End (Alpha)")
                    alphaTrans.m_Trans1End = (int)value;
                else if (e.ChangedItem.Label == "Transition 2-3 Start (Alpha)")
                    alphaTrans.m_Trans2Start = (int)value;
                else if (e.ChangedItem.Label == "Use Alternate Length (Alpha)")
                    alphaTrans.m_UseAltLength = (bool)value;
            }
            else if (e.ChangedItem.PropertyDescriptor.Category == "Texture Sequence")
            {
                Particle.TextureSequence texSeq = sysDef.m_TexSeq;

                if (e.ChangedItem.Label == "Sprite ID 1")
                    texSeq.m_Sprites[0] = (int)value == -1 ? null : m_Textures[(int)value];
                else if (e.ChangedItem.Label == "Sprite ID 2")
                    texSeq.m_Sprites[1] = (int)value == -1 ? null : m_Textures[(int)value];
                else if (e.ChangedItem.Label == "Sprite ID 3")
                    texSeq.m_Sprites[2] = (int)value == -1 ? null : m_Textures[(int)value];
                else if (e.ChangedItem.Label == "Sprite ID 4")
                    texSeq.m_Sprites[3] = (int)value == -1 ? null : m_Textures[(int)value];
                else if (e.ChangedItem.Label == "Sprite ID 5")
                    texSeq.m_Sprites[4] = (int)value == -1 ? null : m_Textures[(int)value];
                else if (e.ChangedItem.Label == "Sprite ID 6")
                    texSeq.m_Sprites[5] = (int)value == -1 ? null : m_Textures[(int)value];
                else if (e.ChangedItem.Label == "Sprite ID 7")
                    texSeq.m_Sprites[6] = (int)value == -1 ? null : m_Textures[(int)value];
                else if (e.ChangedItem.Label == "Sprite ID 8")
                    texSeq.m_Sprites[7] = (int)value == -1 ? null : m_Textures[(int)value];
                else if (e.ChangedItem.Label == "Number of Sprites")
                    texSeq.m_NumSprites = (int)value;
                else if (e.ChangedItem.Label == "Interval")
                    texSeq.m_Interval = (int)value;
                else if (e.ChangedItem.Label == "Use As Options (Texture)")
                    texSeq.m_UseAsOptions = (bool)value;
                else if (e.ChangedItem.Label == "Use Alternate Length (Texture)")
                    texSeq.m_UseAltLength = (bool)value;
            }
            else if (e.ChangedItem.PropertyDescriptor.Category == "Glitter")
            {
                Particle.Glitter glitter = sysDef.m_Glitter;

                if (e.ChangedItem.Label == "Has Effects")
                    glitter.m_HasEffects = (bool)value;
                else if (e.ChangedItem.Label == "Has Scale Transition (Glitter)")
                    glitter.m_HasScaleTrans = (bool)value;
                else if (e.ChangedItem.Label == "Has Alpha Transition (Glitter)")
                    glitter.m_HasAlphaTrans = (bool)value;
                else if (e.ChangedItem.Label == "Angle Copy Mode")
                    glitter.m_AngleCopyMode = (Particle.Glitter.AngleCopyMode)value;
                else if (e.ChangedItem.Label == "Follow System (Glitter)")
                    glitter.m_FollowSystem = (bool)value;
                else if (e.ChangedItem.Label == "Use Glitter Color")
                    glitter.m_UseGlitterColor = (bool)value;
                else if (e.ChangedItem.Label == "Draw Mode (Glitter)")
                    glitter.m_DrawMode = (Particle.MainInfo.DrawMode)value;
                else if (e.ChangedItem.Label == "Speed Randomness (Glitter)")
                    glitter.m_SpeedRand = (float)value;
                else if (e.ChangedItem.Label == "Scale (Glitter)")
                    glitter.m_Scale = (float)value;
                else if (e.ChangedItem.Label == "Lifetime (Glitter)")
                    glitter.m_Lifetime = (int)value;
                else if (e.ChangedItem.Label == "Speed Multiplier")
                    glitter.m_SpeedMult = (int)value;
                else if (e.ChangedItem.Label == "Scale Multiplier")
                    glitter.m_ScaleMult = (int)value;
                else if (e.ChangedItem.Label == "Color (Glitter)")
                    glitter.m_Color = (Color)value;
                else if (e.ChangedItem.Label == "Rate (Glitter)")
                    glitter.m_Rate = (int)value;
                else if (e.ChangedItem.Label == "Wait")
                    glitter.m_Wait = (int)value;
                else if (e.ChangedItem.Label == "Period (Glitter)")
                    glitter.m_Period = (int)value;
                else if (e.ChangedItem.Label == "Sprite ID (Glitter)")
                    glitter.m_Sprite = m_Textures[(int)value];
                else if (e.ChangedItem.Label == "Texture Repeat X (Glitter)")
                    glitter.m_LogTexRepeatHorz = (int)value;
                else if (e.ChangedItem.Label == "Texture Repeat Y (Glitter)")
                    glitter.m_LogTexRepeatVert = (int)value;
            }
            else if (e.ChangedItem.PropertyDescriptor.Category == "Effect: Acceleration")
            {
                Particle.Acceleration acceleration = (Particle.Acceleration)sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.Acceleration)];

                if (e.ChangedItem.Label == "Acceleration X")
                    acceleration.m_Accel.X = (float)value;
                else if (e.ChangedItem.Label == "Acceleration Y")
                    acceleration.m_Accel.Y = (float)value;
                else if (e.ChangedItem.Label == "Acceleration Z")
                    acceleration.m_Accel.Z = (float)value;
            }
            else if (e.ChangedItem.PropertyDescriptor.Category == "Effect: Jitter")
            {
                Particle.Jitter jitter = (Particle.Jitter)sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.Jitter)];

                if (e.ChangedItem.Label == "Impulse X")
                    jitter.m_Mag.X = (float)value;
                else if (e.ChangedItem.Label == "Impulse Y")
                    jitter.m_Mag.Y = (float)value;
                else if (e.ChangedItem.Label == "Impulse Z")
                    jitter.m_Mag.Z = (float)value;
                else if (e.ChangedItem.Label == "Period (Jitter)")
                    jitter.m_Period = (int)value;
            }
            else if (e.ChangedItem.PropertyDescriptor.Category == "Effect: Converge")
            {
                Particle.Converge converge = (Particle.Converge)sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.Converge)];

                if (e.ChangedItem.Label == "Offset X (Converge)")
                    converge.m_Offset.X = (float)value;
                else if (e.ChangedItem.Label == "Offset Y (Converge)")
                    converge.m_Offset.Y = (float)value;
                else if (e.ChangedItem.Label == "Offset Z (Converge)")
                    converge.m_Offset.Z = (float)value;
                else if (e.ChangedItem.Label == "Magnitude (Converge)")
                    converge.m_Mag = (float)value;
            }
            else if (e.ChangedItem.PropertyDescriptor.Category == "Effect: Turn")
            {
                Particle.Turn turn = (Particle.Turn)sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.Turn)];

                if (e.ChangedItem.Label == "Angular Speed")
                    turn.m_AngleSpeed = (short)value;
                else if (e.ChangedItem.Label == "Axis")
                    turn.m_Axis = (Particle.Turn.Axis)value;
            }
            else if (e.ChangedItem.PropertyDescriptor.Category == "Effect: Limit Plane")
            {
                Particle.LimitPlane limitPlane = (Particle.LimitPlane)sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.LimitPlane)];

                if (e.ChangedItem.Label == "Position Y (Limit Plane)")
                    limitPlane.m_PosY = (float)value;
                else if (e.ChangedItem.Label == "Reverse Speed Multiplier")
                    limitPlane.m_ReverseSpeedMult = (float)value;
                else if (e.ChangedItem.Label == "Behavior (Limit Plane)")
                    limitPlane.m_Behavior = (Particle.LimitPlane.Behavior)value;
            }
            else if (e.ChangedItem.PropertyDescriptor.Category == "Effect: Radius Converge")
            {
                Particle.RadiusConverge radiusConverge = (Particle.RadiusConverge)sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.RadiusConverge)];

                if (e.ChangedItem.Label == "Offset X (Radius Converge)")
                    radiusConverge.m_Offset.X = (float)value;
                else if (e.ChangedItem.Label == "Offset Y (Radius Converge)")
                    radiusConverge.m_Offset.Y = (float)value;
                else if (e.ChangedItem.Label == "Offset Z (Radius Converge)")
                    radiusConverge.m_Offset.Z = (float)value;
                else if (e.ChangedItem.Label == "Magnitude (Radius Converge)")
                    radiusConverge.m_Mag = (float)value;
            }
        }

        private void loadSPAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_ROMFileSelect.ReInitialize("Select a SPA file to load", new String[] { ".spa" });
            DialogResult result = m_ROMFileSelect.ShowDialog();
            if (result != DialogResult.OK)
                return;
            else
                m_Name = m_ROMFileSelect.m_SelectedFile;

            try
            {
                m_ParticleArchiveFile = Program.m_ROM.GetFileFromName(m_Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }

            foreach (Particle.Texture texture in m_Textures)
                texture.Unload();
            
            m_ParticleManager = new Particle.Manager();
            m_SysDefs = new List<Particle.System.Def>();
            m_Textures = new List<Particle.Texture>();

            LoadMainSysDefs();

            foreach (Particle.Texture texture in m_Textures)
                texture.Load();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSPA();
        }

        private void SaveSPA()
        {
            // SPA header (1/3)
            m_ParticleArchiveFile.Write16(0x08, (ushort)m_SysDefs.Count);
            m_ParticleArchiveFile.Write16(0x0a, (ushort)builtInTextures);

            if (externalSPT)
            {
                m_ParticleArchiveFile.Write8(0x0a, (byte)m_Textures.Count);
                m_ParticleArchiveFile.Write8(0x0b, (byte)builtInTextures);
            }

            // SPA data
            uint offset = 0x20;
            for (int i = 0; i < m_SysDefs.Count; i++)
            {
                Particle.System.Def sysDef = m_SysDefs[i];
                Particle.MainInfo info = sysDef.m_MainInfo;

                uint currentSysDefFlag = (uint)info.m_SpawnShape;
                currentSysDefFlag |= (uint)info.m_DrawMode << 4;
                currentSysDefFlag |= (uint)info.m_Plane << 6;
                currentSysDefFlag |= Convert.ToUInt32(info.m_Rotate) << 12;
                currentSysDefFlag |= Convert.ToUInt32(info.m_RandomInitAng) << 13;
                currentSysDefFlag |= Convert.ToUInt32(info.m_SelfDestruct) << 14;
                currentSysDefFlag |= Convert.ToUInt32(info.m_FollowSystem) << 15;
                currentSysDefFlag |= Convert.ToUInt32(info.m_WeirdAxis) << 17;
                currentSysDefFlag |= Convert.ToUInt32(info.m_HorzIf3D) << 19;
                currentSysDefFlag |= (uint)(sysDef.m_ScaleTrans == null ? 0 : 1) << 8;
                currentSysDefFlag |= (uint)(sysDef.m_ColorTrans == null ? 0 : 1) << 9;
                currentSysDefFlag |= (uint)(sysDef.m_AlphaTrans == null ? 0 : 1) << 10;
                currentSysDefFlag |= (uint)(sysDef.m_TexSeq == null ? 0 : 1) << 11;
                currentSysDefFlag |= (uint)(sysDef.m_Glitter == null ? 0 : 1) << 16;
                currentSysDefFlag |= (uint)(sysDef.m_Effects.FindIndex(x => x is Particle.Acceleration) == -1 ? 0 : 1) << 24;
                currentSysDefFlag |= (uint)(sysDef.m_Effects.FindIndex(x => x is Particle.Jitter) == -1 ? 0 : 1) << 25;
                currentSysDefFlag |= (uint)(sysDef.m_Effects.FindIndex(x => x is Particle.Converge) == -1 ? 0 : 1) << 26;
                currentSysDefFlag |= (uint)(sysDef.m_Effects.FindIndex(x => x is Particle.Turn) == -1 ? 0 : 1) << 27;
                currentSysDefFlag |= (uint)(sysDef.m_Effects.FindIndex(x => x is Particle.LimitPlane) == -1 ? 0 : 1) << 28;
                currentSysDefFlag |= (uint)(sysDef.m_Effects.FindIndex(x => x is Particle.RadiusConverge) == -1 ? 0 : 1) << 29;

                int texRepeatFlags = (info.m_LogTexRepeatHorz & 0x3) << 0;
                texRepeatFlags |= (info.m_LogTexRepeatVert & 0x3) << 2;

                m_ParticleArchiveFile.Write32(offset, currentSysDefFlag);
                m_ParticleArchiveFile.Write32(offset + 0x4, (uint)(info.m_Rate * 4096.0f));
                m_ParticleArchiveFile.Write32(offset + 0x8, (uint)(info.m_StartHorzDist * 512.0f));
                m_ParticleArchiveFile.Write16(offset + 0xc, (ushort)(info.m_Dir.X * 4096.0f));
                m_ParticleArchiveFile.Write16(offset + 0xe, (ushort)(info.m_Dir.Y * 4096.0f));
                m_ParticleArchiveFile.Write16(offset + 0x10, (ushort)(info.m_Dir.Z * 4096.0f));
                m_ParticleArchiveFile.Write16(offset + 0x12, Helper.ColorToBGR15(info.m_Color));
                m_ParticleArchiveFile.Write32(offset + 0x14, (uint)(info.m_HorzSpeed * 512.0f));
                m_ParticleArchiveFile.Write32(offset + 0x18, (uint)(info.m_VertSpeed * 512.0f));
                m_ParticleArchiveFile.Write32(offset + 0x1c, (uint)(info.m_Scale * 512.0f));
                m_ParticleArchiveFile.Write16(offset + 0x20, (ushort)(info.m_HorzScaleMult * 4096.0f));
                m_ParticleArchiveFile.Write16(offset + 0x24, (ushort)info.m_MinAngleSpeed);
                m_ParticleArchiveFile.Write16(offset + 0x26, (ushort)info.m_MaxAngleSpeed);
                m_ParticleArchiveFile.Write16(offset + 0x28, (ushort)info.m_Frames);
                m_ParticleArchiveFile.Write16(offset + 0x2a, (ushort)info.m_Lifetime);
                m_ParticleArchiveFile.Write8(offset + 0x2c, (byte)info.m_ScaleRand);
                m_ParticleArchiveFile.Write8(offset + 0x2d, (byte)info.m_LifetimeRand);
                m_ParticleArchiveFile.Write8(offset + 0x2e, (byte)info.m_SpeedRand);
                m_ParticleArchiveFile.Write8(offset + 0x30, (byte)info.m_SpawnPeriod);
                m_ParticleArchiveFile.Write8(offset + 0x31, (byte)info.m_Alpha);
                m_ParticleArchiveFile.Write8(offset + 0x32, (byte)info.m_SpeedFalloff);
                m_ParticleArchiveFile.Write8(offset + 0x33, (byte)m_Textures.IndexOf(info.m_Sprite));
                m_ParticleArchiveFile.Write8(offset + 0x34, (byte)info.m_AltLength);
                m_ParticleArchiveFile.Write16(offset + 0x35, (ushort)(info.m_VelStretchFactor * 4096.0f));
                m_ParticleArchiveFile.Write8(offset + 0x37, (byte)texRepeatFlags);

                offset += 0x38;

                if (sysDef.m_ScaleTrans != null)
                {
                    Particle.ScaleTransition scaleTrans = sysDef.m_ScaleTrans;
                    m_ParticleArchiveFile.Write16(offset + 0x00, (ushort)(scaleTrans.m_Start * 4096.0f));
                    m_ParticleArchiveFile.Write16(offset + 0x02, (ushort)(scaleTrans.m_Middle * 4096.0f));
                    m_ParticleArchiveFile.Write16(offset + 0x04, (ushort)(scaleTrans.m_End * 4096.0f));
                    m_ParticleArchiveFile.Write8(offset + 0x06, (byte)scaleTrans.m_Trans1End);
                    m_ParticleArchiveFile.Write8(offset + 0x07, (byte)scaleTrans.m_Trans2Start);
                    m_ParticleArchiveFile.Write8(offset + 0x08, (byte)(scaleTrans.m_UseAltLength ? 1 : 0));

                    offset += 0x0c;
                }
                if (sysDef.m_ColorTrans != null)
                {
                    Particle.ColorTransition colorTrans = sysDef.m_ColorTrans;

                    byte interpFlags = (byte)((colorTrans.m_UseAsOptions ? 1 : 0) << 0);
                    interpFlags |= (byte)((colorTrans.m_UseAltLength ? 1 : 0) << 1);
                    interpFlags |= (byte)((colorTrans.m_SmoothTrans ? 1 : 0) << 2);

                    m_ParticleArchiveFile.Write16(offset + 0x00, Helper.ColorToBGR15(colorTrans.m_Start));
                    m_ParticleArchiveFile.Write16(offset + 0x02, Helper.ColorToBGR15(colorTrans.m_End));
                    m_ParticleArchiveFile.Write8(offset + 0x04, (byte)colorTrans.m_Trans1Start);
                    m_ParticleArchiveFile.Write8(offset + 0x05, (byte)colorTrans.m_Trans2Start);
                    m_ParticleArchiveFile.Write8(offset + 0x06, (byte)colorTrans.m_Trans2End);
                    m_ParticleArchiveFile.Write8(offset + 0x08, interpFlags);

                    offset += 0x0c;
                }
                if (sysDef.m_AlphaTrans != null)
                {
                    Particle.AlphaTransition alphaTrans = sysDef.m_AlphaTrans;

                    uint alphas = (uint)(alphaTrans.m_Start & 0x1f) << 0;
                    alphas |= (uint)(alphaTrans.m_Middle & 0x1f) << 5;
                    alphas |= (uint)(alphaTrans.m_End & 0x1f) << 10;

                    m_ParticleArchiveFile.Write16(offset + 0x00, (ushort)alphas);
                    m_ParticleArchiveFile.Write8(offset + 0x02, (byte)alphaTrans.m_Flicker);
                    m_ParticleArchiveFile.Write8(offset + 0x03, (byte)(alphaTrans.m_UseAltLength ? 1 : 0));
                    m_ParticleArchiveFile.Write8(offset + 0x04, (byte)alphaTrans.m_Trans1End);
                    m_ParticleArchiveFile.Write8(offset + 0x05, (byte)alphaTrans.m_Trans2Start);

                    offset += 0x08;
                }
                if (sysDef.m_TexSeq != null)
                {
                    Particle.TextureSequence texSeq = sysDef.m_TexSeq;

                    uint interpFlags = (uint)(texSeq.m_UseAsOptions ? 1 : 0) << 0;
                    interpFlags |= (uint)(texSeq.m_UseAltLength ? 1 : 0) << 1;

                    for (uint j = 0; j < 8; ++j)
                    {
                        if (texSeq.m_Sprites[j] != null)
                            m_ParticleArchiveFile.Write8(offset + j, (byte)m_Textures.IndexOf(texSeq.m_Sprites[j]));
                        else
                            m_ParticleArchiveFile.Write8(offset + j, 0xff);
                    }
                    m_ParticleArchiveFile.Write8(offset + 0x08, (byte)texSeq.m_NumSprites);
                    m_ParticleArchiveFile.Write8(offset + 0x09, (byte)texSeq.m_Interval);
                    m_ParticleArchiveFile.Write8(offset + 0x0a, (byte)interpFlags);

                    offset += 0x0c;
                }
                if (sysDef.m_Glitter != null)
                {
                    Particle.Glitter glitter = sysDef.m_Glitter;

                    int gFlags = (glitter.m_HasEffects ? 1 : 0) << 0;
                    gFlags |= (glitter.m_HasScaleTrans ? 1 : 0) << 1;
                    gFlags |= (glitter.m_HasAlphaTrans ? 1 : 0) << 2;
                    gFlags |= ((int)glitter.m_AngleCopyMode & 0x3) << 3;
                    gFlags |= (glitter.m_FollowSystem ? 1 : 0) << 5;
                    gFlags |= (glitter.m_UseGlitterColor ? 1 : 0) << 6;
                    gFlags |= ((int)glitter.m_DrawMode & 0x3) << 7;

                    int gTexRepeatFlags = (glitter.m_LogTexRepeatHorz & 0x3) << 0;
                    gTexRepeatFlags |= (glitter.m_LogTexRepeatVert & 0x3) << 2;

                    m_ParticleArchiveFile.Write16(offset + 0x00, (ushort)gFlags);
                    m_ParticleArchiveFile.Write16(offset + 0x02, (ushort)(glitter.m_SpeedRand * 512.0f));
                    m_ParticleArchiveFile.Write16(offset + 0x04, (ushort)(glitter.m_Scale * 4096.0f));
                    m_ParticleArchiveFile.Write16(offset + 0x06, (ushort)glitter.m_Lifetime);
                    m_ParticleArchiveFile.Write8(offset + 0x08, (byte)glitter.m_SpeedMult);
                    m_ParticleArchiveFile.Write8(offset + 0x09, (byte)glitter.m_ScaleMult);
                    m_ParticleArchiveFile.Write16(offset + 0x0a, Helper.ColorToBGR15(glitter.m_Color));
                    m_ParticleArchiveFile.Write8(offset + 0x0c, (byte)glitter.m_Rate);
                    m_ParticleArchiveFile.Write8(offset + 0x0d, (byte)glitter.m_Wait);
                    m_ParticleArchiveFile.Write8(offset + 0x0e, (byte)glitter.m_Period);
                    m_ParticleArchiveFile.Write8(offset + 0x0f, (byte)m_Textures.IndexOf(glitter.m_Sprite));
                    m_ParticleArchiveFile.Write8(offset + 0x10, (byte)gTexRepeatFlags);

                    offset += 0x14;
                }
                if (sysDef.m_Effects.FindIndex(x => x is Particle.Acceleration) != -1)
                {
                    Particle.Acceleration accel = (Particle.Acceleration)sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.Acceleration)];

                    m_ParticleArchiveFile.Write16(offset + 0x00, (ushort)(accel.m_Accel.X * 512.0f));
                    m_ParticleArchiveFile.Write16(offset + 0x02, (ushort)(accel.m_Accel.Y * 512.0f));
                    m_ParticleArchiveFile.Write16(offset + 0x04, (ushort)(accel.m_Accel.Z * 512.0f));

                    offset += 0x08;
                }
                if (sysDef.m_Effects.FindIndex(x => x is Particle.Jitter) != -1)
                {
                    Particle.Jitter jitter = (Particle.Jitter)sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.Jitter)];

                    m_ParticleArchiveFile.Write16(offset + 0x00, (ushort)(jitter.m_Mag.X * 512.0f));
                    m_ParticleArchiveFile.Write16(offset + 0x02, (ushort)(jitter.m_Mag.Y * 512.0f));
                    m_ParticleArchiveFile.Write16(offset + 0x04, (ushort)(jitter.m_Mag.Z * 512.0f));
                    m_ParticleArchiveFile.Write16(offset + 0x06, (ushort)jitter.m_Period);

                    offset += 0x08;
                }
                if (sysDef.m_Effects.FindIndex(x => x is Particle.Converge) != -1)
                {
                    Particle.Converge converge = (Particle.Converge)sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.Converge)];

                    m_ParticleArchiveFile.Write32(offset + 0x00, (uint)(converge.m_Offset.X * 512.0f));
                    m_ParticleArchiveFile.Write32(offset + 0x04, (uint)(converge.m_Offset.Y * 512.0f));
                    m_ParticleArchiveFile.Write32(offset + 0x08, (uint)(converge.m_Offset.Z * 512.0f));
                    m_ParticleArchiveFile.Write32(offset + 0x0c, (uint)(converge.m_Mag * 4096.0f));

                    offset += 0x10;
                }
                if (sysDef.m_Effects.FindIndex(x => x is Particle.Turn) != -1)
                {
                    Particle.Turn turn = (Particle.Turn)sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.Turn)];

                    m_ParticleArchiveFile.Write16(offset + 0x00, (ushort)turn.m_AngleSpeed);
                    m_ParticleArchiveFile.Write16(offset + 0x02, (ushort)turn.m_Axis);

                    offset += 0x04;
                }
                if (sysDef.m_Effects.FindIndex(x => x is Particle.LimitPlane) != -1)
                {
                    Particle.LimitPlane limit = (Particle.LimitPlane)sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.LimitPlane)];

                    m_ParticleArchiveFile.Write32(offset + 0x00, (uint)(limit.m_PosY * 512.0f));
                    m_ParticleArchiveFile.Write16(offset + 0x04, (ushort)(limit.m_ReverseSpeedMult * 4096.0f));
                    m_ParticleArchiveFile.Write8(offset + 0x06, (byte)limit.m_Behavior);

                    offset += 0x08;
                }
                if (sysDef.m_Effects.FindIndex(x => x is Particle.RadiusConverge) != -1)
                {
                    Particle.RadiusConverge converge = (Particle.RadiusConverge)sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.RadiusConverge)];

                    m_ParticleArchiveFile.Write32(offset + 0x00, (uint)(converge.m_Offset.X * 512.0f));
                    m_ParticleArchiveFile.Write32(offset + 0x04, (uint)(converge.m_Offset.Y * 512.0f));
                    m_ParticleArchiveFile.Write32(offset + 0x08, (uint)(converge.m_Offset.Z * 512.0f));
                    m_ParticleArchiveFile.Write16(offset + 0x0c, (ushort)(converge.m_Mag * 4096.0f));

                    offset += 0x10;
                }
            }

            // SPA header (2/3)
            m_ParticleArchiveFile.Write32(0x10, offset - 0x20); // sysdef section size
            m_ParticleArchiveFile.Write32(0x18, offset); // texture section offset
            uint SPT_size = 0;

            // SPT data
            for (int i = 0; i < builtInTextures; ++i)
            {
                uint flags = (uint)m_Textures[i].m_Tex.m_TexType;
                flags |= ((uint)Math.Log(m_Textures[i].m_Tex.m_Width, 2) - 3) << 4;
                flags |= ((uint)Math.Log(m_Textures[i].m_Tex.m_Height, 2) - 3) << 8;
                flags |= (uint)(m_Textures[i].m_RepeatX == Particle.Texture.RepeatMode.REPEAT ? 0x1000 : m_Textures[i].m_RepeatX == Particle.Texture.RepeatMode.FLIP ? 0x5000 : 0);
                flags |= (uint)(m_Textures[i].m_RepeatY == Particle.Texture.RepeatMode.REPEAT ? 0x2000 : m_Textures[i].m_RepeatY == Particle.Texture.RepeatMode.FLIP ? 0xa000 : 0);
                flags |= (uint)(m_Textures[i].m_Tex.m_Colour0Mode != 0 ? 1 : 0) << 16;

                uint texelArrSize = (uint)m_Textures[i].m_Tex.m_RawTextureData.Length;
                uint palOffset = 0x20 + texelArrSize;
                uint palSize = (uint)m_Textures[i].m_Tex.m_RawPaletteData.Length;
                uint totalSize = palOffset + palSize;

                m_ParticleArchiveFile.Write32(offset, 0x53505420); // "SPT " in ascii
                m_ParticleArchiveFile.Write32(offset + 0x04, flags); // flags

                m_ParticleArchiveFile.Write32(offset + 0x08, texelArrSize); // texelArrSize
                m_ParticleArchiveFile.Write32(offset + 0x0c, palOffset); // palOffset
                m_ParticleArchiveFile.Write32(offset + 0x10, palSize); // palSize
                m_ParticleArchiveFile.Write32(offset + 0x1c, totalSize); // totalSize

                m_ParticleArchiveFile.WriteBlock(offset + 0x20, m_Textures[i].m_Tex.m_RawTextureData);
                m_ParticleArchiveFile.WriteBlock(offset + palOffset, m_Textures[i].m_Tex.m_RawPaletteData);

                offset += totalSize;
                SPT_size += totalSize;
            }
            //m_ParticleArchiveFile.WriteBlock(offset, m_SPT_data);
            //offset += (uint)m_SPT_data.Length;

            // SPA header (3/3)
            m_ParticleArchiveFile.Write32(0x14, SPT_size); // texture section size

            // make it so the file is only as big as it needs to be
            if (m_ParticleArchiveFile.m_Data.Length - (int)offset > 0)
                m_ParticleArchiveFile.RemoveSpace(offset, (uint)m_ParticleArchiveFile.m_Data.Length - offset);

            m_ParticleArchiveFile.SaveChanges();
        }

        private void SaveParticleAsSPD(int particleId, string filePath)
        {
            uint offset = 0;
            INitroROMBlock particleData = new INitroROMBlock();
            particleData.m_Data = new byte[1];

            Particle.System.Def sysDef = m_SysDefs[particleId];
            Particle.MainInfo info = sysDef.m_MainInfo;

            bool hasScaleTrans = sysDef.m_ScaleTrans != null;
            bool hasColorTrans = sysDef.m_ColorTrans != null;
            bool hasAlphaTrans = sysDef.m_AlphaTrans != null;
            bool hasTexSeq = sysDef.m_TexSeq != null;
            bool hasGlitter = sysDef.m_Glitter != null;
            bool hasAcceleration = sysDef.m_Effects.FindIndex(x => x is Particle.Acceleration) != -1;
            bool hasJitter = sysDef.m_Effects.FindIndex(x => x is Particle.Jitter) != -1;
            bool hasConverge = sysDef.m_Effects.FindIndex(x => x is Particle.Converge) != -1;
            bool hasTurn = sysDef.m_Effects.FindIndex(x => x is Particle.Turn) != -1;
            bool hasLimitPlane = sysDef.m_Effects.FindIndex(x => x is Particle.LimitPlane) != -1;
            bool hasRadiusConverge = sysDef.m_Effects.FindIndex(x => x is Particle.RadiusConverge) != -1;

            uint currentSysDefFlag = (uint)info.m_SpawnShape;
            currentSysDefFlag |= (uint)info.m_DrawMode << 4;
            currentSysDefFlag |= (uint)info.m_Plane << 6;
            currentSysDefFlag |= Convert.ToUInt32(info.m_Rotate) << 12;
            currentSysDefFlag |= Convert.ToUInt32(info.m_RandomInitAng) << 13;
            currentSysDefFlag |= Convert.ToUInt32(info.m_SelfDestruct) << 14;
            currentSysDefFlag |= Convert.ToUInt32(info.m_FollowSystem) << 15;
            currentSysDefFlag |= Convert.ToUInt32(info.m_WeirdAxis) << 17;
            currentSysDefFlag |= Convert.ToUInt32(info.m_HorzIf3D) << 19;
            currentSysDefFlag |= (uint)(sysDef.m_ScaleTrans == null ? 0 : 1) << 8;
            currentSysDefFlag |= (uint)(sysDef.m_ColorTrans == null ? 0 : 1) << 9;
            currentSysDefFlag |= (uint)(sysDef.m_AlphaTrans == null ? 0 : 1) << 10;
            currentSysDefFlag |= (uint)(sysDef.m_TexSeq == null ? 0 : 1) << 11;
            currentSysDefFlag |= (uint)(sysDef.m_Glitter == null ? 0 : 1) << 16;
            currentSysDefFlag |= (uint)(sysDef.m_Effects.FindIndex(x => x is Particle.Acceleration) == -1 ? 0 : 1) << 24;
            currentSysDefFlag |= (uint)(sysDef.m_Effects.FindIndex(x => x is Particle.Jitter) == -1 ? 0 : 1) << 25;
            currentSysDefFlag |= (uint)(sysDef.m_Effects.FindIndex(x => x is Particle.Converge) == -1 ? 0 : 1) << 26;
            currentSysDefFlag |= (uint)(sysDef.m_Effects.FindIndex(x => x is Particle.Turn) == -1 ? 0 : 1) << 27;
            currentSysDefFlag |= (uint)(sysDef.m_Effects.FindIndex(x => x is Particle.LimitPlane) == -1 ? 0 : 1) << 28;
            currentSysDefFlag |= (uint)(sysDef.m_Effects.FindIndex(x => x is Particle.RadiusConverge) == -1 ? 0 : 1) << 29;

            int texRepeatFlags = (info.m_LogTexRepeatHorz & 0x3) << 0;
            texRepeatFlags |= (info.m_LogTexRepeatVert & 0x3) << 2;

            particleData.Write32(offset, currentSysDefFlag);
            particleData.Write32(offset + 0x4, (uint)(info.m_Rate * 4096.0f));
            particleData.Write32(offset + 0x8, (uint)(info.m_StartHorzDist * 512.0f));
            particleData.Write16(offset + 0xc, (ushort)(info.m_Dir.X * 4096.0f));
            particleData.Write16(offset + 0xe, (ushort)(info.m_Dir.Y * 4096.0f));
            particleData.Write16(offset + 0x10, (ushort)(info.m_Dir.Z * 4096.0f));
            particleData.Write16(offset + 0x12, Helper.ColorToBGR15(info.m_Color));
            particleData.Write32(offset + 0x14, (uint)(info.m_HorzSpeed * 512.0f));
            particleData.Write32(offset + 0x18, (uint)(info.m_VertSpeed * 512.0f));
            particleData.Write32(offset + 0x1c, (uint)(info.m_Scale * 512.0f));
            particleData.Write16(offset + 0x20, (ushort)(info.m_HorzScaleMult * 4096.0f));
            particleData.Write16(offset + 0x24, (ushort)info.m_MinAngleSpeed);
            particleData.Write16(offset + 0x26, (ushort)info.m_MaxAngleSpeed);
            particleData.Write16(offset + 0x28, (ushort)info.m_Frames);
            particleData.Write16(offset + 0x2a, (ushort)info.m_Lifetime);
            particleData.Write8(offset + 0x2c, (byte)info.m_ScaleRand);
            particleData.Write8(offset + 0x2d, (byte)info.m_LifetimeRand);
            particleData.Write8(offset + 0x2e, (byte)info.m_SpeedRand);
            particleData.Write8(offset + 0x30, (byte)info.m_SpawnPeriod);
            particleData.Write8(offset + 0x31, (byte)info.m_Alpha);
            particleData.Write8(offset + 0x32, (byte)info.m_SpeedFalloff);
            particleData.Write8(offset + 0x33, (byte)m_Textures.IndexOf(info.m_Sprite));
            Console.WriteLine($"spriteID ({particleId}) = {m_Textures.IndexOf(info.m_Sprite)}");
            particleData.Write8(offset + 0x34, (byte)info.m_AltLength);
            particleData.Write16(offset + 0x35, (ushort)(info.m_VelStretchFactor * 4096.0f));
            particleData.Write8(offset + 0x37, (byte)texRepeatFlags);

            offset += 0x38;

            if (hasScaleTrans)
            {
                Particle.ScaleTransition scaleTrans = sysDef.m_ScaleTrans;
                particleData.Write16(offset + 0x00, (ushort)(scaleTrans.m_Start * 4096.0f));
                particleData.Write16(offset + 0x02, (ushort)(scaleTrans.m_Middle * 4096.0f));
                particleData.Write16(offset + 0x04, (ushort)(scaleTrans.m_End * 4096.0f));
                particleData.Write8(offset + 0x06, (byte)scaleTrans.m_Trans1End);
                particleData.Write8(offset + 0x07, (byte)scaleTrans.m_Trans2Start);
                particleData.Write8(offset + 0x08, (byte)(scaleTrans.m_UseAltLength ? 1 : 0));

                offset += 0x0c;
            }
            if (hasColorTrans)
            {
                Particle.ColorTransition colorTrans = sysDef.m_ColorTrans;

                byte interpFlags = (byte)((colorTrans.m_UseAsOptions ? 1 : 0) << 0);
                interpFlags |= (byte)((colorTrans.m_UseAltLength ? 1 : 0) << 1);
                interpFlags |= (byte)((colorTrans.m_SmoothTrans ? 1 : 0) << 2);

                particleData.Write16(offset + 0x00, Helper.ColorToBGR15(colorTrans.m_Start));
                particleData.Write16(offset + 0x02, Helper.ColorToBGR15(colorTrans.m_End));
                particleData.Write8(offset + 0x04, (byte)colorTrans.m_Trans1Start);
                particleData.Write8(offset + 0x05, (byte)colorTrans.m_Trans2Start);
                particleData.Write8(offset + 0x06, (byte)colorTrans.m_Trans2End);
                particleData.Write8(offset + 0x08, interpFlags);

                offset += 0x0c;
            }
            if (hasAlphaTrans)
            {
                Particle.AlphaTransition alphaTrans = sysDef.m_AlphaTrans;

                uint alphas = (uint)(alphaTrans.m_Start & 0x1f) << 0;
                alphas |= (uint)(alphaTrans.m_Middle & 0x1f) << 5;
                alphas |= (uint)(alphaTrans.m_End & 0x1f) << 10;

                particleData.Write16(offset + 0x00, (ushort)alphas);
                particleData.Write8(offset + 0x02, (byte)alphaTrans.m_Flicker);
                particleData.Write8(offset + 0x03, (byte)(alphaTrans.m_UseAltLength ? 1 : 0));
                particleData.Write8(offset + 0x04, (byte)alphaTrans.m_Trans1End);
                particleData.Write8(offset + 0x05, (byte)alphaTrans.m_Trans2Start);

                offset += 0x08;
            }
            if (hasTexSeq)
            {
                Particle.TextureSequence texSeq = sysDef.m_TexSeq;

                uint interpFlags = (uint)(texSeq.m_UseAsOptions ? 1 : 0) << 0;
                interpFlags |= (uint)(texSeq.m_UseAltLength ? 1 : 0) << 1;

                for (uint j = 0; j < 8; ++j)
                {
                    if (texSeq.m_Sprites[j] != null)
                        particleData.Write8(offset + j, (byte)m_Textures.IndexOf(texSeq.m_Sprites[j]));
                    else
                        particleData.Write8(offset + j, 0xff);
                }
                particleData.Write8(offset + 0x08, (byte)texSeq.m_NumSprites);
                particleData.Write8(offset + 0x09, (byte)texSeq.m_Interval);
                particleData.Write8(offset + 0x0a, (byte)interpFlags);

                offset += 0x0c;
            }
            if (hasGlitter)
            {
                Particle.Glitter glitter = sysDef.m_Glitter;

                int gFlags = (glitter.m_HasEffects ? 1 : 0) << 0;
                gFlags |= (glitter.m_HasScaleTrans ? 1 : 0) << 1;
                gFlags |= (glitter.m_HasAlphaTrans ? 1 : 0) << 2;
                gFlags |= ((int)glitter.m_AngleCopyMode & 0x3) << 3;
                gFlags |= (glitter.m_FollowSystem ? 1 : 0) << 5;
                gFlags |= (glitter.m_UseGlitterColor ? 1 : 0) << 6;
                gFlags |= ((int)glitter.m_DrawMode & 0x3) << 7;

                int gTexRepeatFlags = (glitter.m_LogTexRepeatHorz & 0x3) << 0;
                gTexRepeatFlags |= (glitter.m_LogTexRepeatVert & 0x3) << 2;

                particleData.Write16(offset + 0x00, (ushort)gFlags);
                particleData.Write16(offset + 0x02, (ushort)(glitter.m_SpeedRand * 512.0f));
                particleData.Write16(offset + 0x04, (ushort)(glitter.m_Scale * 4096.0f));
                particleData.Write16(offset + 0x06, (ushort)glitter.m_Lifetime);
                particleData.Write8(offset + 0x08, (byte)glitter.m_SpeedMult);
                particleData.Write8(offset + 0x09, (byte)glitter.m_ScaleMult);
                particleData.Write16(offset + 0x0a, Helper.ColorToBGR15(glitter.m_Color));
                particleData.Write8(offset + 0x0c, (byte)glitter.m_Rate);
                particleData.Write8(offset + 0x0d, (byte)glitter.m_Wait);
                particleData.Write8(offset + 0x0e, (byte)glitter.m_Period);
                particleData.Write8(offset + 0x0f, (byte)m_Textures.IndexOf(glitter.m_Sprite));
                particleData.Write8(offset + 0x10, (byte)gTexRepeatFlags);

                offset += 0x14;
            }
            if (hasAcceleration)
            {
                Particle.Acceleration accel = (Particle.Acceleration)sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.Acceleration)];

                particleData.Write16(offset + 0x00, (ushort)(accel.m_Accel.X * 512.0f));
                particleData.Write16(offset + 0x02, (ushort)(accel.m_Accel.Y * 512.0f));
                particleData.Write16(offset + 0x04, (ushort)(accel.m_Accel.Z * 512.0f));

                offset += 0x08;
            }
            if (hasJitter)
            {
                Particle.Jitter jitter = (Particle.Jitter)sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.Jitter)];

                particleData.Write16(offset + 0x00, (ushort)(jitter.m_Mag.X * 512.0f));
                particleData.Write16(offset + 0x02, (ushort)(jitter.m_Mag.Y * 512.0f));
                particleData.Write16(offset + 0x04, (ushort)(jitter.m_Mag.Z * 512.0f));
                particleData.Write16(offset + 0x06, (ushort)jitter.m_Period);

                offset += 0x08;
            }
            if (hasConverge)
            {
                Particle.Converge converge = (Particle.Converge)sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.Converge)];

                particleData.Write32(offset + 0x00, (uint)(converge.m_Offset.X * 512.0f));
                particleData.Write32(offset + 0x04, (uint)(converge.m_Offset.Y * 512.0f));
                particleData.Write32(offset + 0x08, (uint)(converge.m_Offset.Z * 512.0f));
                particleData.Write32(offset + 0x0c, (uint)(converge.m_Mag * 4096.0f));

                offset += 0x10;
            }
            if (hasTurn)
            {
                Particle.Turn turn = (Particle.Turn)sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.Turn)];

                particleData.Write16(offset + 0x00, (ushort)turn.m_AngleSpeed);
                particleData.Write16(offset + 0x02, (ushort)turn.m_Axis);

                offset += 0x04;
            }
            if (hasLimitPlane)
            {
                Particle.LimitPlane limit = (Particle.LimitPlane)sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.LimitPlane)];

                particleData.Write32(offset + 0x00, (uint)(limit.m_PosY * 512.0f));
                particleData.Write16(offset + 0x04, (ushort)(limit.m_ReverseSpeedMult * 4096.0f));
                particleData.Write8(offset + 0x06, (byte)limit.m_Behavior);

                offset += 0x08;
            }
            if (hasRadiusConverge)
            {
                Particle.RadiusConverge converge = (Particle.RadiusConverge)sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.RadiusConverge)];

                particleData.Write32(offset + 0x00, (uint)(converge.m_Offset.X * 512.0f));
                particleData.Write32(offset + 0x04, (uint)(converge.m_Offset.Y * 512.0f));
                particleData.Write32(offset + 0x08, (uint)(converge.m_Offset.Z * 512.0f));
                particleData.Write16(offset + 0x0c, (ushort)(converge.m_Mag * 4096.0f));

                offset += 0x10;
            }

            while (particleData.m_Data.Length % 4 != 0)
            {
                particleData.Write8((uint)particleData.m_Data.Length, 0x0);
            }

            System.IO.File.WriteAllBytes(filePath, particleData.m_Data);
        }

        private void SaveParticleAsCPP(int particleId, string filePath)
        {
            int t = 0;
            List<string> codeLines = new List<string>();

            Particle.System.Def sysDef = m_SysDefs[particleId];
            Particle.MainInfo info = sysDef.m_MainInfo;

            bool hasScaleTrans = sysDef.m_ScaleTrans != null;
            bool hasColorTrans = sysDef.m_ColorTrans != null;
            bool hasAlphaTrans = sysDef.m_AlphaTrans != null;
            bool hasTexSeq = sysDef.m_TexSeq != null;
            bool hasGlitter = sysDef.m_Glitter != null;
            bool hasAcceleration = sysDef.m_Effects.FindIndex(x => x is Particle.Acceleration) != -1;
            bool hasJitter = sysDef.m_Effects.FindIndex(x => x is Particle.Jitter) != -1;
            bool hasConverge = sysDef.m_Effects.FindIndex(x => x is Particle.Converge) != -1;
            bool hasTurn = sysDef.m_Effects.FindIndex(x => x is Particle.Turn) != -1;
            bool hasLimitPlane = sysDef.m_Effects.FindIndex(x => x is Particle.LimitPlane) != -1;
            bool hasRadiusConverge = sysDef.m_Effects.FindIndex(x => x is Particle.RadiusConverge) != -1;

            uint currentSysDefFlag = (uint)info.m_SpawnShape;
            currentSysDefFlag |= (uint)info.m_DrawMode << 4;
            currentSysDefFlag |= (uint)info.m_Plane << 6;
            currentSysDefFlag |= Convert.ToUInt32(info.m_Rotate) << 12;
            currentSysDefFlag |= Convert.ToUInt32(info.m_RandomInitAng) << 13;
            currentSysDefFlag |= Convert.ToUInt32(info.m_SelfDestruct) << 14;
            currentSysDefFlag |= Convert.ToUInt32(info.m_FollowSystem) << 15;
            currentSysDefFlag |= Convert.ToUInt32(info.m_WeirdAxis) << 17;
            currentSysDefFlag |= Convert.ToUInt32(info.m_HorzIf3D) << 19;
            currentSysDefFlag |= (uint)(!hasScaleTrans ? 0 : 1) << 8;
            currentSysDefFlag |= (uint)(!hasColorTrans ? 0 : 1) << 9;
            currentSysDefFlag |= (uint)(!hasAlphaTrans ? 0 : 1) << 10;
            currentSysDefFlag |= (uint)(!hasTexSeq ? 0 : 1) << 11;
            currentSysDefFlag |= (uint)(!hasGlitter ? 0 : 1) << 16;
            currentSysDefFlag |= (uint)(!hasAcceleration ? 0 : 1) << 24;
            currentSysDefFlag |= (uint)(!hasJitter ? 0 : 1) << 25;
            currentSysDefFlag |= (uint)(!hasConverge ? 0 : 1) << 26;
            currentSysDefFlag |= (uint)(!hasTurn ? 0 : 1) << 27;
            currentSysDefFlag |= (uint)(!hasLimitPlane ? 0 : 1) << 28;
            currentSysDefFlag |= (uint)(!hasRadiusConverge ? 0 : 1) << 29;

            int texRepeatFlags = (info.m_LogTexRepeatHorz & 0x3) << 0;
            texRepeatFlags |= (info.m_LogTexRepeatVert & 0x3) << 2;

            int numEffects = sysDef.m_Effects.Count();

            codeLines.Capacity = 28;
            codeLines.Add("Particle::MainInfo particleInfo");
            codeLines.Add("{");
            codeLines.Add($"\t(Particle::MainInfo::Flags){Helper.uintToString(currentSysDefFlag)}, // flags");
            codeLines.Add($"\t{Helper.uintToString((uint)(info.m_Rate * 4096.0f))}_f, // rate");
            codeLines.Add($"\t{Helper.uintToString((uint)(info.m_StartHorzDist * 512.0f))}_f, // startHorzDist");
            codeLines.Add($"\tVector3_16f{{{Helper.shortToString((short)(info.m_Dir.X * 4096.0f))}_fs, {Helper.shortToString((short)(info.m_Dir.Y * 4096.0f))}_fs, {Helper.shortToString((short)(info.m_Dir.Z * 4096.0f))}_fs}}, // dir");
            codeLines.Add($"\tColor5Bit({Helper.byteToString(info.m_Color.R)}, {Helper.byteToString(info.m_Color.G)}, {Helper.byteToString(info.m_Color.B)}), // color");
            codeLines.Add($"\t{Helper.uintToString((uint)(info.m_HorzSpeed * 512.0f))}_f, // horzSpeed");
            codeLines.Add($"\t{Helper.uintToString((uint)(info.m_VertSpeed * 512.0f))}_f, // vertSpeed");
            codeLines.Add($"\t{Helper.uintToString((uint)(info.m_Scale * 512.0f))}_f, // scale");
            codeLines.Add($"\t{Helper.shortToString((short)(info.m_HorzScaleMult * 4096.0f))}_fs, // horzScale");
            codeLines.Add($"\t0x0000,");
            codeLines.Add($"\t{Helper.shortToString(info.m_MinAngleSpeed)}, // minAngSpeed");
            codeLines.Add($"\t{Helper.shortToString(info.m_MaxAngleSpeed)}, // maxAngSpeed");
            codeLines.Add($"\t{Helper.ushortToString((ushort)info.m_Frames)}, // frames");
            codeLines.Add($"\t{Helper.ushortToString((ushort)info.m_Lifetime)}, // lifetime");
            codeLines.Add($"\t{Helper.byteToString((byte)info.m_ScaleRand)}, // scaleRand");
            codeLines.Add($"\t{Helper.byteToString((byte)info.m_LifetimeRand)}, // lifetimeRand");
            codeLines.Add($"\t{Helper.byteToString((byte)info.m_SpeedRand)}, // speedRand");
            codeLines.Add($"\t0x00,");
            codeLines.Add($"\t{Helper.byteToString((byte)info.m_SpawnPeriod)}, // spawnPeriod");
            codeLines.Add($"\t{Helper.byteToString((byte)info.m_Alpha)}, // alpha");
            codeLines.Add($"\t{Helper.byteToString((byte)info.m_SpeedFalloff)}, // speedFalloff");
            codeLines.Add($"\t{Helper.byteToString((byte)m_Textures.IndexOf(info.m_Sprite))}, // spriteID");
            codeLines.Add($"\t{Helper.byteToString((byte)info.m_AltLength)}, // altLength");
            codeLines.Add($"\t{Helper.ushortToString((ushort)(info.m_VelStretchFactor * 4096.0f))}_fs, // velStretchFactor"); // is thi the right format?
            codeLines.Add($"\t{Helper.byteToString((byte)texRepeatFlags)} // texRepeatFlags");
            codeLines.Add("};");

            if (hasScaleTrans)
            {
                Particle.ScaleTransition scaleTrans = sysDef.m_ScaleTrans;

                codeLines.Capacity += 11;
                codeLines.Add("");
                codeLines.Add("Particle::ScaleTransition particleScaleTrans");
                codeLines.Add("{");
                codeLines.Add($"\t{Helper.shortToString((short)(scaleTrans.m_Start * 4096.0f))}_fs, // scaleStart");
                codeLines.Add($"\t{Helper.shortToString((short)(scaleTrans.m_Middle * 4096.0f))}_fs, // scaleMiddle");
                codeLines.Add($"\t{Helper.shortToString((short)(scaleTrans.m_End * 4096.0f))}_fs, // scaleEnd");
                codeLines.Add($"\t{Helper.byteToString((byte)scaleTrans.m_Trans1End)}, // trans1End");
                codeLines.Add($"\t{Helper.byteToString((byte)scaleTrans.m_Trans2Start)}, // trans2Start");
                codeLines.Add($"\t{Helper.ushortToString((ushort)(scaleTrans.m_UseAltLength ? 1 : 0))}, // useAltLength");
                codeLines.Add($"\t0x0000");
                codeLines.Add("};");
            }
            if (hasColorTrans)
            {
                Particle.ColorTransition colorTrans = sysDef.m_ColorTrans;

                byte interpFlags = (byte)((colorTrans.m_UseAsOptions ? 1 : 0) << 0);
                interpFlags |= (byte)((colorTrans.m_UseAltLength ? 1 : 0) << 1);
                interpFlags |= (byte)((colorTrans.m_SmoothTrans ? 1 : 0) << 2);

                codeLines.Capacity += 11;
                codeLines.Add("");
                codeLines.Add("Particle::ColorTransition particleColorTrans");
                codeLines.Add("{");
                codeLines.Add($"\tColor5Bit({Helper.byteToString(colorTrans.m_Start.R)}, {Helper.byteToString(colorTrans.m_Start.G)}, {Helper.byteToString(colorTrans.m_Start.B)}), // colorStart");
                codeLines.Add($"\tColor5Bit({Helper.byteToString(colorTrans.m_End.R)}, {Helper.byteToString(colorTrans.m_End.G)}, {Helper.byteToString(colorTrans.m_End.B)}), // colorEnd");
                codeLines.Add($"\t{Helper.byteToString((byte)colorTrans.m_Trans1Start)}, // trans1Start");
                codeLines.Add($"\t{Helper.byteToString((byte)colorTrans.m_Trans2Start)}, // trans2Start");
                codeLines.Add($"\t{Helper.byteToString((byte)colorTrans.m_Trans2End)}, // trans2End");
                codeLines.Add($"\t0x00,");
                codeLines.Add($"\t{Helper.byteToString(interpFlags)}, // interpFlags");
                codeLines.Add($"\t0x00,");
                codeLines.Add($"\t0x0000");
                codeLines.Add("};");
            }
            if (hasAlphaTrans)
            {
                Particle.AlphaTransition alphaTrans = sysDef.m_AlphaTrans;

                uint alphas = (uint)(alphaTrans.m_Start & 0x1f) << 0;
                alphas |= (uint)(alphaTrans.m_Middle & 0x1f) << 5;
                alphas |= (uint)(alphaTrans.m_End & 0x1f) << 10;

                codeLines.Capacity += 10;
                codeLines.Add("");
                codeLines.Add("Particle::AlphaTransition particleAlphaTrans");
                codeLines.Add("{");
                codeLines.Add($"\t{Helper.ushortToString((ushort)alphas)}, // alpha");
                codeLines.Add($"\t{Helper.byteToString((byte)alphaTrans.m_Flicker)}, // flicker");
                codeLines.Add($"\t{Helper.byteToString((byte)(alphaTrans.m_UseAltLength ? 1 : 0))}, // useAltLength");
                codeLines.Add($"\t{Helper.byteToString((byte)alphaTrans.m_Trans1End)}, // trans1End");
                codeLines.Add($"\t{Helper.byteToString((byte)alphaTrans.m_Trans2Start)}, // trans2Start");
                codeLines.Add($"\t0x00");
                codeLines.Add("};");
            }
            if (hasTexSeq)
            {
                Particle.TextureSequence texSeq = sysDef.m_TexSeq;

                uint interpFlags = (uint)(texSeq.m_UseAsOptions ? 1 : 0) << 0;
                interpFlags |= (uint)(texSeq.m_UseAltLength ? 1 : 0) << 1;

                codeLines.Capacity += 16;
                codeLines.Add("");
                codeLines.Add("Particle::TexSeq particleTexSeq");
                codeLines.Add("{");
                for (uint j = 0; j < 8; ++j)
                {
                    if (texSeq.m_Sprites[j] != null)
                        codeLines.Add($"\t{Helper.byteToString((byte)m_Textures.IndexOf(texSeq.m_Sprites[j]))}, // spriteID {j + 1}");
                    else
                        codeLines.Add($"\t0xff, // spriteID {j + 1}");
                }
                codeLines.Add($"\t{Helper.byteToString((byte)texSeq.m_NumSprites)}, // numSprites");
                codeLines.Add($"\t{Helper.byteToString((byte)texSeq.m_Interval)}, // interval");
                codeLines.Add($"\t{Helper.byteToString((byte)interpFlags)}, // interpFlags");
                codeLines.Add($"\t0x00");
                codeLines.Add("};");
            }
            if (hasGlitter)
            {
                Particle.Glitter glitter = sysDef.m_Glitter;

                int gFlags = (glitter.m_HasEffects ? 1 : 0) << 0;
                gFlags |= (glitter.m_HasScaleTrans ? 1 : 0) << 1;
                gFlags |= (glitter.m_HasAlphaTrans ? 1 : 0) << 2;
                gFlags |= ((int)glitter.m_AngleCopyMode & 0x3) << 3;
                gFlags |= (glitter.m_FollowSystem ? 1 : 0) << 5;
                gFlags |= (glitter.m_UseGlitterColor ? 1 : 0) << 6;
                gFlags |= ((int)glitter.m_DrawMode & 0x3) << 7;

                int gTexRepeatFlags = (glitter.m_LogTexRepeatHorz & 0x3) << 0;
                gTexRepeatFlags |= (glitter.m_LogTexRepeatVert & 0x3) << 2;

                codeLines.Capacity += 16;
                codeLines.Add("");
                codeLines.Add("Particle::Glitter particleGlitter");
                codeLines.Add("{");
                codeLines.Add($"\t{Helper.ushortToString((ushort)gFlags)}, // flags");
                codeLines.Add($"\t{Helper.ushortToString((ushort)(glitter.m_SpeedRand * 512.0f))}, // speedRand"); // is this the right format?
                codeLines.Add($"\t{Helper.shortToString((short)(glitter.m_Scale * 4096.0f))}_fs, // scale");
                codeLines.Add($"\t{Helper.ushortToString((ushort)glitter.m_Lifetime)}, // lifetime");
                codeLines.Add($"\t{Helper.byteToString((byte)glitter.m_SpeedMult)}, // speedMult");
                codeLines.Add($"\t{Helper.byteToString((byte)glitter.m_ScaleMult)}, // scaleMult");
                codeLines.Add($"\tColor5Bit({Helper.byteToString(glitter.m_Color.R)}, {Helper.byteToString(glitter.m_Color.G)}, {Helper.byteToString(glitter.m_Color.B)}), // color");
                codeLines.Add($"\t{Helper.byteToString((byte)glitter.m_Rate)}, // rate");
                codeLines.Add($"\t{Helper.byteToString((byte)glitter.m_Wait)}, // wait");
                codeLines.Add($"\t{Helper.byteToString((byte)glitter.m_Period)}, // period");
                codeLines.Add($"\t{Helper.byteToString((byte)m_Textures.IndexOf(glitter.m_Sprite))}, // spriteID");
                codeLines.Add($"\t{Helper.uintToString((byte)gTexRepeatFlags)} // texRepeatFlags");
                codeLines.Add("};");
            }
            if (numEffects != 0)
            {
                if (hasAcceleration)
                {
                    Particle.Acceleration accel = (Particle.Acceleration)sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.Acceleration)];

                    codeLines.Capacity += 5;
                    codeLines.Add("");
                    codeLines.Add("Particle::Acceleration particleAccelerationEffect");
                    codeLines.Add("{");
                    codeLines.Add($"\tVector3_16f{{{Helper.shortToString((short)(accel.m_Accel.X * 512.0f))}_fs, {Helper.shortToString((short)(accel.m_Accel.Y * 512.0f))}_fs, {Helper.shortToString((short)(accel.m_Accel.Z * 512.0f))}_fs}} // acceleration");
                    codeLines.Add("};");
                }
                if (hasJitter)
                {
                    Particle.Jitter jitter = (Particle.Jitter)sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.Jitter)];

                    codeLines.Capacity += 6;
                    codeLines.Add("");
                    codeLines.Add("Particle::Jitter particleJitterEffect");
                    codeLines.Add("{");
                    codeLines.Add($"\tVector3_16f{{{Helper.shortToString((short)(jitter.m_Mag.X * 512.0f))}_fs, {Helper.shortToString((short)(jitter.m_Mag.Y * 512.0f))}_fs, {Helper.shortToString((short)(jitter.m_Mag.Z * 512.0f))}_fs}}, // magnitude");
                    codeLines.Add($"\t{Helper.ushortToString((ushort)jitter.m_Period)} // period");
                    codeLines.Add("};");
                }
                if (hasConverge)
                {
                    Particle.Converge converge = (Particle.Converge)sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.Converge)];

                    codeLines.Capacity += 6;
                    codeLines.Add("");
                    codeLines.Add("Particle::Converge particleConvergeEffect");
                    codeLines.Add("{");
                    codeLines.Add($"\tVector3{{{Helper.intToString((int)(converge.m_Offset.X * 512.0f))}_f, {Helper.intToString((int)(converge.m_Offset.Y * 512.0f))}_f, {Helper.intToString((int)(converge.m_Offset.Z * 512.0f))}_f}}, // offset");
                    codeLines.Add($"\t{Helper.shortToString((short)(converge.m_Mag * 4096.0f))}_fs // magnitude");
                    codeLines.Add("};");
                }
                if (hasTurn)
                {
                    Particle.Turn turn = (Particle.Turn)sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.Turn)];

                    codeLines.Capacity += 6;
                    codeLines.Add("");
                    codeLines.Add("Particle::Turn particleTurnEffect");
                    codeLines.Add("{");
                    codeLines.Add($"\t{Helper.shortToString(turn.m_AngleSpeed)}, // angleSpeed");
                    codeLines.Add($"\t{Helper.ushortToString((ushort)turn.m_Axis)} // axis"); // todo: make this use an enum
                    codeLines.Add("};");
                }
                if (hasLimitPlane)
                {
                    Particle.LimitPlane limit = (Particle.LimitPlane)sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.LimitPlane)];

                    codeLines.Capacity += 7;
                    codeLines.Add("");
                    codeLines.Add("Particle::LimitPlane particleLimitPlaneEffect");
                    codeLines.Add("{");
                    codeLines.Add($"\t{Helper.intToString((int)(limit.m_PosY * 512.0f))}_f, // posY");
                    codeLines.Add($"\t{Helper.shortToString((short)(limit.m_ReverseSpeedMult * 4096.0f))}_fs, // reverseSpeedMult");
                    codeLines.Add($"\t{Helper.byteToString((byte)limit.m_Behavior)} // behavior"); // todo: make this use an enum
                    codeLines.Add("};");
                }
                if (hasRadiusConverge)
                {
                    Particle.RadiusConverge converge = (Particle.RadiusConverge)sysDef.m_Effects[sysDef.m_Effects.FindIndex(x => x is Particle.RadiusConverge)];

                    codeLines.Capacity += 6;
                    codeLines.Add("");
                    codeLines.Add("Particle::RadiusConverge particleRadiusConvergeEffect");
                    codeLines.Add("{");
                    codeLines.Add($"\tVector3{{{Helper.intToString((int)(converge.m_Offset.X * 512.0f))}_f, {Helper.intToString((int)(converge.m_Offset.Y * 512.0f))}_f, {Helper.intToString((int)(converge.m_Offset.Z * 512.0f))}_f}}, // offset");
                    codeLines.Add($"\t{Helper.shortToString((short)(converge.m_Mag * 4096.0f))}_fs // magnitude");
                    codeLines.Add("};");
                }

                codeLines.Capacity += 4;
                codeLines.Add("");
                codeLines.Add("Particle::Effect particleEffects[]");
                codeLines.Add("{");
                if (hasAcceleration)
                {
                    codeLines.Capacity += 5;
                    codeLines.Add("\tParticle::Effect");
                    codeLines.Add("\t{");
                    codeLines.Add("\t\t&Particle::Acceleration::Func,");
                    codeLines.Add("\t\t&particleAccelerationEffect");
                    codeLines.Add("\t}");
                    if (hasJitter || hasConverge || hasTurn || hasLimitPlane || hasRadiusConverge)
                        codeLines[codeLines.Count - 1] += ",";
                }
                if (hasJitter)
                {
                    codeLines.Capacity += 5;
                    codeLines.Add("\tParticle::Effect");
                    codeLines.Add("\t{");
                    codeLines.Add("\t\t&Particle::Jitter::Func,");
                    codeLines.Add("\t\t&particleJitterEffect");
                    codeLines.Add("\t}");
                    if (hasConverge || hasTurn || hasLimitPlane || hasRadiusConverge)
                        codeLines[codeLines.Count - 1] += ",";
                }
                if (hasConverge)
                {
                    codeLines.Capacity += 5;
                    codeLines.Add("\tParticle::Effect");
                    codeLines.Add("\t{");
                    codeLines.Add("\t\t&Particle::Converge::Func,");
                    codeLines.Add("\t\t&particleConvergeEffect");
                    codeLines.Add("\t}");
                    if (hasTurn || hasLimitPlane || hasRadiusConverge)
                        codeLines[codeLines.Count - 1] += ",";
                }
                if (hasTurn)
                {
                    codeLines.Capacity += 5;
                    codeLines.Add("\tParticle::Effect");
                    codeLines.Add("\t{");
                    codeLines.Add("\t\t&Particle::Turn::Func,");
                    codeLines.Add("\t\t&particleTurnEffect");
                    codeLines.Add("\t}");
                    if (hasLimitPlane || hasRadiusConverge)
                        codeLines[codeLines.Count - 1] += ",";
                }
                if (hasLimitPlane)
                {
                    codeLines.Capacity += 5;
                    codeLines.Add("\tParticle::Effect");
                    codeLines.Add("\t{");
                    codeLines.Add("\t\t&Particle::LimitPlane::Func,");
                    codeLines.Add("\t\t&particleLimitPlaneEffect");
                    codeLines.Add("\t}");
                    if (hasRadiusConverge)
                        codeLines[codeLines.Count - 1] += ",";
                }
                if (hasRadiusConverge)
                {
                    codeLines.Capacity += 5;
                    codeLines.Add("\tParticle::Effect");
                    codeLines.Add("\t{");
                    codeLines.Add("\t\t&Particle::RadiusConverge::Func,");
                    codeLines.Add("\t\t&particleRadiusConvergeEffect");
                    codeLines.Add("\t}");
                }
                codeLines.Add("};");
            }

            codeLines.Capacity += 12;
            codeLines.Add("");
            codeLines.Add("Particle::SysDef particleSysDef");
            codeLines.Add("{");
            codeLines.Add("\t&particleInfo,");
            codeLines.Add(sysDef.m_ScaleTrans != null ? "\t&particleScaleTrans," : "\tnullptr,");
            codeLines.Add(sysDef.m_ColorTrans != null ? "\t&particleColorTrans," : "\tnullptr,");
            codeLines.Add(sysDef.m_AlphaTrans != null ? "\t&particleAlphaTrans," : "\tnullptr,");
            codeLines.Add(sysDef.m_TexSeq     != null ? "\t&particleTexSeq,"     : "\tnullptr,");
            codeLines.Add(sysDef.m_Glitter    != null ? "\t&particleGlitter,"     : "\tnullptr,");
            codeLines.Add(numEffects          != 0    ? "\t&particleEffects[0]," : "\tnullptr,");
            codeLines.Add($"\t{numEffects}");
            codeLines.Add("};");

            System.IO.File.WriteAllLines(filePath, codeLines);
        }

        private void ReplaceParticle(int particleId, string filePath)
        {
            byte[] SPD_data = System.IO.File.ReadAllBytes(filePath);
            uint offset = 0x0;

            Particle.System.Def sysDef = new Particle.System.Def();
            Particle.MainInfo info = new Particle.MainInfo();

            uint flags = Helper.Read32(SPD_data, offset);
            int texRepeatFlags = SPD_data[offset + 0x37];
            info.m_SpawnShape = (Particle.MainInfo.SpawnShape)(flags & 0x7);
            info.m_DrawMode = (Particle.MainInfo.DrawMode)(flags >> 4 & 0x3);
            info.m_Plane = (Particle.MainInfo.Plane)(flags >> 6 & 0x3);
            info.m_Rotate = (flags >> 12 & 1) != 0;
            info.m_RandomInitAng = (flags >> 13 & 1) != 0;
            info.m_SelfDestruct = (flags >> 14 & 1) != 0;
            info.m_FollowSystem = (flags >> 15 & 1) != 0;
            info.m_WeirdAxis = (flags >> 17 & 1) != 0;
            info.m_HorzIf3D = (flags >> 19 & 1) != 0;
            info.m_Rate = (int)Helper.Read32(SPD_data, offset + 0x04) / 4096.0f;
            info.m_StartHorzDist = (int)Helper.Read32(SPD_data, offset + 0x08) / 512.0f;
            info.m_Dir = new Vector3(
                (short)Helper.Read16(SPD_data, offset + 0x0c) / 4096.0f,
                (short)Helper.Read16(SPD_data, offset + 0x0e) / 4096.0f,
                (short)Helper.Read16(SPD_data, offset + 0x10) / 4096.0f
                );
            info.m_Color = Helper.BGR15ToColor(Helper.Read16(SPD_data, offset + 0x12));
            info.m_HorzSpeed = (int)Helper.Read32(SPD_data, offset + 0x14) / 512.0f;
            info.m_VertSpeed = (int)Helper.Read32(SPD_data, offset + 0x18) / 512.0f;
            info.m_Scale = (int)Helper.Read32(SPD_data, offset + 0x1c) / 512.0f;
            info.m_HorzScaleMult = (short)Helper.Read16(SPD_data, offset + 0x20) / 4096.0f;
            info.m_MinAngleSpeed = (short)Helper.Read16(SPD_data, offset + 0x24);
            info.m_MaxAngleSpeed = (short)Helper.Read16(SPD_data, offset + 0x26);
            info.m_Frames = Helper.Read16(SPD_data, offset + 0x28);
            info.m_Lifetime = Helper.Read16(SPD_data, offset + 0x2a);
            info.m_ScaleRand = SPD_data[offset + 0x2c];
            info.m_LifetimeRand = SPD_data[offset + 0x2d];
            info.m_SpeedRand = SPD_data[offset + 0x2e];
            info.m_SpawnPeriod = SPD_data[offset + 0x30];
            info.m_Alpha = SPD_data[offset + 0x31];
            info.m_SpeedFalloff = SPD_data[offset + 0x32];
            info.m_Sprite = m_Textures[SPD_data[offset + 0x33]];
            info.m_AltLength = SPD_data[offset + 0x34];
            info.m_VelStretchFactor = (short)Helper.Read16(SPD_data, offset + 0x35) / 4096.0f;
            info.m_LogTexRepeatHorz = texRepeatFlags >> 0 & 0x3;
            info.m_LogTexRepeatVert = texRepeatFlags >> 2 & 0x3;
            sysDef.m_MainInfo = info;
            offset += 0x38;

            if ((flags >> 8 & 1) != 0)
            {
                Console.WriteLine($"ScaleTransition 0x{Convert.ToString(particleId, 16)} started (0x{Convert.ToString(offset, 16)})");
                Particle.ScaleTransition scaleTrans = new Particle.ScaleTransition();
                scaleTrans.m_Start = Helper.Read16(SPD_data, offset + 0x00) / 4096.0f;
                scaleTrans.m_Middle = Helper.Read16(SPD_data, offset + 0x02) / 4096.0f;
                scaleTrans.m_End = Helper.Read16(SPD_data, offset + 0x04) / 4096.0f;
                scaleTrans.m_Trans1End = SPD_data[offset + 0x06];
                scaleTrans.m_Trans2Start = SPD_data[offset + 0x07];
                scaleTrans.m_UseAltLength = (SPD_data[offset + 0x08] & 1) != 0;
                sysDef.m_ScaleTrans = scaleTrans;
                offset += 0x0c;
            }
            if ((flags >> 9 & 1) != 0)
            {
                Console.WriteLine($"ColorTransition 0x{Convert.ToString(particleId, 16)} started (0x{Convert.ToString(offset, 16)})");
                Particle.ColorTransition colorTrans = new Particle.ColorTransition();
                uint interpFlags = SPD_data[offset + 0x08];
                colorTrans.m_Start = Helper.BGR15ToColor(Helper.Read16(SPD_data, offset + 0x00));
                colorTrans.m_End = Helper.BGR15ToColor(Helper.Read16(SPD_data, offset + 0x02));
                colorTrans.m_Trans1Start = SPD_data[offset + 0x04];
                colorTrans.m_Trans2Start = SPD_data[offset + 0x05];
                colorTrans.m_Trans2End = SPD_data[offset + 0x06];
                colorTrans.m_UseAsOptions = (interpFlags >> 0 & 1) != 0;
                colorTrans.m_UseAltLength = (interpFlags >> 1 & 1) != 0;
                colorTrans.m_SmoothTrans = (interpFlags >> 2 & 1) != 0;
                sysDef.m_ColorTrans = colorTrans;
                offset += 0x0c;
            }
            if ((flags >> 10 & 1) != 0)
            {
                Console.WriteLine($"AlphaTransition 0x{Convert.ToString(particleId, 16)} started (0x{Convert.ToString(offset, 16)})");
                Particle.AlphaTransition alphaTrans = new Particle.AlphaTransition();
                uint alphas = Helper.Read16(SPD_data, offset + 0x00);
                alphaTrans.m_Start = (int)(alphas >> 0 & 0x1f);
                alphaTrans.m_Middle = (int)(alphas >> 5 & 0x1f);
                alphaTrans.m_End = (int)(alphas >> 10 & 0x1f);
                alphaTrans.m_Flicker = SPD_data[offset + 0x02];
                alphaTrans.m_UseAltLength = (SPD_data[offset + 0x03] & 1) != 0;
                alphaTrans.m_Trans1End = SPD_data[offset + 0x04];
                alphaTrans.m_Trans2Start = SPD_data[offset + 0x05];
                sysDef.m_AlphaTrans = alphaTrans;
                offset += 0x08;
            }
            if ((flags >> 11 & 1) != 0)
            {
                Console.WriteLine($"TextureSequence 0x{Convert.ToString(particleId, 16)} started (0x{Convert.ToString(offset, 16)})");
                Particle.TextureSequence texSeq = new Particle.TextureSequence();
                texSeq.m_Sprites = new Particle.Texture[8];
                int numSprites = SPD_data[offset + 0x08];
                uint interpFlags = SPD_data[offset + 0x0a];
                for (uint j = 0; j < numSprites; ++j)
                    texSeq.m_Sprites[j] = m_Textures[SPD_data[offset + j]];
                texSeq.m_NumSprites = numSprites;
                texSeq.m_Interval = SPD_data[offset + 0x09];
                texSeq.m_UseAsOptions = (interpFlags >> 0 & 1) != 0;
                texSeq.m_UseAltLength = (interpFlags >> 1 & 1) != 0;
                sysDef.m_TexSeq = texSeq;
                offset += 0x0c;
            }
            if ((flags >> 16 & 1) != 0)
            {
                Console.WriteLine($"Glitter 0x{Convert.ToString(particleId, 16)} started (0x{Convert.ToString(offset, 16)})");
                Particle.Glitter glitter = new Particle.Glitter();
                int gFlags = Helper.Read16(SPD_data, offset + 0x00);
                int gTexRepeatFlags = SPD_data[offset + 0x10];
                glitter.m_HasEffects = (gFlags & 1) != 0;
                glitter.m_HasScaleTrans = (gFlags >> 1 & 1) != 0;
                glitter.m_HasAlphaTrans = (gFlags >> 2 & 1) != 0;
                glitter.m_AngleCopyMode = (Particle.Glitter.AngleCopyMode)(gFlags >> 3 & 0x3);
                glitter.m_FollowSystem = (gFlags >> 5 & 1) != 0;
                glitter.m_UseGlitterColor = (gFlags >> 6 & 1) != 0;
                glitter.m_DrawMode = (Particle.MainInfo.DrawMode)(gFlags >> 7 & 0x3);
                glitter.m_SpeedRand = Helper.Read16(SPD_data, offset + 0x02) / 512.0f;
                glitter.m_Scale = (short)Helper.Read16(SPD_data, offset + 0x04) / 4096.0f; //multiplier
                glitter.m_Lifetime = Helper.Read16(SPD_data, offset + 0x06);
                glitter.m_SpeedMult = SPD_data[offset + 0x08];
                glitter.m_ScaleMult = SPD_data[offset + 0x09];
                glitter.m_Color = Helper.BGR15ToColor(Helper.Read16(SPD_data, offset + 0x0a));
                glitter.m_Rate = SPD_data[offset + 0x0c];
                glitter.m_Wait = SPD_data[offset + 0x0d];
                glitter.m_Period = SPD_data[offset + 0x0e];
                glitter.m_Sprite = m_Textures[SPD_data[offset + 0x0f]];
                glitter.m_LogTexRepeatHorz = gTexRepeatFlags >> 0 & 0x3;
                glitter.m_LogTexRepeatVert = gTexRepeatFlags >> 2 & 0x3;
                sysDef.m_Glitter = glitter;
                offset += 0x14;
            }

            sysDef.m_Effects = new List<Particle.Effect>();
            if ((flags >> 24 & 1) != 0)
            {
                Console.WriteLine($"Acceleration 0x{Convert.ToString(particleId, 16)} started (0x{Convert.ToString(offset, 16)})");
                Particle.Acceleration accel = new Particle.Acceleration();
                accel.m_Accel = new Vector3(
                    (short)Helper.Read16(SPD_data, offset + 0x00) / 512.0f,
                    (short)Helper.Read16(SPD_data, offset + 0x02) / 512.0f,
                    (short)Helper.Read16(SPD_data, offset + 0x04) / 512.0f
                    );
                sysDef.m_Effects.Add(accel);
                offset += 0x08;
            }
            if ((flags >> 25 & 1) != 0)
            {
                Console.WriteLine($"Jitter 0x{Convert.ToString(particleId, 16)} started (0x{Convert.ToString(offset, 16)})");
                Particle.Jitter jitter = new Particle.Jitter();
                jitter.m_Mag = new Vector3(
                    (short)Helper.Read16(SPD_data, offset + 0x00) / 512.0f,
                    (short)Helper.Read16(SPD_data, offset + 0x02) / 512.0f,
                    (short)Helper.Read16(SPD_data, offset + 0x04) / 512.0f
                    );
                jitter.m_Period = Helper.Read16(SPD_data, offset + 0x06);
                sysDef.m_Effects.Add(jitter);
                offset += 0x08;
            }
            if ((flags >> 26 & 1) != 0)
            {
                Console.WriteLine($"Converge 0x{Convert.ToString(particleId, 16)} started (0x{Convert.ToString(offset, 16)})");
                Console.WriteLine($"flags = {Convert.ToString(flags, 16)}");
                Particle.Converge converge = new Particle.Converge();
                converge.m_Offset = new Vector3(
                    (int)Helper.Read32(SPD_data, offset + 0x00) / 512.0f,
                    (int)Helper.Read32(SPD_data, offset + 0x04) / 512.0f,
                    (int)Helper.Read32(SPD_data, offset + 0x08) / 512.0f
                    );
                converge.m_Mag = (short)Helper.Read16(SPD_data, offset + 0x0c) / 4096.0f;
                sysDef.m_Effects.Add(converge);
                offset += 0x10;
            }
            if ((flags >> 27 & 1) != 0)
            {
                Console.WriteLine($"Turn 0x{Convert.ToString(particleId, 16)} started (0x{Convert.ToString(offset, 16)})");
                Particle.Turn turn = new Particle.Turn();
                turn.m_AngleSpeed = (short)Helper.Read16(SPD_data, offset + 0x00);
                turn.m_Axis = (Particle.Turn.Axis)SPD_data[offset + 0x02];
                sysDef.m_Effects.Add(turn);
                offset += 0x04;
            }
            if ((flags >> 28 & 1) != 0) //not used by any of the 0x141 particle system defs
            {
                Console.WriteLine($"LimitPlane 0x{Convert.ToString(particleId, 16)} started (0x{Convert.ToString(offset, 16)})");
                Particle.LimitPlane limit = new Particle.LimitPlane();
                limit.m_PosY = (int)Helper.Read32(SPD_data, offset + 0x00) / 512.0f;
                limit.m_ReverseSpeedMult = (short)Helper.Read16(SPD_data, offset + 0x04) / 4096.0f;
                limit.m_Behavior = (Particle.LimitPlane.Behavior)(SPD_data[offset + 0x06]);
                sysDef.m_Effects.Add(limit);
                offset += 0x08;
            }
            if ((flags >> 29 & 1) != 0)
            {
                Console.WriteLine($"RadiusConverge 0x{Convert.ToString(particleId, 16)} started (0x{Convert.ToString(offset, 16)})");
                Particle.RadiusConverge converge = new Particle.RadiusConverge();
                converge.m_Offset = new Vector3(
                    (int)Helper.Read32(SPD_data, offset + 0x00) / 512.0f,
                    (int)Helper.Read32(SPD_data, offset + 0x04) / 512.0f,
                    (int)Helper.Read32(SPD_data, offset + 0x08) / 512.0f
                    );
                converge.m_Mag = (short)Helper.Read16(SPD_data, offset + 0x0c) / 4096.0f;
                sysDef.m_Effects.Add(converge);
                offset += 0x10;
            }

            m_SysDefs[particleId] = sysDef;
            ParticleSysDefProperties.GenerateProperties(m_SysDefs[particleId], m_Textures);
        }

        private void exportSPDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lbxSysDef.SelectedIndex != -1)
            {
                SaveFileDialog export = new SaveFileDialog();
                export.FileName = $"particle{Convert.ToString(lbxSysDef.SelectedIndex, 16).PadLeft(3, '0')}.spd";
                export.Filter = "Particle Library (*.spd) | *.spd";
                if (export.ShowDialog() == DialogResult.Cancel)
                    return;

                SaveParticleAsSPD(lbxSysDef.SelectedIndex, export.FileName);
            }
            else
            {
                MessageBox.Show("Please select a particle first.");
            }
        }

        private void exportAllSPDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK)
            {
                string folderName = fbd.SelectedPath;
                for (int i = 0; i < m_SysDefs.Count; i++)
                {
                    SaveParticleAsSPD(i, folderName + "/" + $"particle{Convert.ToString(i, 16).PadLeft(3, '0')}.spd");
                }
                MessageBox.Show("Successfully exported " + m_SysDefs.Count + " particle(s) to:\n" + folderName);
            }
        }

        private void exportCppToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lbxSysDef.SelectedIndex != -1)
            {
                SaveFileDialog export = new SaveFileDialog();
                export.FileName = $"particle{Convert.ToString(lbxSysDef.SelectedIndex, 16).PadLeft(3, '0')}.cpp";
                export.Filter = "C++ Code File (*.cpp) | *.cpp";
                if (export.ShowDialog() == DialogResult.Cancel)
                    return;

                SaveParticleAsCPP(lbxSysDef.SelectedIndex, export.FileName);
            }
            else
            {
                MessageBox.Show("Please select a particle first.");
            }
        }

        private void replaceSPDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lbxSysDef.SelectedIndex != -1)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Select a particle";
                DialogResult result = ofd.ShowDialog();
                if (result == DialogResult.Cancel) return;

                try
                {
                    ReplaceParticle(lbxSysDef.SelectedIndex, ofd.FileName);
                    pgSysDefProps.SelectedObject = m_SysDefs[lbxSysDef.SelectedIndex].m_Properties;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + ex.Source + ex.TargetSite + ex.StackTrace);
                }
            }
            else
            {
                MessageBox.Show("Please select a particle first.");
            }
        }

        private void copySelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lbxSysDef.SelectedIndex != -1)
            {
                Particle.System.Def sysDef = m_SysDefs[lbxSysDef.SelectedIndex].Copy();
                ParticleSysDefProperties.GenerateProperties(sysDef, m_Textures);
                m_SysDefs.Add(sysDef);
                lbxSysDef.Items.Add($"particle{Convert.ToString(m_SysDefs.Count - 1, 16).PadLeft(3, '0')}.spd");
            }
            else
            {
                MessageBox.Show("Please select a particle first.");
            }
        }

        private void importSPDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select a particle";
            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.Cancel) return;

            try
            {
                m_SysDefs.Add(new Particle.System.Def());
                ReplaceParticle(m_SysDefs.Count - 1, ofd.FileName); // replace the empty thing we just added
                lbxSysDef.Items.Add($"particle{Convert.ToString(m_SysDefs.Count - 1, 16).PadLeft(3, '0')}.spd");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.Source + ex.TargetSite + ex.StackTrace);
            }
        }

        private void removeSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lbxSysDef.SelectedIndex != -1)
            {
                DialogResult dialogResult = MessageBox.Show($"Are you sure you want to delete particle 0x{Convert.ToString(lbxSysDef.SelectedIndex, 16)}?\nKeep in mind that this changes the index of ALL particles that come after!", "Particle deletion confirmation", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    m_SysDefs.Remove(m_SysDefs[lbxSysDef.SelectedIndex]);
                    lbxSysDef.Items.Clear();

                    // because the indexes have changed
                    for (int i = 0; i < m_SysDefs.Count; i++)
                        lbxSysDef.Items.Add($"particle{Convert.ToString(i, 16).PadLeft(3, '0')}.spd");
                }
            }
            else
            {
                MessageBox.Show("Please select a particle first.");
            }
        }

        private void editTexturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ParticleTextureForm(m_SysDefs, m_Textures, builtInTextures, totalTextures).ShowDialog(this);
        }

        public void TextureFormUpdate(int totalTextures, int builtInTextures)
        {
            this.builtInTextures = builtInTextures;
            this.totalTextures = totalTextures;

            // update the properties because the sprite ids may have changed
            int sysDefID = lbxSysDef.SelectedIndex;
            if (sysDefID != - 1)
                pgSysDefProps.SelectedObject = m_SysDefs[sysDefID].m_Properties;
        }
    }
}
