using System.Linq;
using System.Windows;
using ClosedXML.Excel;
using FBFCheckManagement.Application.Domain;
using FBFCheckManagement.Application.Report;

namespace FBFCheckManagement.WPF.Report
{
    public class WorkSheetMaker
    {
        private int _currentRowIndex;
        private int _summaryRowIndex;
        private IXLWorksheet _sheet;
        private IXLWorksheet _summarySheet;
        private readonly XLWorkbook _workbook;
        private const double FontSize = 16;

        public WorkSheetMaker(XLWorkbook workBook){
            _workbook = workBook;
        }

        public IXLWorksheet MakeWeeklySummary(WeekReportModel report){
            SetDefaultValues();
            _summarySheet = _workbook.Worksheets.Add("Summary");
            CreateCoverdDateSection(report);
            CreateSummaryAmountSection(report); 
            SetAutoAdjustToWeekSummaryContent();
            
            return _summarySheet;
        }

        private void CreateCoverdDateSection(WeekReportModel report){
            var coveredDateLabel = _summarySheet.Cell(_summaryRowIndex, 1);
            coveredDateLabel.Value = "Covered Date";
            coveredDateLabel.DataType = XLCellValues.Text;
            coveredDateLabel.Style.Font.FontSize = FontSize;

            var coveredDate = _summarySheet.Cell(_summaryRowIndex, 2);
            coveredDate.Style.DateFormat.Format = "MMM dd, yyyy";
            coveredDate.Value = report.From.ToString("MMM dd, yyyy") + " - " + report.To.ToString("MMM dd, yyyy");
            coveredDate.Style.Font.FontSize = FontSize;
        }

        private void CreateSummaryAmountSection(WeekReportModel report){
            _summaryRowIndex = _summaryRowIndex + 2;

            var totalLabel = _summarySheet.Cell(_summaryRowIndex, 1);
            totalLabel.Value = "SubTotal";
            totalLabel.DataType = XLCellValues.Text;
            totalLabel.Style.Font.FontSize = FontSize;

            var totalCell = _summarySheet.Cell(_summaryRowIndex, 2);
            totalCell.Value = report.Total;
            totalCell.Style.NumberFormat.Format = "#,##0.00_);[Red](#,##0.00)";
            totalCell.DataType = XLCellValues.Number;
            totalCell.Style.Font.FontSize = FontSize;

            _summaryRowIndex++;
            var settledlabel = _summarySheet.Cell(_summaryRowIndex, 1);
            settledlabel.Value = "Settled";
            settledlabel.DataType = XLCellValues.Text;
            settledlabel.Style.Font.FontSize = FontSize;

            var settledCell = _summarySheet.Cell(_summaryRowIndex, 2);
            settledCell.Value = report.Settled;
            settledCell.Style.NumberFormat.Format = "#,##0.00_);[Red](#,##0.00)";
            settledCell.DataType = XLCellValues.Number;
            settledCell.Style.Font.FontSize = FontSize;

            _summaryRowIndex++;
            var forFundingLabel = _summarySheet.Cell(_summaryRowIndex, 1);
            forFundingLabel.Value = "For Funding";
            forFundingLabel.DataType = XLCellValues.Text;
            forFundingLabel.Style.Font.FontSize = FontSize;

            var forFundingAmountCell = _summarySheet.Cell(_summaryRowIndex, 2);
            forFundingAmountCell.Value = report.Remaining;
            forFundingAmountCell.Style.NumberFormat.Format = "#,##0.00_);[Red](#,##0.00)";
            forFundingAmountCell.DataType = XLCellValues.Number;
            forFundingAmountCell.Style.Border.TopBorder = XLBorderStyleValues.Medium;
            forFundingAmountCell.Style.Font.FontSize = FontSize;
        }


        public IXLWorksheet Make(DailyReportModel report){
            SetDefaultValues();
            _sheet = _workbook.Worksheets.Add(report.Day.ToString("MMM dd, yyyy"));
            
            CreateDateHeader(report);
            CreateColumn();

            foreach (var section in report.SectionsPerBank){
                int initialRowInderForBorder = _currentRowIndex;
                CreateSection(section);
                CreateBorder(initialRowInderForBorder, _currentRowIndex);
                _currentRowIndex = _currentRowIndex + 2;
            }

            CreateTotal(report);
            SetAutoAdjustColumnToContent();          
            _workbook.Dispose();
            return _sheet;
        }

        private void SetDefaultValues(){
            _currentRowIndex = 5;
            _summaryRowIndex = 2;
        }

