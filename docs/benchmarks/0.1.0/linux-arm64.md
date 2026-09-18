# linux-arm64

Hardware: arm64, Linux, GitHub hosted.

Ratios are comparable within this file. Absolute nanoseconds are not comparable
with another platform's file, because the hardware differs.


BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), Arm64 RyuJIT armv8.0-a


 Type                     | Method                            | Categories   | Length | WorkingSet | Mean        | Error    | StdDev   | Median      | Ratio | Gen0   | Allocated | Alloc Ratio |
------------------------- |---------------------------------- |------------- |------- |----------- |------------:|---------:|---------:|------------:|------:|-------:|----------:|------------:|
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 16     | ?          |    51.28 ns | 0.007 ns | 0.006 ns |    51.28 ns |  1.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 16     | ?          |    72.04 ns | 0.021 ns | 0.019 ns |    72.05 ns |  1.40 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 16     | ?          |    74.41 ns | 0.026 ns | 0.023 ns |    74.41 ns |  1.45 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 16     | ?          |    81.41 ns | 0.015 ns | 0.013 ns |    81.41 ns |  1.59 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 64     | ?          |    51.39 ns | 0.017 ns | 0.015 ns |    51.39 ns |  1.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 64     | ?          |    73.10 ns | 0.033 ns | 0.031 ns |    73.09 ns |  1.42 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 64     | ?          |    83.65 ns | 0.029 ns | 0.026 ns |    83.65 ns |  1.63 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 64     | ?          |    85.61 ns | 0.016 ns | 0.015 ns |    85.60 ns |  1.67 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 170    | ?          |    51.37 ns | 0.016 ns | 0.015 ns |    51.37 ns |  1.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 170    | ?          |    76.81 ns | 0.027 ns | 0.025 ns |    76.81 ns |  1.50 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 170    | ?          |    89.20 ns | 0.019 ns | 0.018 ns |    89.20 ns |  1.74 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 170    | ?          |   122.09 ns | 0.026 ns | 0.023 ns |   122.09 ns |  2.38 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 256    | ?          |    52.10 ns | 0.007 ns | 0.007 ns |    52.10 ns |  1.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 256    | ?          |    95.58 ns | 0.025 ns | 0.024 ns |    95.58 ns |  1.83 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 256    | ?          |   105.78 ns | 0.033 ns | 0.031 ns |   105.79 ns |  2.03 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 256    | ?          |   153.09 ns | 0.019 ns | 0.017 ns |   153.08 ns |  2.94 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 1024   | ?          |    51.39 ns | 0.014 ns | 0.011 ns |    51.39 ns |  1.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 1024   | ?          |   132.87 ns | 0.075 ns | 0.070 ns |   132.87 ns |  2.59 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 1024   | ?          |   147.90 ns | 0.023 ns | 0.022 ns |   147.90 ns |  2.88 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 1024   | ?          |   454.07 ns | 0.153 ns | 0.143 ns |   454.04 ns |  8.84 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 4096   | ?          |    51.33 ns | 0.016 ns | 0.015 ns |    51.33 ns |  1.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 4096   | ?          |   291.77 ns | 0.060 ns | 0.056 ns |   291.77 ns |  5.68 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 4096   | ?          |   303.93 ns | 0.132 ns | 0.117 ns |   303.89 ns |  5.92 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 4096   | ?          | 1,560.85 ns | 0.123 ns | 0.109 ns | 1,560.87 ns | 30.41 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
 UrlBenchmarks            | Ada_Basic_CanParse                | W0 validate  | ?      | ?          |    52.11 ns | 0.014 ns | 0.013 ns |    52.11 ns |  0.69 |      - |         - |        0.00 |
 UrlBenchmarks            | SystemUri_Basic_Validate          | W0 validate  | ?      | ?          |    75.69 ns | 0.159 ns | 0.149 ns |    75.71 ns |  1.00 | 0.0008 |      56 B |        1.00 |
                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
 UrlBenchmarks            | Ada_Basic_T1_SpanIn_SpanOut       | W1           | ?      | ?          |   111.89 ns | 0.190 ns | 0.169 ns |   111.93 ns |  0.51 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Basic_T1_ReadEveryComponent   | W1           | ?      | ?          |   128.85 ns | 0.245 ns | 0.229 ns |   128.90 ns |  0.59 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Basic_T2_SpanIn_StringOut     | W1           | ?      | ?          |   134.71 ns | 0.196 ns | 0.164 ns |   134.67 ns |  0.62 | 0.0010 |      72 B |        0.25 |
 UrlBenchmarks            | Ada_Basic_T3_StringIn_StringOut   | W1           | ?      | ?          |   160.41 ns | 0.290 ns | 0.257 ns |   160.41 ns |  0.73 | 0.0010 |      72 B |        0.25 |
 UrlBenchmarks            | SystemUri_Basic                   | W1           | ?      | ?          |   218.40 ns | 0.687 ns | 0.643 ns |   218.64 ns |  1.00 | 0.0043 |     288 B |        1.00 |
                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
 UrlBenchmarks            | Ada_Complex_T1_Normalize          | W2           | ?      | ?          | 1,011.59 ns | 0.522 ns | 0.489 ns | 1,011.72 ns |  0.63 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Complex_T1_SpanIn_SpanOut     | W2           | ?      | ?          | 1,026.83 ns | 0.289 ns | 0.270 ns | 1,026.79 ns |  0.64 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Complex_T3_StringIn_StringOut | W2           | ?      | ?          | 1,161.57 ns | 0.601 ns | 0.502 ns | 1,161.59 ns |  0.72 | 0.0057 |     392 B |        0.18 |
 UrlBenchmarks            | SystemUri_Complex                 | W2           | ?      | ?          | 1,603.73 ns | 2.250 ns | 1.879 ns | 1,603.59 ns |  1.00 | 0.0305 |    2160 B |        1.00 |
                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
 BatchBenchmarks          | Ada_ReadAll                       | W3 full read | ?      | ?          |   252.51 ns | 0.067 ns | 0.056 ns |   252.50 ns |  0.66 |      - |         - |        0.00 |
 BatchBenchmarks          | SystemUri_ReadAll                 | W3 full read | ?      | ?          |   380.19 ns | 0.335 ns | 0.280 ns |   380.10 ns |  1.00 | 0.0054 |     371 B |        1.00 |
                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
 BatchBenchmarks          | Ada_ExtractHostname               | W3 hostname  | ?      | ?          |   247.40 ns | 0.329 ns | 0.292 ns |   247.52 ns |  0.79 |      - |         - |        0.00 |
 BatchBenchmarks          | SystemUri_ExtractHostname         | W3 hostname  | ?      | ?          |   313.31 ns | 0.388 ns | 0.344 ns |   313.31 ns |  1.00 | 0.0029 |     218 B |        1.00 |
                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
 BatchBenchmarks          | SystemUri_Validate                | W3 validate  | ?      | ?          |   121.24 ns | 0.108 ns | 0.096 ns |   121.24 ns |  1.00 | 0.0013 |      91 B |        1.00 |
 BatchBenchmarks          | Ada_Validate                      | W3 validate  | ?      | ?          |   138.70 ns | 0.083 ns | 0.078 ns |   138.67 ns |  1.14 |      - |         - |        0.00 |
                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
 AllocationCostBenchmarks | CanParse                          | W4           | ?      | 100        |   104.66 ns | 0.103 ns | 0.096 ns |   104.63 ns |  1.00 |      - |         - |          NA |
 AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 100        |   215.73 ns | 0.218 ns | 0.182 ns |   215.73 ns |  2.06 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 100        |   233.92 ns | 0.320 ns | 0.299 ns |   233.90 ns |  2.24 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 100        |   238.25 ns | 0.226 ns | 0.189 ns |   238.27 ns |  2.28 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 100        |   245.99 ns | 0.122 ns | 0.108 ns |   246.00 ns |  2.35 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
 AllocationCostBenchmarks | CanParse                          | W4           | ?      | 10000      |   111.51 ns | 0.169 ns | 0.150 ns |   111.54 ns |  1.00 |      - |         - |          NA |
 AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 10000      |   239.58 ns | 0.123 ns | 0.109 ns |   239.56 ns |  2.15 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 10000      |   251.25 ns | 0.296 ns | 0.277 ns |   251.28 ns |  2.25 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 10000      |   254.33 ns | 0.109 ns | 0.091 ns |   254.37 ns |  2.28 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 10000      |   263.58 ns | 0.168 ns | 0.157 ns |   263.63 ns |  2.36 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
 AllocationCostBenchmarks | CanParse                          | W4           | ?      | 200000     |   111.17 ns | 0.046 ns | 0.039 ns |   111.18 ns |  1.00 |      - |         - |          NA |
 AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 200000     |   240.05 ns | 0.115 ns | 0.102 ns |   240.09 ns |  2.16 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 200000     |   252.16 ns | 0.124 ns | 0.104 ns |   252.17 ns |  2.27 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 200000     |   255.46 ns | 0.120 ns | 0.112 ns |   255.48 ns |  2.30 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 200000     |   264.36 ns | 0.154 ns | 0.144 ns |   264.35 ns |  2.38 |      - |         - |          NA |

