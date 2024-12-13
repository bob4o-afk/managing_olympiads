import PyPDF2
from reportlab.lib.pagesizes import letter
from reportlab.pdfgen import canvas
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.pdfbase import pdfmetrics
from io import BytesIO


def add_text_to_pdf(input_pdf, output_pdf, texts, coordinates, font_size=12, font_path=None):
    with open(input_pdf, "rb") as infile:
        reader = PyPDF2.PdfReader(infile)
        writer = PyPDF2.PdfWriter()

        packets = [BytesIO() for _ in range(len(reader.pages))]

        # Create a separate overlay for each page
        for page_index, packet in enumerate(packets):
            c = canvas.Canvas(packet, pagesize=letter)
            pdfmetrics.registerFont(TTFont('Arial', font_path))
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
        print("PDF was filled!")

def main():
    input_pdf = 'Deklaracia.pdf'
    output_pdf = 'filled_documents/Deklaracia_filled.pdf'

    # Coordinates array
    coordinates = [
        [210, 615, 0],  # име, презиме, фамилия на родител/настойник
        [150, 575, 0],  # адрес
        [130, 545, 0],  # телефон
        [90, 520, 0],  # име, презиме, фамилия на ученик
        [250, 475, 0],  # клас
        [340, 475, 0],     # начало на горното поле за училище
        [75, 445, 0],  # долното поле за училище, населено място и област
        [160, 365, 0],
        [0, 0, 1]  # примерни координати за 2ра страница
    ]

    # '-----'
    gender_coordinates = {
        "male":[
            [170, 615, 0],
        ],
        "female":[
            [145, 615, 0],
        ]
    }

    # '-----------'
    additional_coordinates = [
        [245, 545, 0],          # родител
        [295, 545, 0],          # настойник
        [350, 545, 0]           # попечител
    ]


    texts = [
        'Борислав Боянов Миланов',  # Text for [210, 615, 0]
        'Адрес на родител',  # Text for [150, 575, 0]
        'Телефонен номер',  # Text for [130, 545, 0]
        'Име на ученик',  # Text for [90, 520, 0]
        '12Б',  # Text for [250, 475, 0]
        'Аааааааааааааааааааааааааааааааа', #32 symbols max
        'Аааааааааааааааааааааааааааааааа',  # the remaining symbols
        '--',
        'Тестово'  # Text for [0, 0, 1]
    ]

    font_path = r'C:\Windows\Fonts\Arial.ttf'

    add_text_to_pdf(input_pdf, output_pdf, texts, coordinates, font_path=font_path)


if __name__ == "__main__":
    main()
