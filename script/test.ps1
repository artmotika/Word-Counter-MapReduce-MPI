$exitcode = 0
Get-ChildItem -Path .\test\ -Directory |
        ForEach-Object {
            $input = Join-Path $_.FullName input.txt
            $output = Join-Path $_.FullName output.txt
            $diff = mpiexec.exe -n 4 .\bin\Debug\net5.0\mpi-problem.exe $input | Sort-Object | Compare-Object -ReferenceObject (Get-Content $output) -CaseSensitive
            if ($diff)
            {
                Write-Host ("Test Failed: ", $_.Name) -ForegroundColor Red
                $diff
                $exitcode = 1
            }
            else
            {
                Write-Host ("Test Passed: ", $_.Name) -ForegroundColor Green
            }
        }
exit $exitcode
