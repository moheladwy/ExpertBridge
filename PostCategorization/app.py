import json
from flask import Flask
from flask_cors import CORS
import os
from flask import request, jsonify
from pydantic import ValidationError
from Models import Models
from TextCategorizer import TextCategorizer
from InputFormat import CategorizeRequest

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
        # Validate and parse the request body
        request_data = CategorizeRequest.model_validate(request.get_json(), strict=True)

        # Combine title and content for categorization
        post_text = f"Title: {request_data.post.title}\n\nContent:\n{request_data.post.content}\n\nTags:\n{request_data.post.tags}"
        
        # Call the categorize method and return the response
        return text_categorizer.categorize(post_text.strip())
    except ValidationError as ve:
        return jsonify({"error": "Validation error: " + str(ve)})
    except ValueError as ve:
        return jsonify({"error": "Value error: " + str(ve)})
    except Exception as e:
        return jsonify({"error": str(e)})


# Run the app
if __name__ == "__main__":
    app.run()
