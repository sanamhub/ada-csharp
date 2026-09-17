# linux-x64

Hardware: x64, Linux, GitHub hosted.

Ratios are comparable within this file. Absolute nanoseconds are not comparable
with another platform's file, because the hardware differs.


BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4


 Type                     | Method                            | Categories   | Length | WorkingSet | Mean        | Error    | StdDev   | Median      | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
------------------------- |---------------------------------- |------------- |------- |----------- |------------:|---------:|---------:|------------:|------:|--------:|-------:|----------:|------------:|
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 16     | ?          |    44.08 ns | 0.036 ns | 0.032 ns |    44.08 ns |  1.00 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 16     | ?          |    54.25 ns | 0.032 ns | 0.026 ns |    54.25 ns |  1.23 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 16     | ?          |    55.05 ns | 0.026 ns | 0.022 ns |    55.04 ns |  1.25 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 16     | ?          |    64.35 ns | 0.018 ns | 0.017 ns |    64.35 ns |  1.46 |    0.00 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 64     | ?          |    44.34 ns | 0.027 ns | 0.024 ns |    44.34 ns |  1.00 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 64     | ?          |    56.88 ns | 0.022 ns | 0.017 ns |    56.89 ns |  1.28 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 64     | ?          |    67.18 ns | 0.111 ns | 0.104 ns |    67.15 ns |  1.52 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 64     | ?          |    69.47 ns | 0.028 ns | 0.023 ns |    69.47 ns |  1.57 |    0.00 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 170    | ?          |    45.81 ns | 0.053 ns | 0.047 ns |    45.79 ns |  1.00 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 170    | ?          |    56.68 ns | 0.030 ns | 0.028 ns |    56.69 ns |  1.24 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 170    | ?          |    74.94 ns | 0.082 ns | 0.073 ns |    74.91 ns |  1.64 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 170    | ?          |   104.15 ns | 0.045 ns | 0.038 ns |   104.15 ns |  2.27 |    0.00 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 256    | ?          |    44.32 ns | 0.033 ns | 0.031 ns |    44.33 ns |  1.00 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 256    | ?          |    70.22 ns | 0.041 ns | 0.037 ns |    70.21 ns |  1.58 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 256    | ?          |    71.80 ns | 0.137 ns | 0.114 ns |    71.76 ns |  1.62 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 256    | ?          |   138.04 ns | 0.217 ns | 0.181 ns |   138.01 ns |  3.11 |    0.00 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 1024   | ?          |    46.25 ns | 0.035 ns | 0.031 ns |    46.25 ns |  1.00 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 1024   | ?          |    83.61 ns | 0.036 ns | 0.033 ns |    83.62 ns |  1.81 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 1024   | ?          |    89.43 ns | 0.026 ns | 0.020 ns |    89.43 ns |  1.93 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 1024   | ?          |   423.26 ns | 0.294 ns | 0.275 ns |   423.30 ns |  9.15 |    0.01 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 4096   | ?          |    44.63 ns | 0.049 ns | 0.046 ns |    44.62 ns |  1.00 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 4096   | ?          |   168.27 ns | 0.170 ns | 0.159 ns |   168.20 ns |  3.77 |    0.01 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 4096   | ?          |   171.60 ns | 0.060 ns | 0.053 ns |   171.58 ns |  3.84 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 4096   | ?          | 1,487.10 ns | 0.876 ns | 0.732 ns | 1,487.48 ns | 33.32 |    0.04 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 UrlBenchmarks            | Ada_Basic_CanParse                | W0 validate  | ?      | ?          |    44.43 ns | 0.030 ns | 0.027 ns |    44.42 ns |  0.57 |    0.00 |      - |         - |        0.00 |
 UrlBenchmarks            | SystemUri_Basic_Validate          | W0 validate  | ?      | ?          |    77.84 ns | 0.205 ns | 0.181 ns |    77.81 ns |  1.00 |    0.00 | 0.0021 |      56 B |        1.00 |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 UrlBenchmarks            | Ada_Basic_T1_SpanIn_SpanOut       | W1           | ?      | ?          |    79.24 ns | 0.180 ns | 0.150 ns |    79.18 ns |  0.36 |    0.00 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Basic_T1_ReadEveryComponent   | W1           | ?      | ?          |    97.12 ns | 0.136 ns | 0.114 ns |    97.11 ns |  0.45 |    0.00 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Basic_T2_SpanIn_StringOut     | W1           | ?      | ?          |   109.84 ns | 0.290 ns | 0.257 ns |   109.90 ns |  0.50 |    0.00 | 0.0029 |      72 B |        0.25 |
 UrlBenchmarks            | Ada_Basic_T3_StringIn_StringOut   | W1           | ?      | ?          |   124.03 ns | 0.258 ns | 0.216 ns |   124.08 ns |  0.57 |    0.00 | 0.0029 |      72 B |        0.25 |
 UrlBenchmarks            | SystemUri_Basic                   | W1           | ?      | ?          |   217.53 ns | 0.792 ns | 0.702 ns |   217.44 ns |  1.00 |    0.00 | 0.0114 |     288 B |        1.00 |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 UrlBenchmarks            | Ada_Complex_T1_SpanIn_SpanOut     | W2           | ?      | ?          | 1,159.01 ns | 0.992 ns | 0.829 ns | 1,158.89 ns |  0.66 |    0.00 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Complex_T1_Normalize          | W2           | ?      | ?          | 1,184.94 ns | 1.817 ns | 1.611 ns | 1,185.18 ns |  0.68 |    0.00 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Complex_T3_StringIn_StringOut | W2           | ?      | ?          | 1,340.45 ns | 2.639 ns | 2.469 ns | 1,339.18 ns |  0.77 |    0.00 | 0.0153 |     392 B |        0.18 |
 UrlBenchmarks            | SystemUri_Complex                 | W2           | ?      | ?          | 1,749.53 ns | 6.459 ns | 6.041 ns | 1,748.34 ns |  1.00 |    0.00 | 0.0858 |    2160 B |        1.00 |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 BatchBenchmarks          | Ada_ReadAll                       | W3 full read | ?      | ?          |   249.17 ns | 0.684 ns | 0.606 ns |   249.10 ns |  0.60 |    0.00 |      - |         - |        0.00 |
 BatchBenchmarks          | SystemUri_ReadAll                 | W3 full read | ?      | ?          |   417.76 ns | 0.597 ns | 0.529 ns |   417.77 ns |  1.00 |    0.00 | 0.0146 |     371 B |        1.00 |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 BatchBenchmarks          | Ada_ExtractHostname               | W3 hostname  | ?      | ?          |   247.49 ns | 0.578 ns | 0.512 ns |   247.54 ns |  0.73 |    0.00 |      - |         - |        0.00 |
 BatchBenchmarks          | SystemUri_ExtractHostname         | W3 hostname  | ?      | ?          |   338.14 ns | 0.275 ns | 0.230 ns |   338.07 ns |  1.00 |    0.00 | 0.0083 |     218 B |        1.00 |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 BatchBenchmarks          | SystemUri_Validate                | W3 validate  | ?      | ?          |   132.56 ns | 0.594 ns | 0.526 ns |   132.63 ns |  1.00 |    0.01 | 0.0034 |      91 B |        1.00 |
 BatchBenchmarks          | Ada_Validate                      | W3 validate  | ?      | ?          |   139.91 ns | 0.380 ns | 0.356 ns |   139.85 ns |  1.06 |    0.00 |      - |         - |        0.00 |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 AllocationCostBenchmarks | CanParse                          | W4           | ?      | 100        |    90.60 ns | 0.412 ns | 0.365 ns |    90.47 ns |  1.00 |    0.01 |      - |         - |          NA |
 AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 100        |   196.69 ns | 0.385 ns | 0.341 ns |   196.56 ns |  2.17 |    0.01 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 100        |   210.45 ns | 0.152 ns | 0.127 ns |   210.43 ns |  2.32 |    0.01 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 100        |   211.94 ns | 0.419 ns | 0.327 ns |   211.89 ns |  2.34 |    0.01 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 100        |   226.28 ns | 0.196 ns | 0.174 ns |   226.34 ns |  2.50 |    0.01 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 AllocationCostBenchmarks | CanParse                          | W4           | ?      | 10000      |   106.39 ns | 0.094 ns | 0.083 ns |   106.39 ns |  1.00 |    0.00 |      - |         - |          NA |
 AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 10000      |   247.50 ns | 0.520 ns | 0.486 ns |   247.43 ns |  2.33 |    0.00 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 10000      |   257.80 ns | 0.212 ns | 0.198 ns |   257.80 ns |  2.42 |    0.00 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 10000      |   260.95 ns | 0.581 ns | 0.544 ns |   260.92 ns |  2.45 |    0.01 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 10000      |   273.46 ns | 0.338 ns | 0.316 ns |   273.43 ns |  2.57 |    0.00 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 AllocationCostBenchmarks | CanParse                          | W4           | ?      | 200000     |   107.75 ns | 0.347 ns | 0.308 ns |   107.59 ns |  1.00 |    0.00 |      - |         - |          NA |
 AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 200000     |   250.76 ns | 0.987 ns | 0.771 ns |   250.54 ns |  2.33 |    0.01 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 200000     |   260.86 ns | 0.273 ns | 0.228 ns |   260.80 ns |  2.42 |    0.01 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 200000     |   264.91 ns | 0.163 ns | 0.152 ns |   264.92 ns |  2.46 |    0.01 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 200000     |   274.63 ns | 0.518 ns | 0.459 ns |   274.83 ns |  2.55 |    0.01 |      - |         - |          NA |