        private void CreateBorder(int firstRowIndex, int lastRowIndex){
            var firstCell = _sheet.Cell(firstRowIndex, 1);
            var lastCell = _sheet.Cell(lastRowIndex, 6);
            
            var tableRange = _sheet.Range(firstCell, lastCell);
            tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }

        private void CreateDateHeader(DailyReportModel report){
            var dateHeader = _sheet.Cell(1, 2);
            dateHeader.Style.DateFormat.Format = "MMM dd, yyyy";
            dateHeader.Value = report.Day;
            dateHeader.Style.Font.FontSize = FontSize;
        }

        private void CreateColumn(){
            var bankColumn = _sheet.Cell(3, 2);
            bankColumn.Value = "Bank";
            bankColumn.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            bankColumn.Style.Font.Bold = true;
            bankColumn.Style.Font.FontSize = FontSize;

            var checkNumberColumn = _sheet.Cell(3, 3);
            checkNumberColumn.Value = "Check #";
            checkNumberColumn.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            checkNumberColumn.Style.Font.Bold = true;
            checkNumberColumn.Style.Font.FontSize = FontSize;

            var issuedToColumn = _sheet.Cell(3, 4);
            issuedToColumn.Value = "Issued To";
            issuedToColumn.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            issuedToColumn.Style.Font.Bold = true;
            issuedToColumn.Style.Font.FontSize = FontSize;

            var amountColumn = _sheet.Cell(3, 5);
            amountColumn.Value = "Amounts";
            amountColumn.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            amountColumn.Style.Font.Bold = true;
            amountColumn.Style.Font.FontSize = FontSize;

            var statusColumn = _sheet.Cell(3, 6);
            statusColumn.Value = "Status";
            statusColumn.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            statusColumn.Style.Font.Bold = true;
            statusColumn.Style.Font.FontSize = FontSize;
        }

        private void CreateSection(BankSection sec){
            var bankCell = _sheet.Cell(_currentRowIndex, 2);
            bankCell.Value = sec.Bank.BankName;
            bankCell.DataType = XLCellValues.Text;

            foreach (var c in sec.ChecksToDisplay){
                _currentRowIndex++;
                CreateCheckRow(c);
            }

            _currentRowIndex = _currentRowIndex + 2;
            CreateSubTotalRows(sec);

            _currentRowIndex++;
            CreateSettledAmountRow(sec);

            _currentRowIndex++;
            CreateForFundingAmountRow(sec);
        }

        private void CreateCheckRow(Check check){
            var checkNumberCell = _sheet.Cell(_currentRowIndex, 3);
            checkNumberCell.Value = check.CheckNumber;
            checkNumberCell.DataType = XLCellValues.Text;
            checkNumberCell.Style.Font.FontSize = FontSize;

            var issuedToCell = _sheet.Cell(_currentRowIndex, 4);
            issuedToCell.Value = check.IssuedTo;
            issuedToCell.DataType = XLCellValues.Text;
            issuedToCell.Style.Font.FontSize = FontSize;

            var amountCell = _sheet.Cell(_currentRowIndex, 5);
            amountCell.Value = check.Amount;
            amountCell.Style.NumberFormat.Format = "#,##0.00_);[Red](#,##0.00)";
            amountCell.DataType = XLCellValues.Number;
            amountCell.Style.Font.FontSize = FontSize;
            SetBackgroundColor(amountCell, check);
            AddCommentsIfThereIs(amountCell, check);

            var statusCell = _sheet.Cell(_currentRowIndex, 6);
            statusCell.Value = check.Status;
            statusCell.DataType = XLCellValues.Text;
            statusCell.Style.Font.FontSize = FontSize;

        }

        private void SetBackgroundColor(IXLCell cell, Check check){
            if (check.IsSettled){
                cell.Style.Fill.BackgroundColor = XLColor.LightGreen;
            }
            else if (check.IsFunded){
                cell.Style.Fill.BackgroundColor = XLColor.Orange;
            }
            else if (check.IsOnHold){
                cell.Style.Fill.BackgroundColor = XLColor.OrangeRed;
            }
            else{
                cell.Style.Fill.BackgroundColor = XLColor.LightBlue;
            }
        }

        private void AddCommentsIfThereIs(IXLCell cell, Check check){
            if (!string.IsNullOrWhiteSpace(check.Notes)){
                cell.Comment.Style.Alignment.SetAutomaticSize();
                cell.Comment.AddText(check.Notes);
            }
        }

