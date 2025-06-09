param(
    [Parameter(Mandatory=$true)]
    [string]$ComputerName,
    [Parameter(Mandatory=$true)]
    [string]$ServiceName
)

Get-Service -ComputerName $ComputerName -Name $ServiceName
