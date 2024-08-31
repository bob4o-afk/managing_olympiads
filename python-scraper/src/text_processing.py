import re

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


def expand_range(start, end):
    return ' '.join(map(str, range(start, end + 1)))


def expand_ranges_in_text(text):
    pattern = re.compile(r'(\d+)\s*-\s*(\d+)')
    matches = pattern.finditer(text)
    expanded_text = text

    for match in matches:
        start, end = map(int, match.groups())
        expanded_text = expanded_text.replace(match.group(0), expand_range(start, end))

    return expanded_text


def normalize_text(text):
    text = text.upper()
    text = text.replace("Х", "X").replace("І", "I").replace("–", "-")
    return text


def convert_roman_to_arabic(text):
    text = normalize_text(text)
    for roman, arabic in roman_to_arabic.items():
        text = re.sub(rf'\b{roman}\b', str(arabic), text)
    return text