```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4


```
| Type                     | Method                            | Categories   | Length | WorkingSet | Mean        | Error    | StdDev   | Median      | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------- |---------------------------------- |------------- |------- |----------- |------------:|---------:|---------:|------------:|------:|--------:|-------:|----------:|------------:|
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 16     | ?          |    44.08 ns | 0.036 ns | 0.032 ns |    44.08 ns |  1.00 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 16     | ?          |    54.25 ns | 0.032 ns | 0.026 ns |    54.25 ns |  1.23 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 16     | ?          |    55.05 ns | 0.026 ns | 0.022 ns |    55.04 ns |  1.25 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 16     | ?          |    64.35 ns | 0.018 ns | 0.017 ns |    64.35 ns |  1.46 |    0.00 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 64     | ?          |    44.34 ns | 0.027 ns | 0.024 ns |    44.34 ns |  1.00 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 64     | ?          |    56.88 ns | 0.022 ns | 0.017 ns |    56.89 ns |  1.28 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 64     | ?          |    67.18 ns | 0.111 ns | 0.104 ns |    67.15 ns |  1.52 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 64     | ?          |    69.47 ns | 0.028 ns | 0.023 ns |    69.47 ns |  1.57 |    0.00 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 170    | ?          |    45.81 ns | 0.053 ns | 0.047 ns |    45.79 ns |  1.00 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 170    | ?          |    56.68 ns | 0.030 ns | 0.028 ns |    56.69 ns |  1.24 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 170    | ?          |    74.94 ns | 0.082 ns | 0.073 ns |    74.91 ns |  1.64 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 170    | ?          |   104.15 ns | 0.045 ns | 0.038 ns |   104.15 ns |  2.27 |    0.00 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 256    | ?          |    44.32 ns | 0.033 ns | 0.031 ns |    44.33 ns |  1.00 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 256    | ?          |    70.22 ns | 0.041 ns | 0.037 ns |    70.21 ns |  1.58 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 256    | ?          |    71.80 ns | 0.137 ns | 0.114 ns |    71.76 ns |  1.62 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 256    | ?          |   138.04 ns | 0.217 ns | 0.181 ns |   138.01 ns |  3.11 |    0.00 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 1024   | ?          |    46.25 ns | 0.035 ns | 0.031 ns |    46.25 ns |  1.00 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 1024   | ?          |    83.61 ns | 0.036 ns | 0.033 ns |    83.62 ns |  1.81 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 1024   | ?          |    89.43 ns | 0.026 ns | 0.020 ns |    89.43 ns |  1.93 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 1024   | ?          |   423.26 ns | 0.294 ns | 0.275 ns |   423.30 ns |  9.15 |    0.01 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 4096   | ?          |    44.63 ns | 0.049 ns | 0.046 ns |    44.62 ns |  1.00 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 4096   | ?          |   168.27 ns | 0.170 ns | 0.159 ns |   168.20 ns |  3.77 |    0.01 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 4096   | ?          |   171.60 ns | 0.060 ns | 0.053 ns |   171.58 ns |  3.84 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 4096   | ?          | 1,487.10 ns | 0.876 ns | 0.732 ns | 1,487.48 ns | 33.32 |    0.04 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| UrlBenchmarks            | Ada_Basic_CanParse                | W0 validate  | ?      | ?          |    44.43 ns | 0.030 ns | 0.027 ns |    44.42 ns |  0.57 |    0.00 |      - |         - |        0.00 |
| UrlBenchmarks            | SystemUri_Basic_Validate          | W0 validate  | ?      | ?          |    77.84 ns | 0.205 ns | 0.181 ns |    77.81 ns |  1.00 |    0.00 | 0.0021 |      56 B |        1.00 |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| UrlBenchmarks            | Ada_Basic_T1_SpanIn_SpanOut       | W1           | ?      | ?          |    79.24 ns | 0.180 ns | 0.150 ns |    79.18 ns |  0.36 |    0.00 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Basic_T1_ReadEveryComponent   | W1           | ?      | ?          |    97.12 ns | 0.136 ns | 0.114 ns |    97.11 ns |  0.45 |    0.00 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Basic_T2_SpanIn_StringOut     | W1           | ?      | ?          |   109.84 ns | 0.290 ns | 0.257 ns |   109.90 ns |  0.50 |    0.00 | 0.0029 |      72 B |        0.25 |
| UrlBenchmarks            | Ada_Basic_T3_StringIn_StringOut   | W1           | ?      | ?          |   124.03 ns | 0.258 ns | 0.216 ns |   124.08 ns |  0.57 |    0.00 | 0.0029 |      72 B |        0.25 |
| UrlBenchmarks            | SystemUri_Basic                   | W1           | ?      | ?          |   217.53 ns | 0.792 ns | 0.702 ns |   217.44 ns |  1.00 |    0.00 | 0.0114 |     288 B |        1.00 |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| UrlBenchmarks            | Ada_Complex_T1_SpanIn_SpanOut     | W2           | ?      | ?          | 1,159.01 ns | 0.992 ns | 0.829 ns | 1,158.89 ns |  0.66 |    0.00 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Complex_T1_Normalize          | W2           | ?      | ?          | 1,184.94 ns | 1.817 ns | 1.611 ns | 1,185.18 ns |  0.68 |    0.00 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Complex_T3_StringIn_StringOut | W2           | ?      | ?          | 1,340.45 ns | 2.639 ns | 2.469 ns | 1,339.18 ns |  0.77 |    0.00 | 0.0153 |     392 B |        0.18 |
| UrlBenchmarks            | SystemUri_Complex                 | W2           | ?      | ?          | 1,749.53 ns | 6.459 ns | 6.041 ns | 1,748.34 ns |  1.00 |    0.00 | 0.0858 |    2160 B |        1.00 |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| BatchBenchmarks          | Ada_ReadAll                       | W3 full read | ?      | ?          |   249.17 ns | 0.684 ns | 0.606 ns |   249.10 ns |  0.60 |    0.00 |      - |         - |        0.00 |
| BatchBenchmarks          | SystemUri_ReadAll                 | W3 full read | ?      | ?          |   417.76 ns | 0.597 ns | 0.529 ns |   417.77 ns |  1.00 |    0.00 | 0.0146 |     371 B |        1.00 |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| BatchBenchmarks          | Ada_ExtractHostname               | W3 hostname  | ?      | ?          |   247.49 ns | 0.578 ns | 0.512 ns |   247.54 ns |  0.73 |    0.00 |      - |         - |        0.00 |
| BatchBenchmarks          | SystemUri_ExtractHostname         | W3 hostname  | ?      | ?          |   338.14 ns | 0.275 ns | 0.230 ns |   338.07 ns |  1.00 |    0.00 | 0.0083 |     218 B |        1.00 |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| BatchBenchmarks          | SystemUri_Validate                | W3 validate  | ?      | ?          |   132.56 ns | 0.594 ns | 0.526 ns |   132.63 ns |  1.00 |    0.01 | 0.0034 |      91 B |        1.00 |
| BatchBenchmarks          | Ada_Validate                      | W3 validate  | ?      | ?          |   139.91 ns | 0.380 ns | 0.356 ns |   139.85 ns |  1.06 |    0.00 |      - |         - |        0.00 |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| AllocationCostBenchmarks | CanParse                          | W4           | ?      | 100        |    90.60 ns | 0.412 ns | 0.365 ns |    90.47 ns |  1.00 |    0.01 |      - |         - |          NA |
| AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 100        |   196.69 ns | 0.385 ns | 0.341 ns |   196.56 ns |  2.17 |    0.01 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 100        |   210.45 ns | 0.152 ns | 0.127 ns |   210.43 ns |  2.32 |    0.01 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 100        |   211.94 ns | 0.419 ns | 0.327 ns |   211.89 ns |  2.34 |    0.01 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 100        |   226.28 ns | 0.196 ns | 0.174 ns |   226.34 ns |  2.50 |    0.01 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| AllocationCostBenchmarks | CanParse                          | W4           | ?      | 10000      |   106.39 ns | 0.094 ns | 0.083 ns |   106.39 ns |  1.00 |    0.00 |      - |         - |          NA |
| AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 10000      |   247.50 ns | 0.520 ns | 0.486 ns |   247.43 ns |  2.33 |    0.00 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 10000      |   257.80 ns | 0.212 ns | 0.198 ns |   257.80 ns |  2.42 |    0.00 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 10000      |   260.95 ns | 0.581 ns | 0.544 ns |   260.92 ns |  2.45 |    0.01 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 10000      |   273.46 ns | 0.338 ns | 0.316 ns |   273.43 ns |  2.57 |    0.00 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| AllocationCostBenchmarks | CanParse                          | W4           | ?      | 200000     |   107.75 ns | 0.347 ns | 0.308 ns |   107.59 ns |  1.00 |    0.00 |      - |         - |          NA |
| AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 200000     |   250.76 ns | 0.987 ns | 0.771 ns |   250.54 ns |  2.33 |    0.01 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 200000     |   260.86 ns | 0.273 ns | 0.228 ns |   260.80 ns |  2.42 |    0.01 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 200000     |   264.91 ns | 0.163 ns | 0.152 ns |   264.92 ns |  2.46 |    0.01 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 200000     |   274.63 ns | 0.518 ns | 0.459 ns |   274.83 ns |  2.55 |    0.01 |      - |         - |          NA |

