# Use an official Python runtime as a parent image
FROM python:3.9-slim

# Prevent Python from buffering stdout and stderr
ENV PYTHONUNBUFFERED=1

# Set the working directory in the container
WORKDIR /app

# Copy the requirements file into the container at /app
COPY requirements.txt /app/requirements.txt

# Install any needed packages specified in requirements.txt
RUN pip install --upgrade pip && \
    pip install -r requirements.txt

# Copy the rest of the application code into the container
COPY . /app

# Expose port 5000 for the Flask app
EXPOSE 5000

# Define environment variable for Flask (optional if you use a FLASK_APP variable)
ENV FLASK_APP=app.py

# Run the application
CMD ["python", "app.py"]
