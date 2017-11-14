namespace FBFCheckManagement.WPF.HelperClass
{
    public static class DecimalAmountToPhp
    {
        public static string ConvertToCurrency(decimal amounts){
            return string.Format("{0:#,#.00}", amounts);
        }
    }
}