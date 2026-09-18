# win-arm64

Hardware: arm64, Windows, GitHub hosted.

Ratios are comparable within this file. Absolute nanoseconds are not comparable
with another platform's file, because the hardware differs.


BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9168/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), Arm64 RyuJIT armv8.0-a


 Type                     | Method                            | Categories   | Length | WorkingSet | Mean        | Error    | StdDev   | Median      | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
------------------------- |---------------------------------- |------------- |------- |----------- |------------:|---------:|---------:|------------:|------:|--------:|-------:|----------:|------------:|
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 16     | ?          |    37.44 ns | 0.025 ns | 0.021 ns |    37.43 ns |  1.00 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 16     | ?          |    55.24 ns | 0.041 ns | 0.039 ns |    55.23 ns |  1.48 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 16     | ?          |    57.63 ns | 0.050 ns | 0.044 ns |    57.61 ns |  1.54 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 16     | ?          |    58.63 ns | 0.052 ns | 0.047 ns |    58.62 ns |  1.57 |    0.00 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 64     | ?          |    37.48 ns | 0.062 ns | 0.058 ns |    37.48 ns |  1.00 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 64     | ?          |    55.43 ns | 0.030 ns | 0.026 ns |    55.41 ns |  1.48 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 64     | ?          |    55.85 ns | 0.031 ns | 0.028 ns |    55.85 ns |  1.49 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 64     | ?          |    69.93 ns | 0.043 ns | 0.040 ns |    69.93 ns |  1.87 |    0.00 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 170    | ?          |    38.07 ns | 0.031 ns | 0.029 ns |    38.07 ns |  1.00 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 170    | ?          |    62.22 ns | 0.045 ns | 0.042 ns |    62.22 ns |  1.63 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 170    | ?          |    63.73 ns | 0.029 ns | 0.026 ns |    63.73 ns |  1.67 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 170    | ?          |   109.11 ns | 0.099 ns | 0.083 ns |   109.08 ns |  2.87 |    0.00 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 256    | ?          |    38.09 ns | 0.029 ns | 0.026 ns |    38.08 ns |  1.00 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 256    | ?          |    66.73 ns | 0.050 ns | 0.047 ns |    66.73 ns |  1.75 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 256    | ?          |    77.50 ns | 0.045 ns | 0.038 ns |    77.49 ns |  2.03 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 256    | ?          |   138.93 ns | 0.047 ns | 0.044 ns |   138.94 ns |  3.65 |    0.00 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 1024   | ?          |    37.47 ns | 0.040 ns | 0.036 ns |    37.48 ns |  1.00 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 1024   | ?          |   106.96 ns | 0.053 ns | 0.050 ns |   106.95 ns |  2.85 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 1024   | ?          |   115.66 ns | 0.049 ns | 0.041 ns |   115.66 ns |  3.09 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 1024   | ?          |   425.80 ns | 0.078 ns | 0.073 ns |   425.80 ns | 11.36 |    0.01 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 4096   | ?          |    37.57 ns | 0.046 ns | 0.043 ns |    37.56 ns |  1.00 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 4096   | ?          |   266.56 ns | 0.179 ns | 0.158 ns |   266.58 ns |  7.10 |    0.01 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 4096   | ?          |   276.69 ns | 0.123 ns | 0.115 ns |   276.66 ns |  7.37 |    0.01 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 4096   | ?          | 1,531.52 ns | 0.388 ns | 0.363 ns | 1,531.37 ns | 40.77 |    0.05 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 UrlBenchmarks            | Ada_Basic_CanParse                | W0 validate  | ?      | ?          |    38.34 ns | 0.041 ns | 0.038 ns |    38.34 ns |  0.54 |    0.00 |      - |         - |        0.00 |
 UrlBenchmarks            | SystemUri_Basic_Validate          | W0 validate  | ?      | ?          |    70.40 ns | 0.175 ns | 0.146 ns |    70.43 ns |  1.00 |    0.00 | 0.0267 |      56 B |        1.00 |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 UrlBenchmarks            | Ada_Basic_T1_SpanIn_SpanOut       | W1           | ?      | ?          |   147.79 ns | 0.173 ns | 0.161 ns |   147.82 ns |  0.72 |    0.00 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Basic_T1_ReadEveryComponent   | W1           | ?      | ?          |   165.56 ns | 0.112 ns | 0.099 ns |   165.56 ns |  0.81 |    0.00 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Basic_T2_SpanIn_StringOut     | W1           | ?      | ?          |   168.64 ns | 0.307 ns | 0.287 ns |   168.61 ns |  0.83 |    0.00 | 0.0343 |      72 B |        0.25 |
 UrlBenchmarks            | Ada_Basic_T3_StringIn_StringOut   | W1           | ?      | ?          |   194.21 ns | 0.362 ns | 0.339 ns |   194.25 ns |  0.95 |    0.00 | 0.0343 |      72 B |        0.25 |
 UrlBenchmarks            | SystemUri_Basic                   | W1           | ?      | ?          |   204.10 ns | 1.029 ns | 0.912 ns |   203.84 ns |  1.00 |    0.01 | 0.1376 |     288 B |        1.00 |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 UrlBenchmarks            | Ada_Complex_T1_SpanIn_SpanOut     | W2           | ?      | ?          | 1,364.02 ns | 2.577 ns | 2.411 ns | 1,363.16 ns |  0.88 |    0.00 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Complex_T1_Normalize          | W2           | ?      | ?          | 1,364.84 ns | 0.853 ns | 0.798 ns | 1,364.74 ns |  0.88 |    0.00 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Complex_T3_StringIn_StringOut | W2           | ?      | ?          | 1,502.44 ns | 2.024 ns | 1.893 ns | 1,502.84 ns |  0.97 |    0.00 | 0.1869 |     392 B |        0.18 |
 UrlBenchmarks            | SystemUri_Complex                 | W2           | ?      | ?          | 1,546.10 ns | 8.146 ns | 7.620 ns | 1,543.77 ns |  1.00 |    0.01 | 1.0319 |    2160 B |        1.00 |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 BatchBenchmarks          | Ada_ReadAll                       | W3 full read | ?      | ?          |   340.34 ns | 0.191 ns | 0.169 ns |   340.37 ns |  0.93 |    0.00 |      - |         - |        0.00 |
 BatchBenchmarks          | SystemUri_ReadAll                 | W3 full read | ?      | ?          |   364.93 ns | 1.406 ns | 1.315 ns |   365.51 ns |  1.00 |    0.00 | 0.1772 |     371 B |        1.00 |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 BatchBenchmarks          | SystemUri_ExtractHostname         | W3 hostname  | ?      | ?          |   303.15 ns | 0.645 ns | 0.603 ns |   303.14 ns |  1.00 |    0.00 | 0.1040 |     218 B |        1.00 |
 BatchBenchmarks          | Ada_ExtractHostname               | W3 hostname  | ?      | ?          |   334.08 ns | 0.222 ns | 0.208 ns |   334.09 ns |  1.10 |    0.00 |      - |         - |        0.00 |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 BatchBenchmarks          | SystemUri_Validate                | W3 validate  | ?      | ?          |   116.88 ns | 0.357 ns | 0.334 ns |   117.00 ns |  1.00 |    0.00 | 0.0435 |      91 B |        1.00 |
 BatchBenchmarks          | Ada_Validate                      | W3 validate  | ?      | ?          |   161.62 ns | 0.128 ns | 0.119 ns |   161.59 ns |  1.38 |    0.00 |      - |         - |        0.00 |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 AllocationCostBenchmarks | CanParse                          | W4           | ?      | 100        |   103.23 ns | 0.224 ns | 0.210 ns |   103.26 ns |  1.00 |    0.00 |      - |         - |          NA |
 AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 100        |   293.91 ns | 0.572 ns | 0.507 ns |   293.76 ns |  2.85 |    0.01 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 100        |   323.32 ns | 0.401 ns | 0.356 ns |   323.30 ns |  3.13 |    0.01 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 100        |   326.29 ns | 0.419 ns | 0.392 ns |   326.35 ns |  3.16 |    0.01 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 100        |   337.64 ns | 0.328 ns | 0.307 ns |   337.65 ns |  3.27 |    0.01 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 AllocationCostBenchmarks | CanParse                          | W4           | ?      | 10000      |   100.82 ns | 0.157 ns | 0.131 ns |   100.86 ns |  1.00 |    0.00 |      - |         - |          NA |
 AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 10000      |   302.97 ns | 0.185 ns | 0.164 ns |   302.99 ns |  3.01 |    0.00 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 10000      |   329.69 ns | 0.252 ns | 0.224 ns |   329.70 ns |  3.27 |    0.00 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 10000      |   334.76 ns | 0.135 ns | 0.120 ns |   334.77 ns |  3.32 |    0.00 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 10000      |   345.07 ns | 0.223 ns | 0.198 ns |   345.09 ns |  3.42 |    0.00 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 AllocationCostBenchmarks | CanParse                          | W4           | ?      | 200000     |   100.81 ns | 0.113 ns | 0.106 ns |   100.83 ns |  1.00 |    0.00 |      - |         - |          NA |
 AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 200000     |   304.63 ns | 0.297 ns | 0.248 ns |   304.59 ns |  3.02 |    0.00 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 200000     |   331.58 ns | 0.270 ns | 0.239 ns |   331.59 ns |  3.29 |    0.00 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 200000     |   335.09 ns | 0.168 ns | 0.140 ns |   335.09 ns |  3.32 |    0.00 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 200000     |   346.61 ns | 0.617 ns | 0.547 ns |   346.36 ns |  3.44 |    0.01 |      - |         - |          NA |

