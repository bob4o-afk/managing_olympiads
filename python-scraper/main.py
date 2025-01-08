from src.scraper import download_pdf
from src.pdf_processing import pdf_to_word_and_extract_table
from src.data_handling import extract_and_process_text
from src.exceptions import EnvironmentVariableError


def main():
    try:
        download_pdf()
        pdf_to_word_and_extract_table()
        extract_and_process_text()


    except EnvironmentVariableError as e:
        print(f"Configuration error: {e}")
    except Exception as e:
        print(f"An unexpected error occurred: {e}")


if __name__ == "__main__":
    main()

