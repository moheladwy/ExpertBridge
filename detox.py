from flask import Flask, request, jsonify
from detoxify import Detoxify

app = Flask(__name__)

# Load the Detoxify model once when the app starts
model = Detoxify('original')

@app.route('/predict', methods=['POST'])
def predict():
    # Get JSON from the request body
    data = request.get_json(force=True)
    text = data.get("text", "")

    if not text:
        return jsonify({"error": "No text provided"}), 400

    scores = model.predict(text)

    # Set your threshold (adjustable as needed) i tested and got good results on 0.8
    threshold = 0.8
    prediction = "Toxic" if scores['toxicity'] > threshold else "Not Toxic"

    # Return the result as JSON
    return jsonify({
        "text": text,
        "toxicity_score": scores['toxicity'],
        "prediction": prediction,
        "detailed_scores": scores
    })

if __name__ == '__main__':
    app.run(debug=True)
