# linux-x64

Hardware: x64, Linux, GitHub hosted.

Ratios are comparable within this file. Absolute nanoseconds are not comparable
with another platform's file, because the hardware differs.


BenchmarkDotNet v0.15.2, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.400
  [Host]     : .NET 10.0.11 (10.0.1126.37416), X64 RyuJIT AVX2
  DefaultJob : .NET 10.0.11 (10.0.1126.37416), X64 RyuJIT AVX2


 Type                     | Method                            | Categories   | Length | WorkingSet | Mean        | Error    | StdDev   | Median      | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
------------------------- |---------------------------------- |------------- |------- |----------- |------------:|---------:|---------:|------------:|------:|--------:|-------:|----------:|------------:|
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 16     | ?          |    47.35 ns | 0.130 ns | 0.122 ns |    47.32 ns |  1.00 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 16     | ?          |    59.25 ns | 0.044 ns | 0.041 ns |    59.25 ns |  1.25 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 16     | ?          |    59.45 ns | 0.033 ns | 0.026 ns |    59.44 ns |  1.26 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 16     | ?          |    68.90 ns | 0.061 ns | 0.047 ns |    68.90 ns |  1.46 |    0.00 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 64     | ?          |    47.38 ns | 0.023 ns | 0.019 ns |    47.38 ns |  1.00 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 64     | ?          |    59.19 ns | 0.086 ns | 0.076 ns |    59.19 ns |  1.25 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 64     | ?          |    69.06 ns | 0.113 ns | 0.094 ns |    69.00 ns |  1.46 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 64     | ?          |    74.11 ns | 0.069 ns | 0.058 ns |    74.12 ns |  1.56 |    0.00 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 170    | ?          |    47.42 ns | 0.039 ns | 0.032 ns |    47.41 ns |  1.00 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 170    | ?          |    64.26 ns | 0.236 ns | 0.197 ns |    64.27 ns |  1.36 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 170    | ?          |    73.00 ns | 0.063 ns | 0.056 ns |    72.99 ns |  1.54 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 170    | ?          |   111.53 ns | 0.121 ns | 0.101 ns |   111.52 ns |  2.35 |    0.00 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 256    | ?          |    47.41 ns | 0.026 ns | 0.021 ns |    47.42 ns |  1.00 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 256    | ?          |    74.10 ns | 0.153 ns | 0.128 ns |    74.05 ns |  1.56 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 256    | ?          |    80.65 ns | 0.032 ns | 0.028 ns |    80.66 ns |  1.70 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 256    | ?          |   141.43 ns | 0.109 ns | 0.091 ns |   141.45 ns |  2.98 |    0.00 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 1024   | ?          |    47.39 ns | 0.031 ns | 0.027 ns |    47.39 ns |  1.00 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 1024   | ?          |    88.69 ns | 0.060 ns | 0.056 ns |    88.69 ns |  1.87 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 1024   | ?          |    99.61 ns | 0.065 ns | 0.057 ns |    99.60 ns |  2.10 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 1024   | ?          |   432.19 ns | 0.229 ns | 0.214 ns |   432.25 ns |  9.12 |    0.01 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 4096   | ?          |    47.46 ns | 0.014 ns | 0.012 ns |    47.46 ns |  1.00 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 4096   | ?          |   164.67 ns | 0.273 ns | 0.242 ns |   164.75 ns |  3.47 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_Ascii                       |              | 4096   | ?          |   170.71 ns | 0.167 ns | 0.148 ns |   170.70 ns |  3.60 |    0.00 |      - |         - |          NA |
 TranscodeBenchmarks      | Utf16_NonAscii                    |              | 4096   | ?          | 1,534.61 ns | 1.600 ns | 1.249 ns | 1,534.41 ns | 32.34 |    0.03 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 UrlBenchmarks            | Ada_Basic_CanParse                | W1           | ?      | ?          |    47.02 ns | 0.045 ns | 0.042 ns |    47.00 ns |  0.27 |    0.00 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Basic_T1_SpanIn_SpanOut       | W1           | ?      | ?          |    90.21 ns | 0.096 ns | 0.080 ns |    90.20 ns |  0.52 |    0.00 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Basic_T1_ReadEveryComponent   | W1           | ?      | ?          |   103.24 ns | 0.112 ns | 0.093 ns |   103.22 ns |  0.59 |    0.00 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Basic_T2_SpanIn_StringOut     | W1           | ?      | ?          |   112.43 ns | 0.230 ns | 0.192 ns |   112.34 ns |  0.64 |    0.00 | 0.0043 |      72 B |        0.25 |
 UrlBenchmarks            | Ada_Basic_T3_StringIn_StringOut   | W1           | ?      | ?          |   121.85 ns | 0.331 ns | 0.294 ns |   121.77 ns |  0.70 |    0.00 | 0.0043 |      72 B |        0.25 |
 UrlBenchmarks            | SystemUri_Basic                   | W1           | ?      | ?          |   175.14 ns | 0.300 ns | 0.250 ns |   175.05 ns |  1.00 |    0.00 | 0.0172 |     288 B |        1.00 |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 UrlBenchmarks            | Ada_Complex_T1_Normalize          | W2           | ?      | ?          | 1,141.22 ns | 1.179 ns | 0.921 ns | 1,141.40 ns |  0.67 |    0.00 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Complex_T1_SpanIn_SpanOut     | W2           | ?      | ?          | 1,150.31 ns | 4.249 ns | 3.766 ns | 1,149.26 ns |  0.67 |    0.00 |      - |         - |        0.00 |
 UrlBenchmarks            | Ada_Complex_T3_StringIn_StringOut | W2           | ?      | ?          | 1,340.63 ns | 5.492 ns | 4.869 ns | 1,339.68 ns |  0.79 |    0.00 | 0.0229 |     392 B |        0.18 |
 UrlBenchmarks            | SystemUri_Complex                 | W2           | ?      | ?          | 1,705.11 ns | 3.614 ns | 3.204 ns | 1,705.92 ns |  1.00 |    0.00 | 0.1278 |    2160 B |        1.00 |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 BatchBenchmarks          | Ada_ReadAll                       | W3 full read | ?      | ?          |   236.56 ns | 0.659 ns | 0.584 ns |   236.51 ns |  0.67 |    0.00 |      - |         - |        0.00 |
 BatchBenchmarks          | SystemUri_ReadAll                 | W3 full read | ?      | ?          |   353.46 ns | 2.103 ns | 1.864 ns |   353.08 ns |  1.00 |    0.01 | 0.0220 |     371 B |        1.00 |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 BatchBenchmarks          | Ada_ExtractHostname               | W3 hostname  | ?      | ?          |   233.24 ns | 0.718 ns | 0.636 ns |   233.28 ns |  0.80 |    0.01 |      - |         - |        0.00 |
 BatchBenchmarks          | SystemUri_ExtractHostname         | W3 hostname  | ?      | ?          |   291.78 ns | 2.927 ns | 2.738 ns |   291.88 ns |  1.00 |    0.01 | 0.0127 |     218 B |        1.00 |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 BatchBenchmarks          | SystemUri_Validate                | W3 validate  | ?      | ?          |   107.29 ns | 0.507 ns | 0.423 ns |   107.20 ns |  1.00 |    0.01 | 0.0054 |      91 B |        1.00 |
 BatchBenchmarks          | Ada_Validate                      | W3 validate  | ?      | ?          |   126.74 ns | 0.248 ns | 0.207 ns |   126.77 ns |  1.18 |    0.00 |      - |         - |        0.00 |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 AllocationCostBenchmarks | CanParse                          | W4           | ?      | 100        |    86.62 ns | 0.133 ns | 0.111 ns |    86.58 ns |  1.00 |    0.00 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 100        |   188.82 ns | 0.520 ns | 0.434 ns |   188.83 ns |  2.18 |    0.01 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 100        |   192.58 ns | 0.870 ns | 0.771 ns |   192.56 ns |  2.22 |    0.01 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 100        |   202.11 ns | 1.225 ns | 1.085 ns |   201.73 ns |  2.33 |    0.01 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 AllocationCostBenchmarks | CanParse                          | W4           | ?      | 10000      |   105.63 ns | 0.792 ns | 0.702 ns |   105.23 ns |  1.00 |    0.01 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 10000      |   244.86 ns | 0.730 ns | 0.683 ns |   244.72 ns |  2.32 |    0.02 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 10000      |   248.47 ns | 0.767 ns | 0.599 ns |   248.55 ns |  2.35 |    0.02 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 10000      |   256.26 ns | 0.779 ns | 0.651 ns |   256.49 ns |  2.43 |    0.02 |      - |         - |          NA |
                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
 AllocationCostBenchmarks | CanParse                          | W4           | ?      | 200000     |   105.98 ns | 0.260 ns | 0.244 ns |   105.92 ns |  1.00 |    0.00 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 200000     |   246.37 ns | 0.662 ns | 0.552 ns |   246.30 ns |  2.32 |    0.01 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 200000     |   248.62 ns | 0.680 ns | 0.603 ns |   248.45 ns |  2.35 |    0.01 |      - |         - |          NA |
 AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 200000     |   258.76 ns | 0.656 ns | 0.613 ns |   258.75 ns |  2.44 |    0.01 |      - |         - |          NA |

