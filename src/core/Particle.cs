using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using SM64DSe.FormControls;

namespace SM64DSe
{
    //All formulas copied straight from the DS.
    //Please excuse the use of floating point, though.
    public class Particle
    {
        public static uint MAIN_SYS_DEF_ROM_ADDR = 0x75f14;
        //necessary as a change I made to support additional
        //textures changed the structure a bit.
        //This is only one of the changes I made to support additional textures.
        public static uint EXTERNAL_SPT_MARK_ROM_ADDR = 0x4a408;
        public static uint EXTERNAL_SPT_MARK_VALUE = 0xe5da2026;

        private static int GL_XY_LIST;
        private static int GL_XZ_LIST;
        private static Matrix4 VIEW_MATRIX;
        private static Matrix4 SCALE_MATRIX = Matrix4.CreateScale(0.001f);
        private static Vector3 THE_WEIRD_AXIS = new Vector3(-1, -1, -1).Normalized();

        public static uint RNG_STATE = 0x80000000;
        public static int RandomInt()
        {
            RNG_STATE = 0x5eedf715 * RNG_STATE + 0x1b0cb173;
            return (int)RNG_STATE;
        }
        public static uint RandomUInt()
        {
            RNG_STATE = 0x5eedf715 * RNG_STATE + 0x1b0cb173;
            return RNG_STATE;
        }

        public static Vector3 RandomNormalizedVecXY()
        {
            return new Vector3(RandomInt() >> 8, RandomInt() >> 8, 0).Normalized();
        }
        public static Vector3 RandomNormalizedVec()
        {
            return new Vector3(RandomInt() >> 8, RandomInt() >> 8, RandomInt() >> 8).Normalized();
        }

        public static float Rad(short angle)
        {
            return angle * Helper.Tau / 65536;
        }

        public Vector3 m_Pos;
        public Vector3 m_Offset;
        public Vector3 m_Vel;
        public int m_Lifetime;
        public int m_Age;
        public float m_Scale;
        public float m_ScaleMult;
        public short m_Angle;
        public short m_AngleSpeed;
        public Color m_Color;
        public int m_AltLenInv;
        public int m_LifetimeInv;
        public int m_Alpha;
        public int m_AlphaMult; //out of 31
        public Texture m_Sprite;

        public void Init(System system, int index, int total)
        {
            MainInfo info = system.m_Def.m_MainInfo;
            switch (info.m_SpawnShape)
            {
                case MainInfo.SpawnShape.POINT:
                    m_Offset = Vector3.Zero;
                    break;

                case MainInfo.SpawnShape.SPHERE:
                    m_Offset = RandomNormalizedVec() * system.m_StartHorzDist;
                    break;

                case MainInfo.SpawnShape.CIRCLE_RANDOM:
                    SetOffsetOnPlane(system, RandomNormalizedVecXY() * system.m_StartHorzDist);
                    break;

                case MainInfo.SpawnShape.CIRCLE_EVEN:
                    short offsetAngle = (short)(0x10000 * index / total);
                    SetOffsetOnPlane(system,
                        new Vector3((float)Math.Sin(Rad(offsetAngle)), (float)Math.Cos(Rad(offsetAngle)), 0) *
                        system.m_StartHorzDist);
                    break;

                case MainInfo.SpawnShape.BALL:
                    m_Offset = RandomNormalizedVec();
                    m_Offset.X *= system.m_StartHorzDist * (int)((RandomUInt() >> 23 << 12) - 0x100000) / 0x100000;
                    m_Offset.Y *= system.m_StartHorzDist * (int)((RandomUInt() >> 23 << 12) - 0x100000) / 0x100000;
                    m_Offset.Z *= system.m_StartHorzDist * (int)((RandomUInt() >> 23 << 12) - 0x100000) / 0x100000;
                    break;

                case MainInfo.SpawnShape.DISC:
                    Vector3 vec = RandomNormalizedVecXY();
                    vec.X *= system.m_StartHorzDist * (int)((RandomUInt() >> 23 << 12) - 0x100000) / 0x100000;
                    vec.Y *= system.m_StartHorzDist * (int)((RandomUInt() >> 23 << 12) - 0x100000) / 0x100000;
                    SetOffsetOnPlane(system, vec);
                    break;
            }

            float randHorzSpeed = system.m_HorzSpeed *
                (info.m_SpeedRand + 0xff - (int)(info.m_SpeedRand * (RandomUInt() >> 24) >> 7)) / 0x100;
            float randVertSpeed = system.m_VertSpeed *
                (info.m_SpeedRand + 0xff - (int)(info.m_SpeedRand * (RandomUInt() >> 24) >> 7)) / 0x100;

            Vector3 offsetDir;
            if ((int)(m_Offset.X * 0x1000) == 0 &&
                (int)(m_Offset.Y * 0x1000) == 0 &&
                (int)(m_Offset.Z * 0x1000) == 0)
                offsetDir = RandomNormalizedVec();
            else
                offsetDir = m_Offset.Normalized();

            m_Vel = system.m_Dir * randVertSpeed + offsetDir * randHorzSpeed;
            m_Pos = system.m_Pos;

            m_Scale = system.m_Scale *
                (info.m_ScaleRand + 0xff - (int)(info.m_ScaleRand * (RandomUInt() >> 24) >> 7)) / 0x100;
            m_ScaleMult = 1.0f;

            ColorTransition colorTrans = system.m_Def.m_ColorTrans;
            if (colorTrans != null && colorTrans.m_UseAsOptions)
            {
                Color[] colors = { colorTrans.m_Start, info.m_Color, colorTrans.m_End };
                m_Color = colors[RandomUInt() / 0x100000 % 3];
            }
            else
                m_Color = info.m_Color;
            m_Alpha = system.m_Alpha;
            m_AlphaMult = 0x1f;

            m_Angle = info.m_RandomInitAng ? (short)RandomInt() : (short)0;
            if (info.m_Rotate)
                m_AngleSpeed = (short)(((info.m_MaxAngleSpeed - info.m_MinAngleSpeed) *
                    (RandomUInt() >> 20) + (info.m_MinAngleSpeed << 12)) >> 12);
            else
                m_AngleSpeed = 0;

            m_Lifetime = (system.m_ParticleLifetime *
                (0xff - ((int)(info.m_LifetimeRand * (RandomUInt() >> 24)) >> 8)) >> 8) + 1;
            m_Age = 0;

            TextureSequence texSeq = system.m_Def.m_TexSeq;
            if (texSeq != null && texSeq.m_UseAsOptions)
                m_Sprite = texSeq.m_Sprites[RandomUInt() / 0x100000 % texSeq.m_NumSprites];
            else if (texSeq != null)
                m_Sprite = texSeq.m_Sprites[0];
            else
                m_Sprite = info.m_Sprite;

            m_AltLenInv = 0x10000 / (info.m_AltLength != 0 ? info.m_AltLength : 1);
            m_LifetimeInv = 0x10000 / (m_Lifetime != 0 ? m_Lifetime : 1);
        }