```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9168/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), Arm64 RyuJIT armv8.0-a


```
| Type                     | Method                            | Categories   | Length | WorkingSet | Mean        | Error    | StdDev   | Median      | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------- |---------------------------------- |------------- |------- |----------- |------------:|---------:|---------:|------------:|------:|--------:|-------:|----------:|------------:|
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 16     | ?          |    37.44 ns | 0.025 ns | 0.021 ns |    37.43 ns |  1.00 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 16     | ?          |    55.24 ns | 0.041 ns | 0.039 ns |    55.23 ns |  1.48 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 16     | ?          |    57.63 ns | 0.050 ns | 0.044 ns |    57.61 ns |  1.54 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 16     | ?          |    58.63 ns | 0.052 ns | 0.047 ns |    58.62 ns |  1.57 |    0.00 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 64     | ?          |    37.48 ns | 0.062 ns | 0.058 ns |    37.48 ns |  1.00 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 64     | ?          |    55.43 ns | 0.030 ns | 0.026 ns |    55.41 ns |  1.48 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 64     | ?          |    55.85 ns | 0.031 ns | 0.028 ns |    55.85 ns |  1.49 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 64     | ?          |    69.93 ns | 0.043 ns | 0.040 ns |    69.93 ns |  1.87 |    0.00 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 170    | ?          |    38.07 ns | 0.031 ns | 0.029 ns |    38.07 ns |  1.00 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 170    | ?          |    62.22 ns | 0.045 ns | 0.042 ns |    62.22 ns |  1.63 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 170    | ?          |    63.73 ns | 0.029 ns | 0.026 ns |    63.73 ns |  1.67 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 170    | ?          |   109.11 ns | 0.099 ns | 0.083 ns |   109.08 ns |  2.87 |    0.00 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 256    | ?          |    38.09 ns | 0.029 ns | 0.026 ns |    38.08 ns |  1.00 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 256    | ?          |    66.73 ns | 0.050 ns | 0.047 ns |    66.73 ns |  1.75 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 256    | ?          |    77.50 ns | 0.045 ns | 0.038 ns |    77.49 ns |  2.03 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 256    | ?          |   138.93 ns | 0.047 ns | 0.044 ns |   138.94 ns |  3.65 |    0.00 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 1024   | ?          |    37.47 ns | 0.040 ns | 0.036 ns |    37.48 ns |  1.00 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 1024   | ?          |   106.96 ns | 0.053 ns | 0.050 ns |   106.95 ns |  2.85 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 1024   | ?          |   115.66 ns | 0.049 ns | 0.041 ns |   115.66 ns |  3.09 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 1024   | ?          |   425.80 ns | 0.078 ns | 0.073 ns |   425.80 ns | 11.36 |    0.01 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 4096   | ?          |    37.57 ns | 0.046 ns | 0.043 ns |    37.56 ns |  1.00 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 4096   | ?          |   266.56 ns | 0.179 ns | 0.158 ns |   266.58 ns |  7.10 |    0.01 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 4096   | ?          |   276.69 ns | 0.123 ns | 0.115 ns |   276.66 ns |  7.37 |    0.01 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 4096   | ?          | 1,531.52 ns | 0.388 ns | 0.363 ns | 1,531.37 ns | 40.77 |    0.05 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| UrlBenchmarks            | Ada_Basic_CanParse                | W0 validate  | ?      | ?          |    38.34 ns | 0.041 ns | 0.038 ns |    38.34 ns |  0.54 |    0.00 |      - |         - |        0.00 |
| UrlBenchmarks            | SystemUri_Basic_Validate          | W0 validate  | ?      | ?          |    70.40 ns | 0.175 ns | 0.146 ns |    70.43 ns |  1.00 |    0.00 | 0.0267 |      56 B |        1.00 |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| UrlBenchmarks            | Ada_Basic_T1_SpanIn_SpanOut       | W1           | ?      | ?          |   147.79 ns | 0.173 ns | 0.161 ns |   147.82 ns |  0.72 |    0.00 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Basic_T1_ReadEveryComponent   | W1           | ?      | ?          |   165.56 ns | 0.112 ns | 0.099 ns |   165.56 ns |  0.81 |    0.00 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Basic_T2_SpanIn_StringOut     | W1           | ?      | ?          |   168.64 ns | 0.307 ns | 0.287 ns |   168.61 ns |  0.83 |    0.00 | 0.0343 |      72 B |        0.25 |
| UrlBenchmarks            | Ada_Basic_T3_StringIn_StringOut   | W1           | ?      | ?          |   194.21 ns | 0.362 ns | 0.339 ns |   194.25 ns |  0.95 |    0.00 | 0.0343 |      72 B |        0.25 |
| UrlBenchmarks            | SystemUri_Basic                   | W1           | ?      | ?          |   204.10 ns | 1.029 ns | 0.912 ns |   203.84 ns |  1.00 |    0.01 | 0.1376 |     288 B |        1.00 |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| UrlBenchmarks            | Ada_Complex_T1_SpanIn_SpanOut     | W2           | ?      | ?          | 1,364.02 ns | 2.577 ns | 2.411 ns | 1,363.16 ns |  0.88 |    0.00 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Complex_T1_Normalize          | W2           | ?      | ?          | 1,364.84 ns | 0.853 ns | 0.798 ns | 1,364.74 ns |  0.88 |    0.00 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Complex_T3_StringIn_StringOut | W2           | ?      | ?          | 1,502.44 ns | 2.024 ns | 1.893 ns | 1,502.84 ns |  0.97 |    0.00 | 0.1869 |     392 B |        0.18 |
| UrlBenchmarks            | SystemUri_Complex                 | W2           | ?      | ?          | 1,546.10 ns | 8.146 ns | 7.620 ns | 1,543.77 ns |  1.00 |    0.01 | 1.0319 |    2160 B |        1.00 |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| BatchBenchmarks          | Ada_ReadAll                       | W3 full read | ?      | ?          |   340.34 ns | 0.191 ns | 0.169 ns |   340.37 ns |  0.93 |    0.00 |      - |         - |        0.00 |
| BatchBenchmarks          | SystemUri_ReadAll                 | W3 full read | ?      | ?          |   364.93 ns | 1.406 ns | 1.315 ns |   365.51 ns |  1.00 |    0.00 | 0.1772 |     371 B |        1.00 |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| BatchBenchmarks          | SystemUri_ExtractHostname         | W3 hostname  | ?      | ?          |   303.15 ns | 0.645 ns | 0.603 ns |   303.14 ns |  1.00 |    0.00 | 0.1040 |     218 B |        1.00 |
| BatchBenchmarks          | Ada_ExtractHostname               | W3 hostname  | ?      | ?          |   334.08 ns | 0.222 ns | 0.208 ns |   334.09 ns |  1.10 |    0.00 |      - |         - |        0.00 |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| BatchBenchmarks          | SystemUri_Validate                | W3 validate  | ?      | ?          |   116.88 ns | 0.357 ns | 0.334 ns |   117.00 ns |  1.00 |    0.00 | 0.0435 |      91 B |        1.00 |
| BatchBenchmarks          | Ada_Validate                      | W3 validate  | ?      | ?          |   161.62 ns | 0.128 ns | 0.119 ns |   161.59 ns |  1.38 |    0.00 |      - |         - |        0.00 |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| AllocationCostBenchmarks | CanParse                          | W4           | ?      | 100        |   103.23 ns | 0.224 ns | 0.210 ns |   103.26 ns |  1.00 |    0.00 |      - |         - |          NA |
| AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 100        |   293.91 ns | 0.572 ns | 0.507 ns |   293.76 ns |  2.85 |    0.01 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 100        |   323.32 ns | 0.401 ns | 0.356 ns |   323.30 ns |  3.13 |    0.01 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 100        |   326.29 ns | 0.419 ns | 0.392 ns |   326.35 ns |  3.16 |    0.01 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 100        |   337.64 ns | 0.328 ns | 0.307 ns |   337.65 ns |  3.27 |    0.01 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| AllocationCostBenchmarks | CanParse                          | W4           | ?      | 10000      |   100.82 ns | 0.157 ns | 0.131 ns |   100.86 ns |  1.00 |    0.00 |      - |         - |          NA |
| AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 10000      |   302.97 ns | 0.185 ns | 0.164 ns |   302.99 ns |  3.01 |    0.00 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 10000      |   329.69 ns | 0.252 ns | 0.224 ns |   329.70 ns |  3.27 |    0.00 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 10000      |   334.76 ns | 0.135 ns | 0.120 ns |   334.77 ns |  3.32 |    0.00 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 10000      |   345.07 ns | 0.223 ns | 0.198 ns |   345.09 ns |  3.42 |    0.00 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| AllocationCostBenchmarks | CanParse                          | W4           | ?      | 200000     |   100.81 ns | 0.113 ns | 0.106 ns |   100.83 ns |  1.00 |    0.00 |      - |         - |          NA |
| AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 200000     |   304.63 ns | 0.297 ns | 0.248 ns |   304.59 ns |  3.02 |    0.00 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 200000     |   331.58 ns | 0.270 ns | 0.239 ns |   331.59 ns |  3.29 |    0.00 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 200000     |   335.09 ns | 0.168 ns | 0.140 ns |   335.09 ns |  3.32 |    0.00 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 200000     |   346.61 ns | 0.617 ns | 0.547 ns |   346.36 ns |  3.44 |    0.01 |      - |         - |          NA |

