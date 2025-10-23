# Readme
This repo allows for easier implementation of the original HSOP

## Changes
- The code now outputs an exe file which can be run from the command line (Windows Powershell used)
- fixed a supposed error where distance was divided by 10 and caused infeasible solutions

## Instructions
- Ensure that you have the relevant .NET framework (4.6.1 - development version)
- Clean and build using dotnet commands
- Run the exe

## Notes
- Even though the code is virtually untouched, I could not obtained the claimed results from https://doi.org/10.1371/journal.pone.0264584
- Use the Tsiligrides test cases, or translate your test case into that format, you will note I placed it in a folder called tests
