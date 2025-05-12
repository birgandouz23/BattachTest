using BattachApp.Controllers;
using HtmlRendererCore.PdfSharp;
using IronBarCode;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel.Visitors;
using MigraDoc.Rendering;
using PdfSharp;
using PdfSharp.Pdf;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Policy;

namespace BattachApp.Models
{
    public class Test
    {
        public IFormFile file_name { get; set; }
        public string name { get; set; }
        public PdfDocument GetInvoice()
        {
            var document = new Document();

            BuildDocument(document);

            var pdfRenderer = new PdfDocumentRenderer();
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();
            return pdfRenderer.PdfDocument;
        }
        private void BuildDocument(Document document)
        {
            //Section section = document.AddSection();
            //var para = section.AddParagraph();
            //para.AddText("Abdelkabir Battach Developer");
            //para.AddLineBreak();
            //para.AddText("I'm From Morocco, I'm Living In Agadir Right Now");
            //para.AddLineBreak();
            //para.AddLineBreak();
            //para.AddText("Your Welcome To My Service "+ DateTime.Now.ToString());


            //document = new Document();
            document.Info.Title = "DOCUMENT TITLE";
            document.Info.Subject = "DOCUMENT SUBJECT";
            document.Info.Author = "Me";

            // Get the predefined style Normal.
            Style style; //Creates style variable so we can change the different styles in the document as seen below..
            style = document.Styles["Normal"]; //The normal style = default for everything
            style.Font.Name = "Times New Roman"; //Default normal Font Type to Times New Roman

            style = document.Styles[StyleNames.Header]; //Style for Header of document
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = document.Styles[StyleNames.Footer]; //Style for footer of document
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called Table based on style Normal
            style = document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Times New Roman";
            style.Font.Size = 9;

            // Create a new style called Reference based on style Normal
            style = document.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);

            // Create a new style called TextBox based on style Normal
            style = document.Styles.AddStyle("TextBox", "Normal");
            style.ParagraphFormat.Borders.Width = 2.5;
            style.ParagraphFormat.Borders.Distance = "3pt";
            style.ParagraphFormat.Shading.Color = Colors.SkyBlue;

            // Each MigraDoc document needs at least one section.
            Section section = document.AddSection();

            // Create TextFrame to store the text at the top (inside the borderFrame)
            TextFrame addressFrame;
            addressFrame = section.AddTextFrame(); //add this TextFrame to our section in the document
            addressFrame.Height = "1.5cm";  //12 Pt Font  = 0.5cm  so 3 lines = 1.5cm
            addressFrame.Width = "14cm"; //16 cm width - 1 inch left indention - 1 inch right indention = 14cm
            addressFrame.Left = "1cm";
            addressFrame.Top = "1.0cm";
            addressFrame.RelativeVertical = RelativeVertical.Page;
            addressFrame.LineFormat.Width = "1pt"; //Border pixel width = 1pt
            addressFrame.LineFormat.Color = Colors.Red; //Border color = black

            Paragraph addressinfo = addressFrame.AddParagraph(); //Add paragraph to addressFrame to store text
            addressinfo.Format.Alignment = ParagraphAlignment.Center; //Align paragraph in center
            addressinfo.AddFormattedText("Abdelkabir\n", TextFormat.Underline);
            addressinfo.AddFormattedText("Battach\n", TextFormat.Bold);
            //addressinfo.AddFormattedText("LINE 3 EX.\n", TextFormat.Bold);
            addressinfo.Format.Font.Name = "Times New Roman";
            addressinfo.Format.Font.Size = 12;
            addressinfo.Format.SpaceAfter = 0;

            Paragraph Spacing = section.AddParagraph(); //Space between top and datagrid
            Spacing.Format.SpaceAfter = "4cm";

