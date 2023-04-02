# Problem 5: MPI MapReduce

Implement the MapReduce approach using MPI in C #

Task: When starting the program, the path to the text file is passed as a command line argument. 
It is necessary for each word to count the number of its occurrences in the document.

### Input data format

A text file consisting of letters of the Latin alphabet, spaces and line breaks.

### Output data format

A list of `word number_of_occurrence` pairs should be printed to the standard output stream

[Output example](./test/RealText2/output.txt)

## Dependencies

[.NET 5](https://dotnet.microsoft.com/download/dotnet/5.0)

## Install MPI

```powershell
  .\script\install_mpi.ps1
```

## Run tests

```powershell
  .\script\test.ps1
```


## Build and Run Locally

Build project

```powershell
  .\script\build.ps1
```

Run with MPI

```powershell
  mpiexec.exe -n 4 .\bin\Debug\net5.0\test-dotnet-mpi.exe $path_to_input_file
```
