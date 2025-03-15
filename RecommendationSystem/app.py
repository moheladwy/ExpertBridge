from flask import Flask, request, jsonify
from flask_cors import CORS
from Post import Post
from PostRecommender import PostRecommender

app = Flask(__name__)
CORS(app)

# Initialize the Recommender with the API key and the model
post_recommender = PostRecommender()

# Define the recommend route
@app.route("/recommend", methods=["GET"])
def recommend():
    try:
        response = post_recommender.recommend(
            query=request.get_json()["query"],
            tags=request.get_json()["tags"]
        )
        print("=======================================")
        print("Response:")
        print(response)
        print("=======================================")
        return jsonify({"error": "No response"} if not response else response)
    except Exception as e:
        return jsonify({"error": str(e)})


# Define the add-post route
@app.route("/add-post", methods=["POST"])
def add_post():
    try:
        post = Post(request.get_json()["post"])
        post_recommender.add_post(post)
        return jsonify({"message": "Post added successfully"})
    except Exception as e:
        return jsonify({"error": str(e)})


# Run the app
if __name__ == "__main__":
    app.run()
