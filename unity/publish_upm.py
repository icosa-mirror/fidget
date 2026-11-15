#!/usr/bin/env python3
"""Publish reviewed CI packages as UPM Git branches. Intended for the manual CI workflow."""
import argparse
import json
from pathlib import Path
import shutil
import subprocess
import tarfile


def git(directory, *args, capture=False):
    result = subprocess.run(['git', '-C', str(directory), *args], check=True,
                            text=True, capture_output=capture)
    return result.stdout.strip() if capture else None


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--artifacts', type=Path, required=True)
    parser.add_argument('--repository', required=True)
    parser.add_argument('--run-id', required=True)
    parser.add_argument('--work-dir', type=Path, required=True)
    parser.add_argument('--dry-run', action='store_true', help='Validate both packages without creating or pushing Git branches')
    args = parser.parse_args()
    # A new directory prevents touching any pre-existing checkout or local work.
    args.work_dir.mkdir(parents=True, exist_ok=False)
    variants = [('upm', 'device', 7), ('upm-simulator', 'simulator', 3)]
    prepared = []
    for branch, variant, count in variants:
        extracted = args.work_dir / f'extracted-{variant}'
        with tarfile.open(args.artifacts / f'fidget-unity-{variant}.tgz') as archive:
            archive.extractall(extracted, filter='data')
        package = extracted / 'package'
        manifest = json.loads((package / 'package.json').read_text())
        if manifest['name'] != 'com.icosa.fidget':
            raise ValueError('Unexpected package identity')
        if (package / '.git').exists():
            raise ValueError('Package must not contain Git repository metadata')
        libraries = [p for p in package.rglob('*') if p.suffix in ('.dll', '.so', '.dylib', '.a')]
        if len(libraries) != count:
            raise ValueError(f'{branch}: expected {count} platform libraries')
        source_commits = set()
        for library in libraries:
            info = (library.parent / 'BUILD-INFO.txt').read_text()
            if 'ABI: 4\n' not in info or not Path(f'{library}.meta').is_file():
                raise ValueError(f'{library}: missing ABI-v4 provenance or importer metadata')
            source_commits.add(next(line for line in info.splitlines() if line.startswith('Source commit: ')))
        if len(source_commits) != 1:
            raise ValueError('Package contains libraries from different source commits')
        prepared.append((branch, package, manifest['version']))

    if args.dry_run:
        for branch, package, version in prepared:
            print(f'{branch}: validated complete package {version} at {package.resolve()}')
        return

    remote = f'https://github.com/{args.repository}.git'
    for branch, package, version in prepared:
        checkout = args.work_dir / branch
        checkout.mkdir()
        git(checkout, 'init', '-b', branch)
        git(checkout, 'remote', 'add', 'origin', remote)
        exists = git(checkout, 'ls-remote', '--heads', 'origin', f'refs/heads/{branch}', capture=True)
        if exists:
            git(checkout, 'fetch', '--depth', '1', 'origin', branch)
            git(checkout, 'checkout', '-B', branch, 'FETCH_HEAD')
            git(checkout, 'rm', '-r', '--ignore-unmatch', '.')
        shutil.copytree(package, checkout, dirs_exist_ok=True)
        git(checkout, 'config', 'user.name', 'github-actions[bot]')
        git(checkout, 'config', 'user.email', '41898282+github-actions[bot]@users.noreply.github.com')
        git(checkout, 'add', '--all')
        if not git(checkout, 'diff', '--cached', '--name-only', capture=True):
            print(f'{branch}: package already current')
            continue
        git(checkout, 'commit', '-m', f'Publish Fidget {version} from native CI run {args.run_id}')
        # A normal fast-forward push preserves history and rejects concurrent changes.
        git(checkout, 'push', 'origin', f'HEAD:refs/heads/{branch}')
        print(f'{branch}: {git(checkout, "rev-parse", "HEAD", capture=True)}')


if __name__ == '__main__':
    main()
