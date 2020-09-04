# AutoMapper Patcher
Removing AutoMapper (at runtime) with Jitex to increase performance.

### Packages used

AutoMapper 10.0.0

Jitex 2.0.2-alpha

### Result

``` ini
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19041.450 (2004/?/20H1)
Intel Core i5-7600 CPU 3.50GHz (Kaby Lake), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=3.1.401
  [Host]     : .NET Core 3.1.7 (CoreCLR 4.700.20.36602, CoreFX 4.700.20.37001), X64 RyuJIT
  DefaultJob : .NET Core 3.1.7 (CoreCLR 4.700.20.36602, CoreFX 4.700.20.37001), X64 RyuJIT
```
|          Method |      Mean |    Error |   StdDev |
|---------------- |----------:|---------:|---------:|
|    MapWithJitex |  18.45 ns | 0.236 ns | 0.197 ns |
| MapWithoutJitex | 146.10 ns | 2.088 ns | 1.953 ns |

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

