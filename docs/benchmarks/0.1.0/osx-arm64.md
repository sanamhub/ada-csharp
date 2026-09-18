# osx-arm64

Hardware: arm64, Apple Silicon, GitHub hosted.

Ratios are comparable within this file. Absolute nanoseconds are not comparable
with another platform's file, because the hardware differs.


BenchmarkDotNet v0.15.8, macOS Sonoma 14.8.9 (23J631) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), Arm64 RyuJIT armv8.0-a


 Type                     | Method                            | Categories   | Length | WorkingSet | Mean        | Error     | StdDev    | Median      | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
------------------------- |---------------------------------- |------------- |------- |----------- |------------:|----------:|----------:|------------:|------:|--------:|-------:|----------:|------------:|
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 16     | ?          |    32.18 ns |  0.663 ns |  1.442 ns |    31.77 ns |  1.00 |    0.06 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 16     | ?          |    47.11 ns |  0.963 ns |  2.289 ns |    47.17 ns |  1.47 |    0.09 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 16     | ?          |    47.80 ns |  0.746 ns |  0.661 ns |    47.93 ns |  1.49 |    0.07 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 16     | ?          |    49.20 ns |  0.981 ns |  1.130 ns |    49.04 ns |  1.53 |    0.07 |      - |         - |          NA |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 64     | ?          |    34.05 ns |  0.700 ns |  1.856 ns |    34.02 ns |  1.00 |    0.08 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 64     | ?          |    47.28 ns |  0.962 ns |  1.440 ns |    47.10 ns |  1.39 |    0.09 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 64     | ?          |    50.38 ns |  1.036 ns |  1.418 ns |    50.25 ns |  1.48 |    0.09 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 64     | ?          |    60.72 ns |  1.244 ns |  1.221 ns |    61.00 ns |  1.79 |    0.10 |      - |         - |          NA |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 170    | ?          |    32.28 ns |  0.627 ns |  0.586 ns |    32.36 ns |  1.00 |    0.02 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 170    | ?          |    52.44 ns |  1.048 ns |  1.569 ns |    52.71 ns |  1.62 |    0.06 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 170    | ?          |    56.73 ns |  1.132 ns |  1.004 ns |    56.42 ns |  1.76 |    0.04 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 170    | ?          |    90.59 ns |  1.714 ns |  1.519 ns |    90.81 ns |  2.81 |    0.07 |      - |         - |          NA |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 256    | ?          |    32.82 ns |  0.608 ns |  0.928 ns |    32.68 ns |  1.00 |    0.04 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 256    | ?          |    56.87 ns |  1.171 ns |  2.339 ns |    56.45 ns |  1.73 |    0.08 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 256    | ?          |    66.09 ns |  1.047 ns |  0.928 ns |    66.51 ns |  2.02 |    0.06 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 256    | ?          |   117.62 ns |  2.396 ns |  4.730 ns |   116.21 ns |  3.59 |    0.17 |      - |         - |          NA |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 1024   | ?          |    32.74 ns |  0.693 ns |  0.798 ns |    32.54 ns |  1.00 |    0.03 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 1024   | ?          |   101.62 ns |  2.025 ns |  3.599 ns |   102.11 ns |  3.11 |    0.13 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 1024   | ?          |   104.47 ns |  2.130 ns |  4.979 ns |   104.16 ns |  3.19 |    0.17 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 1024   | ?          |   369.33 ns |  7.405 ns |  9.094 ns |   367.84 ns | 11.29 |    0.38 |      - |         - |          NA |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 4096   | ?          |    34.07 ns |  0.709 ns |  1.382 ns |    33.94 ns |  1.00 |    0.06 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 4096   | ?          |   240.70 ns |  4.808 ns |  6.741 ns |   238.22 ns |  7.08 |    0.34 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 4096   | ?          |   246.84 ns |  3.799 ns |  3.367 ns |   245.98 ns |  7.26 |    0.30 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 4096   | ?          | 1,212.03 ns | 21.875 ns | 18.267 ns | 1,204.83 ns | 35.63 |    1.49 |      - |         - |          NA |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 UrlBenchmarks            | Ada_Basic_CanParse                | W0 validate  | ?      | ?          |    33.52 ns |  0.693 ns |  1.037 ns |    33.53 ns |  0.57 |    0.03 |      - |         - |        0.00 |
 UrlBenchmarks            | SystemUri_Basic_Validate          | W0 validate  | ?      | ?          |    58.85 ns |  1.203 ns |  2.107 ns |    58.81 ns |  1.00 |    0.05 | 0.0088 |      56 B |        1.00 |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 UrlBenchmarks            | Ada_Basic_T1_SpanIn_SpanOut       | W1           | ?      | ?          |   100.37 ns |  2.039 ns |  3.929 ns |   100.76 ns |  0.70 |    0.04 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Basic_T1_ReadEveryComponent   | W1           | ?      | ?          |   105.39 ns |  2.125 ns |  3.776 ns |   105.51 ns |  0.74 |    0.04 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Basic_T2_SpanIn_StringOut     | W1           | ?      | ?          |   108.37 ns |  2.170 ns |  3.378 ns |   107.41 ns |  0.76 |    0.04 | 0.0114 |      72 B |        0.25 |
 UrlBenchmarks            | Ada_Basic_T3_StringIn_StringOut   | W1           | ?      | ?          |   130.63 ns |  2.617 ns |  6.116 ns |   128.62 ns |  0.92 |    0.06 | 0.0114 |      72 B |        0.25 |
 UrlBenchmarks            | SystemUri_Basic                   | W1           | ?      | ?          |   142.86 ns |  2.883 ns |  6.388 ns |   143.02 ns |  1.00 |    0.06 | 0.0458 |     288 B |        1.00 |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 UrlBenchmarks            | Ada_Complex_T1_SpanIn_SpanOut     | W2           | ?      | ?          |   942.39 ns | 17.886 ns | 16.731 ns |   946.56 ns |  0.87 |    0.04 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Complex_T1_Normalize          | W2           | ?      | ?          |   973.65 ns | 19.350 ns | 29.549 ns |   970.24 ns |  0.89 |    0.05 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Complex_T3_StringIn_StringOut | W2           | ?      | ?          | 1,032.72 ns | 19.983 ns | 28.014 ns | 1,027.26 ns |  0.95 |    0.05 | 0.0610 |     392 B |        0.18 |
 UrlBenchmarks            | SystemUri_Complex                 | W2           | ?      | ?          | 1,090.58 ns | 21.555 ns | 45.936 ns | 1,077.32 ns |  1.00 |    0.06 | 0.3433 |    2160 B |        1.00 |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 BatchBenchmarks          | Ada_ReadAll                       | W3 full read | ?      | ?          |   255.05 ns |  5.082 ns |  8.766 ns |   254.25 ns |  0.88 |    0.03 |      - |         - |        0.00 |
 BatchBenchmarks          | SystemUri_ReadAll                 | W3 full read | ?      | ?          |   290.75 ns |  5.524 ns |  5.911 ns |   291.49 ns |  1.00 |    0.03 | 0.0591 |     371 B |        1.00 |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 BatchBenchmarks          | SystemUri_ExtractHostname         | W3 hostname  | ?      | ?          |   248.24 ns |  4.579 ns |  4.283 ns |   248.86 ns |  1.00 |    0.02 | 0.0347 |     218 B |        1.00 |
 BatchBenchmarks          | Ada_ExtractHostname               | W3 hostname  | ?      | ?          |   253.40 ns |  5.061 ns |  9.628 ns |   254.45 ns |  1.02 |    0.04 |      - |         - |        0.00 |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 BatchBenchmarks          | SystemUri_Validate                | W3 validate  | ?      | ?          |    95.23 ns |  1.887 ns |  4.259 ns |    95.98 ns |  1.00 |    0.06 | 0.0144 |      91 B |        1.00 |
 BatchBenchmarks          | Ada_Validate                      | W3 validate  | ?      | ?          |   106.64 ns |  1.518 ns |  1.268 ns |   106.12 ns |  1.12 |    0.05 |      - |         - |        0.00 |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 AllocationCostBenchmarks | CanParse                          | W4           | ?      | 100        |    79.80 ns |  1.587 ns |  4.370 ns |    78.62 ns |  1.00 |    0.08 |      - |         - |          NA |
 AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 100        |   203.31 ns |  4.048 ns |  7.503 ns |   203.28 ns |  2.55 |    0.16 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 100        |   228.86 ns |  4.531 ns |  4.238 ns |   229.00 ns |  2.88 |    0.16 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 100        |   236.74 ns |  4.523 ns | 10.392 ns |   235.48 ns |  2.97 |    0.20 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 100        |   238.33 ns |  4.682 ns |  9.773 ns |   237.70 ns |  2.99 |    0.20 |      - |         - |          NA |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 AllocationCostBenchmarks | CanParse                          | W4           | ?      | 10000      |    88.63 ns |  1.714 ns |  4.749 ns |    88.06 ns |  1.00 |    0.08 |      - |         - |          NA |
 AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 10000      |   244.74 ns |  4.860 ns | 10.145 ns |   242.32 ns |  2.77 |    0.19 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 10000      |   254.07 ns |  5.089 ns | 15.005 ns |   249.60 ns |  2.87 |    0.23 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 10000      |   254.20 ns |  4.659 ns |  4.358 ns |   254.51 ns |  2.88 |    0.16 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 10000      |   259.05 ns |  4.993 ns |  6.132 ns |   258.43 ns |  2.93 |    0.17 |      - |         - |          NA |
                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
 AllocationCostBenchmarks | CanParse                          | W4           | ?      | 200000     |    81.47 ns |  1.584 ns |  1.945 ns |    81.71 ns |  1.00 |    0.03 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 200000     |   244.91 ns |  4.744 ns |  5.273 ns |   244.16 ns |  3.01 |    0.09 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 200000     |   248.44 ns |  4.905 ns |  7.035 ns |   248.90 ns |  3.05 |    0.11 |      - |         - |          NA |
 AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 200000     |   252.20 ns |  5.014 ns |  9.294 ns |   250.88 ns |  3.10 |    0.13 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 200000     |   264.97 ns |  5.159 ns |  6.141 ns |   263.50 ns |  3.25 |    0.11 |      - |         - |          NA |

