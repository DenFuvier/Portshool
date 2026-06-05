using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Portshool
{
    public static class ExcelReportHelper
    {
        private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);

        private const int StyleNormal      = 0;
        private const int StyleTitle       = 1;
        private const int StyleSection     = 2;
        private const int StyleTableHeader = 3;
        private const int StyleTableData   = 4;

        // ---------------------------------------------------------------
        // Public entry point
        // ---------------------------------------------------------------

        public static void SaveStudentReport(
            string path,
            string title,
            IEnumerable<string> infoLines,
            DataTable grades,
            DataTable achievements)
        {
            if (File.Exists(path))
                File.Delete(path);

            var imagePlacements = new List<ImagePlacement>();
            List<RowData> rows = BuildRows(title, infoLines, grades, achievements, imagePlacements);

            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (ZipArchive zip = new ZipArchive(fs, ZipArchiveMode.Create))
            {
                bool hasImages = imagePlacements.Count > 0;

                WriteEntry(zip, "[Content_Types].xml",        ContentTypesXml(imagePlacements));
                WriteEntry(zip, "_rels/.rels",                RootRelsXml());
                WriteEntry(zip, "xl/workbook.xml",            WorkbookXml());
                WriteEntry(zip, "xl/_rels/workbook.xml.rels", WorkbookRelsXml());
                WriteEntry(zip, "xl/worksheets/sheet1.xml",   BuildSheetXml(rows, imagePlacements));
                WriteEntry(zip, "xl/styles.xml",              StylesXml());

                if (hasImages)
                {
                    for (int i = 0; i < imagePlacements.Count; i++)
                    {
                        string mediaEntry = "xl/media/image" + (i + 1) + imagePlacements[i].Extension;
                        WriteEntryBytes(zip, mediaEntry, File.ReadAllBytes(imagePlacements[i].FilePath));
                    }

                    WriteEntry(zip, "xl/drawings/drawing1.xml",
                        BuildDrawingXml(imagePlacements));
                    WriteEntry(zip, "xl/drawings/_rels/drawing1.xml.rels",
                        BuildDrawingRelsXml(imagePlacements));
                    WriteEntry(zip, "xl/worksheets/_rels/sheet1.xml.rels",
                        SheetRelsXml());
                }
            }
        }

        // ---------------------------------------------------------------
        // Row building
        // ---------------------------------------------------------------

        private static List<RowData> BuildRows(
            string title,
            IEnumerable<string> infoLines,
            DataTable grades,
            DataTable achievements,
            List<ImagePlacement> imagePlacements)
        {
            var rows = new List<RowData>();

            rows.Add(RowData.Single(title, StyleTitle));
            rows.Add(RowData.Empty());

            if (infoLines != null)
                foreach (string line in infoLines)
                    rows.Add(RowData.Single(line, StyleNormal));

            rows.Add(RowData.Empty());
            rows.Add(RowData.Single("Успеваемость", StyleSection));
            AddTableRows(rows, grades);

            rows.Add(RowData.Empty());
            rows.Add(RowData.Single("Достижения и активность", StyleSection));
            AddAchievementRows(rows, achievements, imagePlacements);

            rows.Add(RowData.Empty());
            rows.Add(RowData.Single(
                "Отчёт сформирован: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
                StyleNormal));

            return rows;
        }

        private static void AddTableRows(List<RowData> rows, DataTable table)
        {
            if (table == null || table.Columns.Count == 0)
            {
                rows.Add(RowData.Single("Нет данных", StyleNormal));
                return;
            }

            var header = new RowData();
            foreach (DataColumn col in table.Columns)
                header.Add(col.ColumnName, StyleTableHeader);
            rows.Add(header);

            if (table.Rows.Count == 0)
            {
                var emptyRow = new RowData();
                emptyRow.Add("Нет данных", StyleTableData);
                for (int i = 1; i < table.Columns.Count; i++)
                    emptyRow.Add(string.Empty, StyleTableData);
                rows.Add(emptyRow);
                return;
            }

            foreach (DataRow dr in table.Rows)
            {
                var dataRow = new RowData();
                foreach (DataColumn col in table.Columns)
                    dataRow.Add(FormatValue(dr[col]), StyleTableData);
                rows.Add(dataRow);
            }
        }

        // Achievements table: skips the hidden 'ФотоПуть' column in text output
        // and records ImagePlacement entries for rows that have an attached photo.
        private static void AddAchievementRows(
            List<RowData> rows,
            DataTable table,
            List<ImagePlacement> imagePlacements)
        {
            if (table == null || table.Columns.Count == 0)
            {
                rows.Add(RowData.Single("Нет данных", StyleNormal));
                return;
            }

            // Locate the ФотоПуть column and the rendered position of the Фото column.
            int photoPathColOrdinal = -1;
            int photoRenderedColIdx = -1;
            int rendered = 0;
            for (int i = 0; i < table.Columns.Count; i++)
            {
                string name = table.Columns[i].ColumnName;
                if (name == "ФотоПуть")
                {
                    photoPathColOrdinal = i;
                    continue;
                }
                if (name == "Фото")
                    photoRenderedColIdx = rendered;
                rendered++;
            }

            // Header
            var header = new RowData();
            foreach (DataColumn col in table.Columns)
            {
                if (col.ColumnName == "ФотоПуть") continue;
                header.Add(col.ColumnName, StyleTableHeader);
            }
            rows.Add(header);

            if (table.Rows.Count == 0)
            {
                var emptyRow = new RowData();
                bool first = true;
                foreach (DataColumn col in table.Columns)
                {
                    if (col.ColumnName == "ФотоПуть") continue;
                    emptyRow.Add(first ? "Нет данных" : string.Empty, StyleTableData);
                    first = false;
                }
                rows.Add(emptyRow);
                return;
            }

            foreach (DataRow dr in table.Rows)
            {
                string photoPath = photoPathColOrdinal >= 0
                    ? dr[photoPathColOrdinal]?.ToString()
                    : null;
                bool hasPhoto = !string.IsNullOrEmpty(photoPath)
                    && photoPath != "—"
                    && File.Exists(photoPath);

                var dataRow = new RowData();
                foreach (DataColumn col in table.Columns)
                {
                    if (col.ColumnName == "ФотоПуть") continue;
                    // Leave the Фото cell blank — the image will be placed there.
                    if (col.ColumnName == "Фото" && hasPhoto)
                        dataRow.Add(string.Empty, StyleTableData);
                    else
                        dataRow.Add(FormatValue(dr[col]), StyleTableData);
                }

                // rows.Count is the 0-based index this row will occupy after Add.
                if (hasPhoto && photoRenderedColIdx >= 0)
                {
                    imagePlacements.Add(new ImagePlacement
                    {
                        RowIndex = rows.Count,
                        ColIndex = photoRenderedColIdx,
                        FilePath = photoPath,
                        Extension = Path.GetExtension(photoPath)
                    });
                }

                rows.Add(dataRow);
            }
        }

        // ---------------------------------------------------------------
        // Sheet XML
        // ---------------------------------------------------------------

        private static string BuildSheetXml(List<RowData> rows, List<ImagePlacement> imagePlacements)
        {
            bool hasImages = imagePlacements.Count > 0;

            var imageRows = new HashSet<int>();
            foreach (var p in imagePlacements)
                imageRows.Add(p.RowIndex);

            var sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");

            if (hasImages)
                sb.Append("<worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" " +
                          "xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">");
            else
                sb.Append("<worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\">");

            if (hasImages)
                sb.Append("<sheetFormatPr defaultRowHeight=\"15\"/>");

            sb.Append("<sheetData>");

            for (int rowIdx = 0; rowIdx < rows.Count; rowIdx++)
            {
                RowData row = rows[rowIdx];
                if (row.Count == 0) continue;

                if (imageRows.Contains(rowIdx))
                    sb.Append("<row r=\"" + (rowIdx + 1) + "\" ht=\"80\" customHeight=\"1\">");
                else
                    sb.Append("<row r=\"" + (rowIdx + 1) + "\">");

                for (int colIdx = 0; colIdx < row.Count; colIdx++)
                {
                    string cellRef = ColLetter(colIdx) + (rowIdx + 1);
                    sb.Append("<c r=\"" + cellRef + "\" t=\"inlineStr\" s=\"" + row.Styles[colIdx] + "\">");
                    sb.Append("<is><t>" + Escape(row.Values[colIdx]) + "</t></is>");
                    sb.Append("</c>");
                }
                sb.Append("</row>");
            }

            sb.Append("</sheetData>");

            if (hasImages)
                sb.Append("<drawing r:id=\"rId1\"/>");

            sb.Append("</worksheet>");
            return sb.ToString();
        }

        // ---------------------------------------------------------------
        // Drawing XML (images anchored to cells)
        // ---------------------------------------------------------------

        private static string BuildDrawingXml(List<ImagePlacement> placements)
        {
            var sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
            sb.Append("<xdr:wsDr " +
                      "xmlns:xdr=\"http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing\" " +
                      "xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" " +
                      "xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">");

            for (int i = 0; i < placements.Count; i++)
            {
                var p = placements[i];
                int drawRow = p.RowIndex;   // 0-based in drawing coords
                int drawCol = p.ColIndex;
                string rId = "rId" + (i + 1);

                sb.Append("<xdr:twoCellAnchor editAs=\"oneCell\">");
                sb.Append("<xdr:from>");
                sb.Append("<xdr:col>" + drawCol + "</xdr:col><xdr:colOff>76200</xdr:colOff>");
                sb.Append("<xdr:row>" + drawRow + "</xdr:row><xdr:rowOff>76200</xdr:rowOff>");
                sb.Append("</xdr:from>");
                sb.Append("<xdr:to>");
                sb.Append("<xdr:col>" + (drawCol + 1) + "</xdr:col><xdr:colOff>0</xdr:colOff>");
                sb.Append("<xdr:row>" + (drawRow + 1) + "</xdr:row><xdr:rowOff>0</xdr:rowOff>");
                sb.Append("</xdr:to>");
                sb.Append("<xdr:pic>");
                sb.Append("<xdr:nvPicPr>");
                sb.Append("<xdr:cNvPr id=\"" + (i + 2) + "\" name=\"Photo" + (i + 1) + "\"/>");
                sb.Append("<xdr:cNvPicPr/>");
                sb.Append("</xdr:nvPicPr>");
                sb.Append("<xdr:blipFill>");
                sb.Append("<a:blip r:embed=\"" + rId + "\"/>");
                sb.Append("<a:stretch><a:fillRect/></a:stretch>");
                sb.Append("</xdr:blipFill>");
                sb.Append("<xdr:spPr>");
                sb.Append("<a:xfrm><a:off x=\"0\" y=\"0\"/><a:ext cx=\"1\" cy=\"1\"/></a:xfrm>");
                sb.Append("<a:prstGeom prst=\"rect\"><a:avLst/></a:prstGeom>");
                sb.Append("</xdr:spPr>");
                sb.Append("</xdr:pic>");
                sb.Append("<xdr:clientData/>");
                sb.Append("</xdr:twoCellAnchor>");
            }

            sb.Append("</xdr:wsDr>");
            return sb.ToString();
        }

        private static string BuildDrawingRelsXml(List<ImagePlacement> placements)
        {
            var sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
            sb.Append("<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">");

            for (int i = 0; i < placements.Count; i++)
            {
                string rId = "rId" + (i + 1);
                string ext = placements[i].Extension;
                sb.Append("<Relationship Id=\"" + rId + "\" " +
                          "Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/image\" " +
                          "Target=\"../media/image" + (i + 1) + ext + "\"/>");
            }

            sb.Append("</Relationships>");
            return sb.ToString();
        }

        private static string SheetRelsXml()
        {
            return
                "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                "<Relationship Id=\"rId1\" " +
                "Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/drawing\" " +
                "Target=\"../drawings/drawing1.xml\"/>" +
                "</Relationships>";
        }

        // ---------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------

        private static string FormatValue(object value)
        {
            if (value == null || value == DBNull.Value)
                return string.Empty;
            if (value is DateTime dt)
                return dt.ToString("dd.MM.yyyy");
            return value.ToString();
        }

        private static string ColLetter(int colIndex)
        {
            string result = string.Empty;
            colIndex++;
            while (colIndex > 0)
            {
                colIndex--;
                result = (char)('A' + colIndex % 26) + result;
                colIndex /= 26;
            }
            return result;
        }

        private static string Escape(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            return text
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;");
        }

        private static void WriteEntry(ZipArchive zip, string entryName, string content)
        {
            ZipArchiveEntry entry = zip.CreateEntry(entryName, CompressionLevel.Optimal);
            using (Stream stream = entry.Open())
            using (var writer = new StreamWriter(stream, Utf8NoBom))
                writer.Write(content);
        }

        private static void WriteEntryBytes(ZipArchive zip, string entryName, byte[] data)
        {
            ZipArchiveEntry entry = zip.CreateEntry(entryName, CompressionLevel.Optimal);
            using (Stream stream = entry.Open())
                stream.Write(data, 0, data.Length);
        }

        // ---------------------------------------------------------------
        // Package XML parts
        // ---------------------------------------------------------------

        private static string ContentTypesXml(List<ImagePlacement> placements)
        {
            var sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
            sb.Append("<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">");
            sb.Append("<Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/>");
            sb.Append("<Default Extension=\"xml\" ContentType=\"application/xml\"/>");

            var addedExts = new HashSet<string>();
            foreach (var p in placements)
            {
                string ext = p.Extension.ToLower().TrimStart('.');
                if (addedExts.Add(ext))
                {
                    string mime = ext == "png" ? "image/png" : "image/jpeg";
                    sb.Append("<Default Extension=\"" + ext + "\" ContentType=\"" + mime + "\"/>");
                }
            }

            sb.Append("<Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/>");
            sb.Append("<Override PartName=\"/xl/worksheets/sheet1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/>");
            sb.Append("<Override PartName=\"/xl/styles.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml\"/>");

            if (placements.Count > 0)
                sb.Append("<Override PartName=\"/xl/drawings/drawing1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.drawing+xml\"/>");

            sb.Append("</Types>");
            return sb.ToString();
        }

        private static string RootRelsXml()
        {
            return
                "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                "<Relationship Id=\"rId1\" " +
                "Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" " +
                "Target=\"xl/workbook.xml\"/>" +
                "</Relationships>";
        }

        private static string WorkbookXml()
        {
            return
                "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                "<workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" " +
                "xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">" +
                "<sheets>" +
                "<sheet name=\"Портфолио\" sheetId=\"1\" r:id=\"rId1\"/>" +
                "</sheets>" +
                "</workbook>";
        }

        private static string WorkbookRelsXml()
        {
            return
                "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                "<Relationship Id=\"rId1\" " +
                "Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" " +
                "Target=\"worksheets/sheet1.xml\"/>" +
                "<Relationship Id=\"rId2\" " +
                "Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles\" " +
                "Target=\"styles.xml\"/>" +
                "</Relationships>";
        }

        private static string StylesXml()
        {
            return
                "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                "<styleSheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\">" +
                "<fonts count=\"3\">" +
                "<font><sz val=\"11\"/><name val=\"Calibri\"/></font>" +
                "<font><b/><sz val=\"16\"/><color rgb=\"FF1F3A6E\"/><name val=\"Calibri\"/></font>" +
                "<font><b/><sz val=\"12\"/><color rgb=\"FF1F3A6E\"/><name val=\"Calibri\"/></font>" +
                "</fonts>" +
                "<fills count=\"3\">" +
                "<fill><patternFill patternType=\"none\"/></fill>" +
                "<fill><patternFill patternType=\"gray125\"/></fill>" +
                "<fill><patternFill patternType=\"solid\"><fgColor rgb=\"FFDCE6F1\"/></patternFill></fill>" +
                "</fills>" +
                "<borders count=\"2\">" +
                "<border><left/><right/><top/><bottom/><diagonal/></border>" +
                "<border>" +
                "<left style=\"thin\"><color rgb=\"FFBFBFBF\"/></left>" +
                "<right style=\"thin\"><color rgb=\"FFBFBFBF\"/></right>" +
                "<top style=\"thin\"><color rgb=\"FFBFBFBF\"/></top>" +
                "<bottom style=\"thin\"><color rgb=\"FFBFBFBF\"/></bottom>" +
                "<diagonal/></border>" +
                "</borders>" +
                "<cellStyleXfs count=\"1\"><xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\"/></cellStyleXfs>" +
                "<cellXfs count=\"5\">" +
                "<xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\" xfId=\"0\"/>" +
                "<xf numFmtId=\"0\" fontId=\"1\" fillId=\"0\" borderId=\"0\" xfId=\"0\" applyFont=\"1\"/>" +
                "<xf numFmtId=\"0\" fontId=\"2\" fillId=\"0\" borderId=\"0\" xfId=\"0\" applyFont=\"1\"/>" +
                "<xf numFmtId=\"0\" fontId=\"2\" fillId=\"2\" borderId=\"1\" xfId=\"0\" applyFont=\"1\" applyFill=\"1\" applyBorder=\"1\"/>" +
                "<xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"1\" xfId=\"0\" applyBorder=\"1\"/>" +
                "</cellXfs>" +
                "<cellStyles count=\"1\"><cellStyle name=\"Normal\" xfId=\"0\" builtinId=\"0\"/></cellStyles>" +
                "</styleSheet>";
        }

        // ---------------------------------------------------------------
        // Inner types
        // ---------------------------------------------------------------

        private struct ImagePlacement
        {
            public int    RowIndex;   // 0-based index in rows list (= drawing row coord)
            public int    ColIndex;   // 0-based column index in sheet
            public string FilePath;
            public string Extension;  // e.g. ".jpg" or ".png"
        }

        private class RowData
        {
            public readonly List<string> Values = new List<string>();
            public readonly List<int>    Styles = new List<int>();

            public int Count => Values.Count;

            public void Add(string value, int style)
            {
                Values.Add(value ?? string.Empty);
                Styles.Add(style);
            }

            public static RowData Single(string value, int style)
            {
                var r = new RowData();
                r.Add(value, style);
                return r;
            }

            public static RowData Empty() => new RowData();
        }
    }
}
