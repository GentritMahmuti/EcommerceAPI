namespace Ecommerce.Consumer.EmailSender
{
    public class SmtpConfiguration
    {
        public string Host { get; set; }
        public string From { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int Port { get; set; } = 587;
        public bool UseSSL { get; set; } = true;
    }
}
