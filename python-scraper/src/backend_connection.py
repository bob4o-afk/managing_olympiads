import json
import os
import requests
import time
from datetime import datetime
from dotenv import load_dotenv
import bcrypt
from src.exceptions import EnvironmentVariableError
from src.load_env import load_environment_variables


def load_environment():
    load_dotenv()
    check_required_env_vars(["USERS_ENDPOINT", "OLYMPIAD_ENDPOINT", "RPC_ENDPOINT", "LOGIN_URL", "USERNAME", "PASSWORD"])


def check_required_env_vars(required_vars):
    missing_vars = [var for var in required_vars if not os.getenv(var)]
    if missing_vars:
        raise EnvironmentVariableError(f"Missing required environment variables: {', '.join(missing_vars)}")


def calculate_academic_year():
    today = datetime.now()
    return 1 if today.month < 9 else 2


def get_api_endpoints():
    return {
        "users_endpoint": os.getenv("USERS_ENDPOINT"),
        "olympiad_endpoint": os.getenv("OLYMPIAD_ENDPOINT"),
        "rpc_endpoint": os.getenv("RPC_ENDPOINT")
    }


def authenticate():

    login_url = os.getenv("LOGIN_URL")
    username = os.getenv("USERNAME")
    password = os.getenv("PASSWORD")

    payload = {
        "usernameOrEmail": username,
        "Password": password
    }

    for attempt in range(5):
        try:
            response = requests.post(login_url, json=payload)
            response.raise_for_status()
            token = response.json().get("token")
            if not token:
                raise ValueError("Authentication failed: No token returned.")
            return token
        except requests.RequestException as e:
            print(f"Authentication failed, retrying... ({attempt + 1}/5): {e}")
            time.sleep(5)

    raise RuntimeError("Authentication failed after multiple attempts.")


def hash_password(password: str):
    salt = bcrypt.gensalt()
    return bcrypt.hashpw(password.encode('utf-8'), salt).decode('utf-8')


def send_to_endpoint(api_endpoint, token, payload):
    headers = {"Authorization": f"Bearer {token}"}
    try:
        response = requests.post(api_endpoint, json=payload, headers=headers)
        response.raise_for_status()
        print(f"Successfully sent: {payload}")

        return None
    except requests.RequestException as e:
        print(f"Error sending payload: {e}")
        return str(e)

def process_and_send_data():
    load_dotenv()
    _, _, _, OUTPUT_JSON_PATH = load_environment_variables()
    if not os.path.exists(OUTPUT_JSON_PATH):
        raise FileNotFoundError(f"File not found: {OUTPUT_JSON_PATH}")

def request_password_reset_for_user(api_endpoint, token, email_or_username):
    payload = {"UsernameOrEmail": email_or_username}
    return send_to_endpoint(f"{api_endpoint}/request-password-change", token, payload)


def process_and_send_data():
    load_environment()
    _, _, _, OUTPUT_JSON_PATH = load_environment_variables()

    if not os.path.exists(OUTPUT_JSON_PATH):
        raise FileNotFoundError(f"File not found: {OUTPUT_JSON_PATH}")

    with open(OUTPUT_JSON_PATH, "r", encoding="utf-8") as file:
        data = json.load(file)

    endpoints = get_api_endpoints()
    academic_year = calculate_academic_year()
    token = authenticate()

    users_data = [
        {
            "Name": "Borislav Milanov",
            "DateOfBirth": "30.05.2006",
            "AcademicYearId": academic_year,
            "Username": "bob4o",
            "Email": "borislav.b.milanov.2020@elsys-bg.org",
            "Password": hash_password("somepassword123"),
            "Gender": "Male",
            "EmailVerified": False,
            "CreatedAt": datetime.now().isoformat()
        },
        {
            "Name": "Milan Borislavov",
            "DateOfBirth": "05.30.2006",
            "AcademicYearId": academic_year,
            "Username": "bob4oo123",
            "Email": "bobi06bobi@gmail.com",
            "Password": hash_password("anotherpassword456"),
            "Gender": "Male",
            "EmailVerified": False,
            "CreatedAt": datetime.now().isoformat()
        }
    ]

    errors = []
    reset_errors = []

    for user in users_data:
        error = send_to_endpoint(endpoints["users_endpoint"], token, user)
        if error:
            errors.append({"user": user["Name"], "error": error})

    for user in users_data:
        reset_error = request_password_reset_for_user(endpoints["rpc_endpoint"], token, user["Email"])
        if reset_error:
            reset_errors.append({"user": user["Name"], "error": reset_error})

    if errors:
        print("\nErrors occurred during user creation:")
        for err in errors:
            print(f"User: {err['user']}, Error: {err['error']}")
    else:
        print("\nUsers created successfully.")

    if reset_errors:
        print("\nErrors occurred during password reset request:")
        for err in reset_errors:
            print(f"User: {err['user']}, Error: {err['error']}")
    else:
        print("\nPassword reset requests sent successfully.")

    olympiad_count = 0
    olympiad_errors = []
    for subject, rings in data.items():
        for ring, ring_data in rings.items():
            classes = ring_data.get("class", [])
            dates = ring_data.get("dates", [])
            time = ring_data.get("time", None)

            if isinstance(dates, list):
                dates = [date.strip() for date in dates[0].split(",")]

            for class_num in classes:
                for date in dates:
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

                    error = send_to_endpoint(payload)
                    if error:
                        olympiad_errors.append({
                            "subject": subject,
                            "ring": ring,
                            "date": date,
                            "class": class_num,
                            "error": error
                        })
                    else:
                        olympiad_count += 1

    if olympiad_errors:
        print("\nErrors occurred during olympiad upload:")
        for err in olympiad_errors:
            print(
                f"Subject: {err['subject']}, Ring: {err['ring']}, Date: {err['date']}, Class: {err['class']}, Error: {err['error']}"
            )
    else:
        print("\nAll data uploaded successfully!")
