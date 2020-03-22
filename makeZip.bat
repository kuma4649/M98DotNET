echo mml2vgm

mkdir output

del /Q .\output\*.*
xcopy .\M98\bin\Release\*.* .\output\ /E /R /Y /I /K
del /Q .\output\*.pdb
del /Q .\output\*.config
copy .\CHANGE.txt .\output\
copy .\README.md .\output\
copy .\usage.txt .\output\
copy .\m98コマンド・リファレンス.pdf .\output\
copy .\M98.bat .\output\

pause
