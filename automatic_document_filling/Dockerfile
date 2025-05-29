FROM python:3.9-slim

RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    build-essential \
    libcairo2-dev \
    pkg-config \
    python3-dev \
    curl \
    fontconfig && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY fonts/ /app/fonts/
COPY requirements.txt ./
RUN pip install --no-cache-dir -r requirements.txt
COPY . ./

EXPOSE 5000

CMD ["gunicorn", "-b", "0.0.0.0:5000", "main:app"]
