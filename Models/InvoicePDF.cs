using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BattachApp.Models
{
    public class InvoicePDF : IDocument
    {
        public string _imgPath;
        public InvoicePDF(string imgPath)
        {
            _imgPath = imgPath;
        }
        public void Compose(IDocumentContainer container) {
            container.Page(page =>
            {
                //page.Margin(20);
                page.Header().Height(200).Background(Colors.Grey.Lighten1).Element(ComposeHeader);
                //page.Content().Background(Colors.Grey.Lighten3).Element(ComposeContent);

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
            container.Padding(10).Row(column =>
            {
                column.RelativeItem().AlignLeft().AlignMiddle().Background("#ffffff").Width(80).Height(49).Image(_imgPath).FitArea();
                //column.Item().AlignCenter().AlignMiddle().Padding(5).Background("#ffffff").Row(row =>
                //{
                //    row.RelativeItem().Background("#00FFFF").Text(x =>
                //    {
                //        x.Span("Invoice").FontSize(20).Bold();
                //        x.Span(" Service").FontSize(20).SemiBold();
                //    });
                //});
                column.RelativeItem().AlignRight().AlignMiddle().Text(x =>
                {
                    x.Span("Invoice").FontSize(32).FontColor("#ffffff").ExtraBold();
                    x.Span(" Service").FontSize(32).SemiBold();
                });
            });
        }
    }
}
