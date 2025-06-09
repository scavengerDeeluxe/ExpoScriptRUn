param(
    [Parameter(Mandatory=$true)]
    [string]$ComputerName
)

Get-ComputerInfo -ComputerName $ComputerName
