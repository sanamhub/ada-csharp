# win-x64

Hardware: x64, Windows, GitHub hosted.

Ratios are comparable within this file. Absolute nanoseconds are not comparable
with another platform's file, because the hardware differs.


BenchmarkDotNet v0.15.2, Windows 10 (10.0.20348.5499) (Hyper-V)
INTEL XEON PLATINUM 8573C 2.30GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.400
  [Host]     : .NET 10.0.11 (10.0.1126.37416), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  DefaultJob : .NET 10.0.11 (10.0.1126.37416), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


 Type                     | Method                            | Categories   | Length | WorkingSet | Mean        | Error    | StdDev   | Median      | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
------------------------- |---------------------------------- |------------- |------- |----------- |------------:|---------:|---------:|------------:|------:|--------:|-------:|----------:|------------:|
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 16     | ?          |    41.69 ns | 0.725 ns | 0.605 ns |    41.70 ns |  1.00 |    0.02 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 16     | ?          |    49.13 ns | 0.180 ns | 0.150 ns |    49.12 ns |  1.18 |    0.02 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 16     | ?          |    50.22 ns | 0.560 ns | 0.524 ns |    50.12 ns |  1.21 |    0.02 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 16     | ?          |    53.10 ns | 0.300 ns | 0.250 ns |    53.10 ns |  1.27 |    0.02 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 64     | ?          |    41.30 ns | 0.360 ns | 0.337 ns |    41.23 ns |  1.00 |    0.01 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 64     | ?          |    50.41 ns | 0.217 ns | 0.203 ns |    50.36 ns |  1.22 |    0.01 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 64     | ?          |    52.95 ns | 0.185 ns | 0.164 ns |    52.97 ns |  1.28 |    0.01 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 64     | ?          |    57.24 ns | 0.361 ns | 0.338 ns |    57.30 ns |  1.39 |    0.01 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 170    | ?          |    41.51 ns | 0.363 ns | 0.340 ns |    41.63 ns |  1.00 |    0.01 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 170    | ?          |    51.93 ns | 0.308 ns | 0.273 ns |    51.90 ns |  1.25 |    0.01 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 170    | ?          |    57.54 ns | 0.240 ns | 0.213 ns |    57.48 ns |  1.39 |    0.01 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 170    | ?          |    79.78 ns | 0.316 ns | 0.280 ns |    79.77 ns |  1.92 |    0.02 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 256    | ?          |    41.37 ns | 0.368 ns | 0.345 ns |    41.28 ns |  1.00 |    0.01 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 256    | ?          |    55.67 ns | 0.167 ns | 0.157 ns |    55.63 ns |  1.35 |    0.01 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 256    | ?          |    65.34 ns | 0.312 ns | 0.292 ns |    65.39 ns |  1.58 |    0.01 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 256    | ?          |   103.67 ns | 0.587 ns | 0.549 ns |   103.48 ns |  2.51 |    0.02 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 1024   | ?          |    41.68 ns | 0.415 ns | 0.388 ns |    41.69 ns |  1.00 |    0.01 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 1024   | ?          |    68.88 ns | 0.135 ns | 0.119 ns |    68.91 ns |  1.65 |    0.02 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 1024   | ?          |    70.71 ns | 0.219 ns | 0.194 ns |    70.66 ns |  1.70 |    0.02 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 1024   | ?          |   296.00 ns | 1.375 ns | 1.149 ns |   295.60 ns |  7.10 |    0.07 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 4096   | ?          |    41.30 ns | 0.457 ns | 0.427 ns |    41.31 ns |  1.00 |    0.01 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 4096   | ?          |   117.65 ns | 0.165 ns | 0.138 ns |   117.62 ns |  2.85 |    0.03 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 4096   | ?          |   120.64 ns | 0.135 ns | 0.126 ns |   120.66 ns |  2.92 |    0.03 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 4096   | ?          |   990.99 ns | 8.150 ns | 7.624 ns |   992.89 ns | 24.00 |    0.30 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 UrlBenchmarks            | Ada_Basic_CanParse                | W1           | ?      | ?          |    40.87 ns | 0.372 ns | 0.330 ns |    40.92 ns |  0.30 |    0.00 |      - |         - |        0.00 |
 UrlBenchmarks            | SystemUri_Basic                   | W1           | ?      | ?          |   135.40 ns | 0.776 ns | 0.648 ns |   135.36 ns |  1.00 |    0.01 | 0.0033 |     288 B |        1.00 |
 UrlBenchmarks            | Ada_Basic_T1_SpanIn_SpanOut       | W1           | ?      | ?          |   136.16 ns | 0.564 ns | 0.500 ns |   136.07 ns |  1.01 |    0.01 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Basic_T1_ReadEveryComponent   | W1           | ?      | ?          |   149.56 ns | 0.521 ns | 0.488 ns |   149.43 ns |  1.10 |    0.01 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Basic_T2_SpanIn_StringOut     | W1           | ?      | ?          |   158.23 ns | 0.920 ns | 0.768 ns |   158.20 ns |  1.17 |    0.01 | 0.0007 |      72 B |        0.25 |
 UrlBenchmarks            | Ada_Basic_T3_StringIn_StringOut   | W1           | ?      | ?          |   171.40 ns | 1.210 ns | 1.010 ns |   171.09 ns |  1.27 |    0.01 | 0.0007 |      72 B |        0.25 |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 UrlBenchmarks            | SystemUri_Complex                 | W2           | ?      | ?          | 1,232.43 ns | 8.921 ns | 8.345 ns | 1,231.50 ns |  1.00 |    0.01 | 0.0248 |    2160 B |        1.00 |
 UrlBenchmarks            | Ada_Complex_T1_Normalize          | W2           | ?      | ?          | 1,391.64 ns | 4.178 ns | 3.704 ns | 1,390.88 ns |  1.13 |    0.01 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Complex_T1_SpanIn_SpanOut     | W2           | ?      | ?          | 1,398.78 ns | 3.245 ns | 3.035 ns | 1,398.72 ns |  1.14 |    0.01 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Complex_T3_StringIn_StringOut | W2           | ?      | ?          | 1,541.47 ns | 8.466 ns | 7.919 ns | 1,538.96 ns |  1.25 |    0.01 | 0.0038 |     392 B |        0.18 |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 BatchBenchmarks          | SystemUri_ReadAll                 | W3 full read | ?      | ?          |   286.98 ns | 1.507 ns | 1.258 ns |   286.62 ns |  1.00 |    0.01 | 0.0044 |     371 B |        1.00 |
 BatchBenchmarks          | Ada_ReadAll                       | W3 full read | ?      | ?          |   345.21 ns | 0.895 ns | 0.747 ns |   345.18 ns |  1.20 |    0.01 |      - |         - |        0.00 |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 BatchBenchmarks          | SystemUri_ExtractHostname         | W3 hostname  | ?      | ?          |   244.12 ns | 2.596 ns | 2.168 ns |   244.24 ns |  1.00 |    0.01 | 0.0024 |     218 B |        1.00 |
 BatchBenchmarks          | Ada_ExtractHostname               | W3 hostname  | ?      | ?          |   338.30 ns | 0.754 ns | 0.705 ns |   338.30 ns |  1.39 |    0.01 |      - |         - |        0.00 |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 BatchBenchmarks          | SystemUri_Validate                | W3 validate  | ?      | ?          |    87.55 ns | 0.890 ns | 0.789 ns |    87.53 ns |  1.00 |    0.01 | 0.0010 |      91 B |        1.00 |
 BatchBenchmarks          | Ada_Validate                      | W3 validate  | ?      | ?          |   173.82 ns | 0.360 ns | 0.301 ns |   173.65 ns |  1.99 |    0.02 |      - |         - |        0.00 |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 AllocationCostBenchmarks | CanParse                          | W4           | ?      | 100        |   106.62 ns | 0.201 ns | 0.178 ns |   106.59 ns |  1.00 |    0.00 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 100        |   310.87 ns | 0.482 ns | 0.377 ns |   310.81 ns |  2.92 |    0.01 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 100        |   317.80 ns | 0.921 ns | 0.816 ns |   317.88 ns |  2.98 |    0.01 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 100        |   330.75 ns | 0.997 ns | 0.832 ns |   330.57 ns |  3.10 |    0.01 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 AllocationCostBenchmarks | CanParse                          | W4           | ?      | 10000      |   118.76 ns | 0.169 ns | 0.149 ns |   118.77 ns |  1.00 |    0.00 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 10000      |   360.72 ns | 0.727 ns | 0.680 ns |   360.53 ns |  3.04 |    0.01 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 10000      |   366.15 ns | 1.746 ns | 1.458 ns |   365.86 ns |  3.08 |    0.01 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 10000      |   374.04 ns | 0.712 ns | 0.595 ns |   374.11 ns |  3.15 |    0.01 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 AllocationCostBenchmarks | CanParse                          | W4           | ?      | 200000     |   119.48 ns | 0.168 ns | 0.141 ns |   119.49 ns |  1.00 |    0.00 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 200000     |   359.76 ns | 0.620 ns | 0.550 ns |   359.74 ns |  3.01 |    0.01 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 200000     |   376.24 ns | 0.692 ns | 0.613 ns |   375.99 ns |  3.15 |    0.01 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 200000     |   378.41 ns | 0.906 ns | 0.757 ns |   378.22 ns |  3.17 |    0.01 |      - |         - |          NA |