            /**************************************************************************************
             *                                  TABLE LOGIC BELOW
             *                                                                                   */
            //Next is all the crap for the table below
            Table table = section.AddTable();
            table.Style = "Table"; //Use the table style we created above
            //table.Borders.Color = new Color(81, 125, 192); //Red, Green, Blue
            table.Borders.Width = 0.25;
            table.Borders.Left.Width = 0.5;
            table.Borders.Right.Width = 0.5;
            table.Rows.LeftIndent = 0;

            decimal tableWidth = 0.0M; //Used so we can center the table properly

            //Before adding any rows, we must add our columns
            Column column = table.AddColumn("1.0cm"); //ID Column
            tableWidth += 1.0M; //Required for table center to be properly calculated
            column.Format.Alignment = ParagraphAlignment.Left;

            column = table.AddColumn("2.5cm"); //Another Column
            tableWidth += 1.0M; //Required for table center to be properly calculated
            column.Format.Alignment = ParagraphAlignment.Left;

            column = table.AddColumn("2.5cm"); //Another Column
            tableWidth += 1.0M; //Required for table center to be properly calculated
            column.Format.Alignment = ParagraphAlignment.Left;

            column = table.AddColumn("3.5cm"); //Another Column
            tableWidth += 1.0M; //Required for table center to be properly calculated
            column.Format.Alignment = ParagraphAlignment.Left;

            column = table.AddColumn("1.0cm"); //Another Column
            tableWidth += 1.0M; //Required for table center to be properly calculated
            column.Format.Alignment = ParagraphAlignment.Left;

            //column = table.AddColumn("1.0cm"); //Another Column
            //tableWidth += 1.0M; //Required for table center to be properly calculated
            //column.Format.Alignment = ParagraphAlignment.Left;

            table.Rows.LeftIndent = ((16 - tableWidth) / 2).ToString() + "cm"; //Use this to center the table - Note: Max table width = 16CM

            //Create Header Row for the table
            Row row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            //row.Shading.Color = new Color(235, 240, 249); //Red Green Blue
            row.Cells[0].AddParagraph("ID");
            row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[1].AddParagraph("First Name");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[2].AddParagraph("Last Name");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[3].AddParagraph("School");
            row.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[4].AddParagraph("Age");
            row.Cells[4].Format.Alignment = ParagraphAlignment.Left;
            //row.Cells[5].AddParagraph("duh");
            //row.Cells[5].Format.Alignment = ParagraphAlignment.Left;
            //table.SetEdge(0, 0, 5, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty); //This is to draw box around header row, arguments are weird, only change 3rd argument to # of columns

            //Now let's add our fake data
            for (int i = 0; i < 50; i++)
            {
                Row newRow = table.AddRow();
                //ID Column [0]
                newRow.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                newRow.Cells[0].AddParagraph("ID#" + i.ToString()); //Ex. ID#1 ID#2
                //Another Column [1]
                newRow.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                newRow.Cells[1].AddParagraph("Abdelkabir");
                //Amount Column [1]
                newRow.Cells[2].Format.Alignment = ParagraphAlignment.Left;
                newRow.Cells[2].AddParagraph("Battach");

                //Another Column [1]
                newRow.Cells[3].Format.Alignment = ParagraphAlignment.Left;
                newRow.Cells[3].AddParagraph("Rahal Ben Ahmed");

                //Another Column [1]
                newRow.Cells[4].Format.Alignment = ParagraphAlignment.Left;
                newRow.Cells[4].AddParagraph("29");

                //Another Column [1]
                //newRow.Cells[5].Format.Alignment = ParagraphAlignment.Left;
                //newRow.Cells[5].AddParagraph("test5");
            }

