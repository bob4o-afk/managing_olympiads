FROM python:3.9-slim

# Install dependencies, including dos2unix
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    build-essential \
    libcairo2-dev \
    pkg-config \
    python3-dev \
    curl \
    fontconfig \
    dos2unix && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY fonts/ /app/fonts/

# Install wait-for-it script
COPY wait-for-it.sh /usr/local/bin/wait-for-it

# Fix Windows line endings if they exist in the wait-for-it script
RUN dos2unix /usr/local/bin/wait-for-it && chmod +x /usr/local/bin/wait-for-it

# Install Python dependencies
COPY requirements.txt ./ 
RUN pip install --no-cache-dir -r requirements.txt

COPY . ./

EXPOSE 5000

# Command to wait for backend and db, then start the app
CMD ["sh", "-c", "wait-for-it backend:5138 --timeout=60 && wait-for-it db:3306 --timeout=60 && gunicorn -b 0.0.0.0:5000 main:app"]
