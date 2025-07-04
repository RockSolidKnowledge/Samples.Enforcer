#!/bin/bash

# Usage: ./update_rsk_enforcer_version.sh 2.3.4

if [ -z "$1" ]; then
  echo "Usage: $0 <new-version>"
  exit 1
fi

NEW_VERSION="$1"

# Find all .csproj files and process them
find . -name "*.csproj" | while read -r FILE; do
  echo "Processing $FILE"

  # Replace Version attribute in matching PackageReference elements
  sed -i.bak -E "s|(<PackageReference[^>]*Include=\"Rsk\.Enforcer[^\"]*\"[^>]*Version=\")[^\"]*(\")|\1$NEW_VERSION\2|g" "$FILE"

  # Replace inner version tag if not an attribute
  sed -i.bak -E "/<PackageReference[^>]*Include=\"Rsk\.Enforcer[^\"]*\"[^>]*>/ {
    N
    s|(<PackageReference[^>]*Include=\"Rsk\.Enforcer[^\"]*\"[^>]*>.*<Version>)[^<]*(</Version>)|\1$NEW_VERSION\2|
  }" "$FILE"

  # Remove backup file created by sed
  rm "$FILE.bak"
done

echo "Done. All Rsk.Enforcer.* package references updated to version $NEW_VERSION."
