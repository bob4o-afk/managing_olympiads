import re
import json
import os
from dotenv import load_dotenv

from src.text_processing import convert_roman_to_arabic, translation_dict
from src.load_env import load_environment_variables


def extract_and_process_text():
    # Load environment variables from .env file
    load_dotenv()
    _, _, TXT_PATH, OUTPUT_JSON_PATH = load_environment_variables()

    olympiad_data = {}

    with open(TXT_PATH, 'r', encoding='utf-8') as file:
        for line in file:
            parts = line.split("\t")
            if len(parts) < 4:
                continue

            subject = parts[0].strip()
            classes_str = parts[1].strip()
            date_str = clean_date(parts[-1].strip())

            translated_subject = translation_dict.get(subject, subject)
            classes_str = convert_roman_to_arabic(classes_str.replace("И", "").strip())
            classes = []

            class_parts = re.split(r'\s+', classes_str)
            for part in class_parts:
                part = part.strip().replace(",", "")
                if "-" in part:
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

            olympiad_data[translated_subject] = {
                "class": classes,
                "date": date_str
            }

    with open(OUTPUT_JSON_PATH, 'w', encoding='utf-8') as json_file:
        json.dump(olympiad_data, json_file, ensure_ascii=False, indent=4)
    print(f"JSON file saved successfully: {OUTPUT_JSON_PATH}")


def clean_date(date_str):
    return date_str.replace("ДО", "").replace("Г.", "").strip()
