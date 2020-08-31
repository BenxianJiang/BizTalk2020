REM use the following to build BizTalk deployment MSI. The MSI result will locate in bin folder of deployment project.
"C:\Windows\Microsoft.NET\Framework\V4.0.30319\MSBuild.exe" /p:Configuration=Debug /t:Installer "C:\GitHub\Ben\Ben.Demo.BizTalk\Ben.Demo.BizTalk.Deployment\Deployment.btdfproj"
Pause