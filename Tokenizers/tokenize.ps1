# Read the CSV file
$files = Import-Csv "Tokenizers\tokenized_files.csv" | Select-Object -ExpandProperty file

# Path to the secrets .env file
$secretsPath = ".\secrets.env"

icacls .\config.inc.php /inheritance:r /grant:r "Users:RW"

# Read the secrets .env file
$secretsContent = Get-Content $secretsPath

# Convert .env content to Hashtable
$secrets = @{}
foreach ($line in $secretsContent) {
    if ($line -match "^\s*([^=]+)\s*=\s*(.*)\s*$") {
        $key = $matches[1].Trim()
        $value = $matches[2].Trim()
        $secrets[$key] = $value
    }
}

# Function to replace detokenized secrets with corresponding tokens in a file
function Replace-DetokenizedWithTokens {
    param (
        [string]$filePath,
        [hashtable]$secrets
    )

    # Read the file content
    $content = Get-Content $filePath

    # Replace each secret value with the corresponding token
    foreach ($key in $secrets.Keys) {
        $secretValue = [regex]::Escape($secrets[$key])
        $token = "{{$key}}"  # Assuming tokens are in the format {{SECRET_KEY}}
        $content = $content -replace $secretValue, $token
    }

    # Write the updated content back to the file
    Set-Content -Path $filePath -Value $content
}

# Iterate over each file and replace the detokenized secrets with tokens
foreach ($file in $files) {
    if (Test-Path $file) {
        Write-Output "Processing file: $file"
        Replace-DetokenizedWithTokens -filePath $file -secrets $secrets
    } else {
        Write-Output "File not found: $file"
    }
}

icacls .\config.inc.php /inheritance:r /grant:r "Users:R" /grant:r "Administrators:F" /grant:r "SYSTEM:F"

Write-Output "Secret tokenization completed."
