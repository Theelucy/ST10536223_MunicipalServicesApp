using System.Drawing;

namespace MunicipalServicesApp
{
    /// <summary>
    /// Centralised colour palette and fonts so every form in the application
    /// shares a single, consistent visual identity (see "Consistency" under
    /// Design Considerations in the brief).
    /// </summary>
    public static class AppTheme
    {
        public static readonly Color HeaderDark = Color.FromArgb(31, 45, 61);
        public static readonly Color Primary = Color.FromArgb(41, 128, 185);
        public static readonly Color PrimaryDark = Color.FromArgb(52, 73, 94);
        public static readonly Color Success = Color.FromArgb(39, 174, 96);
        public static readonly Color Warning = Color.FromArgb(243, 156, 18);
        public static readonly Color Danger = Color.FromArgb(211, 84, 0);
        public static readonly Color Disabled = Color.FromArgb(189, 195, 199);
        public static readonly Color DisabledText = Color.FromArgb(120, 120, 120);
        public static readonly Color Background = Color.FromArgb(241, 244, 247);
        public static readonly Color CardBackground = Color.White;
        public static readonly Color TextMuted = Color.FromArgb(127, 140, 141);

        public static readonly Color TintBlue = Color.FromArgb(232, 244, 253);
        public static readonly Color TintOrange = Color.FromArgb(253, 245, 230);
        public static readonly Color TintGreen = Color.FromArgb(230, 250, 239);

        public static readonly Font FontTitle = new Font("Segoe UI", 18F, FontStyle.Bold);
        public static readonly Font FontSubtitle = new Font("Segoe UI", 10F);
        public static readonly Font FontSectionHeader = new Font("Segoe UI", 12F, FontStyle.Bold);
        public static readonly Font FontCardValue = new Font("Segoe UI", 22F, FontStyle.Bold);
        public static readonly Font FontCardCaption = new Font("Segoe UI", 9F);
        public static readonly Font FontBody = new Font("Segoe UI", 10F);
        public static readonly Font FontBodyBold = new Font("Segoe UI", 10F, FontStyle.Bold);
        public static readonly Font FontButton = new Font("Segoe UI", 11F, FontStyle.Bold);
    }
}
