## File System Database
[File System Database] is a project college to database 1(DB1) subject in Yarmouk Private University(YPU). it is a program that simulates file system in operating system and it controls through commands(CMD) as follows:

  - Folder: (id, FolderName, ParentID).
  - File: (id, FileName, SizeInBytes, ParentID).
  - Program: (id, ProgramName, Command, ParentID).

This program wrote in C# language where appears user interface(UI) like CMD. At the beginning, it shows root directory which considers head of tree in the file system, and it performs the following instructions:
  - CD [folder name]: to enter into a specific folder.
  - CD . :  to go to one folder up.
  - DIR: to view contents of a folder.
  - MKDIR [folder name]: to create folder in current location.
  - RMDIR [folder name]: to remove empty folder in current location.
  - RENAME [old name file or folder name] [new name file or folder name]: to rename folder.
  - MKFILE [file name] [file content]: to create new file in current path and put content inside.
  - RMFILE [file name]: to remove file in current location.
  - MKPROG [program name] [Command]: to create new program in current location.
  - RMPROG [program name]: to remove program in current location.
  - OPEN [file name]: to open file and show content inside in the current location. 
