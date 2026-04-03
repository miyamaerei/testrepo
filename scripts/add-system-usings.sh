#!/bin/bash

# Add System and System.Linq usings to files that need them

# Jobs files - they need System for Exception and System.Linq for Any
for file in EKanban/Jobs/*.cs; do
  if ! grep -q "using System;" "$file"; then
    sed -i '1i using System;' "$file"
  fi
  if ! grep -q "using System.Linq;" "$file"; then
    sed -i '1i using System.Linq;' "$file"
  fi
done

# AiExecution files
for file in EKanban/AiExecution/*.cs; do
  if ! grep -q "using System.Text;" "$file" && grep -q "StringBuilder" "$file"; then
    sed -i '1i using System.Text;' "$file"
  fi
done

# Services files
for file in EKanban/Services/*.cs; do
  if ! grep -q "using System;" "$file"; then
    sed -i '1i using System;' "$file"
  fi
  if ! grep -q "using System.Linq;" "$file"; then
    sed -i '1i using System.Linq;' "$file"
  fi
done

# Specs files
for file in EKanban/Specs/*.cs; do
  if ! grep -q "using System;" "$file"; then
    sed -i '1i using System;' "$file"
  fi
done

# Repositories files
for file in EKanban/Repositories/*.cs; do
  if ! grep -q "using System.Linq;" "$file" && grep -q "Queryable" "$file"; then
    sed -i '1i using System.Linq;' "$file"
  fi
done

echo "Done adding system usings."
