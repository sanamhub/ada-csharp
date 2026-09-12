# Injected through CMAKE_PROJECT_TOP_LEVEL_INCLUDES, so upstream's tree is never patched.
#
# Upstream sets WINDOWS_EXPORT_ALL_SYMBOLS on the ada target unconditionally, with no option
# guarding it, so it cannot be turned off from the command line. The call also runs after this
# file is included, so overriding it here directly would be overwritten a moment later. Deferring
# to the end of the top level directory scope puts our call last, by which point the target
# exists and upstream has already set the property.
#
# With the property off, cmake -E __create_def never runs, so it never reads IL objects and never
# crashes, and /GL becomes usable. The export list comes from a .def generated out of upstream's
# own include/ada_c.h instead. See native/build-windows.ps1.
cmake_language(DEFER DIRECTORY "${CMAKE_SOURCE_DIR}"
  CALL set_target_properties ada PROPERTIES WINDOWS_EXPORT_ALL_SYMBOLS OFF)
