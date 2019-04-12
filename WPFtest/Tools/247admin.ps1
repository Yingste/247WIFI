

# Self-elevate the script if required
#if (-Not ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] 'Administrator')) 
#{
#    if ([int](Get-CimInstance -Class Win32_OperatingSystem | Select-Object -ExpandProperty BuildNumber) -ge 6000) 
#    {
#        $CommandLine = "-File `"" + $MyInvocation.MyCommand.Path + "`" " + $MyInvocation.UnboundArguments
#        Start-Process -FilePath PowerShell.exe -Verb Runas -ArgumentList $CommandLine
#        #Exit
#    }
#}


#NOT FINISHED - need to set the working directory to parent of script directory
#$executingScriptDirectory = $MyInvocation.MyCommand.Path 
#cd ..\



$passInput = 'S7550l10897#Gxb4'  #This would need to be updated depending on what site 
$password = ConvertTo-SecureString $passInput -AsPlainText -Force #convert the password string into a securestring 
New-LocalUser "247admin" -Password $password -FullName "247admin" -Description "247admin" -PasswordNeverExpires #create the 247admin user and set password
Set-LocalUser "247admin" -PasswordNeverExpires $true #double check that password never expires
Set-LocalUser "247admin" -UserMayChangePassword $true #I was told that we need to make sure that they can not change the password?
Add-LocalGroupMember -Group "Administrators" -Member "247admin" #Add the 247admin account to the admin group
Pause