        public void SpawnGlitter(System system, Stack<Particle> freeParticles)
        {
            Glitter glitter = system.m_Def.m_Glitter;
            for (int i = 0; i < glitter.m_Rate; ++i)
            {
                if (freeParticles.Count == 0)
                    return;
                Particle gParticle = freeParticles.Pop();
                system.m_GlitterParticles.Add(gParticle);

                gParticle.m_Offset = m_Offset;
                gParticle.m_Vel.X = (m_Vel.X * glitter.m_SpeedMult / 0x100) +
                    glitter.m_SpeedRand * (int)(RandomUInt() / 0x800000 - 0x100) / 0x100;
                gParticle.m_Vel.Y = (m_Vel.Y * glitter.m_SpeedMult / 0x100) +
                    glitter.m_SpeedRand * (int)(RandomUInt() / 0x800000 - 0x100) / 0x100;
                gParticle.m_Vel.Z = (m_Vel.Z * glitter.m_SpeedMult / 0x100) +
                    glitter.m_SpeedRand * (int)(RandomUInt() / 0x800000 - 0x100) / 0x100;

                gParticle.m_Pos = m_Pos;
                gParticle.m_Scale = m_Scale * m_ScaleMult * (glitter.m_ScaleMult + 1) / 0x40;
                gParticle.m_ScaleMult = 1.0f;
                gParticle.m_Color = glitter.m_UseGlitterColor ? glitter.m_Color : m_Color;
                gParticle.m_Alpha = m_Alpha * (m_AlphaMult + 1) / 0x20;
                gParticle.m_AlphaMult = 0x1f;

                switch (glitter.m_AngleCopyMode)
                {
                    case Glitter.AngleCopyMode.NONE:
                        gParticle.m_Angle = 0;
                        gParticle.m_AngleSpeed = 0;
                        break;

                    case Glitter.AngleCopyMode.ANGLE:
                        gParticle.m_Angle = m_Angle;
                        gParticle.m_AngleSpeed = 0;
                        break;

                    case Glitter.AngleCopyMode.ANGLE_AND_SPEED:
                        gParticle.m_Angle = m_Angle;
                        gParticle.m_AngleSpeed = m_AngleSpeed;
                        break;
                }

                gParticle.m_Lifetime = glitter.m_Lifetime;
                gParticle.m_Age = 0;
                gParticle.m_Sprite = glitter.m_Sprite;
                gParticle.m_AltLenInv = 0x10000 / ((gParticle.m_Lifetime / 2) != 0 ? (gParticle.m_Lifetime / 2) : 1);
                gParticle.m_LifetimeInv = 0x10000 / (gParticle.m_Lifetime != 0 ? gParticle.m_Lifetime : 1);
            }
        }

        public void SetOffsetOnPlane(System system, Vector3 offset2D)
        {
            switch (system.m_Def.m_MainInfo.m_Plane)
            {
                case MainInfo.Plane.Z:
                    m_Offset = new Vector3(offset2D.X, offset2D.Y, 0);
                    break;

                case MainInfo.Plane.Y:
                    m_Offset = new Vector3(offset2D.Y, 0, offset2D.X);
                    break;

                case MainInfo.Plane.X:
                    m_Offset = new Vector3(0, offset2D.X, offset2D.Y);
                    break;

                case MainInfo.Plane.DIR:
                    m_Offset = system.m_Tangent * offset2D.X + system.m_Tangent2 * offset2D.Y;
                    break;
            }
        }

        public static void ProcessScaleTrans(Particle p, System.Def sysDef, int lifetimeFrac)
        {
            ScaleTransition scaleTrans = sysDef.m_ScaleTrans;
            if (lifetimeFrac >= scaleTrans.m_Trans1End)
            {
                if (lifetimeFrac >= scaleTrans.m_Trans2Start)
                {
                    p.m_ScaleMult = scaleTrans.m_End + (lifetimeFrac - 0xff) *
                        (scaleTrans.m_End - scaleTrans.m_Middle) /
                        ((0xff - scaleTrans.m_Trans2Start) != 0 ? (0xff - scaleTrans.m_Trans2Start) : 1);
                }
                else
                    p.m_ScaleMult = scaleTrans.m_Middle;
            }
            else
                p.m_ScaleMult = scaleTrans.m_Start + lifetimeFrac *
                    (scaleTrans.m_Middle - scaleTrans.m_Start) /
                    (scaleTrans.m_Trans1End != 0 ? scaleTrans.m_Trans1End : 1);
        }

        public static void ProcessGlitterScaleTrans(Particle p, System.Def sysDef, int lifetimeFrac)
        {
            Glitter glitter = sysDef.m_Glitter;
            p.m_ScaleMult = glitter.m_Scale + (glitter.m_Scale - 1) * (lifetimeFrac - 0xff) / 0xff;
        }

        public static void ProcessColorTrans(Particle p, System.Def sysDef, int lifetimeFrac)
        {
            MainInfo info = sysDef.m_MainInfo;
            ColorTransition colorTrans = sysDef.m_ColorTrans;

            Color start = colorTrans.m_Start;
            Color middle = info.m_Color;
            Color end = colorTrans.m_End;

            if (lifetimeFrac >= colorTrans.m_Trans1Start)
            {
                if (lifetimeFrac >= colorTrans.m_Trans2Start)
                {
                    if (lifetimeFrac >= colorTrans.m_Trans2End)
                        p.m_Color = end;
                    else
                    {
                        int interpFrac = lifetimeFrac - colorTrans.m_Trans2Start;
                        if (!colorTrans.m_SmoothTrans)
                            interpFrac = colorTrans.m_Trans2End - colorTrans.m_Trans2Start;
                        int div = colorTrans.m_Trans2End - colorTrans.m_Trans2Start;
                        if (div == 0) div = 1;

                        p.m_Color = Color.FromArgb(
                            (byte)((middle.R * 31 / 255 + interpFrac * (end.R * 31 / 255 - middle.R * 31 / 255)
                                / div) * 255 / 31),
                            (byte)((middle.G * 31 / 255 + interpFrac * (end.G * 31 / 255 - middle.G * 31 / 255)
                                / div) * 255 / 31),
                            (byte)((middle.B * 31 / 255 + interpFrac * (end.B * 31 / 255 - middle.B * 31 / 255)
                                / div) * 255 / 31)
                            );
                    }
                }
                else
                {
                    int interpFrac = lifetimeFrac - colorTrans.m_Trans1Start;
                    if (!colorTrans.m_SmoothTrans)
                        interpFrac = colorTrans.m_Trans2Start - colorTrans.m_Trans1Start;
                    int div = colorTrans.m_Trans2Start - colorTrans.m_Trans1Start;
                    if (div == 0) div = 1;

                    p.m_Color = Color.FromArgb(
                        (byte)((start.R * 31 / 255 + interpFrac * (middle.R * 31 / 255 - start.R * 31 / 255)
                            / div) * 255 / 31),
                        (byte)((start.G * 31 / 255 + interpFrac * (middle.G * 31 / 255 - start.G * 31 / 255)
                            / div) * 255 / 31),
                        (byte)((start.B * 31 / 255 + interpFrac * (middle.B * 31 / 255 - start.B * 31 / 255)
                            / div) * 255 / 31)
                        );
                }
            }
            else
                p.m_Color = start;
        }

