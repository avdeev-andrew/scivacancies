pushd %~dp0
set /p input=DropDatabase before applying migrations? [y/n] (default - n)

if "%input%" EQU "y" (rh.exe --databasetype=postgresql -c "Server=localhost;Database=scivacancies;User Id=postgres;Password=postgres" -vf version.txt -env=AMURAVYEV /drop)

rh.exe --databasetype=postgresql -c "Server=localhost;Database=scivacancies;User Id=postgres;Password=postgres" -vf version.txt -env=AMURAVYEV
popd