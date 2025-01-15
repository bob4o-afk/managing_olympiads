FROM node:18

# Install curl and copy wait-for-it script
RUN apt-get update && \
    apt-get install -y --no-install-recommends curl && \
    curl -Lo /usr/local/bin/wait-for-it.sh https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh && \
    chmod +x /usr/local/bin/wait-for-it.sh && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Install wait-for-it script
COPY wait-for-it.sh /usr/local/bin/wait-for-it
RUN chmod +x /usr/local/bin/wait-for-it


# Install dependencies
COPY frontend-app/package*.json ./
RUN npm install

COPY frontend-app/ ./

EXPOSE 3000

# Command to wait for backend and db before starting the frontend
CMD ["sh", "-c", "wait-for-it backend:5138 --timeout=60 && wait-for-it db:3306 --timeout=60 && npm start"]
