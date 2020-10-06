# AutoMapper Patcher
Removing AutoMapper (at runtime) with Jitex to increase performance.

------

This code:

```C#
PersonViewModel vm = Mapper.Map<PersonViewModel> (_person);
```

Will be replaced by this:

```c#
PersonViewModel vm = new PersonViewModel();
vm.Name = _person.Name,
vm.Username = _person.Name;
vm.BirthDate = _person.BirthDate;
vm.Street = _person.Street;
//...
```

### Packages used

- AutoMapper 10.0.0
- Jitex 3.0.4-alpha

### Result

``` ini
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19041.508 (2004/?/20H1)
Intel Core i5-7600 CPU 3.50GHz (Kaby Lake), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=3.1.402
  [Host]     : .NET Core 3.1.8 (CoreCLR 4.700.20.41105, CoreFX 4.700.20.41903), X64 RyuJIT
  DefaultJob : .NET Core 3.1.8 (CoreCLR 4.700.20.41105, CoreFX 4.700.20.41903), X64 RyuJIT
```
|          Method |      Mean |    Error |   StdDev |
|---------------- |----------:|---------:|---------:|
|    MapWithJitex |  18.31 ns | 0.349 ns | 0.402 ns |
| MapWithoutJitex | 154.68 ns | 2.672 ns | 3.077 ns |

### Code

```c#
/// <summary>
/// Method to patch
/// </summary>
/// <returns></returns>
public PersonViewModel MapWithJitex () {
    PersonViewModel vm = Mapper.Map<PersonViewModel> (_person);
    return vm;
}

/// <summary>
/// Method to not patch (AutoMapper)
/// </summary>
/// <returns></returns>
public PersonViewModel MapWithoutJitex () {
    PersonViewModel vm = Mapper.Map<PersonViewModel> (_person);
    return vm;
}
```



------



#### This is just a PoC. To work more properly, it's necessary more some implementations on Patcher. It's just an example, it's not to be used in real world.

