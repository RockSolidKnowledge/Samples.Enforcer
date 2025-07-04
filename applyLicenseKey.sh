#!/bin/bash

# Usage: ./update_license_key.sh "NEW-LICENSE-KEY"

if [ -z "$1" ]; then
  echo "Usage: $0 \"new-license-key\""
  exit 1
fi

NEW_KEY="$1"

# Escape slashes and ampersands for safe replacement
ESCAPED_KEY=$(printf '%s\n' "$NEW_KEY" | sed 's/[\/&]/\\&/g')

echo "🔄 Updating license keys in .cs files..."

find . -type f -name "*.cs" | while read -r FILE; do
  echo "Processing $FILE"

  # Use Perl-style case-insensitive matching (more portable than sed -i with I flag)
  perl -i.bak -pe '
    s/(licenseKey\s*=\s*")[^"]*(")/\1'"$ESCAPED_KEY"'\2/i;
  ' "$FILE"

  rm -f "$FILE.bak"
done

echo "✅ Done. All licenseKey values replaced with: $NEW_KEY"
