# osx-arm64

Hardware: arm64, Apple Silicon, GitHub hosted.

Ratios are comparable within this file. Absolute nanoseconds are not comparable
with another platform's file, because the hardware differs.


BenchmarkDotNet v0.15.2, macOS Sonoma 14.8.7 (23J520) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 10.0.400
  [Host]     : .NET 10.0.11 (10.0.1126.37416), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 10.0.11 (10.0.1126.37416), Arm64 RyuJIT AdvSIMD


 Type                     | Method                            | Categories   | Length | WorkingSet | Mean        | Error     | StdDev    | Median      | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
------------------------- |---------------------------------- |------------- |------- |----------- |------------:|----------:|----------:|------------:|------:|--------:|-------:|----------:|------------:|
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 16     | ?          |    32.38 ns |  0.670 ns |  0.847 ns |    32.17 ns |  1.00 |    0.04 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 16     | ?          |    43.57 ns |  0.104 ns |  0.081 ns |    43.59 ns |  1.35 |    0.03 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 16     | ?          |    48.48 ns |  0.186 ns |  0.156 ns |    48.46 ns |  1.50 |    0.04 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 16     | ?          |    55.21 ns |  1.381 ns |  4.005 ns |    55.50 ns |  1.71 |    0.13 |      - |         - |          NA |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 64     | ?          |    32.13 ns |  0.594 ns |  0.556 ns |    31.87 ns |  1.00 |    0.02 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 64     | ?          |    45.90 ns |  0.053 ns |  0.045 ns |    45.90 ns |  1.43 |    0.02 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 64     | ?          |    48.99 ns |  0.689 ns |  0.611 ns |    48.70 ns |  1.53 |    0.03 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 64     | ?          |    59.50 ns |  0.281 ns |  0.249 ns |    59.48 ns |  1.85 |    0.03 |      - |         - |          NA |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 170    | ?          |    32.16 ns |  0.281 ns |  0.249 ns |    32.07 ns |  1.00 |    0.01 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 170    | ?          |    53.81 ns |  0.508 ns |  0.475 ns |    53.60 ns |  1.67 |    0.02 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 170    | ?          |    61.71 ns |  1.265 ns |  3.128 ns |    61.19 ns |  1.92 |    0.10 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 170    | ?          |   102.02 ns |  1.955 ns |  4.647 ns |   101.63 ns |  3.17 |    0.15 |      - |         - |          NA |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 256    | ?          |    40.71 ns |  0.838 ns |  2.236 ns |    40.47 ns |  1.00 |    0.08 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 256    | ?          |    59.93 ns |  1.243 ns |  3.444 ns |    60.29 ns |  1.48 |    0.12 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 256    | ?          |    79.52 ns |  1.799 ns |  5.305 ns |    79.21 ns |  1.96 |    0.17 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 256    | ?          |   132.07 ns |  3.183 ns |  9.134 ns |   132.01 ns |  3.25 |    0.28 |      - |         - |          NA |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 1024   | ?          |    33.57 ns |  0.900 ns |  2.654 ns |    32.04 ns |  1.01 |    0.11 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 1024   | ?          |    94.36 ns |  0.141 ns |  0.110 ns |    94.35 ns |  2.83 |    0.20 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 1024   | ?          |    96.34 ns |  0.941 ns |  0.786 ns |    95.97 ns |  2.89 |    0.21 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 1024   | ?          |   339.36 ns |  4.310 ns |  3.821 ns |   338.22 ns | 10.17 |    0.74 |      - |         - |          NA |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 4096   | ?          |    32.19 ns |  0.351 ns |  0.328 ns |    32.05 ns |  1.00 |    0.01 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 4096   | ?          |   227.02 ns |  1.543 ns |  1.368 ns |   226.53 ns |  7.05 |    0.08 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 4096   | ?          |   238.22 ns |  1.676 ns |  1.486 ns |   238.23 ns |  7.40 |    0.09 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 4096   | ?          | 1,134.29 ns | 10.081 ns |  8.936 ns | 1,131.00 ns | 35.24 |    0.44 |      - |         - |          NA |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 UrlBenchmarks            | Ada_Basic_CanParse                | W1           | ?      | ?          |    31.11 ns |  0.094 ns |  0.079 ns |    31.09 ns |  0.24 |    0.00 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Basic_T1_SpanIn_SpanOut       | W1           | ?      | ?          |    92.28 ns |  0.118 ns |  0.092 ns |    92.29 ns |  0.70 |    0.00 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Basic_T1_ReadEveryComponent   | W1           | ?      | ?          |   100.93 ns |  0.405 ns |  0.338 ns |   100.84 ns |  0.77 |    0.00 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Basic_T2_SpanIn_StringOut     | W1           | ?      | ?          |   108.28 ns |  1.219 ns |  1.081 ns |   107.82 ns |  0.82 |    0.01 | 0.0114 |      72 B |        0.25 |
 UrlBenchmarks            | SystemUri_Basic                   | W1           | ?      | ?          |   131.52 ns |  0.961 ns |  0.750 ns |   131.44 ns |  1.00 |    0.01 | 0.0458 |     288 B |        1.00 |
 UrlBenchmarks            | Ada_Basic_T3_StringIn_StringOut   | W1           | ?      | ?          |   140.07 ns |  1.446 ns |  1.282 ns |   140.38 ns |  1.07 |    0.01 | 0.0114 |      72 B |        0.25 |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 UrlBenchmarks            | Ada_Complex_T1_SpanIn_SpanOut     | W2           | ?      | ?          |   964.39 ns | 11.504 ns |  9.606 ns |   959.99 ns |  0.87 |    0.01 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Complex_T1_Normalize          | W2           | ?      | ?          |   970.34 ns |  7.520 ns |  6.666 ns |   971.02 ns |  0.88 |    0.01 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Complex_T3_StringIn_StringOut | W2           | ?      | ?          | 1,027.38 ns |  5.296 ns |  4.423 ns | 1,027.03 ns |  0.93 |    0.00 | 0.0610 |     392 B |        0.18 |
 UrlBenchmarks            | SystemUri_Complex                 | W2           | ?      | ?          | 1,104.34 ns |  4.102 ns |  3.202 ns | 1,103.00 ns |  1.00 |    0.00 | 0.3433 |    2160 B |        1.00 |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 BatchBenchmarks          | Ada_ReadAll                       | W3 full read | ?      | ?          |   262.90 ns |  5.101 ns | 11.303 ns |   260.73 ns |  0.83 |    0.11 |      - |         - |        0.00 |
 BatchBenchmarks          | SystemUri_ReadAll                 | W3 full read | ?      | ?          |   321.23 ns | 15.150 ns | 43.468 ns |   310.48 ns |  1.02 |    0.19 | 0.0591 |     371 B |        1.00 |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 BatchBenchmarks          | Ada_ExtractHostname               | W3 hostname  | ?      | ?          |   242.03 ns |  4.319 ns |  3.829 ns |   240.87 ns |  0.92 |    0.05 |      - |         - |        0.00 |
 BatchBenchmarks          | SystemUri_ExtractHostname         | W3 hostname  | ?      | ?          |   264.19 ns |  5.275 ns | 13.425 ns |   262.89 ns |  1.00 |    0.07 | 0.0347 |     218 B |        1.00 |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 BatchBenchmarks          | SystemUri_Validate                | W3 validate  | ?      | ?          |    93.88 ns |  1.075 ns |  0.953 ns |    93.57 ns |  1.00 |    0.01 | 0.0144 |      91 B |        1.00 |
 BatchBenchmarks          | Ada_Validate                      | W3 validate  | ?      | ?          |   112.25 ns |  0.934 ns |  0.729 ns |   112.09 ns |  1.20 |    0.01 |      - |         - |        0.00 |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 AllocationCostBenchmarks | CanParse                          | W4           | ?      | 100        |    84.23 ns |  1.290 ns |  1.144 ns |    84.24 ns |  1.00 |    0.02 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 100        |   253.44 ns |  5.050 ns |  5.816 ns |   252.14 ns |  3.01 |    0.08 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 100        |   257.85 ns |  5.032 ns |  7.054 ns |   255.38 ns |  3.06 |    0.09 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 100        |   263.37 ns |  4.934 ns |  4.615 ns |   261.04 ns |  3.13 |    0.07 |      - |         - |          NA |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 AllocationCostBenchmarks | CanParse                          | W4           | ?      | 10000      |    93.66 ns |  0.769 ns |  0.642 ns |    93.45 ns |  1.00 |    0.01 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 10000      |   288.63 ns |  2.559 ns |  2.268 ns |   288.25 ns |  3.08 |    0.03 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 10000      |   289.97 ns |  5.685 ns |  7.782 ns |   287.78 ns |  3.10 |    0.08 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 10000      |   298.03 ns |  4.729 ns |  3.692 ns |   297.08 ns |  3.18 |    0.04 |      - |         - |          NA |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 AllocationCostBenchmarks | CanParse                          | W4           | ?      | 200000     |    94.78 ns |  1.566 ns |  1.388 ns |    94.42 ns |  1.00 |    0.02 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 200000     |   294.37 ns | 11.179 ns | 32.961 ns |   291.23 ns |  3.11 |    0.35 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 200000     |   296.66 ns |  9.196 ns | 27.115 ns |   293.54 ns |  3.13 |    0.29 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 200000     |   302.17 ns |  5.893 ns | 15.628 ns |   301.70 ns |  3.19 |    0.17 |      - |         - |          NA |

