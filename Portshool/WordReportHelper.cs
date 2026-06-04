using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Portshool
{
    /// <summary>
    /// Формирует настоящий файл Word (.docx) без сторонних библиотек.
    /// Документ .docx — это zip-архив с XML внутри, поэтому используется
    /// только встроенная System.IO.Compression.
    /// </summary>
    public static class WordReportHelper
    {
        private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);

        public static void SaveStudentReport(
            string path,
            string title,
            IEnumerable<string> infoLines,
            DataTable grades,
            DataTable achievements)
        {
            string documentXml = BuildDocumentXml(title, infoLines, grades, achievements);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (ZipArchive zip = new ZipArchive(fs, ZipArchiveMode.Create))
            {
                WriteEntry(zip, "[Content_Types].xml", ContentTypesXml());
                WriteEntry(zip, "_rels/.rels", RootRelationshipsXml());
                WriteEntry(zip, "word/document.xml", documentXml);
            }
        }

        private static void WriteEntry(ZipArchive zip, string entryName, string content)
        {
            ZipArchiveEntry entry = zip.CreateEntry(entryName, CompressionLevel.Optimal);

            using (Stream stream = entry.Open())
            using (StreamWriter writer = new StreamWriter(stream, Utf8NoBom))
            {
                writer.Write(content);
            }
        }

        private static string ContentTypesXml()
        {
            return
                "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                "<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">" +
                "<Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/>" +
                "<Default Extension=\"xml\" ContentType=\"application/xml\"/>" +
                "<Override PartName=\"/word/document.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml\"/>" +
                "</Types>";
        }

        private static string RootRelationshipsXml()
        {
            return
                "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"word/document.xml\"/>" +
                "</Relationships>";
        }

        private static string BuildDocumentXml(
            string title,
            IEnumerable<string> infoLines,
            DataTable grades,
            DataTable achievements)
        {
            StringBuilder body = new StringBuilder();

            body.Append(Heading(title, 32));

            if (infoLines != null)
            {
                foreach (string line in infoLines)
                {
                    body.Append(Paragraph(line));
                }
            }

            body.Append(Paragraph(string.Empty));
            body.Append(Heading("Успеваемость", 26));
            body.Append(Table(grades));

            body.Append(Paragraph(string.Empty));
            body.Append(Heading("Достижения и активность", 26));
            body.Append(Table(achievements));

            body.Append(Paragraph(string.Empty));
            body.Append(Paragraph("Отчёт сформирован: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm")));

            return
                "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                "<w:document xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\">" +
                "<w:body>" +
                body +
                "<w:sectPr><w:pgSz w:w=\"11906\" w:h=\"16838\"/>" +
                "<w:pgMar w:top=\"1134\" w:right=\"850\" w:bottom=\"1134\" w:left=\"1134\" w:header=\"708\" w:footer=\"708\" w:gutter=\"0\"/>" +
                "</w:sectPr>" +
                "</w:body>" +
                "</w:document>";
        }

        private static string Heading(string text, int halfPointSize)
        {
            return
                "<w:p><w:pPr><w:spacing w:before=\"120\" w:after=\"80\"/></w:pPr>" +
                "<w:r><w:rPr><w:b/><w:sz w:val=\"" + halfPointSize + "\"/><w:szCs w:val=\"" + halfPointSize + "\"/></w:rPr>" +
                "<w:t xml:space=\"preserve\">" + Escape(text) + "</w:t></w:r></w:p>";
        }

        private static string Paragraph(string text)
        {
            return
                "<w:p><w:r><w:t xml:space=\"preserve\">" + Escape(text) + "</w:t></w:r></w:p>";
        }

        private static string Table(DataTable table)
        {
            if (table == null || table.Columns.Count == 0)
            {
                return Paragraph("Нет данных.");
            }

            StringBuilder sb = new StringBuilder();

            sb.Append("<w:tbl><w:tblPr><w:tblW w:w=\"0\" w:type=\"auto\"/>");
            sb.Append("<w:tblBorders>");
            foreach (string side in new[] { "top", "left", "bottom", "right", "insideH", "insideV" })
            {
                sb.Append("<w:" + side + " w:val=\"single\" w:sz=\"4\" w:space=\"0\" w:color=\"BFBFBF\"/>");
            }
            sb.Append("</w:tblBorders></w:tblPr>");

            // Заголовок таблицы
            sb.Append("<w:tr>");
            foreach (DataColumn column in table.Columns)
            {
                sb.Append(HeaderCell(column.ColumnName));
            }
            sb.Append("</w:tr>");

            // Данные
            if (table.Rows.Count == 0)
            {
                sb.Append("<w:tr>");
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    sb.Append(Cell(i == 0 ? "Нет данных" : string.Empty, false));
                }
                sb.Append("</w:tr>");
            }
            else
            {
                foreach (DataRow row in table.Rows)
                {
                    sb.Append("<w:tr>");
                    foreach (DataColumn column in table.Columns)
                    {
                        sb.Append(Cell(FormatValue(row[column]), false));
                    }
                    sb.Append("</w:tr>");
                }
            }

            sb.Append("</w:tbl>");
            return sb.ToString();
        }

        private static string HeaderCell(string text)
        {
            return
                "<w:tc><w:tcPr><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"DCE6F1\"/></w:tcPr>" +
                "<w:p><w:r><w:rPr><w:b/></w:rPr>" +
                "<w:t xml:space=\"preserve\">" + Escape(text) + "</w:t></w:r></w:p></w:tc>";
        }

        private static string Cell(string text, bool bold)
        {
            string runProps = bold ? "<w:rPr><w:b/></w:rPr>" : string.Empty;
            return
                "<w:tc><w:p><w:r>" + runProps +
                "<w:t xml:space=\"preserve\">" + Escape(text) + "</w:t></w:r></w:p></w:tc>";
        }

        private static string FormatValue(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return string.Empty;
            }

            if (value is DateTime dateValue)
            {
                return dateValue.ToString("dd.MM.yyyy");
            }

            return value.ToString();
        }

        private static string Escape(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            return text
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }
    }
}
