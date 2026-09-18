# win-x64

Hardware: x64, Windows, GitHub hosted.

Ratios are comparable within this file. Absolute nanoseconds are not comparable
with another platform's file, because the hardware differs.


BenchmarkDotNet v0.15.8, Windows 10 (10.0.20348.5622) (Hyper-V)
INTEL XEON PLATINUM 8573C 2.30GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4


 Type                     | Method                            | Categories   | Length | WorkingSet | Mean        | Error     | StdDev    | Median      | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
------------------------- |---------------------------------- |------------- |------- |----------- |------------:|----------:|----------:|------------:|------:|--------:|-------:|----------:|------------:|
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 16     | ?          |    43.86 ns |  0.919 ns |  0.902 ns |    44.06 ns |  1.00 |    0.03 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 16     | ?          |    49.67 ns |  1.002 ns |  1.192 ns |    49.41 ns |  1.13 |    0.04 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 16     | ?          |    50.76 ns |  1.002 ns |  1.154 ns |    50.53 ns |  1.16 |    0.03 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 16     | ?          |    56.26 ns |  0.795 ns |  0.705 ns |    56.31 ns |  1.28 |    0.03 |      - |         - |          NA |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 64     | ?          |    40.67 ns |  0.367 ns |  0.343 ns |    40.60 ns |  1.00 |    0.01 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 64     | ?          |    52.87 ns |  0.534 ns |  0.499 ns |    52.84 ns |  1.30 |    0.02 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 64     | ?          |    57.67 ns |  0.824 ns |  0.688 ns |    57.72 ns |  1.42 |    0.02 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 64     | ?          |    58.57 ns |  0.814 ns |  0.722 ns |    58.59 ns |  1.44 |    0.02 |      - |         - |          NA |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 170    | ?          |    41.65 ns |  0.587 ns |  0.490 ns |    41.69 ns |  1.00 |    0.02 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 170    | ?          |    54.11 ns |  0.714 ns |  0.668 ns |    54.19 ns |  1.30 |    0.02 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 170    | ?          |    64.89 ns |  1.314 ns |  1.165 ns |    64.66 ns |  1.56 |    0.03 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 170    | ?          |    86.82 ns |  1.054 ns |  0.986 ns |    86.87 ns |  2.09 |    0.03 |      - |         - |          NA |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 256    | ?          |    41.52 ns |  0.801 ns |  0.750 ns |    41.34 ns |  1.00 |    0.02 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 256    | ?          |    62.43 ns |  1.035 ns |  0.968 ns |    62.71 ns |  1.50 |    0.03 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 256    | ?          |    64.79 ns |  0.720 ns |  0.673 ns |    64.68 ns |  1.56 |    0.03 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 256    | ?          |   131.86 ns |  2.683 ns |  2.982 ns |   131.63 ns |  3.18 |    0.09 |      - |         - |          NA |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 1024   | ?          |    41.03 ns |  0.431 ns |  0.403 ns |    41.14 ns |  1.00 |    0.01 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 1024   | ?          |    71.23 ns |  0.820 ns |  0.727 ns |    71.16 ns |  1.74 |    0.02 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 1024   | ?          |    75.41 ns |  0.538 ns |  0.477 ns |    75.41 ns |  1.84 |    0.02 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 1024   | ?          |   423.49 ns |  5.659 ns |  5.293 ns |   423.03 ns | 10.32 |    0.16 |      - |         - |          NA |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 4096   | ?          |    41.75 ns |  0.762 ns |  0.676 ns |    41.49 ns |  1.00 |    0.02 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 4096   | ?          |   126.59 ns |  2.546 ns |  2.257 ns |   126.71 ns |  3.03 |    0.07 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 4096   | ?          |   132.78 ns |  1.579 ns |  1.477 ns |   132.27 ns |  3.18 |    0.06 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 4096   | ?          | 1,528.20 ns | 24.741 ns | 23.143 ns | 1,532.78 ns | 36.61 |    0.78 |      - |         - |          NA |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 UrlBenchmarks            | Ada_Basic_CanParse                | W0 validate  | ?      | ?          |    41.12 ns |  0.578 ns |  0.512 ns |    41.04 ns |  0.79 |    0.01 |      - |         - |        0.00 |
 UrlBenchmarks            | SystemUri_Basic_Validate          | W0 validate  | ?      | ?          |    51.75 ns |  0.565 ns |  0.472 ns |    51.80 ns |  1.00 |    0.01 | 0.0007 |      56 B |        1.00 |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 UrlBenchmarks            | Ada_Basic_T1_SpanIn_SpanOut       | W1           | ?      | ?          |   138.58 ns |  2.434 ns |  2.276 ns |   138.41 ns |  0.88 |    0.02 |      - |         - |        0.00 |
 UrlBenchmarks            | SystemUri_Basic                   | W1           | ?      | ?          |   158.15 ns |  2.475 ns |  2.315 ns |   158.50 ns |  1.00 |    0.02 | 0.0033 |     288 B |        1.00 |
 UrlBenchmarks            | Ada_Basic_T1_ReadEveryComponent   | W1           | ?      | ?          |   158.90 ns |  1.943 ns |  1.817 ns |   158.59 ns |  1.00 |    0.02 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Basic_T2_SpanIn_StringOut     | W1           | ?      | ?          |   163.72 ns |  2.974 ns |  2.636 ns |   163.86 ns |  1.04 |    0.02 | 0.0007 |      72 B |        0.25 |
 UrlBenchmarks            | Ada_Basic_T3_StringIn_StringOut   | W1           | ?      | ?          |   176.71 ns |  3.476 ns |  4.640 ns |   177.22 ns |  1.12 |    0.03 | 0.0007 |      72 B |        0.25 |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 UrlBenchmarks            | SystemUri_Complex                 | W2           | ?      | ?          | 1,583.25 ns | 31.484 ns | 73.592 ns | 1,619.04 ns |  1.00 |    0.07 | 0.0248 |    2160 B |        1.00 |
 UrlBenchmarks            | Ada_Complex_T1_Normalize          | W2           | ?      | ?          | 1,586.08 ns | 31.647 ns | 31.082 ns | 1,584.02 ns |  1.00 |    0.05 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Complex_T1_SpanIn_SpanOut     | W2           | ?      | ?          | 1,632.45 ns | 15.769 ns | 14.750 ns | 1,639.70 ns |  1.03 |    0.05 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Complex_T3_StringIn_StringOut | W2           | ?      | ?          | 1,685.52 ns | 33.159 ns | 32.567 ns | 1,693.96 ns |  1.07 |    0.06 | 0.0038 |     392 B |        0.18 |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 BatchBenchmarks          | SystemUri_ReadAll                 | W3 full read | ?      | ?          |   324.05 ns |  4.917 ns |  4.599 ns |   324.25 ns |  1.00 |    0.02 | 0.0044 |     371 B |        1.00 |
 BatchBenchmarks          | Ada_ReadAll                       | W3 full read | ?      | ?          |   353.17 ns |  2.248 ns |  1.755 ns |   352.91 ns |  1.09 |    0.02 |      - |         - |        0.00 |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 BatchBenchmarks          | SystemUri_ExtractHostname         | W3 hostname  | ?      | ?          |   270.62 ns |  4.199 ns |  3.507 ns |   269.76 ns |  1.00 |    0.02 | 0.0024 |     218 B |        1.00 |
 BatchBenchmarks          | Ada_ExtractHostname               | W3 hostname  | ?      | ?          |   361.52 ns |  7.026 ns |  9.618 ns |   360.04 ns |  1.34 |    0.04 |      - |         - |        0.00 |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 BatchBenchmarks          | SystemUri_Validate                | W3 validate  | ?      | ?          |   107.02 ns |  1.198 ns |  1.120 ns |   107.44 ns |  1.00 |    0.01 | 0.0010 |      91 B |        1.00 |
 BatchBenchmarks          | Ada_Validate                      | W3 validate  | ?      | ?          |   195.36 ns |  3.877 ns |  3.807 ns |   195.48 ns |  1.83 |    0.04 |      - |         - |        0.00 |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 AllocationCostBenchmarks | CanParse                          | W4           | ?      | 100        |   112.68 ns |  2.200 ns |  3.615 ns |   111.20 ns |  1.00 |    0.04 |      - |         - |          NA |
 AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 100        |   279.08 ns |  3.649 ns |  3.047 ns |   278.40 ns |  2.48 |    0.08 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 100        |   340.20 ns |  6.714 ns | 11.401 ns |   338.18 ns |  3.02 |    0.14 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 100        |   345.62 ns |  6.009 ns |  6.678 ns |   343.20 ns |  3.07 |    0.11 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 100        |   350.38 ns |  6.952 ns | 13.559 ns |   351.36 ns |  3.11 |    0.15 |      - |         - |          NA |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 AllocationCostBenchmarks | CanParse                          | W4           | ?      | 10000      |   121.42 ns |  2.348 ns |  2.196 ns |   121.56 ns |  1.00 |    0.02 |      - |         - |          NA |
 AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 10000      |   337.13 ns |  6.586 ns |  7.584 ns |   335.12 ns |  2.78 |    0.08 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 10000      |   369.72 ns |  3.831 ns |  3.199 ns |   370.86 ns |  3.05 |    0.06 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 10000      |   373.03 ns |  4.153 ns |  3.885 ns |   373.35 ns |  3.07 |    0.06 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 10000      |   397.41 ns |  7.845 ns |  9.339 ns |   394.63 ns |  3.27 |    0.09 |      - |         - |          NA |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 AllocationCostBenchmarks | CanParse                          | W4           | ?      | 200000     |   128.10 ns |  2.545 ns |  5.849 ns |   126.49 ns |  1.00 |    0.06 |      - |         - |          NA |
 AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 200000     |   332.03 ns |  3.139 ns |  2.936 ns |   332.80 ns |  2.60 |    0.12 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 200000     |   368.80 ns |  4.654 ns |  4.353 ns |   369.32 ns |  2.88 |    0.13 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 200000     |   375.40 ns |  7.502 ns | 10.759 ns |   371.58 ns |  2.94 |    0.15 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 200000     |   393.91 ns |  4.880 ns |  4.564 ns |   395.53 ns |  3.08 |    0.14 |      - |         - |          NA |

