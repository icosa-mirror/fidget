#!/usr/bin/env python3
"""Check that the Unity example uses a published package built from matching sources."""
import json
from pathlib import Path
import re
import subprocess
import sys


ROOT = Path(__file__).resolve().parent.parent
PACKAGES = ROOT / 'examples/unity/Packages'
PACKAGE = 'com.icosa.fidget'
UPM_REMOTE = 'https://github.com/icosa-mirror/fidget.git'
PACKAGE_INPUTS = (
    '.cargo', '.github/workflows/native.yml', 'Cargo.lock', 'Cargo.toml', 'LICENSE.txt',
    'fidget-core', 'fidget-mesh', 'fidget-native', 'workspace-hack',
    'unity/com.icosa.fidget', 'unity/package_unity.py',
    'examples/unity/Assets/FidgetExamples', 'examples/unity/Assets/FidgetExamples.meta',
)


def git(*args):
    return subprocess.check_output(['git', *args], cwd=ROOT, text=True)


def ensure_commit(commit):
    exists = subprocess.run(['git', 'cat-file', '-e', f'{commit}^{{commit}}'],
                            cwd=ROOT, stderr=subprocess.DEVNULL).returncode == 0
    if not exists:
        depth = ['--depth=1'] if git('rev-parse', '--is-shallow-repository').strip() == 'true' else []
        git('fetch', '--no-tags', '--no-write-fetch-head', *depth, UPM_REMOTE, commit)


def main():
    manifest = json.loads((PACKAGES / 'manifest.json').read_text())
    lockfile = json.loads((PACKAGES / 'packages-lock.json').read_text())
    pin = manifest['dependencies'][PACKAGE]
    locked = lockfile['dependencies'][PACKAGE]
    url, _, package_commit = pin.partition('#')
    if (url != UPM_REMOTE or not re.fullmatch(r'[0-9a-f]{40}', package_commit)
            or locked['source'] != 'git' or locked['version'] != pin
            or locked['hash'] != package_commit):
        sys.exit('Pin the same published Fidget UPM commit in manifest.json and packages-lock.json.')

    ensure_commit(package_commit)
    info = git('show', f'{package_commit}:Runtime/Plugins/macOS/ARM64/BUILD-INFO.txt')
    source = re.search(r'^Source commit: ([0-9a-f]{40})$', info, re.MULTILINE)
    if source is None or 'Working tree dirty: False' not in info.splitlines():
        sys.exit('The pinned Unity package needs clean native CI build provenance.')

    source_commit = source[1]
    ensure_commit(source_commit)
    changed = git('diff', '--name-only', source_commit, 'HEAD', '--', *PACKAGE_INPUTS).splitlines()
    if changed:
        print(f'The example package differs from this checkout in {len(changed)} build input(s).', file=sys.stderr)
        for path in changed[:10]:
            print(f'  {path}', file=sys.stderr)
        sys.exit('Build and publish the matching Unity package, then update both pins.')
    print(f'Unity example package {package_commit} matches the build inputs at {source_commit}.')


if __name__ == '__main__':
    main()