        public static void ProcessAlphaTrans(Particle p, System.Def sysDef, int lifetimeFrac)
        {
            AlphaTransition alphaTrans = sysDef.m_AlphaTrans;
            if (lifetimeFrac >= alphaTrans.m_Trans1End)
            {
                if (lifetimeFrac >= alphaTrans.m_Trans2Start)
                {
                    p.m_AlphaMult = alphaTrans.m_End + (lifetimeFrac - 0xff) *
                        (alphaTrans.m_End - alphaTrans.m_Middle) /
                        ((0xff - alphaTrans.m_Trans2Start) != 0 ? (0xff - alphaTrans.m_Trans2Start) : 1);
                }
                else
                    p.m_AlphaMult = alphaTrans.m_Middle;
            }
            else
                p.m_AlphaMult = alphaTrans.m_Start + lifetimeFrac *
                    (alphaTrans.m_Middle - alphaTrans.m_Start) /
                    (alphaTrans.m_Trans1End != 0 ? alphaTrans.m_Trans1End : 1);

            p.m_AlphaMult = (int)(p.m_AlphaMult *
                (0xff - alphaTrans.m_Flicker * (RandomUInt() >> 24) / 0x100) / 0x100);
        }

        public static void ProcessGlitterAlphaTrans(Particle p, System.Def sysDef, int lifetimeFrac)
        {
            p.m_AlphaMult = 31 * (0xff - lifetimeFrac) / 0xff;
        }

        public static void ProcessTexSeq(Particle p, System.Def sysDef, int lifetimeFrac)
        {
            TextureSequence texSeq = sysDef.m_TexSeq;
            int spriteIndex = 0;
            int intervalTracker = 0;
            while (lifetimeFrac >= intervalTracker + texSeq.m_Interval)
            {
                ++spriteIndex;
                intervalTracker += texSeq.m_Interval;
                if (spriteIndex >= texSeq.m_NumSprites)
                    return;
            }
            p.m_Sprite = texSeq.m_Sprites[spriteIndex];
        }

        public void Update(System system, Stack<Particle> freeParticles)
        {
            MainInfo info = system.m_Def.m_MainInfo;

            if (info.m_FollowSystem)
                m_Pos = system.m_Pos;

            Vector3 velAddend = Vector3.Zero;
            foreach (Effect effect in system.m_Def.m_Effects)
                effect.Process(this, ref velAddend);

            m_Angle += m_AngleSpeed;
            m_Vel = m_Vel * (info.m_SpeedFalloff + 0x180) / 0x200 + velAddend;
            m_Offset += m_Vel + system.m_Vel;

            if (system.m_Def.m_Glitter != null)
            {
                int wait = m_Age * 0x1000 - m_Lifetime * (system.m_Def.m_Glitter.m_Wait + 1) * 0x10;
                if (wait >= 0 && wait / 0x1000 % system.m_Def.m_Glitter.m_Period == 0)
                    SpawnGlitter(system, freeParticles);
            }

            ++m_Age;
        }

        public void UpdateGlitter(System system)
        {
            Glitter glitter = system.m_Def.m_Glitter;

            if (glitter.m_FollowSystem)
                m_Pos = system.m_Pos;

            Vector3 velAddend = Vector3.Zero;
            if (glitter.m_HasEffects)
                foreach (Effect effect in system.m_Def.m_Effects)
                    effect.Process(this, ref velAddend);

            m_Angle += m_AngleSpeed;
            m_Vel = m_Vel * (system.m_Def.m_MainInfo.m_SpeedFalloff + 0x180) / 0x200 + velAddend;
            m_Offset += m_Vel + system.m_Vel;

            ++m_Age;
        }

        public void SetBillboardMatrix(System system)
        {
            float vertScale = m_Scale * m_ScaleMult * 0.001f;
            Vector3 pos = Vector3.Transform((m_Pos + m_Offset) * 0.001f, VIEW_MATRIX);
            float horzScale = vertScale * system.m_Def.m_MainInfo.m_HorzScaleMult;

            Matrix4 transform = new Matrix4
            (
                (float)(horzScale * Math.Cos(Rad(m_Angle))), (float)(horzScale * Math.Sin(Rad(m_Angle))), 0, 0,
                (float)(vertScale * -Math.Sin(Rad(m_Angle))), (float)(vertScale * Math.Cos(Rad(m_Angle))), 0, 0,
                0, 0, 1, 0,
                pos.X, pos.Y, pos.Z, 1
            );
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref transform);
        }

