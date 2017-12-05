using ClosedXML.Excel;
using FBFCheckManagement.Application.Report;

namespace FBFCheckManagement.WPF.Report
{
    public class WeeklySummaryDrawer
    {
        private readonly IXLWorksheet _worksheet;
        private int _currentRowIndex;
        private int _currentColumnIndex;
        private readonly bool _isFirstInterval;

        private readonly int _previousRowIndex;
        private readonly int _previousColumnIndex;
        private const double FontSize = 11;
        private int _indexOfFirstColumn;

        private int _indexOfFirstRowBorder;
        private int _indexOfFirstColumnBorder;
        private int _indexOfLastRowBorder;
        private int _indexOfLastColumnBorder;

        public WeeklySummaryDrawer(IXLWorksheet worksheet, int currentRowIndex,
            int currentColumnIndex, bool isFirstInterval)
        {
            _worksheet = worksheet;
            _currentRowIndex = currentRowIndex;
            _currentColumnIndex = currentColumnIndex;
            _isFirstInterval = isFirstInterval;

            _previousRowIndex = currentRowIndex;
            _previousColumnIndex = currentColumnIndex;
        }

        public void CreateDaySection(DailyReportModel day)
        {
            _currentRowIndex = _currentRowIndex + 5;

            foreach (var dept in day.SectionsPerDepartment)
            {
                _currentColumnIndex = _previousColumnIndex;
                if (_isFirstInterval)
                {
                    CreateDepartmentLabel(dept);
                }

                _currentRowIndex++;
                _currentColumnIndex++;
                _indexOfFirstRowBorder = _currentRowIndex;
                _indexOfFirstColumnBorder = _currentColumnIndex;

                CreateWeekDay(day);

                CreateBanksPerDay(dept);
                SurroundWithBorder();
            }
        }

        private void CreateWeekDay(DailyReportModel day)
        {
            var dayCell = _worksheet.Cell(_currentRowIndex, _currentColumnIndex);
            dayCell.Value = day.DayOfTheWeek;
            dayCell.DataType = XLCellValues.Text;
            dayCell.Style.Font.FontSize = FontSize;
            dayCell.Style.Fill.BackgroundColor = XLColor.PowderBlue;
            if (day.IsTotalForEntireWeek)
                dayCell.Style.Font.Bold = true;

            _currentColumnIndex++;
            _indexOfFirstColumn = _currentColumnIndex - 1;

            var mergedDay = _worksheet.Range(_currentRowIndex, _indexOfFirstColumn, _currentRowIndex,
                _currentColumnIndex);
            mergedDay.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            mergedDay.Merge();
            mergedDay.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            _currentRowIndex++;
        }

        private void CreateDepartmentLabel(DepartmentSection dept)
        {
            var departmentCell = _worksheet.Cell(_currentRowIndex, _currentColumnIndex);
            departmentCell.Value = dept.Department.Name;
            departmentCell.Style.Font.FontSize = FontSize;
            departmentCell.Style.Font.Bold = true;
        }

        private void CreateBanksPerDay(DepartmentSection dept)
        {
            foreach (var bank in dept.BankSections)
            {
                var bankCell = _worksheet.Cell(_currentRowIndex, _indexOfFirstColumn);
                bankCell.Value = bank.Bank.BankName;
                bankCell.DataType = XLCellValues.Text;
                bankCell.Style.Font.FontSize = FontSize;
                bankCell.Style.Fill.BackgroundColor = XLColor.LightGray;

                var bankRange = _worksheet.Range(_currentRowIndex, _indexOfFirstColumn, _currentRowIndex,
                    _currentColumnIndex);
                bankRange.Merge();

                if (bank.IsTotalPerDay)
                {
                    bankRange.Style.Border.TopBorder = XLBorderStyleValues.Medium;
                    bankRange.Style.Font.Bold = true;
                    bankRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                }
                else
                {
                    bankRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                _currentRowIndex++;

                CreateSubTotalRows(bank);
                _currentRowIndex++;
                CreateSettledAmountRow(bank);
                _currentRowIndex++;
                CreateForFundingAmountRow(bank);

                _indexOfLastRowBorder = _currentRowIndex;
                _indexOfLastColumnBorder = _currentColumnIndex;


                _currentRowIndex = _currentRowIndex + 2;
            }
        }

        private void SurroundWithBorder()
        {
            var firstCell = _worksheet.Cell(_indexOfFirstRowBorder, _indexOfFirstColumnBorder);
            var lastCell = _worksheet.Cell(_indexOfLastRowBorder, _indexOfLastColumnBorder);

            var tableRange = _worksheet.Range(firstCell, lastCell);
            tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }

        private void CreateSubTotalRows(BankSection sec)
        {
            var subTotalLabel = _worksheet.Cell(_currentRowIndex, _indexOfFirstColumn);
            subTotalLabel.Value = "Total";
            subTotalLabel.DataType = XLCellValues.Text;
            subTotalLabel.Style.Font.FontSize = FontSize;

            var subTotalCell = _worksheet.Cell(_currentRowIndex, _currentColumnIndex);
            subTotalCell.Value = sec.Amount;
            subTotalCell.Style.NumberFormat.Format = "#,##0.00_);[Red](#,##0.00)";
            subTotalCell.DataType = XLCellValues.Number;
            subTotalCell.Style.Font.FontSize = FontSize;
        }

        private void CreateSettledAmountRow(BankSection sec)
        {
            var settledlabel = _worksheet.Cell(_currentRowIndex, _indexOfFirstColumn);
            settledlabel.Value = "Settled";
            settledlabel.DataType = XLCellValues.Text;
            settledlabel.Style.Font.FontSize = FontSize;

            var settledCell = _worksheet.Cell(_currentRowIndex, _currentColumnIndex);
            settledCell.Value = sec.SettledAmount;
            settledCell.Style.NumberFormat.Format = "#,##0.00_);[Red](#,##0.00)";
            settledCell.DataType = XLCellValues.Number;
            settledCell.Style.Font.FontSize = FontSize;
        }

        private void CreateForFundingAmountRow(BankSection sec)
        {
            var forFundingLabel = _worksheet.Cell(_currentRowIndex, _indexOfFirstColumn);
            forFundingLabel.Value = "For Funding";
            forFundingLabel.DataType = XLCellValues.Text;
            forFundingLabel.Style.Font.FontSize = FontSize;
            forFundingLabel.Style.Font.Bold = true;

            var forFundingAmountCell = _worksheet.Cell(_currentRowIndex, _currentColumnIndex);
            forFundingAmountCell.Value = sec.RemainingAmount;
            forFundingAmountCell.Style.NumberFormat.Format = "#,##0.00_);[Red](#,##0.00)";
            forFundingAmountCell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            forFundingAmountCell.DataType = XLCellValues.Number;
            forFundingAmountCell.Style.Font.FontSize = FontSize;
            forFundingAmountCell.Style.Font.Bold = true;
        }

        public int PreviousRowIndex { get { return _previousRowIndex; } }
        public int PreviousColumnIndex { get { return _previousColumnIndex; } }
        public int CurrentRowIndex { get { return _currentRowIndex; } }
        public int CurrentColumnIndex { get { return _currentColumnIndex; } }

    }
}