import os
from dotenv import load_dotenv
from supabase import create_client, Client

class SupabaseClient:
    def __init__(self):
        load_dotenv()

        self.supabase_url = os.getenv("SUPABASE_URL")
        self.supabase_key = os.getenv("SUPABASE_API_KEY")
        self.bucket_name = os.getenv("SUPABASE_BUCKET_NAME")

        if not all([self.supabase_url, self.supabase_key, self.bucket_name]):
            raise ValueError("Environment variables for Supabase configuration are missing")

        self.client: Client = create_client(self.supabase_url, self.supabase_key)

    def upload_file(self, file_path: str, folder: str) -> None:
        if not os.path.isfile(file_path):
            raise FileNotFoundError(f"File not found: {file_path}")

        with open(file_path, "rb") as file:
            file_data = file.read()

        response = self.client.storage.from_(self.bucket_name).upload(
            f"{folder}/{os.path.basename(file_path)}", file_data
        )

        if response.status_code == 200:
            print(f"File uploaded successfully to folder '{folder}' in Supabase bucket '{self.bucket_name}'.")
        else:
            raise RuntimeError(f"Upload failed with status code {response.status_code}. Response: {response.text}")
