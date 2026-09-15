// Measures what ada_parse allocates, and what those allocations cost, on Windows against Linux.
//
// Issue #20. The managed benchmarks say Ada's validate step costs about the same on both
// platforms while its parse step costs 2.2x more on Windows. Subtracting one from the other
// points at the two allocations ada_parse makes, but a subtraction is not a measurement: the
// same gap also contains the aggregator filling its buffer and computing component offsets.
//
// So this does three things in one process, which is the only way the numbers are comparable:
//
//   1. Counts the allocations, with sizes, by replacing global operator new. No guessing at
//      sizeof(ada::result<ada::url_aggregator>) or at whether the string hits SSO.
//   2. Times ada_can_parse, ada_parse + ada_free, and a replay loop that performs exactly the
//      allocations step 1 recorded and nothing else.
//   3. Prints every timing as a ratio against the can_parse control as well as in nanoseconds,
//      because the Windows and Linux runners are different machines and raw nanoseconds across
//      them mean nothing.
//
// The replay loop turned out to be 4x slower on Windows, which is 76% of the gap, so #20 is
// answered: the allocator. Kept rather than deleted with the issue, on the same reasoning that
// kept the all-symbols build path in ADR-0006: the number has to be recheckable when the pinned
// ada tag moves. See ADR-0007.
#include "ada.h"

// ada_c.h carries no extern "C" guard of its own, so including it from C++ declares every
// ada_* function with C++ linkage and nothing links. That is issue #21's first half, seen
// here rather than argued from the source.
extern "C" {
#include "ada_c.h"
}

#include <algorithm>
#include <chrono>
#include <cstddef>
#include <cstdint>
#include <cstdio>
#include <cstdlib>
#include <cstring>
#include <new>

// --- Allocation recorder -------------------------------------------------------------------
// A fixed array rather than a vector. The recorder runs inside operator new, so anything it
// allocates would recurse.
namespace {

constexpr int kMaxRecorded = 64;

bool g_recording = false;
int g_count = 0;
std::size_t g_sizes[kMaxRecorded];
bool g_overflowed = false;

void record(std::size_t n) {
    if (g_count < kMaxRecorded) {
        g_sizes[g_count++] = n;
    } else {
        g_overflowed = true;
    }
}

}  // namespace

void* operator new(std::size_t n) {
    if (g_recording) record(n);
    void* p = std::malloc(n ? n : 1);
    if (!p) throw std::bad_alloc();
    return p;
}
void* operator new[](std::size_t n) { return ::operator new(n); }
void operator delete(void* p) noexcept { std::free(p); }
void operator delete[](void* p) noexcept { std::free(p); }
void operator delete(void* p, std::size_t) noexcept { std::free(p); }
void operator delete[](void* p, std::size_t) noexcept { std::free(p); }

