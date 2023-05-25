write-Host("Execute this Script Only If This is the FIRST time the code is being deployed on The  environment, if it is a new release DO NOT run this ")
write-Host("Enter Y to Proceed and N to cancel")
$flag = Read-Host
if($flag -eq 'Y')
{
	$ExecuionPolicy = Get-ExecutionPolicy 
write-host("Current Execution Policy Is")
write-host($ExecuionPolicy )
write-host('Setting Execution policy to RemoteSigned TO Execute the script')
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned 
New-EventLog -LogName 'Application' -Source 'BizTalk Ben Demo'
write-host('Resetting the Execution policy to previous value')
Set-ExecutionPolicy -ExecutionPolicy $ExecuionPolicy 

$ExecuionPolicyAfterResetting = Get-ExecutionPolicy 
write-host($ExecuionPolicyAfterResetting )

}
else
{
	write-host("Script Execution Cancelled")
}
Read-Host 