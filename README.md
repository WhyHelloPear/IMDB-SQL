# IMDB-SQL
 
Download IMDB datasets from here: https://developer.imdb.com/non-commercial-datasets/
- name.basics.tsv &#9745;
- title.akas.tsv &#9745;
- title.basics.tsv &#9745;
- title.crew.tsv &#9745;
- title.episode.tsv &#9745;
- title.principals.tsv &#9745;
- title.ratings.tsv &#9745;

# How to Setup

1. Create ___secrets.env___ file in base directory

    ```
        SECRET_DB_ROOTPASSWORD=password_for_db_root_user_here
        SECRET_DB_PASSWORD=password_used_to_signin_with
    ```

2. Open terminal and navigate to base directory of project

3. Run: ```.\Tokenizers\detokenize.ps1```
    - This will detokenize target files that contain the secrets in the secrets.env file we created earlier.
    - .\Tokenizers\tokenized_files.csv contains the files that will be targeted during detokenization.

4. Run: ```docker-compose up -d```
    - This will setup the IMDB database with the created user in .\SQL\DB_Setup.sql
    - This will also setup a simple phpMyAdmin page for you to interact w/ said database.
        - Visit, http://localhost:4080/, to access
        - Login w/ credentials you provided:
            - username: imdb_admin
            - password: password_used_to_signin_with
                - this should match the password you gave in .\secrets.env