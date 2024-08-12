import os
from dotenv import load_dotenv
from supabase import create_client, Client

class SupabaseClient:
    def __init__(self):
        load_dotenv()

        # Load environment variables and validate
        self.supabase_url = os.getenv("SUPABASE_URL")
        self.supabase_key = os.getenv("SUPABASE_API_KEY")
        self.bucket_name = os.getenv("SUPABASE_BUCKET_NAME")

        if not all([self.supabase_url, self.supabase_key, self.bucket_name]):
            raise ValueError("Environment variables for Supabase configuration are missing")

        self.client: Client = create_client(self.supabase_url, self.supabase_key)

    def upload_file(self, file_path: str, folder: str) -> None:
        """Upload a file to a specific folder in a Supabase bucket."""
        try:
            with open(file_path, "rb") as file:
                file_data = file.read()

            # Ensure file_data is properly encoded if needed
            response = self.client.storage.from_(self.bucket_name).upload(f"{folder}/{os.path.basename(file_path)}", file_data)

            # Check response status
            if response.status_code == 200:
                print(f"File uploaded successfully to folder '{folder}' in Supabase bucket '{self.bucket_name}'.")
            else:
                print(f"Failed to upload file: {response.error_message}")

        except FileNotFoundError:
            print(f"File not found: {file_path}")
        except Exception as e:
            print(f"An error occurred: {e}")

