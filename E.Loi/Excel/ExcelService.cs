using ClosedXML.Excel;

namespace E.Loi.Excel;

public class ExcelService
{
    public byte[] GenerateExcelFile(List<AmendmentsListVm> amendments)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Sheet1");
            worksheet.Cell(1, 1).Value = "رقم النظام";
            worksheet.Cell(1, 2).Value = "رقم الفريق";
            worksheet.Cell(1, 3).Value = "رقم الترتيبي";
            worksheet.Cell(1, 4).Value = "عنوان المادة";
            worksheet.Cell(1, 5).Value = "موضوع التعديل";
            worksheet.Cell(1, 6).Value = "نوع التعديل";
            worksheet.Cell(1, 7).Value = "حالة التعديل";
            worksheet.Cell(1, 8).Value = "صاحب التعديل";
            worksheet.Cell(1, 9).Value = "تاريخ التصويت";
            worksheet.Cell(1, 10).Value = "نتيجة التصويت";
            int order = 2;
            foreach (var amendment in amendments)
            {
                worksheet.Cell(order, 1).Value = amendment.NumberSystem;
                worksheet.Cell(order, 2).Value = amendment.Number;
                worksheet.Cell(order, 3).Value = amendment.Order;
                worksheet.Cell(order, 4).Value = amendment.NodeTitle;
                worksheet.Cell(order, 5).Value = amendment.Title;
                worksheet.Cell(order, 6).Value = amendment.AmendmentIntent;
                worksheet.Cell(order, 7).Value = amendment.AmendmentsStatus;
                worksheet.Cell(order, 8).Value = amendment.Team;
                worksheet.Cell(order, 9).Value = amendment.VotingDate;
                worksheet.Cell(order, 10).Value = amendment.VoteResult;
                order++;
            }
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
        }
    }
}
