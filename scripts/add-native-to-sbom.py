#!/usr/bin/env python3
"""Add the vendored native dependency to a CycloneDX SBOM.

The CycloneDX .NET tool reads project.assets.json, so it sees NuGet packages and nothing else.
This package ships a compiled C++ library inside it, and that library is the dependency a
consumer most needs to know about: it is the one delivered as a binary they cannot inspect from
source, and the one whose CVEs would apply to them.

Leaving it out would make the SBOM technically valid and practically misleading. Anyone scanning
it for supply chain risk would see six analyzer packages and conclude that is the whole tree.

Adds one component per shipped native, with the upstream version, licence, and a hash matching
the committed manifest, so a scanner sees exactly what is in the package.
"""

from __future__ import annotations

import pathlib
import sys
import xml.etree.ElementTree as ET

CYCLONEDX_NS = "http://cyclonedx.org/schema/bom/1.7"

# Kept beside the pinned tag in Directory.Build.props and native/build-*. If these drift, the
# SBOM lies, which is worse than not having one.
ADA_VERSION = "4.0.0"
ADA_PURL = f"pkg:github/ada-url/ada@v{ADA_VERSION}"
ADA_LICENCE = "MIT"


def load_checksums(path: pathlib.Path) -> dict[str, str]:
    """Read the committed manifest as {rid/file: sha256}."""
    out: dict[str, str] = {}
    for line in path.read_text(encoding="utf-8").splitlines():
        line = line.strip()
        if not line:
            continue
        digest, _, name = line.partition("  ")
        if digest and name:
            out[name.strip()] = digest.strip()
    return out


def main() -> int:
    if len(sys.argv) != 3:
        print("usage: add-native-to-sbom.py <bom.xml> <native/CHECKSUMS.txt>", file=sys.stderr)
        return 2

    bom_path = pathlib.Path(sys.argv[1])
    manifest_path = pathlib.Path(sys.argv[2])

    if not bom_path.exists():
        print(f"::error::SBOM not found at {bom_path}", file=sys.stderr)
        return 1
    if not manifest_path.exists():
        print(f"::error::checksum manifest not found at {manifest_path}", file=sys.stderr)
        return 1

    checksums = load_checksums(manifest_path)
    if not checksums:
        print("::error::checksum manifest is empty", file=sys.stderr)
        return 1

    ET.register_namespace("", CYCLONEDX_NS)
    tree = ET.parse(bom_path)
    root = tree.getroot()

    components = root.find(f"{{{CYCLONEDX_NS}}}components")
    if components is None:
        components = ET.SubElement(root, f"{{{CYCLONEDX_NS}}}components")

    for artifact, digest in sorted(checksums.items()):
        rid = artifact.split("/")[0]

        component = ET.SubElement(components, f"{{{CYCLONEDX_NS}}}component", {"type": "library"})
        ET.SubElement(component, f"{{{CYCLONEDX_NS}}}name").text = "ada"
        ET.SubElement(component, f"{{{CYCLONEDX_NS}}}version").text = ADA_VERSION
        ET.SubElement(component, f"{{{CYCLONEDX_NS}}}description").text = (
            f"Ada URL parser, compiled for {rid} and shipped in this package as {artifact}."
        )
        ET.SubElement(component, f"{{{CYCLONEDX_NS}}}purl").text = ADA_PURL

        hashes = ET.SubElement(component, f"{{{CYCLONEDX_NS}}}hashes")
        hash_el = ET.SubElement(hashes, f"{{{CYCLONEDX_NS}}}hash", {"alg": "SHA-256"})
        hash_el.text = digest

        licenses = ET.SubElement(component, f"{{{CYCLONEDX_NS}}}licenses")
        license_el = ET.SubElement(licenses, f"{{{CYCLONEDX_NS}}}license")
        ET.SubElement(license_el, f"{{{CYCLONEDX_NS}}}id").text = ADA_LICENCE

        externals = ET.SubElement(component, f"{{{CYCLONEDX_NS}}}externalReferences")
        reference = ET.SubElement(externals, f"{{{CYCLONEDX_NS}}}reference", {"type": "vcs"})
        ET.SubElement(reference, f"{{{CYCLONEDX_NS}}}url").text = "https://github.com/ada-url/ada"

    tree.write(bom_path, encoding="utf-8", xml_declaration=True)
    print(f"added {len(checksums)} native components to {bom_path.name}")
    for artifact in sorted(checksums):
        print(f"  ada {ADA_VERSION} ({artifact})")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