```

BenchmarkDotNet v0.15.8, Windows 10 (10.0.20348.5622) (Hyper-V)
INTEL XEON PLATINUM 8573C 2.30GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4


```
| Type                     | Method                            | Categories   | Length | WorkingSet | Mean        | Error     | StdDev    | Median      | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------- |---------------------------------- |------------- |------- |----------- |------------:|----------:|----------:|------------:|------:|--------:|-------:|----------:|------------:|
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 16     | ?          |    43.86 ns |  0.919 ns |  0.902 ns |    44.06 ns |  1.00 |    0.03 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 16     | ?          |    49.67 ns |  1.002 ns |  1.192 ns |    49.41 ns |  1.13 |    0.04 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 16     | ?          |    50.76 ns |  1.002 ns |  1.154 ns |    50.53 ns |  1.16 |    0.03 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 16     | ?          |    56.26 ns |  0.795 ns |  0.705 ns |    56.31 ns |  1.28 |    0.03 |      - |         - |          NA |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 64     | ?          |    40.67 ns |  0.367 ns |  0.343 ns |    40.60 ns |  1.00 |    0.01 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 64     | ?          |    52.87 ns |  0.534 ns |  0.499 ns |    52.84 ns |  1.30 |    0.02 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 64     | ?          |    57.67 ns |  0.824 ns |  0.688 ns |    57.72 ns |  1.42 |    0.02 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 64     | ?          |    58.57 ns |  0.814 ns |  0.722 ns |    58.59 ns |  1.44 |    0.02 |      - |         - |          NA |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 170    | ?          |    41.65 ns |  0.587 ns |  0.490 ns |    41.69 ns |  1.00 |    0.02 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 170    | ?          |    54.11 ns |  0.714 ns |  0.668 ns |    54.19 ns |  1.30 |    0.02 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 170    | ?          |    64.89 ns |  1.314 ns |  1.165 ns |    64.66 ns |  1.56 |    0.03 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 170    | ?          |    86.82 ns |  1.054 ns |  0.986 ns |    86.87 ns |  2.09 |    0.03 |      - |         - |          NA |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 256    | ?          |    41.52 ns |  0.801 ns |  0.750 ns |    41.34 ns |  1.00 |    0.02 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 256    | ?          |    62.43 ns |  1.035 ns |  0.968 ns |    62.71 ns |  1.50 |    0.03 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 256    | ?          |    64.79 ns |  0.720 ns |  0.673 ns |    64.68 ns |  1.56 |    0.03 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 256    | ?          |   131.86 ns |  2.683 ns |  2.982 ns |   131.63 ns |  3.18 |    0.09 |      - |         - |          NA |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 1024   | ?          |    41.03 ns |  0.431 ns |  0.403 ns |    41.14 ns |  1.00 |    0.01 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 1024   | ?          |    71.23 ns |  0.820 ns |  0.727 ns |    71.16 ns |  1.74 |    0.02 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 1024   | ?          |    75.41 ns |  0.538 ns |  0.477 ns |    75.41 ns |  1.84 |    0.02 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 1024   | ?          |   423.49 ns |  5.659 ns |  5.293 ns |   423.03 ns | 10.32 |    0.16 |      - |         - |          NA |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 4096   | ?          |    41.75 ns |  0.762 ns |  0.676 ns |    41.49 ns |  1.00 |    0.02 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 4096   | ?          |   126.59 ns |  2.546 ns |  2.257 ns |   126.71 ns |  3.03 |    0.07 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 4096   | ?          |   132.78 ns |  1.579 ns |  1.477 ns |   132.27 ns |  3.18 |    0.06 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 4096   | ?          | 1,528.20 ns | 24.741 ns | 23.143 ns | 1,532.78 ns | 36.61 |    0.78 |      - |         - |          NA |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| UrlBenchmarks            | Ada_Basic_CanParse                | W0 validate  | ?      | ?          |    41.12 ns |  0.578 ns |  0.512 ns |    41.04 ns |  0.79 |    0.01 |      - |         - |        0.00 |
| UrlBenchmarks            | SystemUri_Basic_Validate          | W0 validate  | ?      | ?          |    51.75 ns |  0.565 ns |  0.472 ns |    51.80 ns |  1.00 |    0.01 | 0.0007 |      56 B |        1.00 |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| UrlBenchmarks            | Ada_Basic_T1_SpanIn_SpanOut       | W1           | ?      | ?          |   138.58 ns |  2.434 ns |  2.276 ns |   138.41 ns |  0.88 |    0.02 |      - |         - |        0.00 |
| UrlBenchmarks            | SystemUri_Basic                   | W1           | ?      | ?          |   158.15 ns |  2.475 ns |  2.315 ns |   158.50 ns |  1.00 |    0.02 | 0.0033 |     288 B |        1.00 |
| UrlBenchmarks            | Ada_Basic_T1_ReadEveryComponent   | W1           | ?      | ?          |   158.90 ns |  1.943 ns |  1.817 ns |   158.59 ns |  1.00 |    0.02 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Basic_T2_SpanIn_StringOut     | W1           | ?      | ?          |   163.72 ns |  2.974 ns |  2.636 ns |   163.86 ns |  1.04 |    0.02 | 0.0007 |      72 B |        0.25 |
| UrlBenchmarks            | Ada_Basic_T3_StringIn_StringOut   | W1           | ?      | ?          |   176.71 ns |  3.476 ns |  4.640 ns |   177.22 ns |  1.12 |    0.03 | 0.0007 |      72 B |        0.25 |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| UrlBenchmarks            | SystemUri_Complex                 | W2           | ?      | ?          | 1,583.25 ns | 31.484 ns | 73.592 ns | 1,619.04 ns |  1.00 |    0.07 | 0.0248 |    2160 B |        1.00 |
| UrlBenchmarks            | Ada_Complex_T1_Normalize          | W2           | ?      | ?          | 1,586.08 ns | 31.647 ns | 31.082 ns | 1,584.02 ns |  1.00 |    0.05 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Complex_T1_SpanIn_SpanOut     | W2           | ?      | ?          | 1,632.45 ns | 15.769 ns | 14.750 ns | 1,639.70 ns |  1.03 |    0.05 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Complex_T3_StringIn_StringOut | W2           | ?      | ?          | 1,685.52 ns | 33.159 ns | 32.567 ns | 1,693.96 ns |  1.07 |    0.06 | 0.0038 |     392 B |        0.18 |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| BatchBenchmarks          | SystemUri_ReadAll                 | W3 full read | ?      | ?          |   324.05 ns |  4.917 ns |  4.599 ns |   324.25 ns |  1.00 |    0.02 | 0.0044 |     371 B |        1.00 |
| BatchBenchmarks          | Ada_ReadAll                       | W3 full read | ?      | ?          |   353.17 ns |  2.248 ns |  1.755 ns |   352.91 ns |  1.09 |    0.02 |      - |         - |        0.00 |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| BatchBenchmarks          | SystemUri_ExtractHostname         | W3 hostname  | ?      | ?          |   270.62 ns |  4.199 ns |  3.507 ns |   269.76 ns |  1.00 |    0.02 | 0.0024 |     218 B |        1.00 |
| BatchBenchmarks          | Ada_ExtractHostname               | W3 hostname  | ?      | ?          |   361.52 ns |  7.026 ns |  9.618 ns |   360.04 ns |  1.34 |    0.04 |      - |         - |        0.00 |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| BatchBenchmarks          | SystemUri_Validate                | W3 validate  | ?      | ?          |   107.02 ns |  1.198 ns |  1.120 ns |   107.44 ns |  1.00 |    0.01 | 0.0010 |      91 B |        1.00 |
| BatchBenchmarks          | Ada_Validate                      | W3 validate  | ?      | ?          |   195.36 ns |  3.877 ns |  3.807 ns |   195.48 ns |  1.83 |    0.04 |      - |         - |        0.00 |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| AllocationCostBenchmarks | CanParse                          | W4           | ?      | 100        |   112.68 ns |  2.200 ns |  3.615 ns |   111.20 ns |  1.00 |    0.04 |      - |         - |          NA |
| AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 100        |   279.08 ns |  3.649 ns |  3.047 ns |   278.40 ns |  2.48 |    0.08 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 100        |   340.20 ns |  6.714 ns | 11.401 ns |   338.18 ns |  3.02 |    0.14 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 100        |   345.62 ns |  6.009 ns |  6.678 ns |   343.20 ns |  3.07 |    0.11 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 100        |   350.38 ns |  6.952 ns | 13.559 ns |   351.36 ns |  3.11 |    0.15 |      - |         - |          NA |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| AllocationCostBenchmarks | CanParse                          | W4           | ?      | 10000      |   121.42 ns |  2.348 ns |  2.196 ns |   121.56 ns |  1.00 |    0.02 |      - |         - |          NA |
| AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 10000      |   337.13 ns |  6.586 ns |  7.584 ns |   335.12 ns |  2.78 |    0.08 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 10000      |   369.72 ns |  3.831 ns |  3.199 ns |   370.86 ns |  3.05 |    0.06 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 10000      |   373.03 ns |  4.153 ns |  3.885 ns |   373.35 ns |  3.07 |    0.06 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 10000      |   397.41 ns |  7.845 ns |  9.339 ns |   394.63 ns |  3.27 |    0.09 |      - |         - |          NA |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| AllocationCostBenchmarks | CanParse                          | W4           | ?      | 200000     |   128.10 ns |  2.545 ns |  5.849 ns |   126.49 ns |  1.00 |    0.06 |      - |         - |          NA |
| AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 200000     |   332.03 ns |  3.139 ns |  2.936 ns |   332.80 ns |  2.60 |    0.12 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 200000     |   368.80 ns |  4.654 ns |  4.353 ns |   369.32 ns |  2.88 |    0.13 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 200000     |   375.40 ns |  7.502 ns | 10.759 ns |   371.58 ns |  2.94 |    0.15 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 200000     |   393.91 ns |  4.880 ns |  4.564 ns |   395.53 ns |  3.08 |    0.14 |      - |         - |          NA |

