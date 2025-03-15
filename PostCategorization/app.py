import json
from flask import Flask
from flask_cors import CORS
import os
from flask import request, jsonify
from Models import Models
from TextCategorizer import TextCategorizer

app = Flask(__name__)
CORS(app)


# Initialize the API key
API_KEY = os.getenv('GROQ_API_KEY')

# Check if the API key is set
if not API_KEY:
    raise ValueError("Please set the GROQ_API_KEY environment variable")

# Initialize the Categorizer with the API key and the model
text_categorizer = TextCategorizer(
    api_key=API_KEY,
    model=Models().LLAMA3_70B_8192
)


# Define the categorize route
@app.route("/categorize", methods=["POST"])
def categorize():
    try:
        post = str(request.get_json()["post"])
        response = text_categorizer.categorize(post.strip())
        print("=======================================")
        print("Response:")
        print(response)
        print("=======================================")
        return jsonify({"error": "No response"} if not response else json.loads(response))
    except Exception as e:
        return jsonify({"error": str(e)})


# Run the app
if __name__ == "__main__":
    app.run()
