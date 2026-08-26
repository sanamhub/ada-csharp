# linux-arm64

Hardware: arm64, Linux, GitHub hosted.

Ratios are comparable within this file. Absolute nanoseconds are not comparable
with another platform's file, because the hardware differs.


BenchmarkDotNet v0.15.2, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
.NET SDK 10.0.400
  [Host]     : .NET 10.0.11 (10.0.1126.37416), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 10.0.11 (10.0.1126.37416), Arm64 RyuJIT AdvSIMD


 Type                     | Method                            | Categories   | Length | WorkingSet | Mean        | Error    | StdDev   | Median      | Ratio | Gen0   | Allocated | Alloc Ratio |
------------------------- |---------------------------------- |------------- |------- |----------- |------------:|---------:|---------:|------------:|------:|-------:|----------:|------------:|
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 16     | ?          |    51.27 ns | 0.014 ns | 0.013 ns |    51.27 ns |  1.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 16     | ?          |    74.96 ns | 0.035 ns | 0.031 ns |    74.95 ns |  1.46 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 16     | ?          |    76.87 ns | 0.051 ns | 0.047 ns |    76.86 ns |  1.50 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 16     | ?          |    81.53 ns | 0.020 ns | 0.019 ns |    81.53 ns |  1.59 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 64     | ?          |    51.37 ns | 0.012 ns | 0.012 ns |    51.36 ns |  1.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 64     | ?          |    74.00 ns | 0.019 ns | 0.016 ns |    73.99 ns |  1.44 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 64     | ?          |    83.93 ns | 0.023 ns | 0.021 ns |    83.94 ns |  1.63 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 64     | ?          |    90.20 ns | 0.019 ns | 0.017 ns |    90.19 ns |  1.76 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 170    | ?          |    52.10 ns | 0.012 ns | 0.012 ns |    52.10 ns |  1.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 170    | ?          |    84.39 ns | 0.015 ns | 0.013 ns |    84.38 ns |  1.62 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 170    | ?          |    89.34 ns | 0.022 ns | 0.021 ns |    89.35 ns |  1.71 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 170    | ?          |   124.14 ns | 0.019 ns | 0.018 ns |   124.14 ns |  2.38 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 256    | ?          |    51.60 ns | 0.012 ns | 0.011 ns |    51.60 ns |  1.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 256    | ?          |    93.86 ns | 0.019 ns | 0.017 ns |    93.87 ns |  1.82 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 256    | ?          |   107.35 ns | 0.033 ns | 0.030 ns |   107.35 ns |  2.08 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 256    | ?          |   158.04 ns | 0.013 ns | 0.011 ns |   158.04 ns |  3.06 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 1024   | ?          |    51.33 ns | 0.008 ns | 0.008 ns |    51.33 ns |  1.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 1024   | ?          |   134.02 ns | 0.057 ns | 0.053 ns |   134.01 ns |  2.61 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 1024   | ?          |   144.31 ns | 0.045 ns | 0.042 ns |   144.30 ns |  2.81 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 1024   | ?          |   453.34 ns | 0.049 ns | 0.041 ns |   453.33 ns |  8.83 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 4096   | ?          |    52.12 ns | 0.014 ns | 0.013 ns |    52.12 ns |  1.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 4096   | ?          |   295.50 ns | 0.069 ns | 0.064 ns |   295.50 ns |  5.67 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 4096   | ?          |   305.01 ns | 0.058 ns | 0.049 ns |   305.01 ns |  5.85 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 4096   | ?          | 1,559.63 ns | 0.218 ns | 0.193 ns | 1,559.61 ns | 29.92 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
 UrlBenchmarks            | Ada_Basic_CanParse                | W1           | ?      | ?          |    51.59 ns | 0.006 ns | 0.005 ns |    51.59 ns |  0.24 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Basic_T1_SpanIn_SpanOut       | W1           | ?      | ?          |   113.45 ns | 0.028 ns | 0.025 ns |   113.45 ns |  0.52 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Basic_T1_ReadEveryComponent   | W1           | ?      | ?          |   129.40 ns | 0.194 ns | 0.162 ns |   129.44 ns |  0.60 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Basic_T2_SpanIn_StringOut     | W1           | ?      | ?          |   134.98 ns | 0.058 ns | 0.048 ns |   134.98 ns |  0.62 | 0.0010 |      72 B |        0.25 |
 UrlBenchmarks            | Ada_Basic_T3_StringIn_StringOut   | W1           | ?      | ?          |   160.80 ns | 0.275 ns | 0.244 ns |   160.73 ns |  0.74 | 0.0010 |      72 B |        0.25 |
 UrlBenchmarks            | SystemUri_Basic                   | W1           | ?      | ?          |   216.66 ns | 0.212 ns | 0.177 ns |   216.66 ns |  1.00 | 0.0043 |     288 B |        1.00 |
                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
 UrlBenchmarks            | Ada_Complex_T1_Normalize          | W2           | ?      | ?          | 1,016.62 ns | 0.702 ns | 0.657 ns | 1,016.75 ns |  0.63 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Complex_T1_SpanIn_SpanOut     | W2           | ?      | ?          | 1,025.80 ns | 0.266 ns | 0.249 ns | 1,025.83 ns |  0.64 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Complex_T3_StringIn_StringOut | W2           | ?      | ?          | 1,168.56 ns | 1.181 ns | 0.986 ns | 1,168.33 ns |  0.73 | 0.0057 |     392 B |        0.18 |
 UrlBenchmarks            | SystemUri_Complex                 | W2           | ?      | ?          | 1,607.51 ns | 1.694 ns | 1.502 ns | 1,607.68 ns |  1.00 | 0.0305 |    2160 B |        1.00 |
                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
 BatchBenchmarks          | Ada_ReadAll                       | W3 full read | ?      | ?          |   252.48 ns | 0.192 ns | 0.180 ns |   252.50 ns |  0.66 |      - |         - |        0.00 |
 BatchBenchmarks          | SystemUri_ReadAll                 | W3 full read | ?      | ?          |   382.30 ns | 0.548 ns | 0.458 ns |   382.08 ns |  1.00 | 0.0054 |     371 B |        1.00 |
                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
 BatchBenchmarks          | Ada_ExtractHostname               | W3 hostname  | ?      | ?          |   247.49 ns | 0.303 ns | 0.284 ns |   247.64 ns |  0.79 |      - |         - |        0.00 |
 BatchBenchmarks          | SystemUri_ExtractHostname         | W3 hostname  | ?      | ?          |   313.84 ns | 0.208 ns | 0.162 ns |   313.85 ns |  1.00 | 0.0029 |     218 B |        1.00 |
                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
 BatchBenchmarks          | SystemUri_Validate                | W3 validate  | ?      | ?          |   121.25 ns | 0.113 ns | 0.094 ns |   121.23 ns |  1.00 | 0.0013 |      91 B |        1.00 |
 BatchBenchmarks          | Ada_Validate                      | W3 validate  | ?      | ?          |   138.85 ns | 0.408 ns | 0.381 ns |   138.62 ns |  1.15 |      - |         - |        0.00 |
                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
 AllocationCostBenchmarks | CanParse                          | W4           | ?      | 100        |   104.65 ns | 0.124 ns | 0.103 ns |   104.68 ns |  1.00 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 100        |   234.48 ns | 0.560 ns | 0.523 ns |   234.39 ns |  2.24 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 100        |   237.52 ns | 0.267 ns | 0.237 ns |   237.44 ns |  2.27 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 100        |   248.13 ns | 0.693 ns | 0.648 ns |   248.03 ns |  2.37 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
 AllocationCostBenchmarks | CanParse                          | W4           | ?      | 10000      |   111.48 ns | 0.264 ns | 0.247 ns |   111.56 ns |  1.00 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 10000      |   252.01 ns | 0.449 ns | 0.420 ns |   251.81 ns |  2.26 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 10000      |   253.88 ns | 0.132 ns | 0.117 ns |   253.86 ns |  2.28 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 10000      |   263.79 ns | 0.187 ns | 0.175 ns |   263.79 ns |  2.37 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
 AllocationCostBenchmarks | CanParse                          | W4           | ?      | 200000     |   111.18 ns | 0.084 ns | 0.065 ns |   111.15 ns |  1.00 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 200000     |   252.69 ns | 0.070 ns | 0.058 ns |   252.69 ns |  2.27 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 200000     |   255.01 ns | 0.102 ns | 0.096 ns |   255.01 ns |  2.29 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 200000     |   264.92 ns | 0.426 ns | 0.398 ns |   265.04 ns |  2.38 |      - |         - |          NA |

