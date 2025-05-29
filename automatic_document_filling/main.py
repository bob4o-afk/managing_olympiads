from flask import Flask, request, jsonify
from flask_cors import CORS
import requests
import PyPDF2
from reportlab.lib.pagesizes import letter
from reportlab.pdfgen import canvas
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.pdfbase import pdfmetrics
from io import BytesIO
import os
import datetime
from dotenv import load_dotenv

load_dotenv()
app = Flask(__name__)
CORS(app)

SEND_DOCUMENT_URL = os.getenv("SEND_DOCUMENT_URL")
CC_EMAIL = os.getenv("USER_EMAIL")

def add_text_to_pdf(input_pdf, output_pdf, texts, coordinates, font_size=12, font_path=None):
    if not os.path.exists(font_path):
        raise FileNotFoundError(f"Font file not found at: {font_path}")

    with open(input_pdf, "rb") as infile:
        reader = PyPDF2.PdfReader(infile)
        writer = PyPDF2.PdfWriter()

        packets = [BytesIO() for _ in range(len(reader.pages))]

        # Create a separate overlay for each page
        for page_index, packet in enumerate(packets):
            c = canvas.Canvas(packet, pagesize=letter)
            pdfmetrics.registerFont(TTFont("Arial", font_path))
            c.setFont("Arial", font_size)

            for text, (x, y, page) in zip(texts, coordinates):
                if page == page_index:
                    c.drawString(x, y, text)

            c.save()
            packet.seek(0)

        # Merge overlays with the respective pages
        for i, page in enumerate(reader.pages):
            overlay_pdf = PyPDF2.PdfReader(packets[i])
            page.merge_page(overlay_pdf.pages[0])
            writer.add_page(page)

        with open(output_pdf, "wb") as output_file:
            writer.write(output_file)

@app.route("/fill_pdf", methods=["POST"])
def fill_pdf():
    data = request.json

    email = data.get("email")
    if not email:
        return jsonify({"error": "Email is required"}), 400

    parent_name = data.get("parentName", "")
    address = data.get("address", "")
    telephone = data.get("telephone", "")
    student_name = data.get("studentName", "")
    grade = data.get("grade", "")
    school = data.get("school", "")
    gender = data.get("gender", "").lower()

    max_length = 32
    if len(school) > max_length:
        last_space = school[:max_length].rfind(" ")
        first_line = school[:last_space] if last_space != -1 else school[:max_length]
        second_line = school[last_space + 1:] if last_space != -1 else school[max_length:]
    else:
        first_line = school
        second_line = ""

    texts = [
        parent_name,
        address,
        telephone,
        student_name,
        grade,
        first_line,
        second_line,
        datetime.datetime.now().strftime('%d.%m.%Y'),
    ]

    # Coordinates for text placement
    coordinates = [
        [210, 615, 0],  # Parent name
        [150, 575, 0],  # Address
        [130, 545, 0],  # Telephone
        [90, 520, 0],   # Student name
        [250, 475, 0],  # Grade
        [340, 475, 0],  # School first line
        [75, 445, 0],   # School second line
        [375, 232, 1]   # Date (2nd page)
    ]

    gender_coordinates = {
        "male": [170, 615, 0],
        "female": [155, 615, 0]
    }
    if gender in gender_coordinates:
        coordinates.append(gender_coordinates[gender])
        texts.append("X")

    input_pdf = "Deklaracia.pdf"
    output_pdf = "filled_documents/Deklaracia_filled.pdf"

    font_path = os.path.join(os.path.dirname(__file__), "fonts", "Arial.ttf")

    try:
        add_text_to_pdf(input_pdf, output_pdf, texts, coordinates, font_path=font_path)

        response = send_document(email, output_pdf)

        if response.status_code == 200:
            return jsonify({"message": "PDF filled and sent successfully"})
        else:
            return jsonify({"error": f"Failed to send document: {response.text}"}), response.status_code

    except FileNotFoundError as e:
        return jsonify({"error": f"File not found: {e}"}), 404
    except Exception as e:
        import traceback
        traceback.print_exc()


        return jsonify({"error": str(e)}), 500

def send_document(email, file_path):
    payload = {
        "ToEmail": email,
        "Subject": "Filled Document",
        "Body": "Please find the filled document attached.",
        "CcEmail": CC_EMAIL,
    }

    with open(file_path, "rb") as pdf_file:
        files = {"Document": ("Deklaracia_filled.pdf", pdf_file, "application/pdf")}
        return requests.post(SEND_DOCUMENT_URL, data=payload, files=files)

if __name__ == "__main__":
    os.makedirs("filled_documents", exist_ok=True)
    os.makedirs("fonts", exist_ok=True)
    app.run(debug=True)
