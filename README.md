Problem

Instructions
Solve the problem below using c#. The solution must be a console application and all code need to be in 
one .cs file.

Notes
Your code will be assessed on the following:
	> Adherence to S.O.L.I.D design principles
	> Object oriented programming skills
	> Unit Tests

Problem
Given a list of space separated words, reverse the order of the words. Each line of text contains "I" letters 
and "W" words. A line will only consist of letters and space characters. There will be exactly one space 
character between each pair of consecutive words.

#Input
The first line of input gives the number of cases "N"
"N" test cases follow. For each test case there will a line of letters and space characters indicating a list of
space separated words. Spaces will not appear at the start or end of a line.

Output
For each test case, output one line containing "Case 'x':" followed by the list of words in reverse order.

Limits 
1 <= L <=25

-----------------------------------------------------------------------------------------
Sample

	input
		3
		this is a test
		foobar
		all your base

	Output
		Case 1: test a is this
		Case 2: foobar
		Case 3: base your all

-----------------------------------------------------------------------------------------

Systems Requirements: dotnet core8

Test Command: dotnet test --logger "console;verbosity=detailed"

Run Comman:   dotnet run --project .\Lightstone.SentenceParserApp.csproj