            // Create footer
            Paragraph footer = section.Footers.Primary.AddParagraph();
            footer.AddText("Created by Me " + DateTime.Now.Year.ToString());
            footer.Format.Font.Size = 8;
            footer.Format.Alignment = ParagraphAlignment.Center;

        }
        public PdfSharpCore.Pdf.PdfDocument HtmlPdf(List<Students> students, string imgPath, int age)
        {
            //https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcROnYPD5QO8ZJvPQt8ClnJNPXduCeX89dSOxA&usqp=CAU
            var data = new PdfSharpCore.Pdf.PdfDocument();
            string htmlContent = "<div style='margin: 20px auto; max-width: 600px; padding: 20px; border: 1px solid #ccc; background-color: #FFFFFF; font-family: Arial, sans-serif;'>";
            htmlContent += "<div style='margin-bottom: 20px; text-align: center;'>";
            htmlContent += "<img src='" + imgPath + "' alt='School Logo' style='max-width: 100px; margin-bottom: 10px;' >";
            htmlContent += "</div>";
            htmlContent += "<div style='text-align: left;'>";
            htmlContent += "<p style='margin: 0; color: green;'>Abdelkabir Battach</p>";
            htmlContent += "<p style='margin: 0;'>Rahal Ben Ahmed High School</p>";
            htmlContent += "<p style='margin: 0;'>Phone: 0680986898</p>";
            htmlContent += "<p style='margin: 0;'>Agadir, Morocco</p>";
            htmlContent += "</div>";
            htmlContent += "<div style='text-align: center; margin-bottom: 20px;'>";
            htmlContent += "<h1>Fees Structure</h1>";
            htmlContent += "</div>";
            htmlContent += "<h3 style='color: red'>Student Details:</h3>";
            htmlContent += "<p>Name:</p>";
            htmlContent += "<p>STD:</p>";
            htmlContent += "<table style='width: 100%; border: 0px Black Solid;'>";
            htmlContent += "<thead>";
            htmlContent += "<tr>";
            htmlContent += "<th style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>ID</th>";
            htmlContent += "<th style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>Name</th>";
            htmlContent += "<th style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>School</th>";
            htmlContent += "<th style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>Age</th>";
            htmlContent += "</tr><hr/>";
            htmlContent += "</thead>";
            htmlContent += "<tbody>";

            foreach (var item in students)
            {
                htmlContent += "<tr>";
                htmlContent += "<td style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>" + item.id + "</td>";
                htmlContent += "<td style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>" + item.name + "</td>";
                htmlContent += "<td style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>" + item.school + "</td>";
                htmlContent += "<td style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>" + item.age + "</td>";
                htmlContent += "</tr>";
            }

            //htmlContent += "<td style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>1</td>";
            //htmlContent += "<td style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>Abdelkabir Battach</td>";
            //htmlContent += "<td style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>Rahal Ben Ahmed</td>";
            //htmlContent += "<td style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>25</td>";
            //htmlContent += "</tr>";
            //htmlContent += "<tr>";
            //htmlContent += "<td style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>Transportation Fee</td>";
            //htmlContent += "<td style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>RS100/-</td>";
            //htmlContent += "</tr>";
            //htmlContent += "<tr>";
            //htmlContent += "<td style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>Books and Supplies</td>";
            //htmlContent += "<td style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>RS50/-</td>";
            //htmlContent += "</tr>";
            htmlContent += "</tbody>";
            htmlContent += "<tfoot>";
            htmlContent += "<tr>";
            htmlContent += "<td colspan='3' style='padding: 8px; text-align: right; font-weight: bold; border-right: 1px solid #ddd;'>Total:</td>";
            htmlContent += "<td style='padding: 8px; text-align: left;'>" + age + "</td>";
            htmlContent += "</tr>";
            htmlContent += "</tfoot>";
            htmlContent += "</table>";
            htmlContent += "<div style='text-align: center;'";
            htmlContent += "<p>By Abdelkabir Battach</p>";
            htmlContent += "</div>";
            htmlContent += "</div>";

            PdfGenerator.AddPdfPages(data, htmlContent, PdfSharpCore.PageSize.A4);
            return data;
        }


