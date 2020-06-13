using AutoMapper.Example.Model;
using AutoMapper.Example.ViewModel;
using AutoMapper.Patcher;
using BenchmarkDotNet.Attributes;
using System;
using BenchmarkDotNet.Running;

namespace AutoMapper.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkAutoMapper b = new BenchmarkAutoMapper();
            b.MapWithJitex();
            BenchmarkRunner.Run<BenchmarkAutoMapper>();
        }
    }

    public class BenchmarkAutoMapper
    {
        private static readonly IMapper Mapper;

        private readonly Person _person;

        static BenchmarkAutoMapper()
        {
            AutoMapperPatcher.Initialize();

            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Person, PersonViewModel>();
            });

            Mapper = config.CreateMapper();
        }

        public BenchmarkAutoMapper()
        {
            _person = new Person
            {
                Id = 1,
                BirthDate = DateTime.Now,
                HasCar = true,
                Name = "Jason",
                Username = "Bason"
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
