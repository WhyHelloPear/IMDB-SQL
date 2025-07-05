import os
import csv

# Read the CSV file
csv_file = "Tokenizers/tokenized_files.csv"
with open(csv_file, mode='r') as file:
    reader = csv.DictReader(file)
    files = [row['file'] for row in reader]

# Path to the secrets .env file
secrets_path = "./secrets.env"

# Read the secrets .env file
with open(secrets_path, 'r') as file:
    secrets_content = file.readlines()

# Convert .env content to a dictionary
secrets = {}
for line in secrets_content:
    line = line.strip()
    if line and '=' in line:
        key, value = map(str.strip, line.split('=', 1))
        secrets[key] = value

# Function to replace tokens in a file with corresponding secret values
def replace_secrets_in_file(file_path, secrets):
    with open(file_path, 'r') as file:
        content = file.read()

    # Replace each token with the corresponding secret value
    for key, value in secrets.items():
        token = f"{{{{{key}}}}}"  # Assuming tokens are in the format {{SECRET_KEY}}
        content = content.replace(token, value)

    # Write the updated content back to the file
    with open(file_path, 'w') as file:
        file.write(content)

# Iterate over each file and replace the tokens with secret values
for file in files:
    if os.path.exists(file):
        print(f"Processing file: {file}")
        replace_secrets_in_file(file, secrets)
    else:
        print(f"File not found: {file}")

print("Secret replacement completed.")
