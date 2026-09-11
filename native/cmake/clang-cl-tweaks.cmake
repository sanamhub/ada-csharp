# Injected through CMAKE_PROJECT_TOP_LEVEL_INCLUDES, so upstream's tree is never patched.
#
# Upstream compiles ada with /WX for anything that identifies as MSVC, and clang-cl raises
# warnings MSVC does not, so the build stops before producing a DLL. Appending /WX- after
# upstream's options turns those back into warnings, because the last flag on the command line
# wins. Deferring is what puts it last.
#
# Acceptable for a measurement. If clang-cl ever becomes the default for win-x64, the warnings
# get read and either fixed upstream or accepted on purpose, not silenced by this file.
cmake_language(DEFER DIRECTORY "${CMAKE_SOURCE_DIR}"
  CALL target_compile_options ada PRIVATE /WX-)
