import os
import requests
from bs4 import BeautifulSoup
from urllib.parse import urljoin
import datetime
from src.supabase_client import SupabaseClient
from dotenv import load_dotenv
from src.exceptions import EnvironmentVariableError

def check_required_env_vars(required_vars):
    missing_vars = [var for var in required_vars if not os.getenv(var)]
    if missing_vars:
        raise EnvironmentVariableError(f"Missing required environment variables: {', '.join(missing_vars)}")


def download_pdf():
    # Load environment variables
    load_dotenv()

    check_required_env_vars(["BASE_URL", "SUPABASE_URL", "SUPABASE_API_KEY", "SUPABASE_BUCKET_NAME"])


    # Get the base URL from environment variables
    base_url = os.getenv("BASE_URL")

    # Determine the current academic year
    current_date = datetime.datetime.now()
    if current_date.month > 9 or (current_date.month == 9 and current_date.day >= 15):
        start_year = current_date.year - 1
        end_year = current_date.year
    else:
        start_year = current_date.year - 2
        end_year = current_date.year - 1

    # Construct the URL based on the current academic year
    url = f"{base_url}/ol-{start_year}-{end_year}/"

    # Send a GET request to the webpage
    response = requests.get(url)
    response.raise_for_status()  # Check if the request was successful

    # Parse the webpage content
    soup = BeautifulSoup(response.content, 'html.parser')

    # Find the iframe and extract the src attribute
    iframe = soup.find("iframe", class_="ead-iframe")
    if iframe is not None:
        src_link = iframe['src']

        # Handle the case where the src might be a relative URL
        src_link_full = urljoin(url, src_link)

        # Extract the direct link to the PDF
        pdf_url = src_link_full.split('url=')[1].split('&')[0]
        pdf_url = requests.utils.unquote(pdf_url)

        # Download the PDF
        pdf_response = requests.get(pdf_url)
        pdf_response.raise_for_status()  # Check if the request was successful

        # Ensure the "files" directory exists
        os.makedirs("files", exist_ok=True)

        # Save the PDF to the "files" folder
        pdf_filename = os.path.join("files", os.path.basename(pdf_url))
        with open(pdf_filename, 'wb') as f:
            f.write(pdf_response.content)

        print(f"PDF downloaded successfully: {pdf_filename}")

        # Upload the file to Supabase
        upload_file_to_supabase(pdf_filename, start_year, end_year)

    else:
        print("Iframe not found on the page.")


def upload_file_to_supabase(pdf_filename, start_year, end_year):
    folder_name = f"{start_year}-{end_year}"

    print(f"Uploading file: {pdf_filename} to folder: {folder_name}")
    supabase_client = SupabaseClient()

    try:
        # Upload the file to Supabase
        supabase_client.upload_file(pdf_filename, folder_name)
    except Exception as e:
        print(f"Failed to upload file: {e}")