        private void CreateSubTotalRows(BankSection sec){
            var subTotalLabel = _sheet.Cell(_currentRowIndex, 1);
            subTotalLabel.Value = "SubTotal";
            subTotalLabel.DataType = XLCellValues.Text;
            subTotalLabel.Style.Font.FontSize = FontSize;

            var subTotalCell = _sheet.Cell(_currentRowIndex, 5);
            subTotalCell.Value = sec.TotalAmount;
            subTotalCell.Style.NumberFormat.Format = "#,##0.00_);[Red](#,##0.00)";
            subTotalCell.DataType = XLCellValues.Number;
            subTotalCell.Style.Font.FontSize = FontSize;
        }

        private void CreateSettledAmountRow(BankSection sec){
            var settledlabel = _sheet.Cell(_currentRowIndex, 1);
            settledlabel.Value = "Settled";
            settledlabel.DataType = XLCellValues.Text;
            settledlabel.Style.Font.FontSize = FontSize;

            var settledCell = _sheet.Cell(_currentRowIndex, 5);
            settledCell.Value = sec.SettledAmount;
            settledCell.Style.NumberFormat.Format = "#,##0.00_);[Red](#,##0.00)";
            settledCell.DataType = XLCellValues.Number;
            settledCell.Style.Font.FontSize = FontSize;
        }

        private void CreateForFundingAmountRow(BankSection sec){
            var forFundingLabel = _sheet.Cell(_currentRowIndex, 1);
            forFundingLabel.Value = "For Funding";
            forFundingLabel.DataType = XLCellValues.Text;
            forFundingLabel.Style.Font.FontSize = FontSize;

            var forFundingAmountCell = _sheet.Cell(_currentRowIndex, 5);
            forFundingAmountCell.Value = sec.RemainingAmount;
            forFundingAmountCell.Style.NumberFormat.Format = "#,##0.00_);[Red](#,##0.00)";
            forFundingAmountCell.Style.Border.TopBorder = XLBorderStyleValues.Medium;
            forFundingAmountCell.DataType = XLCellValues.Number;
            forFundingAmountCell.Style.Font.FontSize = FontSize;
        }

        private void CreateTotal(DailyReportModel report){
            _currentRowIndex = _currentRowIndex + 2;

            var totalLabel = _sheet.Cell(_currentRowIndex, 1);
            totalLabel.Value = "Total";
            totalLabel.Style.Font.FontSize = FontSize;
            var totalAmount = _sheet.Cell(_currentRowIndex, 5);
            totalAmount.Value = report.TotalAmount;
            totalAmount.Style.NumberFormat.Format = "#,##0.00_);[Red](#,##0.00)";
            totalAmount.DataType = XLCellValues.Number;
            totalAmount.Style.Font.FontSize = FontSize;

            _currentRowIndex++;
            var totalSettledlabel = _sheet.Cell(_currentRowIndex, 1);
            totalSettledlabel.Value = "Total Settled";
            totalSettledlabel.Style.Font.FontSize = FontSize;
            var totalSettledAmount = _sheet.Cell(_currentRowIndex, 5);
            totalSettledAmount.Value = report.TotalSettledAmount;
            totalSettledAmount.Style.NumberFormat.Format = "#,##0.00_);[Red](#,##0.00)";
            totalSettledAmount.DataType = XLCellValues.Number;
            totalSettledAmount.Style.Font.FontSize = FontSize;

            _currentRowIndex++;
            var totalForFundingLabel = _sheet.Cell(_currentRowIndex, 1);
            totalForFundingLabel.Value = "Total For Funding";
            totalForFundingLabel.Style.Font.FontSize = FontSize;
            var totalForFundingAmount = _sheet.Cell(_currentRowIndex, 5);
            totalForFundingAmount.Value = report.TotalRemaining;
            totalForFundingAmount.Style.NumberFormat.Format = "#,##0.00_);[Red](#,##0.00)";
            totalForFundingAmount.DataType = XLCellValues.Number;
            totalForFundingAmount.Style.Border.TopBorder = XLBorderStyleValues.Thick;
            totalForFundingAmount.Style.Font.FontSize = FontSize;
        }

        private void SetAutoAdjustColumnToContent(){
            _sheet.Column(1).AdjustToContents();
            _sheet.Column(2).AdjustToContents();
            _sheet.Column(3).AdjustToContents();
            _sheet.Column(4).AdjustToContents();
            _sheet.Column(5).AdjustToContents();
            _sheet.Column(6).AdjustToContents();            
        }

        private void SetAutoAdjustToWeekSummaryContent(){
            _summarySheet.Column(1).AdjustToContents();
            _summarySheet.Column(2).AdjustToContents();
        }
    }
}