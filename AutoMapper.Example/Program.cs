using AutoMapper.Example.Model;
using AutoMapper.Example.ViewModel;
using BenchmarkDotNet.Attributes;
using System;
using AutoMapper.Module;
using BenchmarkDotNet.Running;
using Jitex;

namespace AutoMapper.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<AutoMapperBenchmark>();
        }
    }

    public class AutoMapperBenchmark
    {
        private static readonly IMapper Mapper;

        private readonly Person _person;

        static AutoMapperBenchmark()
        {
            JitexManager.LoadModule<AutoMapperModule>();

            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Person, PersonViewModel>();
            });

            Mapper = config.CreateMapper();
        }

        public AutoMapperBenchmark()
        {
            _person = new Person
            {
                Id = 1,
                BirthDate = DateTime.Now,
                HasCar = true,
                Name = "Lindsey",
                Username = "R. Martin",
                City = "City ABC XYZ",
                Street = "Street ABC XYZ",
                PhoneNumber = "+2222222200000",
                ZipCode = "XYZ-8520"
            };
        }

        /// <summary>
        /// Method optmized by our library
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public PersonViewModel MapWithJitex()
        {
            PersonViewModel vm = Mapper.Map<PersonViewModel>(_person);
            return vm;
        }

        /// <summary>
        /// Method with original AutoMapper
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public PersonViewModel MapWithoutJitex()
        {
            PersonViewModel vm = Mapper.Map<PersonViewModel>(_person);
            return vm;
        }
    }
}
