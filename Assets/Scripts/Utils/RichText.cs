using System.Text;
using UnityEngine;

namespace ProjectB
{
    public static class RichText
    {
        private static StringBuilder str = new StringBuilder();
        private const string OPEN_TAG = "<", CLOSE_TAG = ">";
        private const string HASHTAG = "#";
        private const string COLOUR_START = "<color=", COLOUR_END = "</color>";
        private const string SIZE_START = "<size=", SIZE_END = "</size>";
        private const string BOLD_START = "<b>", BOLD_END = "</b>";
        private const string ITALICS_START = "<i>", ITALICS_END = "</i>";

        public static string InColour(string text, Color colour)
        {
            str.Clear();
            str.Append(COLOUR_START);
            str.Append(HASHTAG);
            str.Append(ColorUtility.ToHtmlStringRGBA(colour));
            str.Append(CLOSE_TAG);
            str.Append(text);
            str.Append(COLOUR_END);

            return str.ToString();
        }

        public static string InSize(string text, float size)
        {
            str.Clear();
            str.Append(SIZE_START);
            str.Append(size);
            str.Append(CLOSE_TAG);
            str.Append(text);
            str.Append(SIZE_END);

            return str.ToString();
        }

        public static string InBold(string text)
        {
            str.Clear();
            str.Append(BOLD_START);
            str.Append(text);
            str.Append(BOLD_END);

            return str.ToString();
        }

        public static string InItalics(string text)
        {
            str.Clear();
            str.Append(ITALICS_START);
            str.Append(text);
            str.Append(ITALICS_END);

            return str.ToString();
        }

        public static string Highlight(string text, string part, Color colour, bool bold = false)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            if (!text.Contains(part))
                return text;

            int start = text.IndexOf(part);
            string before = text.Substring(0, start);
            string after = text.Substring(start + part.Length);

            str.Clear();
            str.Append(before);

            if (bold)
                str.Append(BOLD_START);

            str.Append(COLOUR_START);
            str.Append(HASHTAG);
            str.Append(ColorUtility.ToHtmlStringRGBA(colour));
            str.Append(CLOSE_TAG);
            str.Append(part);
            str.Append(COLOUR_END);

            if (bold)
                str.Append(BOLD_END);

            str.Append(after);

            return str.ToString();
        }
    }
}