```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), Arm64 RyuJIT armv8.0-a


```
| Type                     | Method                            | Categories   | Length | WorkingSet | Mean        | Error    | StdDev   | Median      | Ratio | Gen0   | Allocated | Alloc Ratio |
|------------------------- |---------------------------------- |------------- |------- |----------- |------------:|---------:|---------:|------------:|------:|-------:|----------:|------------:|
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 16     | ?          |    51.28 ns | 0.007 ns | 0.006 ns |    51.28 ns |  1.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 16     | ?          |    72.04 ns | 0.021 ns | 0.019 ns |    72.05 ns |  1.40 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 16     | ?          |    74.41 ns | 0.026 ns | 0.023 ns |    74.41 ns |  1.45 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 16     | ?          |    81.41 ns | 0.015 ns | 0.013 ns |    81.41 ns |  1.59 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 64     | ?          |    51.39 ns | 0.017 ns | 0.015 ns |    51.39 ns |  1.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 64     | ?          |    73.10 ns | 0.033 ns | 0.031 ns |    73.09 ns |  1.42 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 64     | ?          |    83.65 ns | 0.029 ns | 0.026 ns |    83.65 ns |  1.63 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 64     | ?          |    85.61 ns | 0.016 ns | 0.015 ns |    85.60 ns |  1.67 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 170    | ?          |    51.37 ns | 0.016 ns | 0.015 ns |    51.37 ns |  1.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 170    | ?          |    76.81 ns | 0.027 ns | 0.025 ns |    76.81 ns |  1.50 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 170    | ?          |    89.20 ns | 0.019 ns | 0.018 ns |    89.20 ns |  1.74 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 170    | ?          |   122.09 ns | 0.026 ns | 0.023 ns |   122.09 ns |  2.38 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 256    | ?          |    52.10 ns | 0.007 ns | 0.007 ns |    52.10 ns |  1.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 256    | ?          |    95.58 ns | 0.025 ns | 0.024 ns |    95.58 ns |  1.83 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 256    | ?          |   105.78 ns | 0.033 ns | 0.031 ns |   105.79 ns |  2.03 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 256    | ?          |   153.09 ns | 0.019 ns | 0.017 ns |   153.08 ns |  2.94 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 1024   | ?          |    51.39 ns | 0.014 ns | 0.011 ns |    51.39 ns |  1.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 1024   | ?          |   132.87 ns | 0.075 ns | 0.070 ns |   132.87 ns |  2.59 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 1024   | ?          |   147.90 ns | 0.023 ns | 0.022 ns |   147.90 ns |  2.88 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 1024   | ?          |   454.07 ns | 0.153 ns | 0.143 ns |   454.04 ns |  8.84 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 4096   | ?          |    51.33 ns | 0.016 ns | 0.015 ns |    51.33 ns |  1.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 4096   | ?          |   291.77 ns | 0.060 ns | 0.056 ns |   291.77 ns |  5.68 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 4096   | ?          |   303.93 ns | 0.132 ns | 0.117 ns |   303.89 ns |  5.92 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 4096   | ?          | 1,560.85 ns | 0.123 ns | 0.109 ns | 1,560.87 ns | 30.41 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
| UrlBenchmarks            | Ada_Basic_CanParse                | W0 validate  | ?      | ?          |    52.11 ns | 0.014 ns | 0.013 ns |    52.11 ns |  0.69 |      - |         - |        0.00 |
| UrlBenchmarks            | SystemUri_Basic_Validate          | W0 validate  | ?      | ?          |    75.69 ns | 0.159 ns | 0.149 ns |    75.71 ns |  1.00 | 0.0008 |      56 B |        1.00 |
|                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
| UrlBenchmarks            | Ada_Basic_T1_SpanIn_SpanOut       | W1           | ?      | ?          |   111.89 ns | 0.190 ns | 0.169 ns |   111.93 ns |  0.51 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Basic_T1_ReadEveryComponent   | W1           | ?      | ?          |   128.85 ns | 0.245 ns | 0.229 ns |   128.90 ns |  0.59 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Basic_T2_SpanIn_StringOut     | W1           | ?      | ?          |   134.71 ns | 0.196 ns | 0.164 ns |   134.67 ns |  0.62 | 0.0010 |      72 B |        0.25 |
| UrlBenchmarks            | Ada_Basic_T3_StringIn_StringOut   | W1           | ?      | ?          |   160.41 ns | 0.290 ns | 0.257 ns |   160.41 ns |  0.73 | 0.0010 |      72 B |        0.25 |
| UrlBenchmarks            | SystemUri_Basic                   | W1           | ?      | ?          |   218.40 ns | 0.687 ns | 0.643 ns |   218.64 ns |  1.00 | 0.0043 |     288 B |        1.00 |
|                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
| UrlBenchmarks            | Ada_Complex_T1_Normalize          | W2           | ?      | ?          | 1,011.59 ns | 0.522 ns | 0.489 ns | 1,011.72 ns |  0.63 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Complex_T1_SpanIn_SpanOut     | W2           | ?      | ?          | 1,026.83 ns | 0.289 ns | 0.270 ns | 1,026.79 ns |  0.64 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Complex_T3_StringIn_StringOut | W2           | ?      | ?          | 1,161.57 ns | 0.601 ns | 0.502 ns | 1,161.59 ns |  0.72 | 0.0057 |     392 B |        0.18 |
| UrlBenchmarks            | SystemUri_Complex                 | W2           | ?      | ?          | 1,603.73 ns | 2.250 ns | 1.879 ns | 1,603.59 ns |  1.00 | 0.0305 |    2160 B |        1.00 |
|                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
| BatchBenchmarks          | Ada_ReadAll                       | W3 full read | ?      | ?          |   252.51 ns | 0.067 ns | 0.056 ns |   252.50 ns |  0.66 |      - |         - |        0.00 |
| BatchBenchmarks          | SystemUri_ReadAll                 | W3 full read | ?      | ?          |   380.19 ns | 0.335 ns | 0.280 ns |   380.10 ns |  1.00 | 0.0054 |     371 B |        1.00 |
|                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
| BatchBenchmarks          | Ada_ExtractHostname               | W3 hostname  | ?      | ?          |   247.40 ns | 0.329 ns | 0.292 ns |   247.52 ns |  0.79 |      - |         - |        0.00 |
| BatchBenchmarks          | SystemUri_ExtractHostname         | W3 hostname  | ?      | ?          |   313.31 ns | 0.388 ns | 0.344 ns |   313.31 ns |  1.00 | 0.0029 |     218 B |        1.00 |
|                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
| BatchBenchmarks          | SystemUri_Validate                | W3 validate  | ?      | ?          |   121.24 ns | 0.108 ns | 0.096 ns |   121.24 ns |  1.00 | 0.0013 |      91 B |        1.00 |
| BatchBenchmarks          | Ada_Validate                      | W3 validate  | ?      | ?          |   138.70 ns | 0.083 ns | 0.078 ns |   138.67 ns |  1.14 |      - |         - |        0.00 |
|                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
| AllocationCostBenchmarks | CanParse                          | W4           | ?      | 100        |   104.66 ns | 0.103 ns | 0.096 ns |   104.63 ns |  1.00 |      - |         - |          NA |
| AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 100        |   215.73 ns | 0.218 ns | 0.182 ns |   215.73 ns |  2.06 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 100        |   233.92 ns | 0.320 ns | 0.299 ns |   233.90 ns |  2.24 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 100        |   238.25 ns | 0.226 ns | 0.189 ns |   238.27 ns |  2.28 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 100        |   245.99 ns | 0.122 ns | 0.108 ns |   246.00 ns |  2.35 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
| AllocationCostBenchmarks | CanParse                          | W4           | ?      | 10000      |   111.51 ns | 0.169 ns | 0.150 ns |   111.54 ns |  1.00 |      - |         - |          NA |
| AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 10000      |   239.58 ns | 0.123 ns | 0.109 ns |   239.56 ns |  2.15 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 10000      |   251.25 ns | 0.296 ns | 0.277 ns |   251.28 ns |  2.25 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 10000      |   254.33 ns | 0.109 ns | 0.091 ns |   254.37 ns |  2.28 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 10000      |   263.58 ns | 0.168 ns | 0.157 ns |   263.63 ns |  2.36 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |        |           |             |
| AllocationCostBenchmarks | CanParse                          | W4           | ?      | 200000     |   111.17 ns | 0.046 ns | 0.039 ns |   111.18 ns |  1.00 |      - |         - |          NA |
| AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 200000     |   240.05 ns | 0.115 ns | 0.102 ns |   240.09 ns |  2.16 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 200000     |   252.16 ns | 0.124 ns | 0.104 ns |   252.17 ns |  2.27 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 200000     |   255.46 ns | 0.120 ns | 0.112 ns |   255.48 ns |  2.30 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 200000     |   264.36 ns | 0.154 ns | 0.144 ns |   264.35 ns |  2.38 |      - |         - |          NA |

