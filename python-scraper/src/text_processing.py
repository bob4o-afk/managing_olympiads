import re
from datetime import datetime, timedelta

roman_to_arabic = {
    "I": 1, "II": 2, "III": 3, "IV": 4, "V": 5, "VI": 6, "VII": 7, "VIII": 8,
    "IX": 9, "X": 10, "XI": 11, "XII": 12
}

translation_dict = {
    "ЗНАМ И МОГА": "know_and_can",
    "МАТЕМАТИКА": "math",
    "БЪЛГАРСКИ ЕЗИК И ЛИТЕРАТУРА": "bulgarian_language_and_literature",
    "АНГЛИЙСКИ ЕЗИК": "english",
    "НЕМСКИ ЕЗИК": "german",
    "ИСПАНСКИ ЕЗИК": "spanish",
    "ИТАЛИАНСКИ ЕЗИК": "italian",
    "РУСКИ ЕЗИК": "russian",
    "ФРЕНСКИ ЕЗИК": "french",
    "ИНФОРМАТИКА": "informatics",
    "ИНФОРМАЦИОННИ ТЕXНОЛОГИИ": "information_technologies",
    "ЛИНГВИСТИКА": "linguistics",
    "ФИЛОСОФИЯ": "philosophy",
    "ИСТОРИЯ И ЦИВИЛИЗАЦИИ": "history_and_civilizations",
    "ГЕОГРАФИЯ И ИКОНОМИКА": "geography_and_economics",
    "ГРАЖДАНСКО ОБРАЗОВАНИЕ": "civic_education",
    "ФИЗИКА": "physics",
    "АСТРОНОМИЯ": "astronomy",
    "XИМИЯ И ОПАЗВАНЕ НА ОКОЛНАТА СРЕДА": "chemistry_and_environmental_protection",
    "БИОЛОГИЯ И ЗДРАВНО ОБРАЗОВАНИЕ": "biology_and_health_education",
    "ТЕXНИЧЕСКО ЧЕРТАНЕ": "technical_drawing"
}

competition_levels = {
    "ОБЛАСТЕН КРЪГ": "district_ring",
    "РЕГИОНАЛЕН КРЪГ": "regional_ring",
    "НАЦИОНАЛЕН КРЪГ": "national_ring"
}

def expand_range(start, end):
    return ' '.join(map(str, range(start, end + 1)))

def is_date_range(text):
    return bool(re.search(r'\d{2}\.?\d{2}\s*-\s*\d{2}\.?\d{2}\.\d{4}', text))

def parse_date_range(date_range):
    match = re.match(r'(\d{2})\.?(\d{2})\s*-\s*(\d{2})\.?(\d{2})\.(\d{4})', date_range)
    if match:
        day1, month1, day2, month2, year = match.groups()
        start_date = f"{day1}.{month1}.{year}"
        end_date = f"{day2}.{month2}.{year}"
        return [start_date, end_date]
    return []

def format_date_range(date_range):
    return re.sub(r'\.\s*-\s*', ' - ', date_range)

def clean_date_range(text):
    # Remove the "г." suffix
    text = text.replace(" г.", "")

    # Use format_date_range() to clean and format the date range
    formatted_text = format_date_range(text)

    match = re.search(r'(\d{2})\.(\d{2})\s*-\s*(\d{2})\.(\d{2})\.(\d{4})', formatted_text)
    if match:
        day1, month1, day2, month2, year = match.groups()
        start_date = datetime.strptime(f"{day1}.{month1}.{year}", "%d.%m.%Y")
        end_date = datetime.strptime(f"{day2}.{month2}.{year}", "%d.%m.%Y")

        date_range = [start_date + timedelta(days=i) for i in range((end_date - start_date).days + 1)]

        formatted_dates = ', '.join(date.strftime("%d.%m.%Y") for date in date_range)
        return formatted_dates

    return text

def expand_ranges_in_text(text):
    # Separate text by potential date ranges and class ranges
    parts = re.split(r'(\d{2}\.\d{2}\s*-\s*\d{2}\.\d{2}\.\d{4})', text)
    expanded_parts = []

    for part in parts:
        part = clean_date_range(part)  # Clean the date range
        if is_date_range(part):
            expanded_parts.extend(parse_date_range(part))
        else:
            # Expand class ranges
            pattern = re.compile(r'(\d+)\s*-\s*(\d+)')
            expanded_part = part
            matches = pattern.finditer(expanded_part)

            for match in matches:
                start, end = map(int, match.groups())
                expanded_part = expanded_part.replace(match.group(0), expand_range(start, end))

            expanded_parts.append(expanded_part)

    return ''.join(expanded_parts)

def normalize_text(text):
    text = text.upper()
    text = text.replace("Х", "X").replace("І", "I").replace("–", "-")
    return text

def convert_roman_to_arabic(text):
    text = normalize_text(text)
    for roman, arabic in roman_to_arabic.items():
        text = re.sub(rf'\b{roman}\b', str(arabic), text)
    return text
