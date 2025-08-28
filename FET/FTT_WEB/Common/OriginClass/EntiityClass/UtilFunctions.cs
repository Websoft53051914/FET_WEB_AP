using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace FTT_WEB.Common.OriginClass.EntiityClass
{
    public class UtilFunctions : IDisposable
    {
        private bool IsDisposed = false;

        private Container components = null;

        ~UtilFunctions()
        {
            Trace.WriteLine("Destructor UtilFunctions Class.");
            Dispose(Disposing: false);
        }

        public void Dispose()
        {
            Dispose(Disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool Disposing)
        {
            if (!IsDisposed && Disposing && components != null)
            {
                components.Dispose();
            }

            IsDisposed = true;
            Trace.WriteLine("Dispose UtilFunctions Class.");
        }

        public int FindIndex(string[] arrayString, string sString, bool useCase)
        {
            for (int i = 0; i < arrayString.Length; i++)
            {
                if (useCase)
                {
                    if (arrayString[i] == sString)
                    {
                        return i;
                    }
                }
                else if (arrayString[i].ToUpper() == sString.ToUpper())
                {
                    return i;
                }
            }

            return -1;
        }

        public string Left(string sString, int iLength)
        {
            if (iLength >= sString.Length)
            {
                return sString;
            }

            return sString.Substring(0, iLength);
        }

        public string Right(string sString, int iLength)
        {
            if (iLength >= sString.Length)
            {
                return sString;
            }

            return sString.Substring(sString.Length - iLength);
        }

        public string FilterCharacter(string sString, string[] sCharacter, string[] sReplaced)
        {
            int num = 0;
            foreach (string oldValue in sCharacter)
            {
                sString = sString.Replace(oldValue, sReplaced[num++]);
            }

            return sString;
        }

        public string RemoveDuplcate(string sData)
        {
            string text = ",";
            string[] array = sData.Split(',');
            string[] array2 = array;
            foreach (string text2 in array2)
            {
                if (text2 != "" && text.IndexOf("," + text2 + ",") == -1)
                {
                    text = text + text2 + ",";
                }
            }

            if (Left(text, 1) == ",")
            {
                text = text.Substring(1);
            }

            if (Right(text, 1) == ",")
            {
                text = text.Substring(0, text.Length - 1);
            }

            return text;
        }

        public string RemoveDuplcate(string sData, char cSymbol)
        {
            string text = cSymbol.ToString();
            string[] array = sData.Split(cSymbol);
            string[] array2 = array;
            foreach (string text2 in array2)
            {
                if (text2 != "" && text.IndexOf(cSymbol + text2 + cSymbol) == -1)
                {
                    text = text + text2 + cSymbol;
                }
            }

            if (Left(text, 1) == cSymbol.ToString())
            {
                text = text.Substring(1);
            }

            if (Right(text, 1) == cSymbol.ToString())
            {
                text = text.Substring(0, text.Length - 1);
            }

            return text;
        }

        //public HtmlTable TableGenerator(DataTable dataTable)
        //{
        //    HtmlTable htmlTable = new HtmlTable();
        //    HtmlTableRow htmlTableRow = new HtmlTableRow();
        //    int num = 0;
        //    htmlTable.CellPadding = 1;
        //    htmlTable.CellSpacing = 1;
        //    htmlTableRow.Attributes.Add("class", "Trodd");
        //    num++;
        //    foreach (DataColumn column in dataTable.Columns)
        //    {
        //        HtmlTableCell htmlTableCell = new HtmlTableCell();
        //        htmlTableCell.Controls.Add(new LiteralControl(column.ColumnName));
        //        htmlTableRow.Cells.Add(htmlTableCell);
        //    }

        //    htmlTable.Rows.Add(htmlTableRow);
        //    foreach (DataRow row in dataTable.Rows)
        //    {
        //        htmlTableRow = new HtmlTableRow();
        //        if (num++ % 2 == 1)
        //        {
        //            htmlTableRow.Attributes.Add("class", "Treven");
        //        }
        //        else
        //        {
        //            htmlTableRow.Attributes.Add("class", "Trodd");
        //        }

        //        foreach (DataColumn column2 in dataTable.Columns)
        //        {
        //            HtmlTableCell htmlTableCell2 = new HtmlTableCell();
        //            htmlTableCell2.Controls.Add(new LiteralControl(row[column2].ToString()));
        //            htmlTableRow.Cells.Add(htmlTableCell2);
        //        }

        //        htmlTable.Rows.Add(htmlTableRow);
        //    }

        //    return htmlTable;
        //}

        public string ShowMessage(string Message)
        {
            return "<Script Language=\"JavaScript\">window.alert(\"" + Message + "\");</Script>";
        }

        //public void ShowMessage(string message, ClientScriptManager clientScript, Type clientScriptType)
        //{
        //    if (!clientScript.IsClientScriptBlockRegistered(clientScriptType, "ProgressSuccess"))
        //    {
        //        StringBuilder stringBuilder = new StringBuilder();
        //        stringBuilder.Append("<script type=text/javascript>");
        //        stringBuilder.Append("alert(\"" + message + "\");");
        //        stringBuilder.Append("</script>");
        //        clientScript.RegisterClientScriptBlock(clientScriptType, "ProgressSuccess", stringBuilder.ToString(), addScriptTags: false);
        //    }
        //}

        public string ShowMessageTable(string Message)
        {
            return "<table class=\"MsgTable\">\n\t<tr><td colspan=\"2\" class=\"SeparatorMsgTable\"></td></tr>\n\t<tr>\n\t\t<td class=\"ImageMsgTable\">&nbsp;</td>\n\t\t<td>\n" + Message + "\t\t</td>\n\t</tr>\n\t<tr><td colspan=\"2\" class=\"SeparatorMsgTable\"></td></tr>\n</table>";
        }
    }
}