```

BenchmarkDotNet v0.15.2, macOS Sonoma 14.8.7 (23J520) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 10.0.400
  [Host]     : .NET 10.0.11 (10.0.1126.37416), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 10.0.11 (10.0.1126.37416), Arm64 RyuJIT AdvSIMD


```
| Type                     | Method                            | Categories   | Length | WorkingSet | Mean        | Error     | StdDev    | Median      | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------- |---------------------------------- |------------- |------- |----------- |------------:|----------:|----------:|------------:|------:|--------:|-------:|----------:|------------:|
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 16     | ?          |    32.38 ns |  0.670 ns |  0.847 ns |    32.17 ns |  1.00 |    0.04 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 16     | ?          |    43.57 ns |  0.104 ns |  0.081 ns |    43.59 ns |  1.35 |    0.03 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 16     | ?          |    48.48 ns |  0.186 ns |  0.156 ns |    48.46 ns |  1.50 |    0.04 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 16     | ?          |    55.21 ns |  1.381 ns |  4.005 ns |    55.50 ns |  1.71 |    0.13 |      - |         - |          NA |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 64     | ?          |    32.13 ns |  0.594 ns |  0.556 ns |    31.87 ns |  1.00 |    0.02 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 64     | ?          |    45.90 ns |  0.053 ns |  0.045 ns |    45.90 ns |  1.43 |    0.02 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 64     | ?          |    48.99 ns |  0.689 ns |  0.611 ns |    48.70 ns |  1.53 |    0.03 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 64     | ?          |    59.50 ns |  0.281 ns |  0.249 ns |    59.48 ns |  1.85 |    0.03 |      - |         - |          NA |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 170    | ?          |    32.16 ns |  0.281 ns |  0.249 ns |    32.07 ns |  1.00 |    0.01 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 170    | ?          |    53.81 ns |  0.508 ns |  0.475 ns |    53.60 ns |  1.67 |    0.02 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 170    | ?          |    61.71 ns |  1.265 ns |  3.128 ns |    61.19 ns |  1.92 |    0.10 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 170    | ?          |   102.02 ns |  1.955 ns |  4.647 ns |   101.63 ns |  3.17 |    0.15 |      - |         - |          NA |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 256    | ?          |    40.71 ns |  0.838 ns |  2.236 ns |    40.47 ns |  1.00 |    0.08 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 256    | ?          |    59.93 ns |  1.243 ns |  3.444 ns |    60.29 ns |  1.48 |    0.12 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 256    | ?          |    79.52 ns |  1.799 ns |  5.305 ns |    79.21 ns |  1.96 |    0.17 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 256    | ?          |   132.07 ns |  3.183 ns |  9.134 ns |   132.01 ns |  3.25 |    0.28 |      - |         - |          NA |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 1024   | ?          |    33.57 ns |  0.900 ns |  2.654 ns |    32.04 ns |  1.01 |    0.11 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 1024   | ?          |    94.36 ns |  0.141 ns |  0.110 ns |    94.35 ns |  2.83 |    0.20 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 1024   | ?          |    96.34 ns |  0.941 ns |  0.786 ns |    95.97 ns |  2.89 |    0.21 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 1024   | ?          |   339.36 ns |  4.310 ns |  3.821 ns |   338.22 ns | 10.17 |    0.74 |      - |         - |          NA |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 4096   | ?          |    32.19 ns |  0.351 ns |  0.328 ns |    32.05 ns |  1.00 |    0.01 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 4096   | ?          |   227.02 ns |  1.543 ns |  1.368 ns |   226.53 ns |  7.05 |    0.08 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 4096   | ?          |   238.22 ns |  1.676 ns |  1.486 ns |   238.23 ns |  7.40 |    0.09 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 4096   | ?          | 1,134.29 ns | 10.081 ns |  8.936 ns | 1,131.00 ns | 35.24 |    0.44 |      - |         - |          NA |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| UrlBenchmarks            | Ada_Basic_CanParse                | W1           | ?      | ?          |    31.11 ns |  0.094 ns |  0.079 ns |    31.09 ns |  0.24 |    0.00 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Basic_T1_SpanIn_SpanOut       | W1           | ?      | ?          |    92.28 ns |  0.118 ns |  0.092 ns |    92.29 ns |  0.70 |    0.00 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Basic_T1_ReadEveryComponent   | W1           | ?      | ?          |   100.93 ns |  0.405 ns |  0.338 ns |   100.84 ns |  0.77 |    0.00 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Basic_T2_SpanIn_StringOut     | W1           | ?      | ?          |   108.28 ns |  1.219 ns |  1.081 ns |   107.82 ns |  0.82 |    0.01 | 0.0114 |      72 B |        0.25 |
| UrlBenchmarks            | SystemUri_Basic                   | W1           | ?      | ?          |   131.52 ns |  0.961 ns |  0.750 ns |   131.44 ns |  1.00 |    0.01 | 0.0458 |     288 B |        1.00 |
| UrlBenchmarks            | Ada_Basic_T3_StringIn_StringOut   | W1           | ?      | ?          |   140.07 ns |  1.446 ns |  1.282 ns |   140.38 ns |  1.07 |    0.01 | 0.0114 |      72 B |        0.25 |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| UrlBenchmarks            | Ada_Complex_T1_SpanIn_SpanOut     | W2           | ?      | ?          |   964.39 ns | 11.504 ns |  9.606 ns |   959.99 ns |  0.87 |    0.01 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Complex_T1_Normalize          | W2           | ?      | ?          |   970.34 ns |  7.520 ns |  6.666 ns |   971.02 ns |  0.88 |    0.01 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Complex_T3_StringIn_StringOut | W2           | ?      | ?          | 1,027.38 ns |  5.296 ns |  4.423 ns | 1,027.03 ns |  0.93 |    0.00 | 0.0610 |     392 B |        0.18 |
| UrlBenchmarks            | SystemUri_Complex                 | W2           | ?      | ?          | 1,104.34 ns |  4.102 ns |  3.202 ns | 1,103.00 ns |  1.00 |    0.00 | 0.3433 |    2160 B |        1.00 |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| BatchBenchmarks          | Ada_ReadAll                       | W3 full read | ?      | ?          |   262.90 ns |  5.101 ns | 11.303 ns |   260.73 ns |  0.83 |    0.11 |      - |         - |        0.00 |
| BatchBenchmarks          | SystemUri_ReadAll                 | W3 full read | ?      | ?          |   321.23 ns | 15.150 ns | 43.468 ns |   310.48 ns |  1.02 |    0.19 | 0.0591 |     371 B |        1.00 |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| BatchBenchmarks          | Ada_ExtractHostname               | W3 hostname  | ?      | ?          |   242.03 ns |  4.319 ns |  3.829 ns |   240.87 ns |  0.92 |    0.05 |      - |         - |        0.00 |
| BatchBenchmarks          | SystemUri_ExtractHostname         | W3 hostname  | ?      | ?          |   264.19 ns |  5.275 ns | 13.425 ns |   262.89 ns |  1.00 |    0.07 | 0.0347 |     218 B |        1.00 |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| BatchBenchmarks          | SystemUri_Validate                | W3 validate  | ?      | ?          |    93.88 ns |  1.075 ns |  0.953 ns |    93.57 ns |  1.00 |    0.01 | 0.0144 |      91 B |        1.00 |
| BatchBenchmarks          | Ada_Validate                      | W3 validate  | ?      | ?          |   112.25 ns |  0.934 ns |  0.729 ns |   112.09 ns |  1.20 |    0.01 |      - |         - |        0.00 |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| AllocationCostBenchmarks | CanParse                          | W4           | ?      | 100        |    84.23 ns |  1.290 ns |  1.144 ns |    84.24 ns |  1.00 |    0.02 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 100        |   253.44 ns |  5.050 ns |  5.816 ns |   252.14 ns |  3.01 |    0.08 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 100        |   257.85 ns |  5.032 ns |  7.054 ns |   255.38 ns |  3.06 |    0.09 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 100        |   263.37 ns |  4.934 ns |  4.615 ns |   261.04 ns |  3.13 |    0.07 |      - |         - |          NA |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| AllocationCostBenchmarks | CanParse                          | W4           | ?      | 10000      |    93.66 ns |  0.769 ns |  0.642 ns |    93.45 ns |  1.00 |    0.01 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 10000      |   288.63 ns |  2.559 ns |  2.268 ns |   288.25 ns |  3.08 |    0.03 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 10000      |   289.97 ns |  5.685 ns |  7.782 ns |   287.78 ns |  3.10 |    0.08 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 10000      |   298.03 ns |  4.729 ns |  3.692 ns |   297.08 ns |  3.18 |    0.04 |      - |         - |          NA |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| AllocationCostBenchmarks | CanParse                          | W4           | ?      | 200000     |    94.78 ns |  1.566 ns |  1.388 ns |    94.42 ns |  1.00 |    0.02 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 200000     |   294.37 ns | 11.179 ns | 32.961 ns |   291.23 ns |  3.11 |    0.35 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 200000     |   296.66 ns |  9.196 ns | 27.115 ns |   293.54 ns |  3.13 |    0.29 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 200000     |   302.17 ns |  5.893 ns | 15.628 ns |   301.70 ns |  3.19 |    0.17 |      - |         - |          NA |

