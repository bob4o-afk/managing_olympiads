import json
import os
import requests
import time
from datetime import datetime
from dotenv import load_dotenv

from src.exceptions import EnvironmentVariableError
from src.load_env import load_environment_variables

def check_required_env_vars(required_vars):
    missing_vars = [var for var in required_vars if not os.getenv(var)]
    if missing_vars:
        raise EnvironmentVariableError(f"Missing required environment variables: {', '.join(missing_vars)}")

def calculate_academic_year():
    today = datetime.now()
    return 1 if today.month < 9 else 2

def get_api_endpoint():
    load_dotenv()
    check_required_env_vars(["API_ENDPOINT"])
    return os.getenv("API_ENDPOINT")

def authenticate():
    load_dotenv()
    check_required_env_vars(["LOGIN_URL", "USERNAME", "PASSWORD"])

    login_url = os.getenv("LOGIN_URL")
    username = os.getenv("USERNAME")
    password = os.getenv("PASSWORD")

    payload = {
        "usernameOrEmail": username,
        "Password": password
    }

    max_retries = 5
    retries = 0
    while retries < max_retries:
        try:
            response = requests.post(login_url, json=payload)
            response.raise_for_status()
            token = response.json().get("token")
            if not token:
                raise ValueError("Authentication failed: No token returned.")
            return token
        except requests.RequestException as e:
            print(f"Authentication failed, retrying... ({retries + 1}/{max_retries})")
            retries += 1
            time.sleep(5)

    raise RuntimeError("Authentication failed after multiple attempts.")


def send_to_endpoint(api_endpoint, token, payload):
    headers = {
        "Authorization": f"Bearer {token}"
    }
    try:
        response = requests.post(api_endpoint, json=payload, headers=headers)
        response.raise_for_status()
        print(f"Successfully sent: {payload}")
        return None
    except requests.RequestException as e:
        print(f"Error sending: {payload}. Error: {e}")
        return str(e)

def process_and_send_data():
    load_dotenv()
    _, _, _, OUTPUT_JSON_PATH = load_environment_variables()
    if not os.path.exists(OUTPUT_JSON_PATH):
        raise FileNotFoundError(f"File not found: {OUTPUT_JSON_PATH}")

    with open(OUTPUT_JSON_PATH, "r", encoding="utf-8") as file:
        data = json.load(file)

    api_endpoint = get_api_endpoint()
    academic_year = calculate_academic_year()
    token = authenticate()
    errors = []

    olympiad_count = 0

    for subject, rings in data.items():
        for ring, ring_data in rings.items():
            classes = ring_data.get("class", [])
            dates = ring_data.get("dates", [])
            time = ring_data.get("time", None)

            if isinstance(dates, list):
                dates = [date.strip() for date in dates[0].split(",")]

            for class_num in classes:
                for date in dates:
                    if olympiad_count >= 20:
                        return

                    try:
                        date_obj = datetime.strptime(date, "%d.%m.%Y")
                        formatted_date = date_obj.strftime("%Y-%m-%d")
                    except ValueError:
                        print(f"Error parsing date: {date}")
                        continue

                    start_time = None
                    if time:
                        try:
                            time = time.replace(" o'clock", "")
                            time_obj = datetime.strptime(time, "%H:%M")
                            start_time = f"{formatted_date}T{time_obj.strftime('%H:%M:%S')}"
                        except ValueError:
                            print(f"Error parsing time: {time}")
                            continue

                    payload = {
                        "Subject": subject.replace("_", " ").title(),
                        "Description": "none",
                        "DateOfOlympiad": formatted_date,
                        "Round": ring.replace("_", " ").title(),
                        "Location": "Bulgaria",
                        "StartTime": start_time,
                        "AcademicYearId": academic_year,
                        "ClassNumber": class_num,
                    }

                    if not start_time:
                        del payload["StartTime"]

                    error = send_to_endpoint(api_endpoint, token, payload)
                    if error:
                        errors.append({
                            "subject": subject,
                            "ring": ring,
                            "date": date,
                            "class": class_num,
                            "error": error
                        })
                    else:
                        olympiad_count += 1

    if errors:
        print("\nErrors occurred during upload:")
        for err in errors:
            print(
                f"Subject: {err['subject']}, Ring: {err['ring']}, Date: {err['date']}, Class: {err['class']}, Error: {err['error']}"
            )
    else:
        print("\nAll data uploaded successfully!")
