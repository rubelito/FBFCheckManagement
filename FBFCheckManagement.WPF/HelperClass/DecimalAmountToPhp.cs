namespace FBFCheckManagement.WPF.HelperClass
{
    public static class DecimalAmountToPhp
    {
        public static string ConvertToPhp(decimal amounts){
            return "Php " + string.Format("{0:#,#.00}", amounts);
        }
    }
}