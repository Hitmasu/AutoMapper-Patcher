``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.900 (1909/November2018Update/19H2)
Intel Core i5-7600 CPU 3.50GHz (Kaby Lake), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=3.1.301
  [Host]     : .NET Core 3.1.5 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.27001), X64 RyuJIT
  DefaultJob : .NET Core 3.1.5 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.27001), X64 RyuJIT


```
|          Method |      Mean |    Error |   StdDev |
|---------------- |----------:|---------:|---------:|
|    MapWithJitex |  11.07 ns | 0.095 ns | 0.089 ns |
| MapWithoutJitex | 105.64 ns | 0.342 ns | 0.286 ns |
