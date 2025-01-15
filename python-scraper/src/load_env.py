import os
from dotenv import load_dotenv
from src.exceptions import EnvironmentVariableError


def load_environment_variables():
    load_dotenv()

    # Retrieve environment variables
    pdf_file_path = os.getenv('PDF_FILE_PATH')
    output_word_path = os.getenv('OUTPUT_WORD_PATH')
    output_txt_path = os.getenv('OUTPUT_TXT_PATH')
    output_json_path = os.getenv('OUTPUT_JSON_PATH')

    # Check if any environment variables are missing
    if not all([pdf_file_path, output_word_path, output_txt_path, output_json_path]):
        raise EnvironmentVariableError("One or more environment variables are missing.")

    return pdf_file_path, output_word_path, output_txt_path, output_json_path