```

BenchmarkDotNet v0.15.8, macOS Sonoma 14.8.9 (23J631) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), Arm64 RyuJIT armv8.0-a


```
| Type                     | Method                            | Categories   | Length | WorkingSet | Mean        | Error     | StdDev    | Median      | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------- |---------------------------------- |------------- |------- |----------- |------------:|----------:|----------:|------------:|------:|--------:|-------:|----------:|------------:|
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 16     | ?          |    32.18 ns |  0.663 ns |  1.442 ns |    31.77 ns |  1.00 |    0.06 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 16     | ?          |    47.11 ns |  0.963 ns |  2.289 ns |    47.17 ns |  1.47 |    0.09 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 16     | ?          |    47.80 ns |  0.746 ns |  0.661 ns |    47.93 ns |  1.49 |    0.07 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 16     | ?          |    49.20 ns |  0.981 ns |  1.130 ns |    49.04 ns |  1.53 |    0.07 |      - |         - |          NA |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 64     | ?          |    34.05 ns |  0.700 ns |  1.856 ns |    34.02 ns |  1.00 |    0.08 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 64     | ?          |    47.28 ns |  0.962 ns |  1.440 ns |    47.10 ns |  1.39 |    0.09 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 64     | ?          |    50.38 ns |  1.036 ns |  1.418 ns |    50.25 ns |  1.48 |    0.09 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 64     | ?          |    60.72 ns |  1.244 ns |  1.221 ns |    61.00 ns |  1.79 |    0.10 |      - |         - |          NA |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 170    | ?          |    32.28 ns |  0.627 ns |  0.586 ns |    32.36 ns |  1.00 |    0.02 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 170    | ?          |    52.44 ns |  1.048 ns |  1.569 ns |    52.71 ns |  1.62 |    0.06 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 170    | ?          |    56.73 ns |  1.132 ns |  1.004 ns |    56.42 ns |  1.76 |    0.04 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 170    | ?          |    90.59 ns |  1.714 ns |  1.519 ns |    90.81 ns |  2.81 |    0.07 |      - |         - |          NA |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 256    | ?          |    32.82 ns |  0.608 ns |  0.928 ns |    32.68 ns |  1.00 |    0.04 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 256    | ?          |    56.87 ns |  1.171 ns |  2.339 ns |    56.45 ns |  1.73 |    0.08 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 256    | ?          |    66.09 ns |  1.047 ns |  0.928 ns |    66.51 ns |  2.02 |    0.06 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 256    | ?          |   117.62 ns |  2.396 ns |  4.730 ns |   116.21 ns |  3.59 |    0.17 |      - |         - |          NA |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 1024   | ?          |    32.74 ns |  0.693 ns |  0.798 ns |    32.54 ns |  1.00 |    0.03 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 1024   | ?          |   101.62 ns |  2.025 ns |  3.599 ns |   102.11 ns |  3.11 |    0.13 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 1024   | ?          |   104.47 ns |  2.130 ns |  4.979 ns |   104.16 ns |  3.19 |    0.17 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 1024   | ?          |   369.33 ns |  7.405 ns |  9.094 ns |   367.84 ns | 11.29 |    0.38 |      - |         - |          NA |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 4096   | ?          |    34.07 ns |  0.709 ns |  1.382 ns |    33.94 ns |  1.00 |    0.06 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 4096   | ?          |   240.70 ns |  4.808 ns |  6.741 ns |   238.22 ns |  7.08 |    0.34 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 4096   | ?          |   246.84 ns |  3.799 ns |  3.367 ns |   245.98 ns |  7.26 |    0.30 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 4096   | ?          | 1,212.03 ns | 21.875 ns | 18.267 ns | 1,204.83 ns | 35.63 |    1.49 |      - |         - |          NA |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| UrlBenchmarks            | Ada_Basic_CanParse                | W0 validate  | ?      | ?          |    33.52 ns |  0.693 ns |  1.037 ns |    33.53 ns |  0.57 |    0.03 |      - |         - |        0.00 |
| UrlBenchmarks            | SystemUri_Basic_Validate          | W0 validate  | ?      | ?          |    58.85 ns |  1.203 ns |  2.107 ns |    58.81 ns |  1.00 |    0.05 | 0.0088 |      56 B |        1.00 |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| UrlBenchmarks            | Ada_Basic_T1_SpanIn_SpanOut       | W1           | ?      | ?          |   100.37 ns |  2.039 ns |  3.929 ns |   100.76 ns |  0.70 |    0.04 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Basic_T1_ReadEveryComponent   | W1           | ?      | ?          |   105.39 ns |  2.125 ns |  3.776 ns |   105.51 ns |  0.74 |    0.04 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Basic_T2_SpanIn_StringOut     | W1           | ?      | ?          |   108.37 ns |  2.170 ns |  3.378 ns |   107.41 ns |  0.76 |    0.04 | 0.0114 |      72 B |        0.25 |
| UrlBenchmarks            | Ada_Basic_T3_StringIn_StringOut   | W1           | ?      | ?          |   130.63 ns |  2.617 ns |  6.116 ns |   128.62 ns |  0.92 |    0.06 | 0.0114 |      72 B |        0.25 |
| UrlBenchmarks            | SystemUri_Basic                   | W1           | ?      | ?          |   142.86 ns |  2.883 ns |  6.388 ns |   143.02 ns |  1.00 |    0.06 | 0.0458 |     288 B |        1.00 |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| UrlBenchmarks            | Ada_Complex_T1_SpanIn_SpanOut     | W2           | ?      | ?          |   942.39 ns | 17.886 ns | 16.731 ns |   946.56 ns |  0.87 |    0.04 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Complex_T1_Normalize          | W2           | ?      | ?          |   973.65 ns | 19.350 ns | 29.549 ns |   970.24 ns |  0.89 |    0.05 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Complex_T3_StringIn_StringOut | W2           | ?      | ?          | 1,032.72 ns | 19.983 ns | 28.014 ns | 1,027.26 ns |  0.95 |    0.05 | 0.0610 |     392 B |        0.18 |
| UrlBenchmarks            | SystemUri_Complex                 | W2           | ?      | ?          | 1,090.58 ns | 21.555 ns | 45.936 ns | 1,077.32 ns |  1.00 |    0.06 | 0.3433 |    2160 B |        1.00 |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| BatchBenchmarks          | Ada_ReadAll                       | W3 full read | ?      | ?          |   255.05 ns |  5.082 ns |  8.766 ns |   254.25 ns |  0.88 |    0.03 |      - |         - |        0.00 |
| BatchBenchmarks          | SystemUri_ReadAll                 | W3 full read | ?      | ?          |   290.75 ns |  5.524 ns |  5.911 ns |   291.49 ns |  1.00 |    0.03 | 0.0591 |     371 B |        1.00 |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| BatchBenchmarks          | SystemUri_ExtractHostname         | W3 hostname  | ?      | ?          |   248.24 ns |  4.579 ns |  4.283 ns |   248.86 ns |  1.00 |    0.02 | 0.0347 |     218 B |        1.00 |
| BatchBenchmarks          | Ada_ExtractHostname               | W3 hostname  | ?      | ?          |   253.40 ns |  5.061 ns |  9.628 ns |   254.45 ns |  1.02 |    0.04 |      - |         - |        0.00 |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| BatchBenchmarks          | SystemUri_Validate                | W3 validate  | ?      | ?          |    95.23 ns |  1.887 ns |  4.259 ns |    95.98 ns |  1.00 |    0.06 | 0.0144 |      91 B |        1.00 |
| BatchBenchmarks          | Ada_Validate                      | W3 validate  | ?      | ?          |   106.64 ns |  1.518 ns |  1.268 ns |   106.12 ns |  1.12 |    0.05 |      - |         - |        0.00 |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| AllocationCostBenchmarks | CanParse                          | W4           | ?      | 100        |    79.80 ns |  1.587 ns |  4.370 ns |    78.62 ns |  1.00 |    0.08 |      - |         - |          NA |
| AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 100        |   203.31 ns |  4.048 ns |  7.503 ns |   203.28 ns |  2.55 |    0.16 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 100        |   228.86 ns |  4.531 ns |  4.238 ns |   229.00 ns |  2.88 |    0.16 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 100        |   236.74 ns |  4.523 ns | 10.392 ns |   235.48 ns |  2.97 |    0.20 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 100        |   238.33 ns |  4.682 ns |  9.773 ns |   237.70 ns |  2.99 |    0.20 |      - |         - |          NA |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| AllocationCostBenchmarks | CanParse                          | W4           | ?      | 10000      |    88.63 ns |  1.714 ns |  4.749 ns |    88.06 ns |  1.00 |    0.08 |      - |         - |          NA |
| AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 10000      |   244.74 ns |  4.860 ns | 10.145 ns |   242.32 ns |  2.77 |    0.19 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 10000      |   254.07 ns |  5.089 ns | 15.005 ns |   249.60 ns |  2.87 |    0.23 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 10000      |   254.20 ns |  4.659 ns |  4.358 ns |   254.51 ns |  2.88 |    0.16 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 10000      |   259.05 ns |  4.993 ns |  6.132 ns |   258.43 ns |  2.93 |    0.17 |      - |         - |          NA |
|                          |                                   |              |        |            |             |           |           |             |       |         |        |           |             |
| AllocationCostBenchmarks | CanParse                          | W4           | ?      | 200000     |    81.47 ns |  1.584 ns |  1.945 ns |    81.71 ns |  1.00 |    0.03 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 200000     |   244.91 ns |  4.744 ns |  5.273 ns |   244.16 ns |  3.01 |    0.09 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 200000     |   248.44 ns |  4.905 ns |  7.035 ns |   248.90 ns |  3.05 |    0.11 |      - |         - |          NA |
| AllocationCostBenchmarks | ReuseHandleAndReadHostname        | W4           | ?      | 200000     |   252.20 ns |  5.014 ns |  9.294 ns |   250.88 ns |  3.10 |    0.13 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 200000     |   264.97 ns |  5.159 ns |  6.141 ns |   263.50 ns |  3.25 |    0.11 |      - |         - |          NA |