// --- Timing --------------------------------------------------------------------------------
namespace {

using clock = std::chrono::steady_clock;

// Keeps the optimiser from deleting work whose result nothing reads.
volatile std::uint64_t g_sink = 0;

constexpr int kRounds = 9;
constexpr int kIterations = 100000;

double median(double (&samples)[kRounds]) {
    std::sort(samples, samples + kRounds);
    return samples[kRounds / 2];
}

// Returns nanoseconds per iteration, median of kRounds.
template <class Body>
double measure(Body body) {
    for (int i = 0; i < kIterations / 10; i++) body();  // warm up

    double samples[kRounds];
    for (int r = 0; r < kRounds; r++) {
        const auto start = clock::now();
        for (int i = 0; i < kIterations; i++) body();
        const auto elapsed = clock::now() - start;
        samples[r] = std::chrono::duration<double, std::nano>(elapsed).count() / kIterations;
    }
    return median(samples);
}

struct Recorded {
    int count;
    std::size_t sizes[kMaxRecorded];
};

Recorded record_parse(const char* url, std::size_t length) {
    // One untimed call first. The first parse through a fresh heap pulls pages from the OS and
    // may initialise lazily built tables, and neither belongs in the count.
    ada_free(ada_parse(url, length));

    g_count = 0;
    g_overflowed = false;
    g_recording = true;
    ada_url u = ada_parse(url, length);
    g_recording = false;
    ada_free(u);

    Recorded out{};
    out.count = g_count;
    std::memcpy(out.sizes, g_sizes, sizeof(g_sizes));
    return out;
}

// ada_set_href re-parses into an existing handle, so a caller with a loop can allocate the
// result object once instead of once per URL. It is not free: set_href parses into a fresh
// aggregator and copy assigns it, so the string buffer is still allocated and then copied.
// Whether that trades well is a question for the timer, not for reading the source.
int record_set_href(ada_url handle, const char* url, std::size_t length) {
    ada_set_href(handle, url, length);

    g_count = 0;
    g_recording = true;
    g_sink += ada_set_href(handle, url, length) ? 1 : 0;
    g_recording = false;
    return g_count;
}

int record_can_parse(const char* url, std::size_t length) {
    ada_can_parse(url, length);

    g_count = 0;
    g_recording = true;
    g_sink += ada_can_parse(url, length) ? 1 : 0;
    g_recording = false;
    return g_count;
}

void run(const char* label, const char* url) {
    const std::size_t length = std::strlen(url);

    const int validate_allocations = record_can_parse(url, length);
    const Recorded parsed = record_parse(url, length);

    ada_url reused = ada_parse(url, length);
    const int reuse_allocations = record_set_href(reused, url, length);

    std::printf("\n## %s\n", label);
    std::printf("url            %s\n", url);
    std::printf("bytes          %zu\n", length);
    std::printf("can_parse      %d allocation(s)\n", validate_allocations);
    std::printf("set_href       %d allocation(s)\n", reuse_allocations);
    std::printf("parse          %d allocation(s)", parsed.count);
    if (g_overflowed) std::printf("  (RECORDER OVERFLOWED, count is a floor)");
    std::printf("\nparse sizes    ");
    for (int i = 0; i < parsed.count; i++) std::printf("%zu ", parsed.sizes[i]);
    std::printf("\n");

    const double control = measure([&] {
        g_sink += ada_can_parse(url, length) ? 1u : 0u;
    });

    const double full = measure([&] {
        ada_url u = ada_parse(url, length);
        g_sink += reinterpret_cast<std::uintptr_t>(u);
        ada_free(u);
    });

    // Exactly the allocations the parse made, in the order it made them, freed in reverse.
    // Nothing else: no parsing, no copying, no offset arithmetic.
    const int count = parsed.count;
    const std::size_t* sizes = parsed.sizes;
    const double replay = measure([&] {
        void* blocks[kMaxRecorded];
        for (int i = 0; i < count; i++) blocks[i] = ::operator new(sizes[i]);
        for (int i = count - 1; i >= 0; i--) ::operator delete(blocks[i]);
        g_sink += count ? reinterpret_cast<std::uintptr_t>(blocks[0]) : 0u;
    });

    const double reuse = measure([&] {
        g_sink += ada_set_href(reused, url, length) ? 1u : 0u;
    });
    ada_free(reused);

    std::printf("\n%-22s %10s %10s\n", "", "ns/op", "x control");
    std::printf("%-22s %10.1f %10.2f\n", "can_parse (control)", control, 1.0);
    std::printf("%-22s %10.1f %10.2f\n", "parse + free", full, full / control);
    std::printf("%-22s %10.1f %10.2f\n", "set_href, handle kept", reuse, reuse / control);
    std::printf("%-22s %10.1f %10.2f\n", "allocations only", replay, replay / control);
    std::printf("%-22s %10.1f %10.2f\n", "parse - can_parse", full - control,
                (full - control) / control);
    std::printf("%-22s %10.1f %9.0f%%\n", "allocations share", replay,
                100.0 * replay / (full - control));
}

}  // namespace

int main() {
    std::printf("ada %s\n", ADA_VERSION);
#if defined(_WIN32)
    std::printf("platform       windows\n");
#elif defined(__linux__)
    std::printf("platform       linux\n");
#else
    std::printf("platform       other\n");
#endif
    std::printf("pointer        %zu bytes\n", sizeof(void*));
    std::printf("rounds         %d of %d iterations, median\n", kRounds, kIterations);

    run("W1 plain", "https://example.com/path");
    run("W2 hard",
        "https://user:p%40ss@sub.d\xc3\xb8main.example.co.uk:8443/a/../b/./c%2Fd/e%20f"
        "?q=hello+world&filter[]=1&filter[]=2&token=%E2%9C%93"
        "&redirect=https%3A%2F%2Fother.example%2Fx#section-2%20anchor");

    if (g_sink == 0x7fffffffffffffffULL) std::printf("unreachable\n");
    return 0;
}
