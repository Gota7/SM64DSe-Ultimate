using System;

namespace SM64DSe.core.cli.utils
{
    public class RomUriUtils
    {
        public static Uri Parse(string value)
        {
            Uri uri = new Uri(value);
            if (!IsValidAuthority(uri.Authority))
                throw new NotSupportedException($"authority {uri.Authority} is not supported.");
            if (uri.Scheme != "rom")
                throw new NotSupportedException($"Only rom scheme is supported. Provided {uri.Scheme}");
            return uri;
        }

        public static bool IsRomUri(string value)
        {
            try
            {
                Parse(value);
                return true;
            }
            catch (Exception _)
            {
                return false;
            }
        }

        static bool IsValidAuthority(string authority)
        {
            switch (authority)
            {
                case "overlay":
                case "file":
                    return true;
                default:
                    return false;
            }
        }
        
        public static string GetInternalPathFromUri(Uri uri)
        {
            string internalPath = uri.AbsolutePath;
            // The internal path do not use / to indicate root.
            if (internalPath.StartsWith("/"))
            {
                internalPath = internalPath.Substring(1);
            }

            return internalPath;
        }
    }
}