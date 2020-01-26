using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;

#if WINDOWS
using System.Windows.Forms;
#endif

namespace libfintx
{
    /// <summary>
    /// Helper object needed for entering a TAN.
    /// </summary>
    public class TANDialog
    {
#if WINDOWS
        public PictureBox PictureBox { get; }
#else
        public object PictureBox { get; }
#endif

        public bool RenderFlickerCodeAsGif { get; }

        public int FlickerWidth { get; }

        public int FlickerHeight { get; }

        public HBCIDialogResult DialogResult { get; internal set; }

        public Image<Rgba32> FlickerImage { get; internal set; }

        public Image<Rgba32> MatrixImage { get; internal set; }

        private readonly Func<TANDialog, string> _waitForTan;

        /// <summary>
        /// Render Flickercode as GIF.
        /// </summary>
        /// <param name="waitForTan"></param>
        /// <param name="dialogResult"></param>
        /// <param name="flickerImage"></param>
        /// <param name="flickerWidth"></param>
        /// <param name="flickerHeight"></param>
        public TANDialog(Func<TANDialog, string> waitForTan, int flickerWidth = 320, int flickerHeight = 120)
            : this(waitForTan)
        {
            RenderFlickerCodeAsGif = true;
            FlickerWidth = flickerWidth;
            FlickerHeight = flickerHeight;
        }

        /// <summary>
        /// Render TANCode (Flicker/Matrix) in WinForms.
        /// </summary>
        /// <param name="waitForTan"></param>
        /// <param name="dialogResult"></param>
        /// <param name="pictureBox"></param>
        public TANDialog(Func<TANDialog, string> waitForTan, object pictureBox)
            : this(waitForTan)
        {
            PictureBox = pictureBox;
        }


        /// <summary>
        /// Enter a TAN without any visual components, e.g. pushTAN or mobileTAN.
        /// </summary>
        /// <param name="waitForTan">Function which takes a </param>
        /// <param name="dialogResult"></param>
        /// <param name="matrixImage"></param>
        public TANDialog(Func<TANDialog, string> waitForTan)
        {
            _waitForTan = waitForTan;
        }

        /// <summary>
        /// Wait for the user to enter a TAN.
        /// </summary>
        /// <param name="dialogResult">The <code>HBCIDialogResult</code> from the bank which requests the TAN. Can be used to display bank messages in the dialog.</param>
        /// <returns></returns>
        internal string WaitForTAN()
        {
            return _waitForTan?.Invoke(this);
        }
    }
}