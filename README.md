# AppRunner 

AppRunner lets you run an application and send arguments to it using a settings text file named AppRunner.settings (You should create it)

The setting file includes 3 parameters:

**ExeFile** – The name of the .exe file you want to run (e.g. MyApp.exe)  
**WorkingDir** – The working directory path (e.g C:\Temp\)  
**CommandLineArgs** – Argument you would like to send to the application (e.g /ini=c:\temp\Myini.ini)  

You can also create a shortcut and specify the command line argument in the shortcut

Notice that the ExeFile and the WorkingDir parameters are mandatory.
