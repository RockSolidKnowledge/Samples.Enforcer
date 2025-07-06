#!/bin/bash

# Usage: ./upgrade_all_packages_and_target.sh net8.0

if [ -z "$1" ]; then
  echo "Usage: $0 <target-framework-version> (e.g., net8.0)"
  exit 1
fi

TARGET_FRAMEWORK="$1"

# 1. Update all PackageReferences to latest using dotnet CLI
echo "Restoring and upgrading packages..."

# Restore and upgrade for each project
find . -name "*.csproj" | while read -r CSPROJ; do
  echo "Upgrading packages in $CSPROJ"
  dotnet list "$CSPROJ" package --outdated \
    | awk '/>/ {print $1}' \
    | while read -r PACKAGE; do
      if [[ -n "$PACKAGE" && "$PACKAGE" != ">" ]]; then
        echo "Upgrading $PACKAGE in $CSPROJ"
        dotnet add "$CSPROJ" package "$PACKAGE" --version latest
      fi
    done
done

# 2. Update the TargetFramework in each .csproj
echo "Updating target frameworks to $TARGET_FRAMEWORK..."

find . -name "*.csproj" | while read -r FILE; do
  echo "Updating framework in $FILE"
  sed -i.bak -E "s|<TargetFramework>.*</TargetFramework>|<TargetFramework>$TARGET_FRAMEWORK</TargetFramework>|" "$FILE"
  rm "$FILE.bak"
done

echo "✅ Upgrade complete. All packages set to latest and target framework set to $TARGET_FRAMEWORK."
