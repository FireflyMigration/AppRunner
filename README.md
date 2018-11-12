# AppRunner 

AppRunner lets you run an application and send arguments to it using a settings text file named AppRunner.settings (You should create it)

The setting file includes 3 parameters:

**ExeFile** – The name of the .exe file you want to run (e.g. MyApp.exe)  
**WorkingDir** – The working directory path (e.g C:\Temp\)  
**CommandLineArgs** – Argument you would like to send to the application (e.g /ini=c:\temp\Myini.ini)  
**ShadowCopyFiles=Y** - This option is needed only if your run the application from a mapped network drive and get an SEH Exception from time to time.

Here's an example of `AppRunner.settings` file
```
ExeFile=x:\fireflymigration\northwind\northwind.exe
WorkingDir=x:\fireflymigration\northwind
CommandLineArgs=/ini=x:\somewhere\northwind.ini
ShadowCopyFile=Y
```


> Note that you can change the name of AppRunner.exe to any name you like, as long as you have a settings file with the same name

You can also create a shortcut and specify the command line argument in the shortcut

Note that the ExeFile and the WorkingDir parameters are mandatory.
