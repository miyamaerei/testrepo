#!/bin/bash

# Fix IRepositories files - add VOL.Core.BaseProvider
echo "Fixing IRepositories..."
for file in EKanban/IRepositories/*.cs; do
  if ! grep -q "VOL.Core.BaseProvider" "$file"; then
    # Check the content
    echo "Processing $file..."
    # Add using after the last using or at the top
    awk '
      { lines[NR] = $0 }
      END {
        found = 0
        for (i = 1; i <= NR; i++) {
          if (lines[i] !~ /^using/) {
            print "using VOL.Core.BaseProvider;"
            found = 1
          }
          print lines[i]
        }
        if (!found) {
          print "using VOL.Core.BaseProvider;"
        }
      }
    ' "$file" > "$file.tmp" && mv "$file.tmp" "$file"
  fi
done

# Fix Repositories files - add VOL.Core.BaseProvider
echo "Fixing Repositories..."
for file in EKanban/Repositories/*.cs; do
  if ! grep -q "VOL.Core.BaseProvider" "$file"; then
    echo "Processing $file..."
    awk '
      { lines[NR] = $0 }
      END {
        found = 0
        for (i = 1; i <= NR; i++) {
          if (lines[i] !~ /^using/) {
            print "using VOL.Core.BaseProvider;"
            found = 1
          }
          print lines[i]
        }
        if (!found) {
          print "using VOL.Core.BaseProvider;"
        }
      }
    ' "$file" > "$file.tmp" && mv "$file.tmp" "$file"
  fi
done

echo "Done."
