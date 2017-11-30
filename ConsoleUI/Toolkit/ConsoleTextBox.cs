using System;
using System.Collections.Generic;

namespace CKAN.ConsoleUI.Toolkit {

    /// <summary>
    /// Object displaying a long screen in a big box
    /// </summary>
    public class ConsoleTextBox : ScreenObject {

        /// <summary>
        /// Initialize the text box
        /// </summary>
        /// <param name="l">X coordinate of left edge</param>
        /// <param name="t">Y coordinate of top edge</param>
        /// <param name="r">X coordinate of right edge</param>
        /// <param name="b">Y coordinate of bottom edge</param>
        /// <param name="autoScroll">If true, keep the bottommost row visible, else keep the topmost row visible</param>
        /// <param name="ta">Alignment of the contents</param>
        /// <param name="bgFunc">Function returning the background color for the text</param>
        /// <param name="fgFunc">Function returning the foreground color for the text</param>
        public ConsoleTextBox(
                int l, int t, int r, int b,
                bool autoScroll = true,
                TextAlign ta = TextAlign.Left,
                Func<ConsoleColor> bgFunc = null,
                Func<ConsoleColor> fgFunc = null)
            : base(l, t, r, b)
        {
            scrollToBottom = autoScroll;
            align          = ta;
            getFgColor     = fgFunc;
            getBgColor     = bgFunc;
        }

        /// <summary>
        /// Add a line to the text box
        /// </summary>
        /// <param name="line">String to add</param>
        public void AddLine(string line)
        {
            lines.AddRange(Formatting.WordWrap(line, GetRight() - GetLeft() + 1));
            if (scrollToBottom) {
                ScrollToBottom();
            } else {
                // No auto-scrolling
            }
        }

        /// <summary>
        /// Scroll the text box to the top
        /// </summary>
        public void ScrollToTop()
        {
            topLine = 0;
        }

        /// <summary>
        /// Scroll the text box to the bottom
        /// </summary>
        public void ScrollToBottom()
        {
            int h   = GetBottom() - GetTop() + 1;
            topLine = lines.Count - h;
        }

        /// <summary>
        /// Scroll the text box up one page
        /// </summary>
        public void ScrollUp()
        {
            int h   =  GetBottom() - GetTop() + 1;
            topLine -= h;
            if (topLine < 0) {
                topLine = 0;
            }
        }

        /// <summary>
        /// Scroll the text box down one page
        /// </summary>
        public void ScrollDown()
        {
            int h   =  GetBottom() - GetTop() + 1;
            if (topLine +  h <= lines.Count - h) {
                topLine += h;
            } else {
                ScrollToBottom();
            }
        }

        /// <summary>
        /// Draw the text box
        /// </summary>
        /// <param name="focused">Framework parameter not relevant to this object</param>
        public override void Draw(bool focused)
        {
            int l     = GetLeft();
            int h     = GetBottom() - GetTop() + 1;
            int index = lines.Count < h ? 0 : topLine;
            // Chop one col off the right if we need a scrollbar
            int w     = GetRight() - l + 1 + (lines.Count > h ? -1 : 0);

            if (getBgColor != null) {
                Console.BackgroundColor = getBgColor();
            } else {
                Console.BackgroundColor = ConsoleTheme.Current.TextBoxBg;
            }
            if (getFgColor != null) {
                Console.ForegroundColor = getFgColor();
            } else {
                Console.ForegroundColor = ConsoleTheme.Current.TextBoxFg;
            }
            for (int y = GetTop(); y <= GetBottom(); ++y, ++index) {
                Console.SetCursorPosition(l, y);
                if (index < lines.Count) {
                    switch (align) {
                        case TextAlign.Left:
                            Console.Write(lines[index].PadRight(w));
                            break;
                        case TextAlign.Center:
                            Console.Write(ScreenObject.PadCenter(lines[index], w));
                            break;
                        case TextAlign.Right:
                            Console.Write(lines[index].PadLeft(w));
                            break;
                    }
                } else {
                    Console.Write("".PadRight(w));
                }
            }

            // Scrollbar
            if (lines.Count > h) {
                DrawScrollbar(
                    GetRight(), GetTop(), GetBottom(),
                    GetTop() + 1 + (h - 3) * topLine / (lines.Count - h)
                );
            }
        }

        /// <summary>
        /// Tell the container we can't receive focus
        /// </summary>
        public override bool Focusable() { return false; }

        private bool         scrollToBottom;
        private int          topLine;
        private TextAlign    align;
        private List<string> lines = new List<string>();
        private Func<ConsoleColor> getBgColor;
        private Func<ConsoleColor> getFgColor;
    }

    /// <summary>
    /// Alignment of text box
    /// </summary>
    public enum TextAlign {
        /// <summary>
        /// Left aligned, padding on right
        /// </summary>
        Left,
        /// <summary>
        /// Centered, padding on both left and right
        /// </summary>
        Center,
        /// <summary>
        /// Right aligned, padding on left
        /// </summary>
        Right
    }

}
