using System;

namespace AutoMapper.Example.Model
{
    internal class Person
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public DateTime BirthDate { get; set; }
        public bool HasCar { get; set; }
    }
}
