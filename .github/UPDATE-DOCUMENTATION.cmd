@echo off
cd ..
doxygen
git add docs/\*
git commit -m "Update Documentation"
echo.
echo.
echo.
echo.
echo.
echo.
echo Attempted to stage commit with updated documentation. See above for results.
echo.
echo.
echo.
echo.
echo.
echo.
pause