```

BenchmarkDotNet v0.15.2, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
.NET SDK 10.0.400
  [Host]     : .NET 10.0.11 (10.0.1126.37416), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 10.0.11 (10.0.1126.37416), Arm64 RyuJIT AdvSIMD


```
| Type                     | Method                            | Categories   | Length | WorkingSet | Mean        | Error    | StdDev   | Median      | Ratio | Gen0   | Allocated | Alloc Ratio |
|------------------------- |---------------------------------- |------------- |------- |----------- |------------:|---------:|---------:|------------:|------:|-------:|----------:|------------:|
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 16     | ?          |    51.27 ns | 0.014 ns | 0.013 ns |    51.27 ns |  1.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 16     | ?          |    74.96 ns | 0.035 ns | 0.031 ns |    74.95 ns |  1.46 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 16     | ?          |    76.87 ns | 0.051 ns | 0.047 ns |    76.86 ns |  1.50 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 16     | ?          |    81.53 ns | 0.020 ns | 0.019 ns |    81.53 ns |  1.59 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 64     | ?          |    51.37 ns | 0.012 ns | 0.012 ns |    51.36 ns |  1.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 64     | ?          |    74.00 ns | 0.019 ns | 0.016 ns |    73.99 ns |  1.44 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 64     | ?          |    83.93 ns | 0.023 ns | 0.021 ns |    83.94 ns |  1.63 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 64     | ?          |    90.20 ns | 0.019 ns | 0.017 ns |    90.19 ns |  1.76 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 170    | ?          |    52.10 ns | 0.012 ns | 0.012 ns |    52.10 ns |  1.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 170    | ?          |    84.39 ns | 0.015 ns | 0.013 ns |    84.38 ns |  1.62 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 170    | ?          |    89.34 ns | 0.022 ns | 0.021 ns |    89.35 ns |  1.71 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 170    | ?          |   124.14 ns | 0.019 ns | 0.018 ns |   124.14 ns |  2.38 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 256    | ?          |    51.60 ns | 0.012 ns | 0.011 ns |    51.60 ns |  1.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 256    | ?          |    93.86 ns | 0.019 ns | 0.017 ns |    93.87 ns |  1.82 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 256    | ?          |   107.35 ns | 0.033 ns | 0.030 ns |   107.35 ns |  2.08 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 256    | ?          |   158.04 ns | 0.013 ns | 0.011 ns |   158.04 ns |  3.06 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 1024   | ?          |    51.33 ns | 0.008 ns | 0.008 ns |    51.33 ns |  1.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 1024   | ?          |   134.02 ns | 0.057 ns | 0.053 ns |   134.01 ns |  2.61 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 1024   | ?          |   144.31 ns | 0.045 ns | 0.042 ns |   144.30 ns |  2.81 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 1024   | ?          |   453.34 ns | 0.049 ns | 0.041 ns |   453.33 ns |  8.83 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 4096   | ?          |    52.12 ns | 0.014 ns | 0.013 ns |    52.12 ns |  1.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 4096   | ?          |   295.50 ns | 0.069 ns | 0.064 ns |   295.50 ns |  5.67 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 4096   | ?          |   305.01 ns | 0.058 ns | 0.049 ns |   305.01 ns |  5.85 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 4096   | ?          | 1,559.63 ns | 0.218 ns | 0.193 ns | 1,559.61 ns | 29.92 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
| UrlBenchmarks            | Ada_Basic_CanParse                | W1           | ?      | ?          |    51.59 ns | 0.006 ns | 0.005 ns |    51.59 ns |  0.24 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Basic_T1_SpanIn_SpanOut       | W1           | ?      | ?          |   113.45 ns | 0.028 ns | 0.025 ns |   113.45 ns |  0.52 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Basic_T1_ReadEveryComponent   | W1           | ?      | ?          |   129.40 ns | 0.194 ns | 0.162 ns |   129.44 ns |  0.60 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Basic_T2_SpanIn_StringOut     | W1           | ?      | ?          |   134.98 ns | 0.058 ns | 0.048 ns |   134.98 ns |  0.62 | 0.0010 |      72 B |        0.25 |
| UrlBenchmarks            | Ada_Basic_T3_StringIn_StringOut   | W1           | ?      | ?          |   160.80 ns | 0.275 ns | 0.244 ns |   160.73 ns |  0.74 | 0.0010 |      72 B |        0.25 |
| UrlBenchmarks            | SystemUri_Basic                   | W1           | ?      | ?          |   216.66 ns | 0.212 ns | 0.177 ns |   216.66 ns |  1.00 | 0.0043 |     288 B |        1.00 |
|                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
| UrlBenchmarks            | Ada_Complex_T1_Normalize          | W2           | ?      | ?          | 1,016.62 ns | 0.702 ns | 0.657 ns | 1,016.75 ns |  0.63 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Complex_T1_SpanIn_SpanOut     | W2           | ?      | ?          | 1,025.80 ns | 0.266 ns | 0.249 ns | 1,025.83 ns |  0.64 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Complex_T3_StringIn_StringOut | W2           | ?      | ?          | 1,168.56 ns | 1.181 ns | 0.986 ns | 1,168.33 ns |  0.73 | 0.0057 |     392 B |        0.18 |
| UrlBenchmarks            | SystemUri_Complex                 | W2           | ?      | ?          | 1,607.51 ns | 1.694 ns | 1.502 ns | 1,607.68 ns |  1.00 | 0.0305 |    2160 B |        1.00 |
|                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
| BatchBenchmarks          | Ada_ReadAll                       | W3 full read | ?      | ?          |   252.48 ns | 0.192 ns | 0.180 ns |   252.50 ns |  0.66 |      - |         - |        0.00 |
| BatchBenchmarks          | SystemUri_ReadAll                 | W3 full read | ?      | ?          |   382.30 ns | 0.548 ns | 0.458 ns |   382.08 ns |  1.00 | 0.0054 |     371 B |        1.00 |
|                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
| BatchBenchmarks          | Ada_ExtractHostname               | W3 hostname  | ?      | ?          |   247.49 ns | 0.303 ns | 0.284 ns |   247.64 ns |  0.79 |      - |         - |        0.00 |
| BatchBenchmarks          | SystemUri_ExtractHostname         | W3 hostname  | ?      | ?          |   313.84 ns | 0.208 ns | 0.162 ns |   313.85 ns |  1.00 | 0.0029 |     218 B |        1.00 |
|                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
| BatchBenchmarks          | SystemUri_Validate                | W3 validate  | ?      | ?          |   121.25 ns | 0.113 ns | 0.094 ns |   121.23 ns |  1.00 | 0.0013 |      91 B |        1.00 |
| BatchBenchmarks          | Ada_Validate                      | W3 validate  | ?      | ?          |   138.85 ns | 0.408 ns | 0.381 ns |   138.62 ns |  1.15 |      - |         - |        0.00 |
|                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
| AllocationCostBenchmarks | CanParse                          | W4           | ?      | 100        |   104.65 ns | 0.124 ns | 0.103 ns |   104.68 ns |  1.00 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 100        |   234.48 ns | 0.560 ns | 0.523 ns |   234.39 ns |  2.24 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 100        |   237.52 ns | 0.267 ns | 0.237 ns |   237.44 ns |  2.27 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 100        |   248.13 ns | 0.693 ns | 0.648 ns |   248.03 ns |  2.37 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
| AllocationCostBenchmarks | CanParse                          | W4           | ?      | 10000      |   111.48 ns | 0.264 ns | 0.247 ns |   111.56 ns |  1.00 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 10000      |   252.01 ns | 0.449 ns | 0.420 ns |   251.81 ns |  2.26 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 10000      |   253.88 ns | 0.132 ns | 0.117 ns |   253.86 ns |  2.28 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 10000      |   263.79 ns | 0.187 ns | 0.175 ns |   263.79 ns |  2.37 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
| AllocationCostBenchmarks | CanParse                          | W4           | ?      | 200000     |   111.18 ns | 0.084 ns | 0.065 ns |   111.15 ns |  1.00 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 200000     |   252.69 ns | 0.070 ns | 0.058 ns |   252.69 ns |  2.27 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 200000     |   255.01 ns | 0.102 ns | 0.096 ns |   255.01 ns |  2.29 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 200000     |   264.92 ns | 0.426 ns | 0.398 ns |   265.04 ns |  2.38 |      - |         - |          NA |

