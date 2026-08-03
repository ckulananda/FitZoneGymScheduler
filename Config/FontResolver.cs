using PdfSharp.Fonts;

namespace FitZoneGymScheduler.Helpers
{
    public class FontResolver : IFontResolver
    {
        public byte[]? GetFont(string faceName)
        {
            return null;
        }

        public FontResolverInfo? ResolveTypeface(
            string familyName,
            bool isBold,
            bool isItalic)
        {
            return PlatformFontResolver.ResolveTypeface(
                familyName,
                isBold,
                isItalic);
        }
    }
}