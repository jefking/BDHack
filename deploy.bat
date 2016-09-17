@echo off 
echo ---Deploying site 
 
REM ---Deploy the wwwroot folder in repository to default target (wwwroot)
xcopy %DEPLOYMENT_SOURCE%\wwwroot\* %DEPLOYMENT_TARGET%/Y /s 
 
REM ---Deploy the myapp folder in repository to folder above default target (wwwroot\..\myapp)
xcopy %DEPLOYMENT_SOURCE%\myapp\* %DEPLOYMENT_TARGET%\..\src\bot\products\products /Y /s 