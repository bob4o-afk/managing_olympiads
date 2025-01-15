import re
import json
import os
from datetime import datetime, timedelta
from dotenv import load_dotenv

from src.text_processing import convert_roman_to_arabic, translation_dict, competition_levels
from src.load_env import load_environment_variables


def extract_and_process_text():
    # Load environment variables from .env file
    load_dotenv()
    _, _, TXT_PATH, OUTPUT_JSON_PATH = load_environment_variables()

    olympiad_data = {}

    current_competition_level = None

    with open(TXT_PATH, 'r', encoding='utf-8') as file:
        for line in file:
            parts = line.split("\t")
            if len(parts) == 1:
                # If the line has only one part, it's likely a competition level
                competition_level = parts[0].strip()
                if competition_level in competition_levels:
                    current_competition_level = competition_levels[competition_level]
                continue

            if len(parts) < 4 or current_competition_level is None:
                continue

            subject = parts[0].strip()
            classes_str = parts[1].strip()
            date_str = clean_date(parts[-1].strip())

            translated_subject = translation_dict.get(subject, subject)
            classes_str = convert_roman_to_arabic(classes_str.replace("И", "").strip())
            classes = []

            class_parts = re.split(r'[,\s]+', classes_str)
            for part in class_parts:
                part = part.strip()
                if "-" in part and not is_date_range(part):
                    try:
                        start, end = map(int, part.split("-"))
                        classes.extend(range(start, end + 1))
                    except ValueError:
                        print(f"Warning: Unable to process class range '{part}'.")
                else:
                    try:
                        classes.append(int(part))
                    except ValueError:
                        print(f"Warning: Unable to process class part '{part}'.")

            classes = sorted(set(classes))

            if translated_subject not in olympiad_data:
                olympiad_data[translated_subject] = {}

            # Parse dates and time
            dates, time = extract_dates_and_time(date_str)

            # Add data under the current competition level
            olympiad_data[translated_subject][current_competition_level] = {
                "class": classes,
                "dates": dates
            }
            if time:
                olympiad_data[translated_subject][current_competition_level]["time"] = time

    with open(OUTPUT_JSON_PATH, 'w', encoding='utf-8') as json_file:
        json.dump(olympiad_data, json_file, ensure_ascii=False, indent=4)
    print(f"JSON file saved successfully: {OUTPUT_JSON_PATH}")


def clean_date(date_str):
    return date_str.replace("ДО", "").replace("Г.", "").strip()


def extract_dates_and_time(date_str):
    # Regular expression for detecting time
    time_match = re.search(r'НАЧАЛО\s+(\d{1,2}:\d{2})\s*Ч\.?', date_str)
    time = time_match.group(1) + " o'clock" if time_match else None

    # Remove time part from date string if time was found
    if time:
        date_str = date_str[:time_match.start()].strip()

    # Regular expression for date range
    date_range_match = re.search(r'(\d{2}\.\d{2})\s*-\s*(\d{2}\.\d{2})\s*(\d{4})', date_str)

    if date_range_match:
        start_date_str = date_range_match.group(1) + "." + date_range_match.group(3)
        end_date_str = date_range_match.group(2) + "." + date_range_match.group(3)
        start_date = datetime.strptime(start_date_str, '%d.%m.%Y')
        end_date = datetime.strptime(end_date_str, '%d.%m.%Y')

        # Generate all dates in the range
        dates = []
        current_date = start_date
        while current_date <= end_date:
            dates.append(current_date.strftime('%d.%m.%Y'))
            current_date += timedelta(days=1)
    else:
        # Handle single date or non-matching date formats
        try:
            single_date = datetime.strptime(date_str, '%d.%m.%Y')
            dates = [single_date.strftime('%d.%m.%Y')]
        except ValueError:
            # If the date doesn't match the expected format, include it as is
            dates = [date_str]

    return dates, time


def is_date_range(text):
    return bool(re.search(r'\d{2}\.\d{2}\s*-\s*\d{2}\.\d{2}', text))
