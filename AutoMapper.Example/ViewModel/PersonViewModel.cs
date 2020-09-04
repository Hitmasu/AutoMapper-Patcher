using System;

namespace AutoMapper.Example.ViewModel
{
    public class PersonViewModel
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public DateTime BirthDate { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string ZipCode { get; set; }
        public string PhoneNumber { get; set; }
    }
}