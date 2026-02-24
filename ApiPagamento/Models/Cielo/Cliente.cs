namespace PagamentoApi.Models.Cielo
{
    public class Cliente
    {
        public Cliente(string nome)
        {
            Name = nome;
        }

        public Cliente()
        {

        }

        public string Name { get; set; }
        public string Identity { get; set; }
        public string Identitytype { get; set; }
        public string Birthdate { get; set; }
        public string Email { get; set; }
        public Address Address { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public string Number { get; set; }
        public string Complement { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }
}
