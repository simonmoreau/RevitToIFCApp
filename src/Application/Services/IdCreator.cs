namespace Application.Services
{
    public class IdCreator
    {
        private static Random random = new Random();
        public static string RandomString()
        {
            Guid guid = Guid.NewGuid();
            return Base64Encode(guid.ToString());
        }
        public static string Base64Encode(string plainText)
        {
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            try
            {
                byte[] base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
