FROM node:18

# Install dos2unix for line-ending conversion
RUN apt-get update && \
    apt-get install -y --no-install-recommends dos2unix && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Copy the wait-for-it script from the local directory
COPY wait-for-it.sh /usr/local/bin/wait-for-it

# Fix Windows line endings if they exist in the wait-for-it script
RUN dos2unix /usr/local/bin/wait-for-it && chmod +x /usr/local/bin/wait-for-it

# Install dependencies
COPY frontend-app/package*.json ./
RUN npm install

COPY frontend-app/ ./

EXPOSE 3000

# Command to wait for backend and db before starting the frontend
CMD ["sh", "-c", "wait-for-it backend:5138 --timeout=60 && wait-for-it db:3306 --timeout=60 && npm start"]