```

BenchmarkDotNet v0.15.2, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.400
  [Host]     : .NET 10.0.11 (10.0.1126.37416), X64 RyuJIT AVX2
  DefaultJob : .NET 10.0.11 (10.0.1126.37416), X64 RyuJIT AVX2


```
| Type                     | Method                            | Categories   | Length | WorkingSet | Mean        | Error    | StdDev   | Median      | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------- |---------------------------------- |------------- |------- |----------- |------------:|---------:|---------:|------------:|------:|--------:|-------:|----------:|------------:|
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 16     | ?          |    47.35 ns | 0.130 ns | 0.122 ns |    47.32 ns |  1.00 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 16     | ?          |    59.25 ns | 0.044 ns | 0.041 ns |    59.25 ns |  1.25 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 16     | ?          |    59.45 ns | 0.033 ns | 0.026 ns |    59.44 ns |  1.26 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 16     | ?          |    68.90 ns | 0.061 ns | 0.047 ns |    68.90 ns |  1.46 |    0.00 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 64     | ?          |    47.38 ns | 0.023 ns | 0.019 ns |    47.38 ns |  1.00 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 64     | ?          |    59.19 ns | 0.086 ns | 0.076 ns |    59.19 ns |  1.25 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 64     | ?          |    69.06 ns | 0.113 ns | 0.094 ns |    69.00 ns |  1.46 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 64     | ?          |    74.11 ns | 0.069 ns | 0.058 ns |    74.12 ns |  1.56 |    0.00 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 170    | ?          |    47.42 ns | 0.039 ns | 0.032 ns |    47.41 ns |  1.00 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 170    | ?          |    64.26 ns | 0.236 ns | 0.197 ns |    64.27 ns |  1.36 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 170    | ?          |    73.00 ns | 0.063 ns | 0.056 ns |    72.99 ns |  1.54 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 170    | ?          |   111.53 ns | 0.121 ns | 0.101 ns |   111.52 ns |  2.35 |    0.00 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 256    | ?          |    47.41 ns | 0.026 ns | 0.021 ns |    47.42 ns |  1.00 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 256    | ?          |    74.10 ns | 0.153 ns | 0.128 ns |    74.05 ns |  1.56 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 256    | ?          |    80.65 ns | 0.032 ns | 0.028 ns |    80.66 ns |  1.70 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 256    | ?          |   141.43 ns | 0.109 ns | 0.091 ns |   141.45 ns |  2.98 |    0.00 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 1024   | ?          |    47.39 ns | 0.031 ns | 0.027 ns |    47.39 ns |  1.00 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 1024   | ?          |    88.69 ns | 0.060 ns | 0.056 ns |    88.69 ns |  1.87 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 1024   | ?          |    99.61 ns | 0.065 ns | 0.057 ns |    99.60 ns |  2.10 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 1024   | ?          |   432.19 ns | 0.229 ns | 0.214 ns |   432.25 ns |  9.12 |    0.01 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| TranscodeBenchmarks      | Utf8_NoTranscode                  |              | 4096   | ?          |    47.46 ns | 0.014 ns | 0.012 ns |    47.46 ns |  1.00 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_CallerTranscodes            |              | 4096   | ?          |   164.67 ns | 0.273 ns | 0.242 ns |   164.75 ns |  3.47 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_Ascii                       |              | 4096   | ?          |   170.71 ns | 0.167 ns | 0.148 ns |   170.70 ns |  3.60 |    0.00 |      - |         - |          NA |
| TranscodeBenchmarks      | Utf16_NonAscii                    |              | 4096   | ?          | 1,534.61 ns | 1.600 ns | 1.249 ns | 1,534.41 ns | 32.34 |    0.03 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| UrlBenchmarks            | Ada_Basic_CanParse                | W1           | ?      | ?          |    47.02 ns | 0.045 ns | 0.042 ns |    47.00 ns |  0.27 |    0.00 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Basic_T1_SpanIn_SpanOut       | W1           | ?      | ?          |    90.21 ns | 0.096 ns | 0.080 ns |    90.20 ns |  0.52 |    0.00 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Basic_T1_ReadEveryComponent   | W1           | ?      | ?          |   103.24 ns | 0.112 ns | 0.093 ns |   103.22 ns |  0.59 |    0.00 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Basic_T2_SpanIn_StringOut     | W1           | ?      | ?          |   112.43 ns | 0.230 ns | 0.192 ns |   112.34 ns |  0.64 |    0.00 | 0.0043 |      72 B |        0.25 |
| UrlBenchmarks            | Ada_Basic_T3_StringIn_StringOut   | W1           | ?      | ?          |   121.85 ns | 0.331 ns | 0.294 ns |   121.77 ns |  0.70 |    0.00 | 0.0043 |      72 B |        0.25 |
| UrlBenchmarks            | SystemUri_Basic                   | W1           | ?      | ?          |   175.14 ns | 0.300 ns | 0.250 ns |   175.05 ns |  1.00 |    0.00 | 0.0172 |     288 B |        1.00 |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| UrlBenchmarks            | Ada_Complex_T1_Normalize          | W2           | ?      | ?          | 1,141.22 ns | 1.179 ns | 0.921 ns | 1,141.40 ns |  0.67 |    0.00 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Complex_T1_SpanIn_SpanOut     | W2           | ?      | ?          | 1,150.31 ns | 4.249 ns | 3.766 ns | 1,149.26 ns |  0.67 |    0.00 |      - |         - |        0.00 |
| UrlBenchmarks            | Ada_Complex_T3_StringIn_StringOut | W2           | ?      | ?          | 1,340.63 ns | 5.492 ns | 4.869 ns | 1,339.68 ns |  0.79 |    0.00 | 0.0229 |     392 B |        0.18 |
| UrlBenchmarks            | SystemUri_Complex                 | W2           | ?      | ?          | 1,705.11 ns | 3.614 ns | 3.204 ns | 1,705.92 ns |  1.00 |    0.00 | 0.1278 |    2160 B |        1.00 |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| BatchBenchmarks          | Ada_ReadAll                       | W3 full read | ?      | ?          |   236.56 ns | 0.659 ns | 0.584 ns |   236.51 ns |  0.67 |    0.00 |      - |         - |        0.00 |
| BatchBenchmarks          | SystemUri_ReadAll                 | W3 full read | ?      | ?          |   353.46 ns | 2.103 ns | 1.864 ns |   353.08 ns |  1.00 |    0.01 | 0.0220 |     371 B |        1.00 |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| BatchBenchmarks          | Ada_ExtractHostname               | W3 hostname  | ?      | ?          |   233.24 ns | 0.718 ns | 0.636 ns |   233.28 ns |  0.80 |    0.01 |      - |         - |        0.00 |
| BatchBenchmarks          | SystemUri_ExtractHostname         | W3 hostname  | ?      | ?          |   291.78 ns | 2.927 ns | 2.738 ns |   291.88 ns |  1.00 |    0.01 | 0.0127 |     218 B |        1.00 |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| BatchBenchmarks          | SystemUri_Validate                | W3 validate  | ?      | ?          |   107.29 ns | 0.507 ns | 0.423 ns |   107.20 ns |  1.00 |    0.01 | 0.0054 |      91 B |        1.00 |
| BatchBenchmarks          | Ada_Validate                      | W3 validate  | ?      | ?          |   126.74 ns | 0.248 ns | 0.207 ns |   126.77 ns |  1.18 |    0.00 |      - |         - |        0.00 |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| AllocationCostBenchmarks | CanParse                          | W4           | ?      | 100        |    86.62 ns | 0.133 ns | 0.111 ns |    86.58 ns |  1.00 |    0.00 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 100        |   188.82 ns | 0.520 ns | 0.434 ns |   188.83 ns |  2.18 |    0.01 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 100        |   192.58 ns | 0.870 ns | 0.771 ns |   192.56 ns |  2.22 |    0.01 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 100        |   202.11 ns | 1.225 ns | 1.085 ns |   201.73 ns |  2.33 |    0.01 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| AllocationCostBenchmarks | CanParse                          | W4           | ?      | 10000      |   105.63 ns | 0.792 ns | 0.702 ns |   105.23 ns |  1.00 |    0.01 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 10000      |   244.86 ns | 0.730 ns | 0.683 ns |   244.72 ns |  2.32 |    0.02 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 10000      |   248.47 ns | 0.767 ns | 0.599 ns |   248.55 ns |  2.35 |    0.02 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 10000      |   256.26 ns | 0.779 ns | 0.651 ns |   256.49 ns |  2.43 |    0.02 |      - |         - |          NA |
|                          |                                   |              |        |            |             |          |          |             |       |         |        |           |             |
| AllocationCostBenchmarks | CanParse                          | W4           | ?      | 200000     |   105.98 ns | 0.260 ns | 0.244 ns |   105.92 ns |  1.00 |    0.00 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndDispose                   | W4           | ?      | 200000     |   246.37 ns | 0.662 ns | 0.552 ns |   246.30 ns |  2.32 |    0.01 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadHostname              | W4           | ?      | 200000     |   248.62 ns | 0.680 ns | 0.603 ns |   248.45 ns |  2.35 |    0.01 |      - |         - |          NA |
| AllocationCostBenchmarks | ParseAndReadFive                  | W4           | ?      | 200000     |   258.76 ns | 0.656 ns | 0.613 ns |   258.75 ns |  2.44 |    0.01 |      - |         - |          NA |

