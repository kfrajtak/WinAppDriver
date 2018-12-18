using System;
using System.Text;

namespace CodePlex.XPathParser
{
    public class XPathParserException : System.Exception {
        public string queryString;
        public int    startChar;
        public int    endChar;

        public XPathParserException(string queryString, int startChar, int endChar, string message) : base(message) {
            this.queryString = queryString;
            this.startChar = startChar;
            this.endChar = endChar;
        }

        private enum TrimType {
            Left,
            Right,
            Middle,
        }

        // This function is used to prevent long quotations in error messages
        private static void AppendTrimmed(StringBuilder sb, string value, int startIndex, int count, TrimType trimType) {
            const int    TrimSize   = 32;
            const string TrimMarker = "...";

            if (count <= TrimSize) {
                sb.Append(value, startIndex, count);
            } else {
                switch (trimType) {
                case TrimType.Left:
                    sb.Append(TrimMarker);
                    sb.Append(value, startIndex + count - TrimSize, TrimSize);
                    break;
                case TrimType.Right:
                    sb.Append(value, startIndex, TrimSize);
                    sb.Append(TrimMarker);
                    break;
                case TrimType.Middle:
                    sb.Append(value, startIndex, TrimSize / 2);
                    sb.Append(TrimMarker);
                    sb.Append(value, startIndex + count - TrimSize / 2, TrimSize / 2);
                    break;
                }
            }
        }

        internal string MarkOutError() {
            if (queryString == null || queryString.Trim(' ').Length == 0) {
                return null;
            }

            int len = endChar - startChar;
            StringBuilder sb = new StringBuilder();

            AppendTrimmed(sb, queryString, 0, startChar, TrimType.Left);
            if (len > 0) {
                sb.Append(" -->");
                AppendTrimmed(sb, queryString, startChar, len, TrimType.Middle);
            }

            sb.Append("<-- ");
            AppendTrimmed(sb, queryString, endChar, queryString.Length - endChar, TrimType.Right);

            return sb.ToString();
        }


        private string FormatDetailedMessage() {
            string message = Message;
            string error = MarkOutError();

            if (error != null && error.Length > 0) {
                if (message.Length > 0) {
                    message += Environment.NewLine;
                }
                message += error;
            }
            return message;
        }

        public override string ToString() {
            string result = this.GetType().FullName;
            string info = FormatDetailedMessage();
            if (info != null && info.Length > 0) {
                result += ": " + info;
            }
            if (StackTrace != null) {
                result += Environment.NewLine + StackTrace;
            }
            return result;
        }

    }
}
