import pdfplumber
from docx import Document
import os
from dotenv import load_dotenv

from src.text_processing import convert_roman_to_arabic, expand_ranges_in_text
from src.load_env import load_environment_variables


def pdf_to_word_and_extract_table():
    # Load environment variables from .env file
    PDF_FILE_PATH, OUTPUT_WORD_PATH, OUTPUT_TXT_PATH, _ = load_environment_variables()


    with pdfplumber.open(PDF_FILE_PATH) as pdf:
        page = pdf.pages[1]  # Page 2 in 0-indexed format
        tables = page.extract_tables()

        if tables:
            target_table = tables[0]
            doc = Document()
            doc.add_heading('Extracted Table', 0)
            table_in_doc = doc.add_table(rows=1, cols=len(target_table[0]))
            hdr_cells = table_in_doc.rows[0].cells

            for i, header in enumerate(target_table[0]):
                hdr_cells[i].text = convert_roman_to_arabic(header)

            for row in target_table[1:]:
                row_cells = table_in_doc.add_row().cells
                for i, cell in enumerate(row):
                    row_cells[i].text = convert_roman_to_arabic(cell)

            doc.save(OUTPUT_WORD_PATH)
            print(f"Word document saved successfully: {OUTPUT_WORD_PATH}")

            with open(OUTPUT_TXT_PATH, 'w', encoding='utf-8') as txt_file:
                buffer = ""
                for row in target_table[1:]:
                    for cell in row:
                        formatted_cell = convert_roman_to_arabic(cell).strip().replace('\n', ' ')
                        expanded_cell = expand_ranges_in_text(formatted_cell)
                        buffer += expanded_cell + "\t"
                        if "Г." in expanded_cell:
                            txt_file.write(buffer.strip() + '\n')
                            buffer = ""

                if buffer.strip():
                    txt_file.write(buffer.strip() + '\n')

            print(f"Text file saved successfully: {OUTPUT_TXT_PATH}")

        else:
            print("No tables found on page 2.")
