from typing import List
from detoxify import Detoxify
import pandas as pd

class InappropriateLanguageDetection:
    def __init__(self, model_type: str = 'original'):
        self.detoxify = Detoxify(model_type)

    def predict(self, input_text: str) -> dict:
        prediction = self.detoxify.predict(input_text)
        return {k: round(float(v), 5) for k, v in prediction.items()}

    def predict_batch(self, input_text: List[str]) -> dict:
        results = self.detoxify.predict(input_text)
        results = {k: [round(float(v), 5) for v in values] for k, values in results.items()}
        return pd.DataFrame(results, index=pd.Index(input_text)).to_dict('index')