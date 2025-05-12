using QRCoder;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Drawing;
using System.Reflection;

namespace BattachApp.Models
{
    public class InvoiceDocument : IDocument
    {
        public List<Students> Model { get; }
        public string _imagePath;

        public InvoiceDocument(List<Students> model , string imagePath)
        {
            Model = model;
            _imagePath = imagePath;
        }

        //public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        //public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(20);
                    page.Header().Height(100).Background(Colors.Grey.Lighten1).Element(ComposeHeader);
                    page.Content().Background(Colors.Grey.Lighten3).Element(ComposeContent);

                    page.Footer().Height(50).Background(Colors.Grey.Lighten1).AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
        }

        void ComposeHeader(IContainer container)
        {
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    QRCodeGenerator qr = new QRCodeGenerator();
            //    QRCodeData qrData = qr.CreateQrCode("Battach" , QRCodeGenerator.ECCLevel.Q);
            //    QRCode qrCode = new QRCode(qrData);
            //    using (Bitmap bitmap = qrCode.GetGraphic(20))
            //    {
            //        bitmap.Save(ms, ImageFormat.Png);
            //        string url = "data:Image/png;base64," + Convert.ToBase64String(ms.ToArray());
            //    }

            //}
            Test test = new Test();
                container.Padding(10).Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Item()
                            .Text("Invoice #12458888")
                            .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                        column.Item().Text(text =>
                        {
                            text.Span("Issue date: ").SemiBold();
                            text.Span("12/02/2000");
                        });

                        column.Item().Text(text =>
                        {
                            text.Span("Due date: ").SemiBold();
                            text.Span("01/11/2025");
                        });
                        //column.Item().Width(40).Image(_imagePath);
                    });

                    //row.ConstantItem(100).Height(50).Placeholder();
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().AlignMiddle().AlignCenter().Width(50).Height(50).AlignCenter().Image(_imagePath);
                        column.Item().Text("Abdelkabir Battach").FontColor(Colors.Blue.Medium);
                    });
                });
        }

        void ComposeContent(IContainer container)
        {
            //container
            //    .PaddingVertical(40)
            //    .Height(250)
            //    .Background(Colors.Grey.Lighten3)
            //    .AlignCenter()
            //    .AlignMiddle()
            //    .Text("Content").FontSize(16);
            container.PaddingVertical(15).Column(column =>
            {
                //column.Spacing(5);

                column.Item().Element(ComposeTable);

            
               column.Item().PaddingTop(15).Element(ComposeComments);
            });
        }
        void ComposeComments(IContainer container)
        {
            container.Padding(10).Background(Colors.Grey.Lighten3).Column(column =>
            {
                //column.Spacing(20);
                column.Item().Text("Comments").FontSize(16);
                column.Item().Text("Begin by defining the number, position, and size of your columns. After that, add cells which can be either auto-arranged by the layout engine or explicitly placed at specific rows and columns. You can even have cells span multiple columns or rows.");
                column.Item().AlignCenter().AlignMiddle().PaddingVertical(10).Width(100).Height(100).Image(_imagePath);
            });
        }
        void ComposeTable(IContainer container)
        {
            container.Padding(10).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.ConstantColumn(125);
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Name");
                    header.Cell().Element(CellStyle).AlignCenter().Text("School");
                    header.Cell().Element(CellStyle).AlignRight().Text("Age");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container
                            .Background(Colors.Blue.Darken2)
                            .DefaultTextStyle(x => x.FontColor(Colors.White).Bold())
                            .PaddingVertical(8)
                            .PaddingHorizontal(16);
                    }
                });

                for (int i=0; i<Model.Count; i++)
                {
                    //var weatherIndex = Random.Shared.Next(0, weatherIcons.Length);

                    table.Cell().Element(CellStyle)
                        .Text(Model[i].name);
                    table.Cell().Element(CellStyle)
                        .Text(Model[i].school);
                    //table.Cell().Element(CellStyle).AlignCenter().Height(24)
                    //    .Svg($"Resources/WeatherIcons/{weatherIcons[weatherIndex]}");

                    table.Cell().Element(CellStyle).AlignRight()
                        .Text(Model[i].age.ToString());

                    IContainer CellStyle(IContainer container)
                    {
                        var backgroundColor = i % 2 == 0
                            ? Colors.Blue.Lighten5
                            : Colors.Blue.Lighten4;

                        return container
                            .Background(backgroundColor)
                            .PaddingVertical(8)
                            .PaddingHorizontal(16);
                    }
                }
            });
        }

    }   
    
    public class Invoice
    {
        public IDocument questPdf()
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(20));

                        page.Header()
                            .Text("Hello PDF!")
                            .SemiBold().FontSize(36).FontColor(Colors.Blue.Medium);

                        page.Content()
                            .PaddingVertical(1, Unit.Centimetre)
                            .Column(x =>
                            {
                                x.Spacing(20);

                                x.Item().Text(Placeholders.LoremIpsum());
                                x.Item().Image(Placeholders.Image(200, 100));
                            });
                        page.Footer()
                            .AlignCenter()
                            .Text(x =>
                            {
                                x.Span("Page ");
                                x.CurrentPageNumber();
                            });
                    });
                });
            //.GeneratePdf("hello.pdf");
        }
    }
}
