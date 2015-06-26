@ECHO OFF
SET ConfigurationName=%~1
SET SolutionDir=%~2
SET TargetDir=%~3
SET TargetPath=%~4
SET TargetName=%~5
IF /I "%ConfigurationName%" == "Release" (
  @ECHO ON
  "%SolutionDir%packages\ilmerge.2.14.1208\tools\ILMerge.exe" /out:"%TargetDir%%TargetName%M.exe" "%TargetPath%" "%TargetDir%CommandLineLib.dll" "%TargetDir%Interop.PortableDeviceApiLib.dll" "%TargetDir%Interop.PortableDeviceTypesLib.dll"
  del "%TargetPath%"
  move "%TargetDir%%TargetName%M.exe" "%TargetPath%"
)