        public PdfSharpCore.Pdf.PdfDocument paypalPdf(Payer payer)
        {
            //https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcROnYPD5QO8ZJvPQt8ClnJNPXduCeX89dSOxA&usqp=CAU
            var data = new PdfSharpCore.Pdf.PdfDocument();
            string htmlContent = "<div style='margin: 20px auto; max-width: 600px; padding: 20px; border: 1px solid #ccc; background-color: #FFFFFF; font-family: Arial, sans-serif;'>";
            htmlContent += "<div style='margin-bottom: 20px; text-align: center;'>";
            htmlContent += "<img src='https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcROnYPD5QO8ZJvPQt8ClnJNPXduCeX89dSOxA&usqp=CAU' alt='School Logo' style='max-width: 100px; margin-bottom: 10px;' >";
            htmlContent += "</div>";
            htmlContent += "<div style='text-align: left;'>";
            htmlContent += "<p style='margin: 0; color: green;'>Abdelkabir Battach</p>";
            htmlContent += "<p style='margin: 0;'>Rahal Ben Ahmed High School</p>";
            htmlContent += "<p style='margin: 0;'>Phone: 0680986898</p>";
            htmlContent += "<p style='margin: 0;'>Agadir, Morocco</p>";
            htmlContent += "</div>";
            htmlContent += "<div style='text-align: center; margin-bottom: 20px;'>";
            htmlContent += "<h1>Fees Structure</h1>";
            htmlContent += "</div>";
            htmlContent += "<h3 style='color: red'>Student Details:</h3>";
            htmlContent += "<p>First Name: " + payer.FirstName + "</p>";
            htmlContent += "<p>Last Name: " + payer.LastName + "</p>";
            htmlContent += "<table style='width: 100%; border: 0px Black Solid;'>";
            htmlContent += "<thead>";
            htmlContent += "<tr>";
            htmlContent += "<th style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>Order ID</th>";
            //htmlContent += "<th style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>Status</th>";
            htmlContent += "<th style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>Address</th>";
            htmlContent += "</tr><hr/>";
            htmlContent += "</thead>";
            htmlContent += "<tbody>";

            htmlContent += "<tr>";
            htmlContent += "<td style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>" + payer.Id + "</td>";
            //htmlContent += "<td style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>" + payer.Status + "</td>";
            htmlContent += "<td style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>" + payer.Address + "</td>";
            htmlContent += "</tr>";


            //htmlContent += "<td style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>1</td>";
            //htmlContent += "<td style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>Abdelkabir Battach</td>";
            //htmlContent += "<td style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>Rahal Ben Ahmed</td>";
            //htmlContent += "<td style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>25</td>";
            //htmlContent += "</tr>";
            //htmlContent += "<tr>";
            //htmlContent += "<td style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>Transportation Fee</td>";
            //htmlContent += "<td style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>RS100/-</td>";
            //htmlContent += "</tr>";
            //htmlContent += "<tr>";
            //htmlContent += "<td style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>Books and Supplies</td>";
            //htmlContent += "<td style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>RS50/-</td>";
            //htmlContent += "</tr>";
            htmlContent += "</tbody>";
            htmlContent += "<tfoot>";
            htmlContent += "<tr>";
            htmlContent += "<td colspan='1' style='padding: 8px; text-align: right; font-weight: bold; border-right: 1px solid #ddd;'>Total:</td>";
            htmlContent += "<td style='padding: 8px; text-align: left;'>" + payer.Price + "</td>";
            htmlContent += "</tr>";
            htmlContent += "</tfoot>";
            htmlContent += "</table>";
            htmlContent += "<div style='text-align: center;'";
            htmlContent += "<p>By Abdelkabir Battach</p>";
            htmlContent += "</div>";
            htmlContent += "</div>";

            PdfGenerator.AddPdfPages(data, htmlContent, PdfSharpCore.PageSize.A4);
            return data;
        }
        public string qrCodeGenerator()
        {
            string url = "";
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator qr = new QRCodeGenerator();
                QRCodeData qrData = qr.CreateQrCode("Battach", QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrData);
                using (Bitmap bitmap = qrCode.GetGraphic(20, System.Drawing.Color.AliceBlue, System.Drawing.Color.Bisque, true))
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    url = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }

            }

            return url;
        }
       
    }
}
    
    

