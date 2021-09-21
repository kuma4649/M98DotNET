echo mml2vgm

mkdir output
mkdir output\NET5

del /Q .\output\*.*
xcopy .\M98\bin\Release\*.* .\output\ /E /R /Y /I /K
xcopy .\M98DotNET_NET5\bin\Release\net5.0\*.* .\output\NET5 /E /R /Y /I /K
del /Q .\output\*.pdb
del /Q .\output\*.config
copy .\CHANGE.txt .\output\
copy .\README.md .\output\
copy .\usage.txt .\output\
copy .\m98コマンド・リファレンス.pdf .\output\
copy .\M98.bat .\output\

pause