        public void SetVelStretchMatrix(System system, ref bool render)
        {
            Vector3 viewCrossVel = Vector3.Cross(m_Vel, VIEW_MATRIX.Column2.Xyz);
            if ((int)(viewCrossVel.X * 0x1000) == 0 &&
                (int)(viewCrossVel.Y * 0x1000) == 0 &&
                (int)(viewCrossVel.Z * 0x1000) == 0)
            {
                render = false;
                return;
            }

            Vector3 viewCrossVelDir = Vector3.Transform(viewCrossVel.Normalized(), VIEW_MATRIX.ClearTranslation());
            Vector3 viewPos = Vector3.Transform((m_Pos + m_Offset) * 0.001f, VIEW_MATRIX);

            float velDot = Math.Abs(Vector3.Dot(m_Vel.Normalized(), -VIEW_MATRIX.Column2.Xyz));

            float scale = m_Scale * m_ScaleMult * 0.001f;
            float vertScale = scale * ((1 - velDot) * system.m_Def.m_MainInfo.m_VelStretchFactor + 1);
            float horzScale = scale * system.m_Def.m_MainInfo.m_HorzScaleMult;

            Matrix4 transform = new Matrix4
            (
                horzScale * viewCrossVelDir.X, horzScale * viewCrossVelDir.Y, 0, 0,
                vertScale * -viewCrossVelDir.Y, vertScale * viewCrossVelDir.X, 0, 0,
                0, 0, 1, 0,
                viewPos.X, viewPos.Y, viewPos.Z, 1
            );
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref transform);
        }

        public void Set3DMatrix(System system)
        {
            float vertScale = m_Scale * m_ScaleMult * 0.001f;
            float horzScale = vertScale * system.m_Def.m_MainInfo.m_HorzScaleMult;

            Matrix4 transform = new Matrix4
            (
                horzScale, 0, 0, 0,
                0, vertScale, 0, 0,
                0, 0, vertScale, 0,
                0, 0, 0, 1
            );
            Matrix4 rotMat = system.m_Def.m_MainInfo.m_WeirdAxis ?
                Matrix4.CreateFromAxisAngle(THE_WEIRD_AXIS, Rad(m_Angle)) :
                Matrix4.CreateRotationY(Rad(m_Angle));

            transform *= rotMat; //matrices are column-major
            transform *= Matrix4.CreateTranslation((m_Pos + m_Offset) * 0.001f);
            transform *= VIEW_MATRIX;

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref transform);
        }

        public void Render(System system, bool isGlitter)
        {
            MainInfo info = system.m_Def.m_MainInfo;
            Glitter glitter = system.m_Def.m_Glitter;

            if (!isGlitter && system.m_Def.m_TexSeq != null)
                m_Sprite.SetAsCurrent(1 << info.m_LogTexRepeatHorz, 1 << info.m_LogTexRepeatVert, false);

            byte alpha = (byte)(m_Alpha * (m_AlphaMult + 1) / 0x20);
            if (alpha > 0)
            {
                bool render = true;
                switch (isGlitter ? glitter.m_DrawMode : info.m_DrawMode)
                {
                    case MainInfo.DrawMode.BILLBOARD: SetBillboardMatrix(system); break;
                    case MainInfo.DrawMode.VEL_STRETCH: SetVelStretchMatrix(system, ref render); break;
                    case MainInfo.DrawMode.THREE_D: Set3DMatrix(system); break;
                }
                if (!render)
                    return;

                GL.Color4((byte)(m_Color.R * 31 / 255 * 255 / 31),
                          (byte)(m_Color.G * 31 / 255 * 255 / 31),
                          (byte)(m_Color.B * 31 / 255 * 255 / 31),
                          (byte)(alpha * 255 / 31));
                if (info.m_DrawMode != MainInfo.DrawMode.THREE_D || !info.m_HorzIf3D)
                    GL.CallList(GL_XY_LIST);
                else
                    GL.CallList(GL_XZ_LIST);
            }
        }

        public class Texture
        {
            public static Texture CURR_TEX = null;

            public enum RepeatMode { CLAMP, REPEAT, FLIP };
            public int m_ID { get; private set; }
            public SM64DSFormats.NitroTexture m_Tex { get; private set; }
            public RepeatMode m_RepeatX { get; set; }
            public RepeatMode m_RepeatY { get; set; }
            public int m_SpriteID;

            public Texture(byte[] texdata, byte[] paldata, int width, int height, byte colour0Mode, int textype,
                RepeatMode x, RepeatMode y, int spriteID)
            {
                m_ID = 0;
                m_Tex = SM64DSFormats.NitroTexture.FromDataAndType(0, "", 0, "",
                    texdata, paldata, width, height, colour0Mode, textype);
                m_RepeatX = x;
                m_RepeatY = y;
                m_SpriteID = spriteID;
            }

            public Texture(Bitmap bmp, int textype, RepeatMode x, RepeatMode y, int spriteID)
            {
                m_ID = 0;
                m_Tex = SM64DSFormats.NitroTexture.FromBitmapAndType(0, "", 0, "", bmp, textype);
                m_Tex.FromRaw(m_Tex.m_RawTextureData, m_Tex.m_RawPaletteData);
                m_RepeatX = x;
                m_RepeatY = y;
                m_SpriteID = spriteID;
            }

            // creates a dummy particle texture (for external textures that haven't been loaded yet)
            public Texture(int spriteID = 0)
            {
                m_ID = 0;
                m_Tex = SM64DSFormats.NitroTexture.FromBitmapAndType(0, "", 0, "", Properties.Resources.DummyTex, 2);
                m_Tex.FromRaw(m_Tex.m_RawTextureData, m_Tex.m_RawPaletteData);
                m_RepeatX = RepeatMode.CLAMP;
                m_RepeatY = RepeatMode.CLAMP;
                m_SpriteID = spriteID;
            }

            public void Load()
            {
                m_ID = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, m_ID);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Four,
                    m_Tex.m_Width, m_Tex.m_Height, 0, PixelFormat.Bgra,
                    PixelType.UnsignedByte, m_Tex.GetARGB());

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                    (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                    (int)TextureMagFilter.Nearest);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                    (int)(m_RepeatX == RepeatMode.CLAMP ? TextureWrapMode.Clamp :
                    m_RepeatX == RepeatMode.REPEAT ? TextureWrapMode.Repeat :
                    TextureWrapMode.MirroredRepeat));
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                    (int)(m_RepeatY == RepeatMode.CLAMP ? TextureWrapMode.Clamp :
                    m_RepeatY == RepeatMode.REPEAT ? TextureWrapMode.Repeat :
                    TextureWrapMode.MirroredRepeat));
            }

            public void SetAsCurrent(int repeatX, int repeatY, bool force)
            {
                if (!force && this == CURR_TEX)
                    return;

                CURR_TEX = this;
                GL.BindTexture(TextureTarget.Texture2D, m_ID);
                GL.MatrixMode(MatrixMode.Texture);
                GL.LoadIdentity();
                GL.Scale(repeatX, repeatY, 1);
            }

            public void Unload()
            {
                if (m_ID != 0)
                    GL.DeleteTexture(m_ID);
            }
        }

        public class MainInfo
        {
            public enum SpawnShape { POINT, SPHERE, CIRCLE_RANDOM, CIRCLE_EVEN, BALL, DISC }
            public enum DrawMode { BILLBOARD, VEL_STRETCH, THREE_D }
            public enum Plane { Z, Y, X, DIR }

            public SpawnShape m_SpawnShape;
            public DrawMode m_DrawMode;
            public Plane m_Plane;
            public bool m_Rotate;
            public bool m_RandomInitAng;
            public bool m_FollowSystem;
            public bool m_WeirdAxis; //to be more precise, <-1, -1, -1>
            public bool m_HorzIf3D;

            public bool m_SelfDestruct;

            public float m_Rate;
            public float m_StartHorzDist;
            public Vector3 m_Dir;
            public Color m_Color;
            public float m_HorzSpeed;
            public float m_VertSpeed;
            public float m_Scale;
            public float m_HorzScaleMult;
            public short m_MinAngleSpeed;
            public short m_MaxAngleSpeed;
            public int m_Frames; //of system
            public int m_Lifetime; //of particles
            public int m_ScaleRand;
            public int m_LifetimeRand;
            public int m_SpeedRand;
            public int m_SpawnPeriod;
            public int m_Alpha;
            public int m_SpeedFalloff; //0x80 is normal
            public Texture m_Sprite;
            public int m_AltLength;
            public float m_VelStretchFactor;
            public int m_LogTexRepeatHorz;
            public int m_LogTexRepeatVert;

            public MainInfo()
            {
                m_SpawnPeriod = 1;
            }

            public MainInfo Copy()
            {
                MainInfo newMainInfo = new MainInfo();

                newMainInfo.m_SpawnShape = m_SpawnShape;
                newMainInfo.m_SpawnShape = m_SpawnShape;
                newMainInfo.m_DrawMode = m_DrawMode;
                newMainInfo.m_Plane = m_Plane;
                newMainInfo.m_Rotate = m_Rotate;
                newMainInfo.m_RandomInitAng = m_RandomInitAng;
                newMainInfo.m_FollowSystem = m_FollowSystem;
                newMainInfo.m_WeirdAxis = m_WeirdAxis;
                newMainInfo.m_HorzIf3D = m_HorzIf3D;

                newMainInfo.m_SelfDestruct = m_SelfDestruct;

                newMainInfo.m_Rate = m_Rate;
                newMainInfo.m_StartHorzDist = m_StartHorzDist;
                newMainInfo.m_Dir = new Vector3(m_Dir.X, m_Dir.Y, m_Dir.Z); // copy value
                newMainInfo.m_Color = Helper.BGR15ToColor(Helper.ColorToBGR15(m_Color)); // copy value
                newMainInfo.m_HorzSpeed = m_HorzSpeed;
                newMainInfo.m_VertSpeed = m_VertSpeed;
                newMainInfo.m_Scale = m_Scale;
                newMainInfo.m_HorzScaleMult = m_HorzScaleMult;
                newMainInfo.m_MinAngleSpeed = m_MinAngleSpeed;
                newMainInfo.m_MaxAngleSpeed = m_MaxAngleSpeed;
                newMainInfo.m_Frames = m_Frames;
                newMainInfo.m_Lifetime = m_Lifetime;
                newMainInfo.m_ScaleRand = m_ScaleRand;
                newMainInfo.m_LifetimeRand = m_LifetimeRand;
                newMainInfo.m_SpeedRand = m_SpeedRand;
                newMainInfo.m_SpawnPeriod = m_SpawnPeriod;
                newMainInfo.m_Alpha = m_Alpha;
                newMainInfo.m_SpeedFalloff = m_SpeedFalloff;
                newMainInfo.m_Sprite = m_Sprite; // copy reference
                newMainInfo.m_AltLength = m_AltLength;
                newMainInfo.m_VelStretchFactor = m_VelStretchFactor;
                newMainInfo.m_LogTexRepeatHorz = m_LogTexRepeatHorz;
                newMainInfo.m_LogTexRepeatVert = m_LogTexRepeatVert;

                return newMainInfo;
            }
        }

        public delegate void TransProcessFunc(Particle p, System.Def sysDef, int lifetimeFrac);
        public struct TransProcessFuncData
        {
            public TransProcessFunc m_Func;
            public bool m_UseAltLength;
        }

        public abstract class Transition
        {

        }

        public class ScaleTransition : Transition
        {
            public float m_Start;
            public float m_Middle;
            public float m_End;
            public int m_Trans1End;
            public int m_Trans2Start;
            public bool m_UseAltLength;

            public ScaleTransition Copy()
            {
                ScaleTransition newScaleTransition = new ScaleTransition();

                newScaleTransition.m_Start = m_Start;
                newScaleTransition.m_Middle = m_Middle;
                newScaleTransition.m_End = m_End;
                newScaleTransition.m_Trans1End = m_Trans1End;
                newScaleTransition.m_Trans2Start = m_Trans2Start;
                newScaleTransition.m_UseAltLength = m_UseAltLength;

                return newScaleTransition;
            }
        }

        public class ColorTransition : Transition
        {
            public Color m_Start;
            public Color m_End;
            public int m_Trans1Start;
            public int m_Trans2Start;
            public int m_Trans2End;
            public bool m_UseAsOptions;
            public bool m_UseAltLength;
            public bool m_SmoothTrans;

            public ColorTransition Copy()
            {
                ColorTransition newColorTransition = new ColorTransition();

                newColorTransition.m_Start = Helper.BGR15ToColor(Helper.ColorToBGR15(m_Start)); // copy value
                newColorTransition.m_End = Helper.BGR15ToColor(Helper.ColorToBGR15(m_End)); // copy value
                newColorTransition.m_Trans1Start = m_Trans1Start;
                newColorTransition.m_Trans2Start = m_Trans2Start;
                newColorTransition.m_Trans2End = m_Trans2End;
                newColorTransition.m_UseAsOptions = m_UseAsOptions;
                newColorTransition.m_UseAltLength = m_UseAltLength;
                newColorTransition.m_SmoothTrans = m_SmoothTrans;

                return newColorTransition;
            }
        }

        public class AlphaTransition : Transition
        {
            public int m_Start;
            public int m_Middle;
            public int m_End;
            public int m_Flicker;
            public bool m_UseAltLength;
            public int m_Trans1End;
            public int m_Trans2Start;

            public AlphaTransition Copy()
            {
                AlphaTransition newAlphaTransition = new AlphaTransition();

                newAlphaTransition.m_Start = m_Start;
                newAlphaTransition.m_Middle = m_Middle;
                newAlphaTransition.m_End = m_End;
                newAlphaTransition.m_Flicker = m_Flicker;
                newAlphaTransition.m_UseAltLength = m_UseAltLength;
                newAlphaTransition.m_Trans1End = m_Trans1End;
                newAlphaTransition.m_Trans2Start = m_Trans2Start;

                return newAlphaTransition;
            }
        }

        public class TextureSequence : Transition
        {
            public Texture[] m_Sprites;
            public int m_NumSprites;
            public int m_Interval;
            public bool m_UseAsOptions;
            public bool m_UseAltLength;

            public TextureSequence Copy()
            {
                TextureSequence newTextureSequence = new TextureSequence();

                newTextureSequence.m_Sprites = new Texture[8];
                m_Sprites.CopyTo(newTextureSequence.m_Sprites, 1);
                newTextureSequence.m_NumSprites = m_NumSprites;
                newTextureSequence.m_Interval = m_Interval;
                newTextureSequence.m_UseAsOptions = m_UseAsOptions;
                newTextureSequence.m_UseAltLength = m_UseAltLength;

                return newTextureSequence;
            }
        }

        public class Glitter
        {
            public enum AngleCopyMode { NONE, ANGLE, ANGLE_AND_SPEED }
            public bool m_HasEffects;
            public bool m_HasScaleTrans;
            public bool m_HasAlphaTrans;
            public AngleCopyMode m_AngleCopyMode;
            public bool m_FollowSystem;
            public bool m_UseGlitterColor;
            public MainInfo.DrawMode m_DrawMode;
            public float m_SpeedRand;
            public float m_Scale;
            public int m_Lifetime;
            public int m_SpeedMult;
            public int m_ScaleMult;
            public Color m_Color;
            public int m_Rate;
            public int m_Wait;
            public int m_Period;
            public Texture m_Sprite;
            public int m_LogTexRepeatHorz;
            public int m_LogTexRepeatVert;

            public Glitter()
            {
                m_Period = 1;
            }

            public Glitter Copy()
            {
                Glitter newGlitter = new Glitter();

                newGlitter.m_HasEffects = m_HasEffects;
                newGlitter.m_HasScaleTrans = m_HasScaleTrans;
                newGlitter.m_HasAlphaTrans = m_HasAlphaTrans;
                newGlitter.m_AngleCopyMode = m_AngleCopyMode;
                newGlitter.m_FollowSystem = m_FollowSystem;
                newGlitter.m_UseGlitterColor = m_UseGlitterColor;
                newGlitter.m_DrawMode = m_DrawMode;
                newGlitter.m_SpeedRand = m_SpeedRand;
                newGlitter.m_Scale = m_Scale;
                newGlitter.m_Lifetime = m_Lifetime;
                newGlitter.m_SpeedMult = m_SpeedMult;
                newGlitter.m_ScaleMult = m_ScaleMult;
                newGlitter.m_Color = Helper.BGR15ToColor(Helper.ColorToBGR15(m_Color)); // copy value
                newGlitter.m_Rate = m_Rate;
                newGlitter.m_Wait = m_Wait;
                newGlitter.m_Period = m_Period;
                newGlitter.m_Sprite = m_Sprite;
                newGlitter.m_LogTexRepeatHorz = m_LogTexRepeatHorz;
                newGlitter.m_LogTexRepeatVert = m_LogTexRepeatVert;

                return newGlitter;
            }
        }

        public abstract class Effect
        {
            public abstract void Process(Particle particle, ref Vector3 velocity);
        }

        public class Acceleration : Effect
        {
            public Vector3 m_Accel;
            public override void Process(Particle particle, ref Vector3 velocity)
            {
                velocity += m_Accel;
            }

            public Acceleration Copy()
            {
                Acceleration newAcceleration = new Acceleration();

                newAcceleration.m_Accel = new Vector3(m_Accel.X, m_Accel.Y, m_Accel.Z); // copy value

                return newAcceleration;
            }
        }

        public class Jitter : Effect
        {
            public Vector3 m_Mag;
            public int m_Period;
            public override void Process(Particle particle, ref Vector3 velocity)
            {
                if (particle.m_Age % m_Period == 0)
                {
                    velocity.X += m_Mag.X * (int)(RandomUInt() / 0x800000 - 0x100) / 0x100;
                    velocity.Y += m_Mag.Y * (int)(RandomUInt() / 0x800000 - 0x100) / 0x100;
                    velocity.Z += m_Mag.Z * (int)(RandomUInt() / 0x800000 - 0x100) / 0x100;
                }
            }

            public Jitter()
            {
                m_Period = 1;
            }

            public Jitter Copy()
            {
                Jitter newJitter = new Jitter();

                newJitter.m_Mag = new Vector3(m_Mag.X, m_Mag.Y, m_Mag.Z); // copy value
                newJitter.m_Period = m_Period;

                return newJitter;
            }
        }

        public class Converge : Effect
        {
            public Vector3 m_Offset;
            public float m_Mag;
            public override void Process(Particle particle, ref Vector3 velocity)
            {
                velocity += m_Mag * (m_Offset - particle.m_Offset - particle.m_Vel);
            }

            public Converge Copy()
            {
                Converge newConverge = new Converge();

                newConverge.m_Offset = new Vector3(m_Offset.X, m_Offset.Y, m_Offset.Z); // copy value
                newConverge.m_Mag = m_Mag;

                return newConverge;
            }
        }

        public class Turn : Effect
        {
            public enum Axis { X, Y, Z }
            public short m_AngleSpeed;
            public Axis m_Axis;
            public override void Process(Particle particle, ref Vector3 velocity)
            {
                Matrix4 turnMat = new Matrix4();
                switch (m_Axis)
                {
                    case Axis.X: turnMat = Matrix4.CreateRotationX(Rad(m_AngleSpeed)); break;
                    case Axis.Y: turnMat = Matrix4.CreateRotationY(Rad(m_AngleSpeed)); break;
                    case Axis.Z: turnMat = Matrix4.CreateRotationZ(Rad(m_AngleSpeed)); break;
                }
                particle.m_Offset = Vector3.Transform(particle.m_Offset, turnMat);
            }

            public Turn Copy()
            {
                Turn newTurn = new Turn();

                newTurn.m_AngleSpeed = m_AngleSpeed;
                newTurn.m_Axis = m_Axis;

                return newTurn;
            }
        }

        public class LimitPlane : Effect //not used by any of the 0x141 particle system defs
        {
            public enum Behavior { KILL, CHANGE_SPEED }
            public float m_PosY;
            public float m_ReverseSpeedMult;
            public Behavior m_Behavior;
            public override void Process(Particle particle, ref Vector3 velocity)
            {
                if (m_Behavior == Behavior.CHANGE_SPEED)
                {
                    if (particle.m_Pos.Y < m_PosY && m_PosY < particle.m_Pos.Y + particle.m_Offset.Y)
	                {
		                particle.m_Offset.Y = m_PosY - particle.m_Pos.Y;
		                particle.m_Vel.Y *= -m_ReverseSpeedMult;
	                }
	                else if (particle.m_Pos.Y > m_PosY && particle.m_Pos.Y + particle.m_Offset.Y < m_PosY)
	                {
		                particle.m_Offset.Y = m_PosY - particle.m_Pos.Y;
		                particle.m_Vel.Y *= -m_ReverseSpeedMult;
	                }
                }
                else if (m_Behavior == Behavior.KILL)
                {
                    if (particle.m_Pos.Y < m_PosY && m_PosY < particle.m_Pos.Y + particle.m_Offset.Y)
                        particle.m_Age = particle.m_Lifetime;
                    else if (m_PosY < particle.m_Pos.Y && particle.m_Pos.Y + particle.m_Offset.Y < m_PosY)
                        particle.m_Age = particle.m_Lifetime;
                }
            }

            public LimitPlane Copy()
            {
                LimitPlane newLimitPlane = new LimitPlane();

                newLimitPlane.m_PosY = m_PosY;
                newLimitPlane.m_ReverseSpeedMult = m_ReverseSpeedMult;
                newLimitPlane.m_Behavior = m_Behavior;

                return newLimitPlane;
            }
        }

        public class RadiusConverge : Effect
        {
            public Vector3 m_Offset;
            public float m_Mag;
            public override void Process(Particle particle, ref Vector3 velocity)
            {
                particle.m_Offset += m_Mag * (m_Offset - particle.m_Offset);
            }

            public RadiusConverge Copy()
            {
                RadiusConverge newRadiusConverge = new RadiusConverge();

                newRadiusConverge.m_Offset = new Vector3(m_Offset.X, m_Offset.Y, m_Offset.Z); // copy value
                newRadiusConverge.m_Mag = m_Mag;

                return newRadiusConverge;
            }
        }

        public class System
        {
            public class Def
            {
                public MainInfo m_MainInfo;
                public ScaleTransition m_ScaleTrans;
                public ColorTransition m_ColorTrans;
                public AlphaTransition m_AlphaTrans;
                public TextureSequence m_TexSeq;
                public Glitter m_Glitter;
                public List<Effect> m_Effects;
                public PropertyTable m_Properties;

                public Def Copy()
                {
                    Def sysDef = new Def();

                    sysDef.m_MainInfo   = m_MainInfo.Copy();
                    sysDef.m_ScaleTrans = m_ScaleTrans == null ? null : m_ScaleTrans.Copy();
                    sysDef.m_ColorTrans = m_ColorTrans == null ? null : m_ColorTrans.Copy();
                    sysDef.m_AlphaTrans = m_AlphaTrans == null ? null : m_AlphaTrans.Copy();
                    sysDef.m_TexSeq     = m_TexSeq     == null ? null : m_TexSeq.Copy();
                    sysDef.m_Glitter    = m_Glitter    == null ? null : m_Glitter.Copy();
                    
                    sysDef.m_Effects = new List<Effect>();

                    int indexAcceleration   = m_Effects.FindIndex(x => x is Acceleration);
                    int indexJitter         = m_Effects.FindIndex(x => x is Jitter);
                    int indexConverge       = m_Effects.FindIndex(x => x is Converge);
                    int indexTurn           = m_Effects.FindIndex(x => x is Turn);
                    int indexLimitPlane     = m_Effects.FindIndex(x => x is LimitPlane);
                    int indexRadiusConverge = m_Effects.FindIndex(x => x is RadiusConverge);

                    if (indexAcceleration != -1)
                        sysDef.m_Effects.Add(((Acceleration)m_Effects[indexAcceleration]).Copy());
                    if (indexJitter != -1)
                        sysDef.m_Effects.Add(((Jitter)m_Effects[indexJitter]).Copy());
                    if (indexConverge != -1)
                        sysDef.m_Effects.Add(((Converge)m_Effects[indexConverge]).Copy());
                    if (indexTurn != -1)
                        sysDef.m_Effects.Add(((Turn)m_Effects[indexTurn]).Copy());
                    if (indexLimitPlane != -1)
                        sysDef.m_Effects.Add(((LimitPlane)m_Effects[indexLimitPlane]).Copy());
                    if (indexRadiusConverge != -1)
                        sysDef.m_Effects.Add(((RadiusConverge)m_Effects[indexRadiusConverge]).Copy());

                    return sysDef;
                }
            }

            public List<Particle> m_Particles { get; private set; }
            public List<Particle> m_GlitterParticles { get; private set; }
            public Def m_Def { get; private set; }
            public bool m_Stopped;
            public bool m_SpawnPaused;
            public bool m_Paused;
            public Vector3 m_Pos { get; private set; }
            public Vector3 m_Vel { get; private set; }
            public int m_Age { get; private set; }
            public int m_RateTracker { get; private set; }
            public Vector3 m_Dir { get; private set; }
            public Vector3 m_Tangent { get; private set; }
            public Vector3 m_Tangent2 { get; private set; }
            public float m_StartHorzDist { get; private set; }
            public float m_HorzSpeed { get; private set; }
            public float m_VertSpeed { get; private set; }
            public float m_Scale { get; private set; }
            public int m_ParticleLifetime { get; private set; }
            public int m_SpawnPeriod { get; private set; }
            public int m_Alpha { get; private set; } //out of 31

            public System()
            {
                m_Particles = new List<Particle>();
                m_GlitterParticles = new List<Particle>();
                m_Def = null;
                m_Stopped = true;
            }

            public void Init(Def sysDef, Vector3 thePos, Vector3? theDir)
            {
                m_Particles.Clear();
                m_GlitterParticles.Clear();
                m_Def = sysDef;
                m_Stopped = false;
                m_Paused = false;
                m_Pos = thePos;
                m_Vel = Vector3.Zero;
                m_Age = 0;
                m_RateTracker = 0;

                m_Dir = theDir != null ? (Vector3)theDir : m_Def.m_MainInfo.m_Dir;
                m_StartHorzDist = m_Def.m_MainInfo.m_StartHorzDist;
                m_HorzSpeed = m_Def.m_MainInfo.m_HorzSpeed;
                m_VertSpeed = m_Def.m_MainInfo.m_VertSpeed;
                m_Scale = m_Def.m_MainInfo.m_Scale;
                m_ParticleLifetime = m_Def.m_MainInfo.m_Lifetime;
                m_SpawnPeriod = m_Def.m_MainInfo.m_SpawnPeriod;
                m_Alpha = m_Def.m_MainInfo.m_Alpha;
            }

            public void CalcTangents()
            {
                Vector3 vec = Vector3.UnitY;
                Vector3 normalDir = m_Dir.Normalized();
                if ((int)Math.Abs(normalDir.X * 0x1000) == 0 &&
                    (int)Math.Abs(normalDir.Y * 0x1000) == 0x1000 &&
                    (int)Math.Abs(normalDir.Z * 0x1000) == 0)
                    vec = Vector3.UnitX;
                m_Tangent = Vector3.Cross(normalDir, vec);
                m_Tangent2 = Vector3.Cross(normalDir, m_Tangent);
            }

            public void AddParticles(Stack<Particle> freeParticles)
            {
                MainInfo info = m_Def.m_MainInfo;

                int numToSpawn = m_RateTracker + (int)(info.m_Rate * 0x1000);
                m_RateTracker = numToSpawn & 0xfff;
                numToSpawn /= 0x1000;

                if ((info.m_SpawnShape == MainInfo.SpawnShape.CIRCLE_EVEN ||
                    info.m_SpawnShape == MainInfo.SpawnShape.CIRCLE_RANDOM ||
                    info.m_SpawnShape == MainInfo.SpawnShape.DISC) &&
                    info.m_Plane == MainInfo.Plane.DIR)
                    CalcTangents();

                for (int i = 0; i < numToSpawn; ++i)
                {
                    if (freeParticles.Count == 0)
                        return;

                    Particle particle = freeParticles.Pop();
                    m_Particles.Add(particle);
                    particle.Init(this, i, numToSpawn);
                }
            }

            public void Update(Manager manager)
            {
                MainInfo info = m_Def.m_MainInfo;
                if ((info.m_Frames == 0 || m_Age < info.m_Frames) &&
                    m_Age % m_SpawnPeriod == 0 && !m_Stopped && !m_SpawnPaused)
                    AddParticles(manager.m_FreeParticles);

                TransProcessFuncData[] transProcessData = new TransProcessFuncData[4];
                int numTransitions = 0;
                if (m_Def.m_ScaleTrans != null)
                {
                    transProcessData[numTransitions].m_Func = ProcessScaleTrans;
                    transProcessData[numTransitions].m_UseAltLength = m_Def.m_ScaleTrans.m_UseAltLength;
                    ++numTransitions;
                }
                if (m_Def.m_ColorTrans != null && !m_Def.m_ColorTrans.m_UseAsOptions)
                {
                    transProcessData[numTransitions].m_Func = ProcessColorTrans;
                    transProcessData[numTransitions].m_UseAltLength = m_Def.m_ColorTrans.m_UseAltLength;
                    ++numTransitions;
                }
                if (m_Def.m_AlphaTrans != null)
                {
                    transProcessData[numTransitions].m_Func = ProcessAlphaTrans;
                    transProcessData[numTransitions].m_UseAltLength = m_Def.m_AlphaTrans.m_UseAltLength;
                    ++numTransitions;
                }
                if (m_Def.m_TexSeq != null && !m_Def.m_TexSeq.m_UseAsOptions)
                {
                    transProcessData[numTransitions].m_Func = ProcessTexSeq;
                    transProcessData[numTransitions].m_UseAltLength = m_Def.m_TexSeq.m_UseAltLength;
                    ++numTransitions;
                }

                //Copy to avoid invalidation when a particle is removed
                List<Particle> particlesCopy = new List<Particle>(m_Particles);
                foreach (Particle particle in particlesCopy)
                {
                    for (int i = 0; i < numTransitions; ++i)
                        transProcessData[i].m_Func(particle, m_Def,
                            (transProcessData[i].m_UseAltLength ? particle.m_AltLenInv : particle.m_LifetimeInv)
                            * particle.m_Age / 0x100 & 0xff);

                    particle.Update(this, manager.m_FreeParticles);
                    if (particle.m_Age > particle.m_Lifetime)
                    {
                        manager.m_FreeParticles.Push(particle);
                        m_Particles.Remove(particle);
                    }
                }

                if (m_Def.m_Glitter != null)
                {
                    Glitter glitter = m_Def.m_Glitter;
                    numTransitions = 0;

                    if (glitter.m_HasScaleTrans)
                    {
                        transProcessData[numTransitions].m_Func = ProcessGlitterScaleTrans;
                        transProcessData[numTransitions].m_UseAltLength = false;
                        ++numTransitions;
                    }
                    if (glitter.m_HasAlphaTrans)
                    {
                        transProcessData[numTransitions].m_Func = ProcessGlitterAlphaTrans;
                        transProcessData[numTransitions].m_UseAltLength = false;
                        ++numTransitions;
                    }

                    particlesCopy = new List<Particle>(m_GlitterParticles);
                    foreach (Particle particle in particlesCopy)
                    {
                        for (int i = 0; i < numTransitions; ++i)
                            transProcessData[i].m_Func(particle, m_Def,
                                particle.m_LifetimeInv * particle.m_Age / 0x100 & 0xff);

                        particle.UpdateGlitter(this);
                        if (particle.m_Age > particle.m_Lifetime)
                        {
                            manager.m_FreeParticles.Push(particle);
                            m_GlitterParticles.Remove(particle);
                        }
                    }
                }

                ++m_Age;
            }

            public void Render()
            {
                MainInfo info = m_Def.m_MainInfo;

                info.m_Sprite.SetAsCurrent(1 << info.m_LogTexRepeatHorz, 1 << info.m_LogTexRepeatVert, true);
                foreach (Particle particle in m_Particles)
                    particle.Render(this, false);

                Glitter glitter = m_Def.m_Glitter;
                if (glitter == null)
                    return;

                glitter.m_Sprite.SetAsCurrent(1 << glitter.m_LogTexRepeatHorz, 1 << glitter.m_LogTexRepeatVert, true);
                foreach (Particle particle in m_GlitterParticles)
                    particle.Render(this, true);
            }

        }

        public class Manager
        {
            public List<System> m_Systems { get; private set; }
            public Stack<Particle> m_FreeParticles { get; private set; }
            public Stack<System> m_FreeSystems { get; private set; }

            System[] m_AllocSystems;
            Particle[] m_AllocParticles;

            public Manager()
            {
                m_Systems = new List<System>();

                m_AllocSystems = new System[64];
                for (int i = 0; i < 64; ++i)
                    m_AllocSystems[i] = new System();
                m_FreeSystems = new Stack<System>(m_AllocSystems);

                m_AllocParticles = new Particle[256];
                for (int i = 0; i < 256; ++i)
                    m_AllocParticles[i] = new Particle();
                m_FreeParticles = new Stack<Particle>(m_AllocParticles);
            }

            public System AddSystem(System.Def sysDef, Vector3 pos, Vector3? dir)
            {
                if (m_FreeSystems.Count == 0)
                    return null;

                System system = m_FreeSystems.Pop();
                m_Systems.Add(system);
                system.Init(sysDef, pos, dir);
                return system;
            }

            public void Update()
            {
                List<System> systemsCopy = new List<System>(m_Systems);
                foreach (System system in systemsCopy)
                {
                    if (!system.m_Paused)
                        system.Update(this);

                    MainInfo info = system.m_Def.m_MainInfo;
                    if ((info.m_Frames != 0 && system.m_Age > info.m_Frames || system.m_Stopped) &&
                        system.m_Particles.Count == 0 && system.m_GlitterParticles.Count == 0)
                    {
                        m_FreeSystems.Push(system);
                        system.m_Stopped = true;
                        m_Systems.Remove(system);
                    }
                }
            }

            public void Render(int[] displayLists, Matrix4 viewMatrix)
            {
                VIEW_MATRIX = viewMatrix;
                if (GL_XY_LIST == 0)
                {
                    GL_XY_LIST = GL.GenLists(2);
                    GL_XZ_LIST = GL_XY_LIST + 1;

                    GL.NewList(GL_XY_LIST, ListMode.Compile);
                    GL.Begin(PrimitiveType.Quads);
                    GL.TexCoord2(0, 0);
                    GL.Vertex3(-1, 1, 0);
                    GL.TexCoord2(1, 0);
                    GL.Vertex3(1, 1, 0);
                    GL.TexCoord2(1, 1);
                    GL.Vertex3(1, -1, 0);
                    GL.TexCoord2(0, 1);
                    GL.Vertex3(-1, -1, 0);
                    GL.End();
                    GL.EndList();

                    GL.NewList(GL_XZ_LIST, ListMode.Compile);
                    GL.Begin(PrimitiveType.Quads);
                    GL.TexCoord2(0, 0);
                    GL.Vertex3(-1, 0, 1);
                    GL.TexCoord2(1, 0);
                    GL.Vertex3(1, 0, 1);
                    GL.TexCoord2(1, 1);
                    GL.Vertex3(1, 0, -1);
                    GL.TexCoord2(0, 1);
                    GL.Vertex3(-1, 0, -1);
                    GL.End();
                    GL.EndList();
                }

                if (displayLists[0] == 0)
                    displayLists[0] = GL.GenLists(1);

                GL.NewList(displayLists[0], ListMode.Compile);

                GL.PushAttrib(AttribMask.AllAttribBits);
                GL.Disable(EnableCap.CullFace);
                GL.Disable(EnableCap.Lighting);
                GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode,
                    (int)TextureEnvMode.Modulate);
                GL.DepthMask(false);
                GL.MatrixMode(MatrixMode.Texture);
                GL.PushMatrix();
                GL.MatrixMode(MatrixMode.Modelview);
                GL.PushMatrix();

                foreach (System system in m_Systems)
                    system.Render();

                GL.MatrixMode(MatrixMode.Texture);
                GL.PopMatrix();
                GL.MatrixMode(MatrixMode.Modelview);
                GL.PopMatrix();
                GL.DepthMask(true);
                GL.PopAttrib();
                GL.EndList();
            }

            public void RemoveLists()
            {
                GL.DeleteLists(GL_XY_LIST, 2);
                GL_XY_LIST = GL_XZ_LIST = 0;
            }
        }
    }

}
