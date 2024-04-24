#!/usr/bin/env python
from datetime import datetime
import subprocess
import sys
import os
import shutil

# Constants
GODOT_BINARY = "godot-bin/godot.exe" # Replace this with an actual path to Godot binary.
OUTPUT_DIR = "Builds"
CONSTANTS_PATH = "Source/Constants.cs"

# Root paths to native library folders.
BINARIES = [
    "bin/fmod/", 
    "bin/steam/",
    "bin/discord/"
    ]

# Build export template names.
WINDOWS_DEBUG = "Windows Debug"
LINUX_DEBUG = "Linux Debug"
WINDOWS_RELEASE = "Windows Release"
LINUX_RELEASE = "Linux Release"

# Date
date = datetime.now()
day = date.strftime("%d")
month = date.strftime("%m")
year = date.strftime("%y")

if len(sys.argv) < 2:
    print("Usage: build.py <target>")
    print("Example: build.py windows_release")
    exit(1)

# Arguments
export_string = sys.argv[1] # example: linux_debug, windows_release
export_platform = export_string.split("_")[0] # example: linux
export_target = export_string.split("_")[1] # example: release


def get_version_from_line(line):
    version_string = line.strip().split(" ")[5]
    return version_string.replace(";", "")


def get_buildtype_from_line(line):
    buildtype_string = line.strip().split(" ")[5]
    return buildtype_string.replace("\"", "").replace(";", "")


def get_game_version():
    major = "0"
    minor = "0"
    patch = "0"
    build_type = ""
    with open(CONSTANTS_PATH) as f:
        content = f.readlines()
        for line in content:
            if "short VERSION_MAJOR" in line:
                major = get_version_from_line(line)
            if "short VERSION_MINOR" in line:
                minor = get_version_from_line(line)
            if "short VERSION_PATCH" in line:
                patch = get_version_from_line(line)
            if "string BUILD_TYPE" in line:
                build_type = get_buildtype_from_line(line)
    return (major, minor, patch, build_type)


# Game version
VERSION_FULL = get_game_version()
VERSION_MAJOR = VERSION_FULL[0]
VERSION_MINOR = VERSION_FULL[1]
VERSION_PATCH = VERSION_FULL[2]
VERSION_BUILD = VERSION_FULL[3]
VERSION_STRING = f"{VERSION_MAJOR}.{VERSION_MINOR}.{VERSION_PATCH}-{VERSION_BUILD}"


def get_git_revision_short_hash():
    return subprocess.check_output(['git', 'rev-parse', '--short', 'HEAD']).decode('ascii').strip()


def get_target_name(platform, target):
    if platform.lower() == "linux":
            if target.lower() == "debug":
                return LINUX_DEBUG
            elif target.lower() == "release":
                return LINUX_RELEASE
    if platform.lower() == "windows":
            if target.lower() == "debug":
                return WINDOWS_DEBUG
            elif target.lower() == "release":
                return WINDOWS_RELEASE


def cleanup_output():
    print(f"Cleaning up...")
    shutil.rmtree("Output")
    pass


def build_game(target_name):
    print(f"Building {target_name}...")
    args = [GODOT_BINARY, "--export", target_name, "--no-window", "--quiet"]
    process = subprocess.run(args)


def create_folder(path) -> bool:
    exists = os.path.isdir(path)
    if not exists:
        os.mkdir(path)

    return exists


# Output path
version_output_path = f"{OUTPUT_DIR}/{VERSION_STRING}"
version_date_output_path = f"{version_output_path}/{day}-{month}-{year}"
version_date_hash_output_path = f"{version_date_output_path}/{get_git_revision_short_hash()}"

EXPORT_PATH = f"{version_date_output_path}/{get_git_revision_short_hash()}/{get_target_name(export_platform, export_target)}"
BINARIES_TARGET_PATH = f"{EXPORT_PATH}/data_Jump Adventures/Assemblies/"


def copy_files_to_new_dir(source, to):
    exists = os.path.isdir(to)
    if not exists:
        shutil.copytree(source, to)


def copy_files(source, to):
    for filename in os.listdir(source):
        src = source + filename
        print(f"Copying: {src}")
        dest = to + filename

        if os.path.isfile(src):
            shutil.copy(src, dest)


def copy_build_files(folder):
    print(f"Moving build files...")
    exists = os.path.isdir(folder)
    if exists:
        shutil.rmtree(folder)
    copy_files_to_new_dir("Output", folder)


def copy_binaries(platform):
    print("Copying binaries...")
    for path in BINARIES:
        copy_files(path + platform + "/", BINARIES_TARGET_PATH)

def create_env_file():
    with open('.env', 'w') as writer:
     writer.write(f'export JA_EXPORT_PATH="{EXPORT_PATH}"')

target_name = get_target_name(export_platform, export_target)
build_game(target_name)
copy_build_files(EXPORT_PATH)
copy_binaries(export_platform)
cleanup_output()

create_env_file()

print(f"Done! ({EXPORT_PATH})")
