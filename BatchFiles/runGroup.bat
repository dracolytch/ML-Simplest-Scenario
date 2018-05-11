echo.1>layers.txt
for /l %%x in (1, 1, 5) do (
  start /wait learn.py threeinputs-xLayers --run-id=threeInputs-1x-r%%x --train --save-freq=5000
)

echo.5>layers.txt
for /l %%x in (1, 1, 5) do (
  start /wait learn.py threeinputs-xLayers --run-id=threeInputs-5x-r%%x --train --save-freq=5000
)

echo.10>layers.txt
for /l %%x in (1, 1, 5) do (
  start /wait learn.py threeinputs-xLayers --run-id=threeInputs-10x-r%%x --train --save-freq=5000
)

echo.20>layers.txt
for /l %%x in (1, 1, 5) do (
  start /wait learn.py threeinputs-xLayers --run-id=threeInputs-20x-r%%x --train --save-freq=5000
)

echo.50>layers.txt
for /l %%x in (1, 1, 5) do (
  start /wait learn.py threeinputs-xLayers --run-id=threeInputs-50x-r%%x --train --save-freq=5000
)

echo.100>layers.txt
for /l %%x in (1, 1, 5) do (
  start /wait learn.py threeinputs-xLayers --run-id=threeInputs-100x-r%%x --train --save-freq=5000
)