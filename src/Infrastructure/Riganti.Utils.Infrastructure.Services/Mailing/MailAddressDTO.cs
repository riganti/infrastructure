namespace Riganti.Utils.Infrastructure.Services.Mailing
{
    public class MailAddressDTO
    {

        public string DisplayName { get; set; }

        public string Address { get; set; }


        public MailAddressDTO()
        {
        }

        public MailAddressDTO(string address) : this()
        {
            Address = address;
        }

        public MailAddressDTO(string address, string displayName) : this(address)
        {
            DisplayName = displayName;
        }

    }
}