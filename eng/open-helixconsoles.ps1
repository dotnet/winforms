[CmdletBinding(PositionalBinding=$false)]
param (
    # Something like https://helix.dot.net/api/jobs/{guid}/workitems?api-version=2019-06-17
    [Parameter(Mandatory = $true)]
    [string] $helixWorkItemUrl,
    [Parameter(ValueFromRemainingArguments=$true)][String[]]$properties
)

$workItemDefinition =  Invoke-RestMethod -Uri $helixWorkItemUrl
$workItemDefinition | ForEach-Object {
    $jobName = $_.Name
    if (!$jobName.EndsWith('.dll')) {
        return;
    }

    $jobId = $_.Job
    $jobState = $_.State

    $consoleUrl = "https://helix.dot.net/api/2019-06-17/jobs/$jobId/workitems/$jobName/console"

    if ($jobState -ne 'Finished') {
        Write-Host " --> ($jobState) $jobName" -NoNewline -ForegroundColor Yellow
    }
    else {
        Write-Host " --> ($jobState) $jobName" -NoNewline -ForegroundColor Green
    }
    Write-Host ": $consoleUrl"
}