from flask import Flask, request
from flask_cors import CORS
from InappropriateLanguageDetection import InappropriateLanguageDetection as ILD

ild = ILD()
app = Flask(__name__)
CORS(app)

@app.route('/predict', methods=['POST'])
def predict():
    try:
      input_text = str(request.get_json()['text'])
      print(input_text)
    except:
      return 'Error: Invalid input'
    return ild.predict(input_text)
    
if __name__ == '__main__':
    app.run(debug=True)