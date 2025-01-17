FROM python:3.9-slim

WORKDIR /app

# Install wait-for-it (and netcat-openbsd)
RUN apt-get update && apt-get install -y --no-install-recommends netcat-openbsd dos2unix && rm -rf /var/lib/apt/lists/*

# Copy requirements and install
COPY requirements.txt ./ 
RUN pip install --no-cache-dir -r requirements.txt

COPY . ./

# Add the wait-for-it script
COPY wait-for-it.sh /usr/local/bin/wait-for-it

# Fix Windows line endings if they exist in the wait-for-it script
RUN dos2unix /usr/local/bin/wait-for-it && chmod +x /usr/local/bin/wait-for-it

CMD ["python", "main.py"]
