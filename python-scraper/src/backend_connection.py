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
    check_required_env_vars(["USERS_ENDPOINT", "OLYMPIAD_ENDPOINT", "RPC_ENDPOINT", "LOGIN_URL", "USERNAME", "PASSWORD", "ROLE_ASSIGNMENT_ENDPOINT"])


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
        "rpc_endpoint": os.getenv("RPC_ENDPOINT"),
        "role_assignment_endpoint": os.getenv("ROLE_ASSIGNMENT_ENDPOINT")
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

def fetch_all_users(api_endpoint, token):
    headers = {"Authorization": f"Bearer {token}"}
    try:
        response = requests.get(api_endpoint, headers=headers)
        response.raise_for_status()
        return response.json()
    except requests.RequestException as e:
        print(f"Error fetching users: {e}")
        return str(e)


def assign_role_to_user(api_endpoint, token, user_id, role_id=2):
    headers = {"Authorization": f"Bearer {token}"}
    assigned_at = datetime.now().strftime("%Y-%m-%dT%H:%M:%S")
    payload = {
        "UserId": user_id,
        "RoleId": role_id,
        "AssignedAt": assigned_at
    }
    try:
        response = requests.post(api_endpoint, json=payload, headers=headers)
        response.raise_for_status()
        print(f"Role ID {role_id} successfully assigned to User ID {user_id}")
    except requests.RequestException as e:
        print(f"Error assigning role: {e}")


def request_password_reset_for_user(api_endpoint, token, email_or_username):
    payload = {"UsernameOrEmail": email_or_username}
    return send_to_endpoint(f"{api_endpoint}", token, payload)


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

    user1 = {
        "Name": "Borislav Milanov",
        "DateOfBirth": "2006-05-30",
        "AcademicYearId": int(academic_year),
        "Username": "bob4o",
        "Email": "borislav.b.milanov.2020@elsys-bg.org",
        "Password": "somepassword123",
        "Gender": "Male",
        "EmailVerified": False
    }

    user2 = {
        "Name": "Milan Borislavov",
        "DateOfBirth": "2006-06-30",
        "AcademicYearId": int(academic_year),
        "Username": "bob4oto",
        "Email": "bobi06bobi@gmail.com",
        "Password": "somepassword123",
        "Gender": "Male",
        "EmailVerified": False
    }
    
    errors = []
    reset_errors = []

    for user in [user1, user2]:
        error = send_to_endpoint(endpoints["users_endpoint"], token, user)
        if error:
            errors.append({"user": user["Name"], "error": error})

    if errors:
        print("\nErrors during user creation:")
        for err in errors:
            print(f"User: {err['user']}, Error: {err['error']}")
    else:
        print("\nUsers created successfully.")


    users = fetch_all_users(endpoints["users_endpoint"], token)
    if not users:
        print("Failed to fetch users. Cannot assign roles.")
        return        

    print(users)

    for user in [user1, user2]:
        matching_user = next(
                (u for u in users if u.get("email", "").strip().lower() == user["Email"].strip().lower()), 
                None
            )
        if not matching_user:
            print(f"User {user['Name']} not found in the system.")
            continue
        try:
            assign_role_to_user(endpoints["role_assignment_endpoint"], token, matching_user["userId"])
            print(f"Sending password reset request for: {user['Email']}")
            
            reset_error = request_password_reset_for_user(endpoints["rpc_endpoint"], token, user["Email"])
            if reset_error:
                reset_errors.append({"user": user["Name"], "error": reset_error})
        except Exception as e:
            reset_errors.append({"user": user["Name"], "error": str(e)})

    if reset_errors:
        print("\nErrors during password reset requests:")
        for err in reset_errors:
            print(f"User: {err['user']}, Error: {err['error']}")
    else:
        print("\nPassword reset requests sent successfully.")

    if errors:
        print("\nErrors occurred during user creation:")
        for err in errors:
            print(f"User: {err['user']}, Error: {err['error']}")
    else:
        print("\nAll users created and roles assigned successfully.")

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
                    if olympiad_count >= 10:
                        print("\nLimit of 10 Olympiads reached. Stopping further uploads.")
                        if olympiad_errors:
                            print("\nErrors occurred during olympiad upload:")
                            for err in olympiad_errors:
                                print(
                                    f"Subject: {err['subject']}, Ring: {err['ring']}, Date: {err['date']}, Class: {err['class']}, Error: {err['error']}"
                                )
                        else:
                            print("\nAll data uploaded successfully!")
                        
                        return 0
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

                    error = send_to_endpoint(endpoints["olympiad_endpoint"], token, payload)
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


