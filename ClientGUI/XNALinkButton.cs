using System;
using Rampastring.XNAUI;
using Rampastring.Tools;
using ClientCore;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ClientGUI
{
    public class XNALinkButton : XNAClientButton
    {
        public XNALinkButton(WindowManager windowManager) : base(windowManager) { }

        public string URL { get; set; }
        public string UnixURL { get; set; }
        public string Arguments { get; set; }

        private string _altDate;
        private DateRange? _altDateRange;

        private string _idleTextureAltPath;
        private string _hoverTextureAltPath;

        private Texture2D _idleTextureRegular;
        private Texture2D _hoverTextureRegular;

        private bool _altTexturesActive;
        private DateTime _lastCheckedDate = DateTime.MinValue;
        private string _lastCheckedAltDate;

        /// <summary>
        /// The date on which the alternative idle and hover textures are displayed
        /// instead of the regular ones, as a single date (MM-DD) or a date range (MM-DD~MM-DD).
        /// Ranges may wrap around the new year (for example 12-31~01-02).
        /// </summary>
        public string AltDate
        {
            get => _altDate;
            set
            {
                _altDate = value;
                _altDateRange = null;

                if (string.IsNullOrWhiteSpace(value))
                    return;

                if (DateRange.TryParse(value, out DateRange range))
                    _altDateRange = range;
                else
                    Logger.Log($"XNALinkButton: Invalid AltDate value \"{value}\" for control \"{Name}\". " +
                        "Expected MM-DD or MM-DD~MM-DD.");
            }
        }

        /// <summary>
        /// The idle texture that is displayed while the current date matches <see cref="AltDate"/>.
        /// </summary>
        public Texture2D IdleTextureAlt { get; set; }

        /// <summary>
        /// The hover texture that is displayed while the current date matches <see cref="AltDate"/>.
        /// </summary>
        public Texture2D HoverTextureAlt { get; set; }

        protected override void ParseControlINIAttribute(IniFile iniFile, string key, string value)
        {
            if (key == "URL")
            {
                URL = value;
                return;
            }

            if (key == "UnixURL")
            {
                UnixURL = value;
                return;
            }

            if (key == "Arguments")
            {
                Arguments = value;
                return;
            }

            if (key == "AltDate")
            {
                AltDate = value;
                return;
            }

            if (key == "IdleTextureAlt")
            {
                _idleTextureAltPath = value;
                return;
            }

            if (key == "HoverTextureAlt")
            {
                _hoverTextureAltPath = value;
                return;
            }

            base.ParseControlINIAttribute(iniFile, key, value);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            DateTime today = DateTime.Today;

            // The alternative textures are applied on the first update (when the control's
            // INI attributes have been read) and whenever the date or AltDate changes.
            if (today != _lastCheckedDate || _lastCheckedAltDate != AltDate)
            {
                _lastCheckedDate = today;
                _lastCheckedAltDate = AltDate;
                UpdateDateTextures(today);
            }
        }

        /// <summary>
        /// Makes the button use the texture set that belongs to the given date.
        /// </summary>
        private void UpdateDateTextures(DateTime date)
        {
            bool useAltTextures = _altDateRange.HasValue && _altDateRange.Value.Contains(date);

            if (useAltTextures == _altTexturesActive)
                return;

            if (useAltTextures)
            {
                if (IdleTextureAlt == null && !string.IsNullOrWhiteSpace(_idleTextureAltPath))
                    IdleTextureAlt = AssetLoader.LoadTexture(_idleTextureAltPath);

                if (HoverTextureAlt == null && !string.IsNullOrWhiteSpace(_hoverTextureAltPath))
                    HoverTextureAlt = AssetLoader.LoadTexture(_hoverTextureAltPath);

                // Remember the regular textures so that they can be restored later.
                _idleTextureRegular = IdleTexture;
                _hoverTextureRegular = HoverTexture;

                // If only one of the alternative textures has been specified, keep the
                // regular texture for the other one.
                IdleTexture = IdleTextureAlt ?? _idleTextureRegular;
                HoverTexture = HoverTextureAlt ?? _hoverTextureRegular;
            }
            else
            {
                IdleTexture = _idleTextureRegular;
                HoverTexture = _hoverTextureRegular;
            }

            _altTexturesActive = useAltTextures;
        }

        public override void Kill()
        {
            base.Kill();

            // base.Kill() disposes the textures that are currently in use, so dispose
            // the remaining texture set (the alternative or the regular one).
            DisposeIfNotInUse(IdleTextureAlt);
            DisposeIfNotInUse(HoverTextureAlt);
            DisposeIfNotInUse(_idleTextureRegular);
            DisposeIfNotInUse(_hoverTextureRegular);
        }

        private void DisposeIfNotInUse(Texture2D texture)
        {
            if (texture != null && !ReferenceEquals(texture, IdleTexture) && !ReferenceEquals(texture, HoverTexture))
                texture.Dispose();
        }

        public override void OnLeftClick(InputEventArgs inputEventArgs)
        {
            inputEventArgs.Handled = true;
            
            OSVersion osVersion = ClientConfiguration.Instance.GetOperatingSystemVersion();

            if (osVersion == OSVersion.UNIX && !string.IsNullOrEmpty(UnixURL))
                ProcessLauncher.StartShellProcess(UnixURL, Arguments);
            else if (!string.IsNullOrEmpty(URL))
                ProcessLauncher.StartShellProcess(URL, Arguments);

            base.OnLeftClick(inputEventArgs);
        }
    }
}
