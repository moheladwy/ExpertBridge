from flask import Flask
from flask_cors import CORS
import os
from flask import request, jsonify
from pydantic import ValidationError
from Models import Models
from TextCategorizer import TextCategorizer
from InputFormat import CategorizeRequest, TranslateTagsRequest

app = Flask(__name__)
CORS(app)

# Default to 'development' if not set
ENV = os.getenv('FLASK_ENV', 'development')

# Use FLASK_DEBUG instead of FLASK_ENV
DEBUG = os.getenv('FLASK_DEBUG', '1') == '1'
app.config['DEBUG'] = DEBUG


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
        print(request.get_json())
        # Validate and parse the request body
        request_data = CategorizeRequest.model_validate(
            request.get_json(), strict=True)

        # Combine title and content for categorization
        post_text = f"Title: {request_data.title}\n\nContent:\n{request_data.content}\n\nTags:\n{request_data.tags}"

        # Call the categorize method and return the response
        return text_categorizer.categorize(post_text.strip())
    except ValidationError as ve:
        return jsonify({"error": "Validation error: " + str(ve)}), 400
    except ValueError as ve:
        return jsonify({"error": "Value error: " + str(ve)}), 400
    except Exception as e:
        return jsonify({"error": str(e)}), 500


@app.route("/translate-tags", methods=["POST"])
def translate_tags():
    try:
        # Validate and parse the request body
        request_data = TranslateTagsRequest.model_validate(
            request.get_json(), strict=True)

        # Call the translate_tags method and return the response
        return text_categorizer.translate_tags(request_data.tags)
    except ValidationError as ve:
        return jsonify({"error": "Validation error: " + str(ve)}), 400
    except ValueError as ve:
        return jsonify({"error": "Value error: " + str(ve)}), 400
    except Exception as e:
        return jsonify({"error": str(e)}), 500


# Run the app
if __name__ == "__main__":
    app.run()
