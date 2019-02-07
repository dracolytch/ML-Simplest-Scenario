@echo off
set /p agents=<".\Assets\BatchFiles\ml-agents-path.txt"
set /p machine=<".\Assets\BatchFiles\machine-name.txt"

cd %agents%

start tensorboard.exe --logdir=summaries
start http://%machine%:6006