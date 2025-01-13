import json
import os
import requests
from datetime import datetime

API_ENDPOINT = "http://localhost:5138/api/olympiad"

file_path = "../files/extracted_table.json"
if not os.path.exists(file_path):
    raise FileNotFoundError(f"File not found: {file_path}")

with open(file_path, "r", encoding="utf-8") as file:
    data = json.load(file)


def send_to_endpoint(payload):
    try:
        response = requests.post(API_ENDPOINT, json=payload)
        response.raise_for_status()
        print(f"Successfully sent: {payload}")
        return None  # No errors
    except requests.RequestException as e:
        print(f"Error sending: {payload}. Error: {e}")
        return str(e)


academic_year = 1  # To be calculated

errors = []
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
                    errors.append({"subject": subject, "ring": ring, "date": date, "class": class_num, "error": error})

if errors:
    print("\nErrors occurred during upload:")
    for err in errors:
        print(
            f"Subject: {err['subject']}, Ring: {err['ring']}, Date: {err['date']}, Class: {err['class']}, Error: {err['error']}")
else:
    print("\nAll data uploaded successfully!")
