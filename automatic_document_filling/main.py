import PyPDF2
from reportlab.lib.pagesizes import letter
from reportlab.pdfgen import canvas
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.pdfbase import pdfmetrics
from io import BytesIO

def add_text_to_pdf(input_pdf, output_pdf, text, x, y, font_size=12, font_path=None):
    with open(input_pdf, "rb") as infile:
        reader = PyPDF2.PdfReader(infile)
        writer = PyPDF2.PdfWriter()

        packet = BytesIO()
        c = canvas.Canvas(packet, pagesize=letter)

        pdfmetrics.registerFont(TTFont('Arial', font_path))
        c.setFont("Arial", font_size)

        c.drawString(x, y, text)
        c.save()

        packet.seek(0)
        new_pdf = PyPDF2.PdfReader(packet)

        for i in range(len(reader.pages)):
            page = reader.pages[i]
            if i == 0:
                page.merge_page(new_pdf.pages[0])
            writer.add_page(page)

        with open(output_pdf, "wb") as output_file:
            writer.write(output_file)
        print("PDF with Bulgarian text overlay has been created!")

def main():
    input_pdf = 'Deklaracia.pdf'
    output_pdf = 'filled_documents/Deklaracia_filled.pdf'
    text = 'Тестови текст'
    x = 100
    y = 500
    font_path = r'C:\Windows\Fonts\Arial.ttf'

    add_text_to_pdf(input_pdf, output_pdf, text, x, y, font_path=font_path)

if __name__ == "__main__":
    main()
