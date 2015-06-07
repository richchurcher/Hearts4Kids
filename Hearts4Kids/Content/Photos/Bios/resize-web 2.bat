:: @echo off
setlocal enableDelayedExpansion

set IM=C:\Program Files (x86)\ImageMagick-6.8.7-Q16
set pb=\set-IM-path.bat
if EXIST %pb% call %pb%
set public_folder=%cd%/
:: 312x232 slides
set smallsize=104x76
set prevwsize=312x232
set largesize=936x696
rem set thumbcrop=-crop 282x300+84+0
set smallfolder=%public_folder%th
set prevwfolder=%public_folder%pv
set largefolder=%public_folder%lg

md "%smallfolder%" >nul: 2>&1
md "%Prevwfolder%" >nul: 2>&1
md "%largefolder%" >nul: 2>&1

:: goto finish

:: echo on
for %%f in (*.jpg) do (
  echo %%f  to  sf/%%f
  "%IM%/convert.exe" "%%f" -quality 95 %thumbcrop%  -resize %smallsize%   "%smallfolder%/%%f"
  
  "%IM%/convert.exe" "%%f" -quality 95 -resize %prevwsize%   "%prevwfolder%/%%f"
  "%IM%/convert.exe" "%%f" -quality 95 -resize %largesize%   "%largefolder%/%%f"
)
:png
for %%f in (*.png) do (
  set fn=%%f
  set of=!fn:.png=!.jpg
  echo %%f  to  !of!
  "%IM%/convert.exe" "%%f" -quality 95 %thumbcrop%  -resize %smallsize%   "%smallfolder%/!of!"
  
  "%IM%/convert.exe" "%%f" -quality 95 -resize %prevwsize%   "%prevwfolder%/!of!"
  "%IM%/convert.exe" "%%f" -quality 95 -resize %largesize%   "%largefolder%/!of!"
)
:finish    
@echo off
    
endlocal
pause