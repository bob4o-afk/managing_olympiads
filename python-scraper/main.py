from src.scraper import download_pdf
from src.extract_info import pdf_to_word_and_extract_table

if __name__ == "__main__":
    download_pdf()
    pdf_to_word_and_extract_table()

#note: pip install --upgrade supabase gotrue


#TODO:
#1. Better error handling
#2. Saving into a db in supabase