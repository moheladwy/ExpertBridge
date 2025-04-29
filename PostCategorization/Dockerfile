# Use the official Python image from the Docker Hub
FROM python:3.12-alpine

# Set the working directory in the container
WORKDIR /app

# Copy the requirements file into the container
COPY requirements.txt .

# Install the dependencies
RUN pip install --no-cache-dir -r requirements.txt

# Copy the rest of the application code into the container
COPY . .

# Expose the port the app runs on
EXPOSE 5000

# Command to run the application
CMD ["flask", "run", "--host=0.0.0.0", "--port=5000"]

# CMD ["gunicorn","--bind", "0.0.0.0:5000", "app:app"]

# Use Gunicorn for production, Flask dev server for development
# CMD ["sh", "-c", "if [ \"$FLASK_DEBUG\" = \"1\" ]; then flask run --host=0.0.0.0 --port=5000; else gunicorn --bind 0.0.0.0:5000 app:app; fi"]