using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace SM64DSe
{
    [AttributeUsage(AttributeTargets.All)]
    public class IntRangeAttribute : Attribute
    {
        public int m_Min { get; private set; }
        public int m_Max { get; private set; }

        public IntRangeAttribute(int min, int max)
        {
            m_Min = min;
            m_Max = max;
        }

        public bool IsInRange(int value)
        {
            return value >= m_Min && value <= m_Max;
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    public class FloatRangeAttribute : Attribute
    {
        public enum Inclusivity { EXCLUSIVE, INCLUDE_FIRST, INCLUDE_LAST, INCLUSIVE };
        public float m_Min { get; private set; }
        public float m_Max { get; private set; }
        public Inclusivity m_Inclusivity { get; private set; }
        public string m_InclusiveStr
        {
            get
            {
                switch (m_Inclusivity)
                {
                    case Inclusivity.EXCLUSIVE: return "exclusive";
                    case Inclusivity.INCLUDE_FIRST: return "including the first, excluding the last";
                    case Inclusivity.INCLUDE_LAST: return "excluding the first, including the last";
                    case Inclusivity.INCLUSIVE: return "inclusive";
                }
                return "BUG! This should not happen!";
            }
        }


        public FloatRangeAttribute(float min, float max, Inclusivity inclusivity)
        {
            m_Min = min;
            m_Max = max;
            m_Inclusivity = inclusivity;
        }

        public bool IsInRange(float value)
        {
            return value >= m_Min && value <= m_Max;
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    public class ItemListAttribute : Attribute
    {
        public List<string> m_Items { get; private set; }

        public ItemListAttribute(List<string> items)
        {
            m_Items = items;
        }
    }

    class AngleTypeConverter : IntTypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
                return ((short)value * 360.0f / 65536).ToString() + "°";
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            float ret = 0;
            string _val = (string)value;
            _val = _val.Trim();
            if (_val.Last() == '°')
                _val = _val.Substring(0, _val.Length - 1).Trim();

            if (!float.TryParse(_val, NumberStyles.Float, culture, out ret))
                if (!float.TryParse(_val, NumberStyles.Float, new CultureInfo("en-US"), out ret))
                    throw new ArgumentException("Invalid value.");

            return (short)(ret * 65536 / 360);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return false;
        }
    }

    class ItemListTypeConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value is string)
                    return value;
                return GetItemList(context)[(int)value];
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            List<string> items = GetItemList(context);
            if (items.IndexOf((string)value) == -1)
                throw new ArgumentException("Invalid value.");
            return items.IndexOf((string)value);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return false;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(GetItemList(context));
        }
        public List<string> GetItemList(ITypeDescriptorContext context)
        {
            foreach (Attribute attr in context.PropertyDescriptor.Attributes)
                if (attr is ItemListAttribute)
                    return ((ItemListAttribute)attr).m_Items;
            return null;
        }
    }

    class BoolTypeConverter : ExpandableObjectConverter
    {
        public static Regex FALSE_REGEX = new Regex(@"\b(?i)false\b");
        public static Regex TRUE_REGEX = new Regex(@"\b(?i)true\b");

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value is bool)
                    return (bool)value ? "True" : "False";
                return value;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            int ret;
            string _val = (string)value;
            _val = _val.Trim();

            if (int.TryParse((string)value, out ret))
                return ret != 0;

            if (TRUE_REGEX.IsMatch((string)value))
                return true;

            if (FALSE_REGEX.IsMatch((string)value))
                return false;

            throw new ArgumentException("Invalid value. Must be false or true.");
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return false;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new string[] { "False", "True" });
        }
    }

    class ColorTypeEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            ColorDialog dialog = new ColorDialog();
            int oldColor = (int)(((Color)value).ToArgb() & ~0xff000000);
            oldColor = oldColor & 0x0000ff00 | oldColor << 16 & 0x00ff0000 | oldColor >> 16 & 0x000000ff;
            dialog.CustomColors = new int[] { oldColor };
            dialog.Color = (Color)value;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Color color = dialog.Color;
                color = Helper.BGR15ToColor(Helper.ColorToBGR15(color));
                return color;
            }

            return value;
        }
    }

    class ColorTypeConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                Color color = (Color)value;
                return string.Format("#{0:x2}{1:x2}{2:x2}", color.R, color.G, color.B);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            int bytes = 0;
            string _val = ((string)value).Remove(0, 1).Trim();

            if (!Helper.TryParseHexInt(_val, ref bytes) || bytes < 0 || bytes >= 0x1000000)
                throw new ArgumentException("Invalid value.\r\nIt should be a color.");

            return Color.FromArgb(
                (byte)(bytes >> 0 & 0xff),
                (byte)(bytes >> 8 & 0xff),
                (byte)(bytes >> 16 & 0xff));
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return false;
        }
    }

    public class ParticleSysDefProperties
    {
        public static int highestTextureID = 0x2d;

        public static void GenerateScaleTransitionProperties(Particle.System.Def def, List<Particle.Texture> textures, PropertyTable props)
        {
            Particle.ScaleTransition scaleTrans = def.m_ScaleTrans;
            props.AddProperty(new PropertySpec("Scale Multiplier 1", typeof(float), "Scale Transition", "The first scale multiplier", scaleTrans.m_Start, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-8, 8, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
            props.AddProperty(new PropertySpec("Scale Multiplier 2", typeof(float), "Scale Transition", "The second scale multiplier", scaleTrans.m_Middle, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-8, 8, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
            props.AddProperty(new PropertySpec("Scale Multiplier 3", typeof(float), "Scale Transition", "The last scale multiplier", scaleTrans.m_End, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-8, 8, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
            props.AddProperty(new PropertySpec("Transition 1-2 End (Scale)", typeof(int), "Scale Transition", "When the transition from 1st to 2nd values should end (x / 256)", scaleTrans.m_Trans1End, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(0, 0xff) }));
            props.AddProperty(new PropertySpec("Transition 2-3 Start (Scale)", typeof(int), "Scale Transition", "When the transition from 2nd to last values should start (x / 256)", scaleTrans.m_Trans2Start, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(0, 0xff) }));
            props.AddProperty(new PropertySpec("Use Alternate Length (Scale)", typeof(bool), "Scale Transition", "Whether to base the 'percents' off the alternate length instead of the lifetime", scaleTrans.m_UseAltLength, "", typeof(BoolTypeConverter)));
        }

        public static void GenerateColorTransitionProperties(Particle.System.Def def, List<Particle.Texture> textures, PropertyTable props)
        {
            Particle.ColorTransition colorTrans = def.m_ColorTrans;
            props.AddProperty(new PropertySpec("Color 1", typeof(Color), "Color Transition", "The first color. The second color is the main color.", colorTrans.m_Start, typeof(ColorTypeEditor), typeof(ColorTypeConverter)));
            props.AddProperty(new PropertySpec("Color 3", typeof(Color), "Color Transition", "The last color. The second color is the main color.", colorTrans.m_End, typeof(ColorTypeEditor), typeof(ColorTypeConverter)));
            props.AddProperty(new PropertySpec("Transition 1-2 Start (Color)", typeof(int), "Color Transition", "When the transition from 1st to 2nd values should start (x / 256)", colorTrans.m_Trans1Start, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(0, 0xff) }));
            props.AddProperty(new PropertySpec("Transition 2-3 Start (Color)", typeof(int), "Color Transition", "When the transition from 2nd to last values should start (x / 256)", colorTrans.m_Trans2Start, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(0, 0xff) }));
            props.AddProperty(new PropertySpec("Transition 2-3 End (Color)", typeof(int), "Color Transition", "When the transition from 2nd to last values should end (x / 256)", colorTrans.m_Trans2End, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(0, 0xff) }));
            props.AddProperty(new PropertySpec("Use As Options (Color)", typeof(bool), "Color Transition", "Whether the values should be assigned randomly to particles instead of having a transition", colorTrans.m_UseAsOptions, "", typeof(BoolTypeConverter)));
            props.AddProperty(new PropertySpec("Use Alternate Length (Color)", typeof(bool), "Color Transition", "Whether to base the 'percents' off the alternate length instead of the lifetime", colorTrans.m_UseAltLength, "", typeof(BoolTypeConverter)));
            props.AddProperty(new PropertySpec("Smooth Transition", typeof(bool), "Color Transition", "Whether the transitions should be smooth and not sharp", colorTrans.m_SmoothTrans, "", typeof(BoolTypeConverter)));
        }

        public static void GenerateAlphaTransitionProperties(Particle.System.Def def, List<Particle.Texture> textures, PropertyTable props)
        {
            Particle.AlphaTransition alphaTrans = def.m_AlphaTrans;
            props.AddProperty(new PropertySpec("Opacity 1", typeof(int), "Alpha Transition", "The first opacity multiplier on a scale of 0 to 31", alphaTrans.m_Start, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(0, 0x1f) }));
            props.AddProperty(new PropertySpec("Opacity 2", typeof(int), "Alpha Transition", "The second opacity multiplier on a scale of 0 to 31", alphaTrans.m_Middle, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(0, 0x1f) }));
            props.AddProperty(new PropertySpec("Opacity 3", typeof(int), "Alpha Transition", "The last opacity multiplier on a scale of 0 to 31", alphaTrans.m_End, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(0, 0x1f) }));
            props.AddProperty(new PropertySpec("Flicker", typeof(int), "Alpha Transition", "The magnitude of the flicker. Varies between 0 and 255", alphaTrans.m_Flicker, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(0, 0xff) }));
            props.AddProperty(new PropertySpec("Transition 1-2 End (Alpha)", typeof(int), "Alpha Transition", "When the transition from 1st to 2nd values should end (x / 256)", alphaTrans.m_Trans1End, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(0, 0xff) }));
            props.AddProperty(new PropertySpec("Transition 2-3 Start (Alpha)", typeof(int), "Alpha Transition", "When the transition from 2nd to last values should start (x / 256)", alphaTrans.m_Trans2Start, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(0, 0xff) }));
            props.AddProperty(new PropertySpec("Use Alternate Length (Alpha)", typeof(bool), "Alpha Transition", "Whether to base the 'percents' off the alternate length instead of the lifetime", alphaTrans.m_UseAltLength, "", typeof(BoolTypeConverter)));
        }

        public static void GenerateTextureSequenceProperties(Particle.System.Def def, List<Particle.Texture> textures, PropertyTable props)
        {
            Particle.TextureSequence texSeq = def.m_TexSeq;
            props.AddProperty(new PropertySpec("Sprite ID 1", typeof(int), "Texture Sequence", "The 1st texture's ID (-1 if not used)", textures.IndexOf(texSeq.m_Sprites[0]), "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(-1, highestTextureID) })); //TODO: Make limit depend on number of textures loaded
            props.AddProperty(new PropertySpec("Sprite ID 2", typeof(int), "Texture Sequence", "The 2nd texture's ID (-1 if not used)", textures.IndexOf(texSeq.m_Sprites[1]), "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(-1, highestTextureID) })); //TODO: Make limit depend on number of textures loaded
            props.AddProperty(new PropertySpec("Sprite ID 3", typeof(int), "Texture Sequence", "The 3rd texture's ID (-1 if not used)", textures.IndexOf(texSeq.m_Sprites[2]), "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(-1, highestTextureID) })); //TODO: Make limit depend on number of textures loaded
            props.AddProperty(new PropertySpec("Sprite ID 4", typeof(int), "Texture Sequence", "The 4th texture's ID (-1 if not used)", textures.IndexOf(texSeq.m_Sprites[3]), "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(-1, highestTextureID) })); //TODO: Make limit depend on number of textures loaded
            props.AddProperty(new PropertySpec("Sprite ID 5", typeof(int), "Texture Sequence", "The 5th texture's ID (-1 if not used)", textures.IndexOf(texSeq.m_Sprites[4]), "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(-1, highestTextureID) })); //TODO: Make limit depend on number of textures loaded
            props.AddProperty(new PropertySpec("Sprite ID 6", typeof(int), "Texture Sequence", "The 6th texture's ID (-1 if not used)", textures.IndexOf(texSeq.m_Sprites[5]), "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(-1, highestTextureID) })); //TODO: Make limit depend on number of textures loaded
            props.AddProperty(new PropertySpec("Sprite ID 7", typeof(int), "Texture Sequence", "The 7th texture's ID (-1 if not used)", textures.IndexOf(texSeq.m_Sprites[6]), "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(-1, highestTextureID) })); //TODO: Make limit depend on number of textures loaded
            props.AddProperty(new PropertySpec("Sprite ID 8", typeof(int), "Texture Sequence", "The 8th texture's ID (-1 if not used)", textures.IndexOf(texSeq.m_Sprites[7]), "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(-1, highestTextureID) })); //TODO: Make limit depend on number of textures loaded
            props.AddProperty(new PropertySpec("Number of Sprites", typeof(int), "Texture Sequence", "How many sprites to actually use", texSeq.m_NumSprites, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(0, 8) }));
            props.AddProperty(new PropertySpec("Interval", typeof(int), "Texture Sequence", "The time (x / 256) that each sprite should show up", texSeq.m_Interval, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(0, 255) }));
            props.AddProperty(new PropertySpec("Use As Options (Texture)", typeof(bool), "Texture Sequence", "Whether the values should be assigned randomly to particles instead of having a transition", texSeq.m_UseAsOptions, "", typeof(BoolTypeConverter)));
            props.AddProperty(new PropertySpec("Use Alternate Length (Texture)", typeof(bool), "Texture Sequence", "Whether to base the 'percents' off the alternate length instead of the lifetime", texSeq.m_UseAltLength, "", typeof(BoolTypeConverter)));
        }

        public static void GenerateGlitterProperties(Particle.System.Def def, List<Particle.Texture> textures, PropertyTable props)
        {
            Particle.Glitter glitter = def.m_Glitter;
            props.AddProperty(new PropertySpec("Has Effects", typeof(bool), "Glitter", "Whether the glitter particles use the same effects", glitter.m_HasEffects, "", typeof(BoolTypeConverter)));
            props.AddProperty(new PropertySpec("Has Scale Transition (Glitter)", typeof(bool), "Glitter", "Whether to transition the scale multiplier to 1.0", glitter.m_HasScaleTrans, "", typeof(BoolTypeConverter)));
            props.AddProperty(new PropertySpec("Has Alpha Transition (Glitter)", typeof(bool), "Glitter", "Whether to fade glitter particles away", glitter.m_HasAlphaTrans, "", typeof(BoolTypeConverter)));
            props.AddProperty(new PropertySpec("Angle Copy Mode", typeof(int), "Glitter", "What angle properties glitter particles should copy from their parent particles", (int)glitter.m_AngleCopyMode, "", typeof(ItemListTypeConverter), new Attribute[] { new ItemListAttribute(new string[] { "None", "Angle", "Angle and Angular Speed" }.ToList()) }));
            props.AddProperty(new PropertySpec("Follow System (Glitter)", typeof(bool), "Glitter", "Whether glitter particles should follow their parent system when it moves", glitter.m_FollowSystem, "", typeof(BoolTypeConverter)));
            props.AddProperty(new PropertySpec("Use Glitter Color", typeof(bool), "Glitter", "Whether glitter particles use the glitter color instead of the main color", glitter.m_UseGlitterColor, "", typeof(BoolTypeConverter)));
            props.AddProperty(new PropertySpec("Draw Mode (Glitter)", typeof(int), "Glitter", "How glitter particles are drawn on screen", (int)glitter.m_DrawMode, "", typeof(ItemListTypeConverter), new Attribute[] { new ItemListAttribute(new string[] { "Billboard", "Velocity Stretch", "3D" }.ToList()) }));
            props.AddProperty(new PropertySpec("Speed Randomness (Glitter)", typeof(float), "Glitter", "The maximum difference in velocity between a glitter particle and its parent", glitter.m_SpeedRand, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-64, 64, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
            props.AddProperty(new PropertySpec("Scale (Glitter)", typeof(float), "Glitter", "The initial scale if glitter particles have a scale transition", glitter.m_Scale, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-0x80000, 0x80000, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
            props.AddProperty(new PropertySpec("Lifetime (Glitter)", typeof(int), "Glitter", "The number of frames glitter particles exist for", glitter.m_Lifetime, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(0, 0xffff) }));
            props.AddProperty(new PropertySpec("Speed Multiplier", typeof(int), "Glitter", "How fast glitter particles go compared to their parent particles (x / 256)", glitter.m_SpeedMult, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(0, 0xff) }));
            props.AddProperty(new PropertySpec("Scale Multiplier", typeof(int), "Glitter", "How big glitter particles are compared to their parent particles assuming no scale transition ((x + 1) / 64)", glitter.m_ScaleMult, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(0, 0xff) }));
            props.AddProperty(new PropertySpec("Color (Glitter)", typeof(Color), "Glitter", "The glitter color of the particles", glitter.m_Color, typeof(ColorTypeEditor), typeof(ColorTypeConverter)));
            props.AddProperty(new PropertySpec("Rate (Glitter)", typeof(int), "Glitter", "The number of glitter particles each particle spawns per frame spawning particles (integer)", glitter.m_Rate, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(0, 0xff) }));
            props.AddProperty(new PropertySpec("Wait", typeof(int), "Glitter", "The fraction of a particle's lifetime that should pass before it spawns glitter ((x + 1) / 256)", glitter.m_Wait, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(0, 0xff) }));
            props.AddProperty(new PropertySpec("Period (Glitter)", typeof(int), "Glitter", "One more than the number of frames between glitter particle spawnings.", glitter.m_Period, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(1, 0xff) }));
            props.AddProperty(new PropertySpec("Sprite ID (Glitter)", typeof(int), "Glitter", "The ID of the texture to use", textures.IndexOf(glitter.m_Sprite), "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(0, highestTextureID) })); //TODO: Make limit depend on number of textures loaded
            props.AddProperty(new PropertySpec("Texture Repeat X (Glitter)", typeof(int), "Glitter", "How much to repeat the texture in the X direction", glitter.m_LogTexRepeatHorz, "", typeof(ItemListTypeConverter), new Attribute[] { new ItemListAttribute(new string[] { "1", "2", "4", "8" }.ToList()) }));
            props.AddProperty(new PropertySpec("Texture Repeat Y (Glitter)", typeof(int), "Glitter", "How much to repeat the texture in the Y direction", glitter.m_LogTexRepeatVert, "", typeof(ItemListTypeConverter), new Attribute[] { new ItemListAttribute(new string[] { "1", "2", "4", "8" }.ToList()) }));
        }

        public static void GenerateAccelerationProperties(Particle.System.Def def, List<Particle.Texture> textures, PropertyTable props)
        {
            Particle.Acceleration accel = (Particle.Acceleration)def.m_Effects.Find(x => x is Particle.Acceleration);
            props.AddProperty(new PropertySpec("Acceleration X", typeof(float), "Effect: Acceleration", "The acceleration of the particles", accel.m_Accel.X, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-64, 64, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
            props.AddProperty(new PropertySpec("Acceleration Y", typeof(float), "Effect: Acceleration", "The acceleration of the particles", accel.m_Accel.Y, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-64, 64, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
            props.AddProperty(new PropertySpec("Acceleration Z", typeof(float), "Effect: Acceleration", "The acceleration of the particles", accel.m_Accel.Z, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-64, 64, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
        }

        public static void GenerateJitterProperties(Particle.System.Def def, List<Particle.Texture> textures, PropertyTable props)
        {
            Particle.Jitter jitter = (Particle.Jitter)def.m_Effects.Find(x => x is Particle.Jitter);
            props.AddProperty(new PropertySpec("Impulse X", typeof(float), "Effect: Jitter", "The change in velocity of the particles", jitter.m_Mag.X, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-64, 64, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
            props.AddProperty(new PropertySpec("Impulse Y", typeof(float), "Effect: Jitter", "The change in velocity of the particles", jitter.m_Mag.Y, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-64, 64, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
            props.AddProperty(new PropertySpec("Impulse Z", typeof(float), "Effect: Jitter", "The change in velocity of the particles", jitter.m_Mag.Z, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-64, 64, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
            props.AddProperty(new PropertySpec("Period (Jitter)", typeof(int), "Effect: Jitter", "The period of velocity changes.", jitter.m_Period, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(1, 0xffff) }));
        }

        public static void GenerateConvergeProperties(Particle.System.Def def, List<Particle.Texture> textures, PropertyTable props)
        {
            Particle.Converge converge = (Particle.Converge)def.m_Effects.Find(x => x is Particle.Converge);
            props.AddProperty(new PropertySpec("Offset X (Converge)", typeof(float), "Effect: Converge", "The offset from system position the particles converge to", converge.m_Offset.X, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-0x400000, 0x400000, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
            props.AddProperty(new PropertySpec("Offset Y (Converge)", typeof(float), "Effect: Converge", "The offset from system position the particles converge to", converge.m_Offset.Y, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-0x400000, 0x400000, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
            props.AddProperty(new PropertySpec("Offset Z (Converge)", typeof(float), "Effect: Converge", "The offset from system position the particles converge to", converge.m_Offset.Z, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-0x400000, 0x400000, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
            props.AddProperty(new PropertySpec("Magnitude (Converge)", typeof(float), "Effect: Converge", "The magnitude of convergence", converge.m_Mag, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-8, 8, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
        }

        public static void GenerateTurnProperties(Particle.System.Def def, List<Particle.Texture> textures, PropertyTable props)
        {
            Particle.Turn turn = (Particle.Turn)def.m_Effects.Find(x => x is Particle.Turn);
            props.AddProperty(new PropertySpec("Angular Speed", typeof(int), "Effect: Turn", "The angular speed, of course", turn.m_AngleSpeed, "", typeof(AngleTypeConverter)));
            props.AddProperty(new PropertySpec("Axis", typeof(int), "Effect: Turn", "The rotation axis, which goes through the system's position", (int)turn.m_Axis, "", typeof(ItemListTypeConverter), new Attribute[] { new ItemListAttribute(new string[] { "X", "Y", "Z" }.ToList()) }));
        }

        public static void GenerateLimitPlaneProperties(Particle.System.Def def, List<Particle.Texture> textures, PropertyTable props)
        {
            Particle.LimitPlane limitPlane = (Particle.LimitPlane)def.m_Effects.Find(x => x is Particle.LimitPlane);
            props.AddProperty(new PropertySpec("Position Y (Limit Plane)", typeof(float), "Effect: Limit Plane", "The ABSOLUTE Y position of the limit plane.", limitPlane.m_PosY, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-0x400000, 0x400000, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
            props.AddProperty(new PropertySpec("Reverse Speed Multiplier", typeof(float), "Effect: Limit Plane", "The amount to multiply the speed by when a particle bounces off the limit plane.", limitPlane.m_ReverseSpeedMult, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-8, 8, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
            props.AddProperty(new PropertySpec("Behavior (Limit Plane)", typeof(int), "Effect: Limit Plane", "What should happen if a particle hits the limit plane", (int)limitPlane.m_Behavior, "", typeof(ItemListTypeConverter), new Attribute[] { new ItemListAttribute(new string[] { "Disappear", "Bounce" }.ToList()) }));
        }

        public static void GenerateRadiusConvergeProperties(Particle.System.Def def, List<Particle.Texture> textures, PropertyTable props)
        {
            Particle.RadiusConverge converge = (Particle.RadiusConverge)def.m_Effects.Find(x => x is Particle.RadiusConverge);
            props.AddProperty(new PropertySpec("Offset X (Radius Converge)", typeof(float), "Effect: Radius Converge", "The offset from system position the particles converge to", converge.m_Offset.X, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-0x400000, 0x400000, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
            props.AddProperty(new PropertySpec("Offset Y (Radius Converge)", typeof(float), "Effect: Radius Converge", "The offset from system position the particles converge to", converge.m_Offset.Y, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-0x400000, 0x400000, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
            props.AddProperty(new PropertySpec("Offset Z (Radius Converge)", typeof(float), "Effect: Radius Converge", "The offset from system position the particles converge to", converge.m_Offset.Z, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-0x400000, 0x400000, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
            props.AddProperty(new PropertySpec("Magnitude (Radius Converge)", typeof(float), "Effect: Radius Converge", "The magnitude of convergence", converge.m_Mag, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-8, 8, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
        }

        public static void GenerateProperties(Particle.System.Def def, List<Particle.Texture> textures)
        {
            PropertyTable props = new PropertyTable();
            Particle.MainInfo info = def.m_MainInfo;

            props.AddProperty(new PropertySpec("Spawn Shape", typeof(int), "General", "The shape that the particles spawn from", (int)info.m_SpawnShape, "", typeof(ItemListTypeConverter), new Attribute[] { new ItemListAttribute(new string[] { "Point", "Sphere", "Circle (Randomly)", "Circle (Evenly)", "Ball", "Disc" }.ToList()) }));
            props.AddProperty(new PropertySpec("Draw Mode", typeof(int), "General", "How the particle is drawn on screen", (int)info.m_DrawMode, "", typeof(ItemListTypeConverter), new Attribute[] { new ItemListAttribute(new string[] { "Billboard", "Velocity Stretch", "3D" }.ToList()) }));
            props.AddProperty(new PropertySpec("Plane", typeof(int), "General", "If the spawn shape is a plane (circle or disc), which axis the plane is perpendicular to", (int)info.m_Plane, "", typeof(ItemListTypeConverter), new Attribute[] { new ItemListAttribute(new string[] { "Z", "Y", "X", "System Direction" }.ToList()) }));
            props.AddProperty(new PropertySpec("Rotate", typeof(bool), "General", "Whether the particle can rotate", info.m_Rotate, "", typeof(BoolTypeConverter)));
            props.AddProperty(new PropertySpec("Random Initial Angle", typeof(bool), "General", "Whether to use a random initial angle instead of 0°", info.m_RandomInitAng, "", typeof(BoolTypeConverter)));
            props.AddProperty(new PropertySpec("Self Destruct", typeof(bool), "General", "Whether the particle system should self destruct. Usually false for long particles, usually true for short particles.", info.m_SelfDestruct, "", typeof(BoolTypeConverter)));
            props.AddProperty(new PropertySpec("Follow System", typeof(bool), "General", "Whether the particles should follow their parent system when it moves", info.m_FollowSystem, "", typeof(BoolTypeConverter)));
            props.AddProperty(new PropertySpec("Rotate Around Weird Axis", typeof(bool), "General", "If the particles are rendered in 3D, whether to rotate the particles around the weird axis <-1/sqrt(3), -1/sqrt(3), -1/sqrt(3)> instead of <0, 1, 0>.", info.m_WeirdAxis, "", typeof(BoolTypeConverter)));
            props.AddProperty(new PropertySpec("Horizontal if 3D", typeof(bool), "General", "If the particle is rendered in 3D, whether the particle should be horizontal and not vertical", info.m_HorzIf3D, "", typeof(BoolTypeConverter)));
            props.AddProperty(new PropertySpec("Rate", typeof(float), "General", "The number of particles the system spawns each frame (can be a fraction)", info.m_Rate, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-0x80000, 0x80000, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
            props.AddProperty(new PropertySpec("Distance", typeof(float), "General", "The initial maximum distance from the system's position", info.m_StartHorzDist, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-0x400000, 0x400000, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
            props.AddProperty(new PropertySpec("Direction X", typeof(float), "General", "The direction the system points (It's recommended to normalize the vector)", info.m_Dir.X, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-8, 8, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
            props.AddProperty(new PropertySpec("Direction Y", typeof(float), "General", "The direction the system points (It's recommended to normalize the vector)", info.m_Dir.Y, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-8, 8, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
            props.AddProperty(new PropertySpec("Direction Z", typeof(float), "General", "The direction the system points (It's recommended to normalize the vector)", info.m_Dir.Z, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-8, 8, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
            props.AddProperty(new PropertySpec("Color", typeof(Color), "General", "The main color of the particles", info.m_Color, typeof(ColorTypeEditor), typeof(ColorTypeConverter)));
            props.AddProperty(new PropertySpec("Horizontal Speed", typeof(float), "General", "The initial speed away from the particle system's position", info.m_HorzSpeed, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-0x400000, 0x400000, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
            props.AddProperty(new PropertySpec("Vertical Speed", typeof(float), "General", "The initial speed in the particle system's direction", info.m_VertSpeed, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-0x400000, 0x400000, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
            props.AddProperty(new PropertySpec("Scale", typeof(float), "General", "Half the side length of the particle square", info.m_Scale, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-0x400000, 0x400000, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
            props.AddProperty(new PropertySpec("Horizontal Scale Multiplier", typeof(float), "General", "What the horizontal scale is multiplied by", info.m_HorzScaleMult, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-8, 8, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
            props.AddProperty(new PropertySpec("Minimum Angular Speed", typeof(int), "General", "The minimum angular speed in degrees per frame", info.m_MinAngleSpeed, "", typeof(AngleTypeConverter)));
            props.AddProperty(new PropertySpec("Maximum Angular Speed", typeof(int), "General", "The maximum angular speed in degrees per frame", info.m_MaxAngleSpeed, "", typeof(AngleTypeConverter)));
            props.AddProperty(new PropertySpec("Frames", typeof(int), "General", "The number of frames the particle system can spawn particles", info.m_Frames, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(0, 0xffff) }));
            props.AddProperty(new PropertySpec("Lifetime", typeof(int), "General", "The number of frames the particle exists for", info.m_Lifetime, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(0, 0xffff) }));
            props.AddProperty(new PropertySpec("Scale Randomness", typeof(int), "General", "How random the scales of the particles are. Varies between 0 and 255", info.m_ScaleRand, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(0, 0xff) }));
            props.AddProperty(new PropertySpec("Lifetime Randomness", typeof(int), "General", "How random the lifetimes of the particles are. Varies between 0 and 255", info.m_LifetimeRand, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(0, 0xff) }));
            props.AddProperty(new PropertySpec("Speed Randomness", typeof(int), "General", "How random the speeds of the particles are. Varies between 0 and 255", info.m_SpeedRand, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(0, 0xff) }));
            props.AddProperty(new PropertySpec("Period", typeof(int), "General", "One more than the number of frames between particle spawnings.", info.m_SpawnPeriod, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(1, 0xff) }));
            props.AddProperty(new PropertySpec("Opacity", typeof(int), "General", "The opacity of the particles on a scale of 0 to 31", info.m_Alpha, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(0, 0x1f) }));
            props.AddProperty(new PropertySpec("Speed Falloff", typeof(int), "General", "Dictates how fast the speed should decelerate or accelerate. 128 is normal speed", info.m_SpeedFalloff, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(0, 0xff) }));
            props.AddProperty(new PropertySpec("Sprite ID", typeof(int), "General", "The ID of the texture to use", textures.IndexOf(info.m_Sprite), "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(0, highestTextureID) }));
            props.AddProperty(new PropertySpec("Alternate Length", typeof(int), "General", "An alternate number of frames to use for transitions instead of the lifetime", info.m_AltLength, "", typeof(IntTypeConverter), new Attribute[] { new IntRangeAttribute(0, 0xff) }));
            props.AddProperty(new PropertySpec("Velocity Stretch Factor", typeof(float), "General", "If the particles are velocity stretched, by how much to velocity stretch them", info.m_VelStretchFactor, "", typeof(FloatTypeConverter), new Attribute[] { new FloatRangeAttribute(-8, 8, FloatRangeAttribute.Inclusivity.INCLUDE_FIRST) }));
            props.AddProperty(new PropertySpec("Texture Repeat X", typeof(int), "General", "How much to repeat the texture in the X direction", info.m_LogTexRepeatHorz, "", typeof(ItemListTypeConverter), new Attribute[] { new ItemListAttribute(new string[] { "1", "2", "4", "8" }.ToList()) }));
            props.AddProperty(new PropertySpec("Texture Repeat Y", typeof(int), "General", "How much to repeat the texture in the Y direction", info.m_LogTexRepeatVert, "", typeof(ItemListTypeConverter), new Attribute[] { new ItemListAttribute(new string[] { "1", "2", "4", "8" }.ToList()) }));
            props.AddProperty(new PropertySpec("Has Scale Transition", typeof(bool), "General", "Whether to transition the particles' sizes", def.m_ScaleTrans != null, "", typeof(BoolTypeConverter)));
            props.AddProperty(new PropertySpec("Has Color Transition", typeof(bool), "General", "Whether to transition the particles' colors", def.m_ColorTrans != null, "", typeof(BoolTypeConverter)));
            props.AddProperty(new PropertySpec("Has Alpha Transition", typeof(bool), "General", "Whether to transition the particles' opacities", def.m_AlphaTrans != null, "", typeof(BoolTypeConverter)));
            props.AddProperty(new PropertySpec("Has Texture Sequence", typeof(bool), "General", "Whether to transition the particles' sprites", def.m_TexSeq != null, "", typeof(BoolTypeConverter)));
            props.AddProperty(new PropertySpec("Has Glitter", typeof(bool), "General", "Whether to have particles spawning particles", def.m_Glitter != null, "", typeof(BoolTypeConverter)));
            props.AddProperty(new PropertySpec("Has Acceleration Effect", typeof(bool), "General", "Whether to accelerate the particles", def.m_Effects.FindIndex(x => x is Particle.Acceleration) != -1, "", typeof(BoolTypeConverter)));
            props.AddProperty(new PropertySpec("Has Jitter Effect", typeof(bool), "General", "Whether to have the particles jitter", def.m_Effects.FindIndex(x => x is Particle.Jitter) != -1, "", typeof(BoolTypeConverter)));
            props.AddProperty(new PropertySpec("Has Converge Effect", typeof(bool), "General", "Whether the particles should converge to a point", def.m_Effects.FindIndex(x => x is Particle.Converge) != -1, "", typeof(BoolTypeConverter)));
            props.AddProperty(new PropertySpec("Has Turn Effect", typeof(bool), "General", "Whether to rotate the entire particle system", def.m_Effects.FindIndex(x => x is Particle.Turn) != -1, "", typeof(BoolTypeConverter)));
            props.AddProperty(new PropertySpec("Has Limit Plane Effect", typeof(bool), "General", "Whether to limit the particles' heights (unused and untested)", def.m_Effects.FindIndex(x => x is Particle.LimitPlane) != -1, "", typeof(BoolTypeConverter)));
            props.AddProperty(new PropertySpec("Has Radius Converge Effect", typeof(bool), "General", "Whether the particles should converge to a circle", def.m_Effects.FindIndex(x => x is Particle.RadiusConverge) != -1, "", typeof(BoolTypeConverter)));

            if (def.m_ScaleTrans != null)
                GenerateScaleTransitionProperties(def, textures, props);
            if (def.m_ColorTrans != null)
                GenerateColorTransitionProperties(def, textures, props);
            if (def.m_AlphaTrans != null)
                GenerateAlphaTransitionProperties(def, textures, props);
            if (def.m_TexSeq != null)
                GenerateTextureSequenceProperties(def, textures, props);
            if (def.m_Glitter != null)
                GenerateGlitterProperties(def, textures, props);
            if (def.m_Effects.FindIndex(x => x is Particle.Acceleration) != -1)
                GenerateAccelerationProperties(def, textures, props);
            if (def.m_Effects.FindIndex(x => x is Particle.Jitter) != -1)
                GenerateJitterProperties(def, textures, props);
            if (def.m_Effects.FindIndex(x => x is Particle.Converge) != -1)
                GenerateConvergeProperties(def, textures, props);
            if (def.m_Effects.FindIndex(x => x is Particle.Turn) != -1)
                GenerateTurnProperties(def, textures, props);
            if (def.m_Effects.FindIndex(x => x is Particle.LimitPlane) != -1)
                GenerateLimitPlaneProperties(def, textures, props);
            if (def.m_Effects.FindIndex(x => x is Particle.RadiusConverge) != -1)
                GenerateRadiusConvergeProperties(def, textures, props);

            def.m_Properties = props;
        }
    }
}
