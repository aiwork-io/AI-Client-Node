namespace Domain.Extensions
{
    public static class StringExtensions
    {
        public static byte[] ToByteArray(this string str)
        {
            return System.Text.Encoding.ASCII.GetBytes(str);
        }
    }
}