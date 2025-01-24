@echo off
cls
title Download Manager Project
echo.
echo Setting up project...
echo Working directory: %cd%
echo.
REM Check if ffmpeg exists in the project directory
if exist "%cd%\ffmpeg.exe" (
	echo ffmpeg already exists. The project is setup.
	goto done
) else (
	echo ffmpeg does not exist in the project directory. The project is not ready.
	echo Downloading ffmpeg...
	REM Download ffmpeg using powershell
	powershell $url = 'https://github.com/Download-Manager-Community/ffmpeg/releases/download/ffmpeg/ffmpeg.exe'; $out = '%cd%\ffmpeg.exe'; Invoke-WebRequest -Uri $url -OutFile $out
	
	REM If ffmpeg failed to download and the file does not exist, go to error
	if not exist "%cd%\ffmpeg.exe" goto error
	goto done
)

:done
echo.
echo Project setup complete.
echo Press any key exit...
pause > nul
exit

:error
echo.
echo Failed to download ffmpeg. Project setup incomplete.
echo Press any key to exit...
pause > nul
exit %ERRORLEVEL%