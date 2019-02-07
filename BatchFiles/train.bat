@echo off
set /p Agents=<ml-agents-path.txt

cd %agents%

if [%1]==[] goto usage
@echo Starting Unity ML Agents
mlagents-learn config/trainer_config.yaml --run-id=%1 --train
goto :eof
:usage
@echo Usage: %0 ^<RunId^>
exit /B 1