```

BenchmarkDotNet v0.15.2, Windows 10 (10.0.20348.5499) (Hyper-V)
INTEL XEON PLATINUM 8573C 2.30GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.400
  [Host]     : .NET 10.0.11 (10.0.1126.37416), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  DefaultJob : .NET 10.0.11 (10.0.1126.37416), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


```
| Type                     | Method                            | Categories   | Length | WorkingSet | Mean        | Error    | StdDev   | Median      | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------- |---------------------------------- |------------- |------- |----------- |------------:|---------:|---------:|------------:|------:|--------:|-------:|----------:|------------:|
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 16     | ?          |    41.69 ns | 0.725 ns | 0.605 ns |    41.70 ns |  1.00 |    0.02 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 16     | ?          |    49.13 ns | 0.180 ns | 0.150 ns |    49.12 ns |  1.18 |    0.02 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 16     | ?          |    50.22 ns | 0.560 ns | 0.524 ns |    50.12 ns |  1.21 |    0.02 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 16     | ?          |    53.10 ns | 0.300 ns | 0.250 ns |    53.10 ns |  1.27 |    0.02 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 64     | ?          |    41.30 ns | 0.360 ns | 0.337 ns |    41.23 ns |  1.00 |    0.01 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 64     | ?          |    50.41 ns | 0.217 ns | 0.203 ns |    50.36 ns |  1.22 |    0.01 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 64     | ?          |    52.95 ns | 0.185 ns | 0.164 ns |    52.97 ns |  1.28 |    0.01 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 64     | ?          |    57.24 ns | 0.361 ns | 0.338 ns |    57.30 ns |  1.39 |    0.01 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 170    | ?          |    41.51 ns | 0.363 ns | 0.340 ns |    41.63 ns |  1.00 |    0.01 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 170    | ?          |    51.93 ns | 0.308 ns | 0.273 ns |    51.90 ns |  1.25 |    0.01 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 170    | ?          |    57.54 ns | 0.240 ns | 0.213 ns |    57.48 ns |  1.39 |    0.01 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 170    | ?          |    79.78 ns | 0.316 ns | 0.280 ns |    79.77 ns |  1.92 |    0.02 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 256    | ?          |    41.37 ns | 0.368 ns | 0.345 ns |    41.28 ns |  1.00 |    0.01 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 256    | ?          |    55.67 ns | 0.167 ns | 0.157 ns |    55.63 ns |  1.35 |    0.01 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 256    | ?          |    65.34 ns | 0.312 ns | 0.292 ns |    65.39 ns |  1.58 |    0.01 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 256    | ?          |   103.67 ns | 0.587 ns | 0.549 ns |   103.48 ns |  2.51 |    0.02 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 1024   | ?          |    41.68 ns | 0.415 ns | 0.388 ns |    41.69 ns |  1.00 |    0.01 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 1024   | ?          |    68.88 ns | 0.135 ns | 0.119 ns |    68.91 ns |  1.65 |    0.02 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 1024   | ?          |    70.71 ns | 0.219 ns | 0.194 ns |    70.66 ns |  1.70 |    0.02 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 1024   | ?          |   296.00 ns | 1.375 ns | 1.149 ns |   295.60 ns |  7.10 |    0.07 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 4096   | ?          |    41.30 ns | 0.457 ns | 0.427 ns |    41.31 ns |  1.00 |    0.01 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 4096   | ?          |   117.65 ns | 0.165 ns | 0.138 ns |   117.62 ns |  2.85 |    0.03 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 4096   | ?          |   120.64 ns | 0.135 ns | 0.126 ns |   120.66 ns |  2.92 |    0.03 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 4096   | ?          |   990.99 ns | 8.150 ns | 7.624 ns |   992.89 ns | 24.00 |    0.30 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| UrlBenchmarks            | Ada_Basic_CanParse                | W1           | ?      | ?          |    40.87 ns | 0.372 ns | 0.330 ns |    40.92 ns |  0.30 |    0.00 |      - |         - |        0.00 |
| UrlBenchmarks            | SystemUri_Basic                   | W1           | ?      | ?          |   135.40 ns | 0.776 ns | 0.648 ns |   135.36 ns |  1.00 |    0.01 | 0.0033 |     288 B |        1.00 |
| UrlBenchmarks            | Ada_Basic_T1_SpanIn_SpanOut       | W1           | ?      | ?          |   136.16 ns | 0.564 ns | 0.500 ns |   136.07 ns |  1.01 |    0.01 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Basic_T1_ReadEveryComponent   | W1           | ?      | ?          |   149.56 ns | 0.521 ns | 0.488 ns |   149.43 ns |  1.10 |    0.01 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Basic_T2_SpanIn_StringOut     | W1           | ?      | ?          |   158.23 ns | 0.920 ns | 0.768 ns |   158.20 ns |  1.17 |    0.01 | 0.0007 |      72 B |        0.25 |
| UrlBenchmarks            | Ada_Basic_T3_StringIn_StringOut   | W1           | ?      | ?          |   171.40 ns | 1.210 ns | 1.010 ns |   171.09 ns |  1.27 |    0.01 | 0.0007 |      72 B |        0.25 |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| UrlBenchmarks            | SystemUri_Complex                 | W2           | ?      | ?          | 1,232.43 ns | 8.921 ns | 8.345 ns | 1,231.50 ns |  1.00 |    0.01 | 0.0248 |    2160 B |        1.00 |
| UrlBenchmarks            | Ada_Complex_T1_Normalize          | W2           | ?      | ?          | 1,391.64 ns | 4.178 ns | 3.704 ns | 1,390.88 ns |  1.13 |    0.01 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Complex_T1_SpanIn_SpanOut     | W2           | ?      | ?          | 1,398.78 ns | 3.245 ns | 3.035 ns | 1,398.72 ns |  1.14 |    0.01 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Complex_T3_StringIn_StringOut | W2           | ?      | ?          | 1,541.47 ns | 8.466 ns | 7.919 ns | 1,538.96 ns |  1.25 |    0.01 | 0.0038 |     392 B |        0.18 |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| BatchBenchmarks          | SystemUri_ReadAll                 | W3 full read | ?      | ?          |   286.98 ns | 1.507 ns | 1.258 ns |   286.62 ns |  1.00 |    0.01 | 0.0044 |     371 B |        1.00 |
| BatchBenchmarks          | Ada_ReadAll                       | W3 full read | ?      | ?          |   345.21 ns | 0.895 ns | 0.747 ns |   345.18 ns |  1.20 |    0.01 |      - |         - |        0.00 |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| BatchBenchmarks          | SystemUri_ExtractHostname         | W3 hostname  | ?      | ?          |   244.12 ns | 2.596 ns | 2.168 ns |   244.24 ns |  1.00 |    0.01 | 0.0024 |     218 B |        1.00 |
| BatchBenchmarks          | Ada_ExtractHostname               | W3 hostname  | ?      | ?          |   338.30 ns | 0.754 ns | 0.705 ns |   338.30 ns |  1.39 |    0.01 |      - |         - |        0.00 |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| BatchBenchmarks          | SystemUri_Validate                | W3 validate  | ?      | ?          |    87.55 ns | 0.890 ns | 0.789 ns |    87.53 ns |  1.00 |    0.01 | 0.0010 |      91 B |        1.00 |
| BatchBenchmarks          | Ada_Validate                      | W3 validate  | ?      | ?          |   173.82 ns | 0.360 ns | 0.301 ns |   173.65 ns |  1.99 |    0.02 |      - |         - |        0.00 |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| AllocationCostBenchmarks | CanParse                          | W4           | ?      | 100        |   106.62 ns | 0.201 ns | 0.178 ns |   106.59 ns |  1.00 |    0.00 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 100        |   310.87 ns | 0.482 ns | 0.377 ns |   310.81 ns |  2.92 |    0.01 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 100        |   317.80 ns | 0.921 ns | 0.816 ns |   317.88 ns |  2.98 |    0.01 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 100        |   330.75 ns | 0.997 ns | 0.832 ns |   330.57 ns |  3.10 |    0.01 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| AllocationCostBenchmarks | CanParse                          | W4           | ?      | 10000      |   118.76 ns | 0.169 ns | 0.149 ns |   118.77 ns |  1.00 |    0.00 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 10000      |   360.72 ns | 0.727 ns | 0.680 ns |   360.53 ns |  3.04 |    0.01 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 10000      |   366.15 ns | 1.746 ns | 1.458 ns |   365.86 ns |  3.08 |    0.01 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 10000      |   374.04 ns | 0.712 ns | 0.595 ns |   374.11 ns |  3.15 |    0.01 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| AllocationCostBenchmarks | CanParse                          | W4           | ?      | 200000     |   119.48 ns | 0.168 ns | 0.141 ns |   119.49 ns |  1.00 |    0.00 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 200000     |   359.76 ns | 0.620 ns | 0.550 ns |   359.74 ns |  3.01 |    0.01 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 200000     |   376.24 ns | 0.692 ns | 0.613 ns |   375.99 ns |  3.15 |    0.01 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 200000     |   378.41 ns | 0.906 ns | 0.757 ns |   378.22 ns |  3.17 |    0.01 |      - |         - |          NA |

